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
    }
}
