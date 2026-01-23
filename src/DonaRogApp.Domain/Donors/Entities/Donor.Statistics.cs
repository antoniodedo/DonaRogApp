using DonaRogApp.Domain.Donors.Events;
using DonaRogApp.Enums.Donors;
using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace DonaRogApp.Domain.Donors.Entities
{
    public partial class Donor : FullAuditedAggregateRoot<Guid>
    {
        public void UpdateStatistics(decimal donationAmount)
        {
            Check.Positive(donationAmount, nameof(donationAmount));

            TotalDonated += donationAmount;
            DonationCount++;
            AverageDonationAmount = DonationCount > 0 ? TotalDonated / DonationCount : 0;

            if (FirstDonationDate == null)
            {
                FirstDonationDate = DateTime.UtcNow;
                FirstDonationAmount = donationAmount;
            }

            LastDonationDate = DateTime.UtcNow;
            LastDonationAmount = donationAmount;

            UpdateCategory();
            RecalculateRFM();

            AddLocalEvent(new DonorStatisticsUpdatedEvent(this.Id, TotalDonated, DonationCount, AverageDonationAmount));
        }

        public void RecalculateRFM()
        {
            RecalculateRecencyScore();
            RecalculateFrequencyScore();
            RecalculateMonetaryScore();
            CalculateRfmSegment();

            AddLocalEvent(new DonorRfmRecalculatedEvent(this.Id, RecencyScore, FrequencyScore, MonetaryScore, RfmSegment));
        }

        private void RecalculateRecencyScore()
        {
            if (!LastDonationDate.HasValue)
            {
                RecencyScore = 1;
                return;
            }

            var daysSinceLastDonation = (DateTime.UtcNow - LastDonationDate.Value).Days;

            RecencyScore = daysSinceLastDonation switch
            {
                <= 30 => 5,
                <= 90 => 4,
                <= 180 => 3,
                <= 365 => 2,
                _ => 1
            };
        }

        private void RecalculateFrequencyScore()
        {
            FrequencyScore = DonationCount switch
            {
                0 => 1,
                1 => 2,
                >= 2 and <= 5 => 3,
                >= 6 and <= 12 => 4,
                > 12 => 5
            };
        }

        private void RecalculateMonetaryScore()
        {
            MonetaryScore = TotalDonated switch
            {
                < 100 => 1,
                >= 100 and < 500 => 2,
                >= 500 and < 2000 => 3,
                >= 2000 and < 10000 => 4,
                >= 10000 => 5
            };
        }

        private void CalculateRfmSegment()
        {
            var totalScore = RecencyScore + FrequencyScore + MonetaryScore;

            RfmSegment = totalScore switch
            {
                >= 13 => "Champions",
                >= 11 => "Loyal",
                >= 9 => "Potential",
                >= 7 => "AtRisk",
                >= 5 => "Dormant",
                _ => "Lost"
            };
        }

        private void UpdateCategory()
        {
            var oldCategory = Category;

            Category = TotalDonated switch
            {
                >= 50000 => DonorCategory.Major,
                >= 10000 => DonorCategory.Gold,
                >= 5000 => DonorCategory.Silver,
                >= 1000 => DonorCategory.Bronze,
                _ => DonorCategory.Standard
            };

            if (oldCategory != Category)
            {
                AddLocalEvent(new DonorCategoryChangedEvent(this.Id, oldCategory, Category));
            }
        }

        public bool IsLapsedDonor(int inactiveDaysThreshold = 540)
        {
            if (!LastDonationDate.HasValue)
                return false;

            var daysSinceLastDonation = (DateTime.UtcNow - LastDonationDate.Value).Days;
            return daysSinceLastDonation > inactiveDaysThreshold;
        }

        public void MarkAsLapsed()
        {
            if (Status != DonorStatus.Lapsed)
            {
                var oldStatus = Status;
                Status = DonorStatus.Lapsed;

                AddLocalEvent(new DonorStatusChangedEvent(this.Id, oldStatus, Status, "Lapsed: No donations in 18 months"));
            }
        }

        public void ReactivateLapsedDonor()
        {
            if (Status == DonorStatus.Lapsed)
            {
                Status = DonorStatus.Active;
                RecencyScore = 5;

                AddLocalEvent(new DonorStatusChangedEvent(this.Id, DonorStatus.Lapsed, DonorStatus.Active, "Reactivated"));
            }
        }
    }
}
