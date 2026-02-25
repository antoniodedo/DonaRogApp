using DonaRogApp.Domain.Donations.Events;
using DonaRogApp.Enums.Donations;
using System;
using System.Linq;
using Volo.Abp;

namespace DonaRogApp.Domain.Donations.Entities
{
    public partial class Donation
    {
        /// <summary>
        /// Verify a pending donation
        /// Changes status from Pending to Verified
        /// </summary>
        public void Verify(
            Guid verifiedBy,
            Guid? campaignId = null,
            Guid? bankAccountId = null,
            Guid? thankYouTemplateId = null,
            string? notes = null,
            string? internalNotes = null)
        {
            if (Status != DonationStatus.Pending)
            {
                throw new BusinessException("DonaRog:DonationCanOnlyVerifyPending")
                    .WithData("donationId", Id)
                    .WithData("currentStatus", Status);
            }

            Status = DonationStatus.Verified;
            VerifiedAt = DateTime.UtcNow;
            VerifiedBy = verifiedBy;
            
            CampaignId = campaignId;
            BankAccountId = bankAccountId;
            ThankYouTemplateId = thankYouTemplateId;

            if (!string.IsNullOrWhiteSpace(notes))
            {
                Notes = notes;
            }

            if (!string.IsNullOrWhiteSpace(internalNotes))
            {
                InternalNotes = internalNotes;
            }

            // Raise event for statistics update
            var projectAllocations = Projects
                .Select(p => (p.ProjectId, p.AllocatedAmount))
                .ToList();

            AddLocalEvent(new DonationVerifiedEvent(
                Id,
                DonorId,
                TotalAmount,
                DonationDate,
                verifiedBy,
                VerifiedAt.Value,
                CampaignId,
                projectAllocations
            ));
        }

        /// <summary>
        /// Reject a pending donation
        /// Changes status from Pending to Rejected
        /// </summary>
        public void Reject(
            Guid rejectedBy,
            RejectionReason reason,
            string? notes = null)
        {
            if (Status != DonationStatus.Pending)
            {
                throw new BusinessException("DonaRog:DonationCanOnlyRejectPending")
                    .WithData("donationId", Id)
                    .WithData("currentStatus", Status);
            }

            Status = DonationStatus.Rejected;
            RejectionReason = reason;
            RejectionNotes = notes;
            RejectedAt = DateTime.UtcNow;
            RejectedBy = rejectedBy;

            // Raise event for audit
            AddLocalEvent(new DonationRejectedEvent(
                Id,
                DonorId,
                reason,
                rejectedBy,
                RejectedAt.Value,
                notes
            ));
        }

        /// <summary>
        /// Update donor (can be done before verification)
        /// </summary>
        public void UpdateDonor(Guid donorId)
        {
            if (Status != DonationStatus.Pending)
            {
                throw new BusinessException("DonaRog:DonationCanOnlyChangeDonorWhenPending")
                    .WithData("donationId", Id)
                    .WithData("currentStatus", Status);
            }

            DonorId = Check.NotNull(donorId, nameof(donorId));
        }

        /// <summary>
        /// Update thank-you template
        /// </summary>
        public void UpdateThankYouTemplate(Guid? thankYouTemplateId)
        {
            ThankYouTemplateId = thankYouTemplateId;
        }

        /// <summary>
        /// Update notes
        /// </summary>
        public void UpdateNotes(string? notes, string? internalNotes = null)
        {
            if (!string.IsNullOrWhiteSpace(notes))
            {
                Notes = notes;
            }

            if (!string.IsNullOrWhiteSpace(internalNotes))
            {
                InternalNotes = internalNotes;
            }
        }

        /// <summary>
        /// Update core donation data (ONLY for manually registered donations)
        /// Cannot update external donations to preserve data integrity
        /// </summary>
        public void UpdateCoreData(
            DonationChannel channel,
            decimal totalAmount,
            DateTime donationDate,
            DateTime? creditDate = null)
        {
            // Business Rule: Cannot modify core data for donations from external flows
            if (!string.IsNullOrWhiteSpace(ExternalId))
            {
                throw new BusinessException("DonaRog:CannotUpdateExternalDonationCoreData")
                    .WithData("donationId", Id)
                    .WithData("externalId", ExternalId);
            }

            // Business Rule: Can only update verified donations
            if (Status != DonationStatus.Verified)
            {
                throw new BusinessException("DonaRog:CanOnlyUpdateVerifiedDonations")
                    .WithData("donationId", Id)
                    .WithData("currentStatus", Status);
            }

            Channel = channel;
            TotalAmount = Check.Positive(totalAmount, nameof(totalAmount));
            DonationDate = donationDate;
            CreditDate = creditDate;

            VerifyInvariants();
        }

        /// <summary>
        /// Update donation metadata (campaign, bank account, notes)
        /// Can be done for both manual and external donations
        /// </summary>
        public void UpdateMetadata(
            Guid? campaignId = null,
            Guid? bankAccountId = null,
            Guid? thankYouTemplateId = null,
            string? notes = null,
            string? internalNotes = null)
        {
            // Business Rule: Can only update verified donations
            if (Status != DonationStatus.Verified)
            {
                throw new BusinessException("DonaRog:CanOnlyUpdateVerifiedDonations")
                    .WithData("donationId", Id)
                    .WithData("currentStatus", Status);
            }

            CampaignId = campaignId;
            BankAccountId = bankAccountId;
            ThankYouTemplateId = thankYouTemplateId;

            if (!string.IsNullOrWhiteSpace(notes))
            {
                Notes = notes;
            }

            if (!string.IsNullOrWhiteSpace(internalNotes))
            {
                InternalNotes = internalNotes;
            }
        }

        /// <summary>
        /// Check if donation is from external flow
        /// </summary>
        public bool IsFromExternalFlow() => !string.IsNullOrWhiteSpace(ExternalId);

        /// <summary>
        /// Check if donation can be edited (core data)
        /// </summary>
        public bool CanEditCoreData() => IsVerified() && !IsFromExternalFlow();
    }
}
