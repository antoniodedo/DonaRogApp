using DonaRogApp.Enums.Donations;
using System;
using Volo.Abp;

namespace DonaRogApp.Domain.Donations.Entities
{
    public partial class Donation
    {
        /// <summary>
        /// Factory method to create a new donation (manual registration)
        /// Creates donation with Verified status
        /// </summary>
        public static Donation CreateVerified(
            Guid id,
            Guid? tenantId,
            string reference,
            Guid donorId,
            DonationChannel channel,
            decimal totalAmount,
            DateTime donationDate,
            DateTime? creditDate = null,
            Guid? campaignId = null,
            Guid? bankAccountId = null,
            string? notes = null,
            string? internalNotes = null)
        {
            Check.NotNull(id, nameof(id));
            Check.NotNullOrWhiteSpace(reference, nameof(reference));
            Check.NotNull(donorId, nameof(donorId));
            Check.Positive(totalAmount, nameof(totalAmount));

            return new Donation(
                id,
                tenantId,
                reference,
                donorId,
                channel,
                DonationStatus.Verified,
                totalAmount,
                donationDate,
                creditDate,
                campaignId,
                bankAccountId,
                null, // No external ID for manual donations
                notes,
                internalNotes
            );
        }

        /// <summary>
        /// Factory method to create a pending donation (from external flow)
        /// Creates donation with Pending status awaiting verification
        /// </summary>
        public static Donation CreatePending(
            Guid id,
            Guid? tenantId,
            string reference,
            Guid donorId,
            DonationChannel channel,
            decimal totalAmount,
            DateTime donationDate,
            string externalId,
            DateTime? creditDate = null,
            string? notes = null)
        {
            Check.NotNull(id, nameof(id));
            Check.NotNullOrWhiteSpace(reference, nameof(reference));
            Check.NotNull(donorId, nameof(donorId));
            Check.Positive(totalAmount, nameof(totalAmount));
            Check.NotNullOrWhiteSpace(externalId, nameof(externalId));

            return new Donation(
                id,
                tenantId,
                reference,
                donorId,
                channel,
                DonationStatus.Pending,
                totalAmount,
                donationDate,
                creditDate,
                null, // Campaign will be set during verification
                null, // Bank account will be set during verification
                externalId,
                notes,
                null // Internal notes will be added during verification
            );
        }

        /// <summary>
        /// Generate donation reference
        /// Format: DON-YYYY-NNNNN
        /// </summary>
        public static string GenerateReference(int sequenceNumber, int? year = null)
        {
            var y = year ?? DateTime.UtcNow.Year;
            return $"DON-{y}-{sequenceNumber:D5}";
        }
    }
}
