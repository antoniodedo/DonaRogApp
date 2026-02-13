using System.Linq;

namespace DonaRogApp.Domain.Campaigns.Entities
{
    public partial class Campaign
    {
        /// <summary>
        /// Update campaign statistics
        /// </summary>
        public void UpdateStatistics()
        {
            // Count extracted donors
            ExtractedDonorCount = CampaignDonors.Count(cd => cd.RemovedAt == null);

            // Count dispatched
            DispatchedCount = CampaignDonors.Count(cd => cd.DispatchedAt.HasValue && cd.RemovedAt == null);

            // Count responses (opened or clicked or donated)
            ResponseCount = CampaignDonors.Count(cd => 
                (cd.OpenedAt.HasValue || cd.ClickedAt.HasValue || cd.DonationAmount.HasValue) 
                && cd.RemovedAt == null);

            // Calculate response rate
            if (DispatchedCount > 0)
            {
                ResponseRate = (decimal)ResponseCount / DispatchedCount * 100;
            }
            else
            {
                ResponseRate = 0;
            }

            // Count donations
            DonationCount = CampaignDonors.Count(cd => cd.DonationAmount.HasValue && cd.RemovedAt == null);

            // Calculate total raised
            TotalRaised = CampaignDonors
                .Where(cd => cd.DonationAmount.HasValue && cd.RemovedAt == null)
                .Sum(cd => cd.DonationAmount!.Value);

            // Calculate average donation
            if (DonationCount > 0)
            {
                AverageDonation = TotalRaised / DonationCount;
            }
            else
            {
                AverageDonation = 0;
            }

            // Calculate conversion rate
            if (DispatchedCount > 0)
            {
                ConversionRate = (decimal)DonationCount / DispatchedCount * 100;
            }
            else
            {
                ConversionRate = 0;
            }

            // Calculate ROI
            ROI = CalculateROI();
        }

        /// <summary>
        /// Update total cost
        /// </summary>
        public void UpdateTotalCost(decimal cost)
        {
            TotalCost = cost;
            ROI = CalculateROI();
        }

        /// <summary>
        /// Add to total raised
        /// </summary>
        public void AddToTotalRaised(decimal amount)
        {
            TotalRaised += amount;
            UpdateStatistics();
        }

        /// <summary>
        /// Subtract from total raised
        /// </summary>
        public void SubtractFromTotalRaised(decimal amount)
        {
            TotalRaised -= amount;
            if (TotalRaised < 0)
                TotalRaised = 0;

            UpdateStatistics();
        }

        /// <summary>
        /// Set target donor count
        /// </summary>
        public void SetTargetDonorCount(int count)
        {
            TargetDonorCount = count;
        }
    }
}
