using DonaRogApp.Domain.BankAccounts.Entities;
using DonaRogApp.Domain.Campaigns.Entities;
using DonaRogApp.Domain.Donors.Entities;
using DonaRogApp.LetterTemplates;
using DonaRogApp.Enums.Donations;
using System;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace DonaRogApp.Domain.Donations.Entities
{
    /// <summary>
    /// Donation Aggregate Root
    /// 
    /// RESPONSIBILITY:
    /// - Store complete donation information
    /// - Manage donation verification workflow (Pending → Verified/Rejected)
    /// - Track project allocations
    /// - Associate with donor, campaign, bank account, thank-you template
    /// - Support multi-channel donations (postal, bank, PayPal, etc.)
    /// 
    /// Business logic is split across partial classes:
    /// - Donation.Factory.cs: Creation factory methods
    /// - Donation.Verification.cs: Verification workflow methods
    /// - Donation.Projects.cs: Project allocation methods
    /// </summary>
    public partial class Donation : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {
        // ======================================================================
        // MULTI-TENANCY
        // ======================================================================
        /// <summary>
        /// Tenant ID
        /// </summary>
        public Guid? TenantId { get; private set; }

        // ======================================================================
        // IDENTIFICATION
        // ======================================================================
        /// <summary>
        /// Donation reference (generated or from payment slip)
        /// Example: "DON-2026-00123", "BOLL-92041656"
        /// </summary>
        public string Reference { get; private set; }

        /// <summary>
        /// External ID from microservice (for donations from external flows)
        /// Used to prevent duplicates and track source
        /// </summary>
        public string? ExternalId { get; private set; }

        // ======================================================================
        // RELATIONSHIPS
        // ======================================================================
        /// <summary>
        /// Donor ID (required)
        /// </summary>
        public Guid DonorId { get; private set; }

        /// <summary>
        /// Navigation property: Donor
        /// </summary>
        public virtual Donor Donor { get; private set; }

        /// <summary>
        /// Campaign ID (optional - donation may or may not be from a campaign)
        /// </summary>
        public Guid? CampaignId { get; private set; }

        /// <summary>
        /// Navigation property: Campaign
        /// </summary>
        public virtual Campaign? Campaign { get; private set; }

        /// <summary>
        /// Bank account ID (optional - where donation was credited)
        /// </summary>
        public Guid? BankAccountId { get; private set; }

        /// <summary>
        /// Navigation property: BankAccount
        /// </summary>
        public virtual BankAccount? BankAccount { get; private set; }

        /// <summary>
        /// Thank-you template ID (optional - letter to send)
        /// </summary>
        public Guid? ThankYouTemplateId { get; private set; }

        /// <summary>
        /// Navigation property: ThankYouTemplate
        /// </summary>
        public virtual LetterTemplate? ThankYouTemplate { get; private set; }

        // ======================================================================
        // DONATION DETAILS
        // ======================================================================
        /// <summary>
        /// Donation channel/payment method
        /// </summary>
        public DonationChannel Channel { get; private set; }

        /// <summary>
        /// Donation status (Pending/Verified/Rejected)
        /// </summary>
        public DonationStatus Status { get; private set; }

        /// <summary>
        /// Total donation amount
        /// </summary>
        public decimal TotalAmount { get; private set; }

        /// <summary>
        /// Currency (default EUR)
        /// </summary>
        public string Currency { get; private set; }

        /// <summary>
        /// Donation date (when donor made the donation)
        /// </summary>
        public DateTime DonationDate { get; private set; }

        /// <summary>
        /// Credit date (when organization received the money, optional)
        /// </summary>
        public DateTime? CreditDate { get; private set; }

        // ======================================================================
        // REJECTION INFO (only for Rejected status)
        // ======================================================================
        /// <summary>
        /// Rejection reason (only if status = Rejected)
        /// </summary>
        public RejectionReason? RejectionReason { get; private set; }

        /// <summary>
        /// Rejection notes
        /// </summary>
        public string? RejectionNotes { get; private set; }

        /// <summary>
        /// When donation was rejected
        /// </summary>
        public DateTime? RejectedAt { get; private set; }

        /// <summary>
        /// Who rejected the donation
        /// </summary>
        public Guid? RejectedBy { get; private set; }

        // ======================================================================
        // VERIFICATION INFO (only for Verified status)
        // ======================================================================
        /// <summary>
        /// When donation was verified
        /// </summary>
        public DateTime? VerifiedAt { get; private set; }

        /// <summary>
        /// Who verified the donation
        /// </summary>
        public Guid? VerifiedBy { get; private set; }

        // ======================================================================
        // ADDITIONAL DATA
        // ======================================================================
        /// <summary>
        /// Public notes (visible to donor if needed)
        /// </summary>
        public string? Notes { get; private set; }

        /// <summary>
        /// Internal notes (for operators only)
        /// </summary>
        public string? InternalNotes { get; private set; }

        // ======================================================================
        // PROJECT ALLOCATIONS
        // ======================================================================
        /// <summary>
        /// Project allocations (zero, one, or multiple)
        /// </summary>
        public virtual ICollection<DonationProject> Projects { get; private set; }
        
        /// <summary>
        /// Documents attached to this donation (receipts, images, PDFs)
        /// </summary>
        public virtual ICollection<DonationDocument> Documents { get; private set; }

        // ======================================================================
        // CONSTRUCTOR
        // ======================================================================
        /// <summary>
        /// Private constructor for EF Core
        /// </summary>
        private Donation()
        {
            Reference = string.Empty;
            Currency = "EUR";
            Projects = new List<DonationProject>();
            Documents = new List<DonationDocument>();
        }

        /// <summary>
        /// Constructor for creating new donation
        /// </summary>
        internal Donation(
            Guid id,
            Guid? tenantId,
            string reference,
            Guid donorId,
            DonationChannel channel,
            DonationStatus status,
            decimal totalAmount,
            DateTime donationDate,
            DateTime? creditDate = null,
            Guid? campaignId = null,
            Guid? bankAccountId = null,
            string? externalId = null,
            string? notes = null,
            string? internalNotes = null)
            : base(id)
        {
            TenantId = tenantId;
            Reference = Check.NotNullOrWhiteSpace(reference, nameof(reference), maxLength: 50);
            DonorId = Check.NotNull(donorId, nameof(donorId));
            Channel = channel;
            Status = status;
            TotalAmount = Check.Positive(totalAmount, nameof(totalAmount));
            Currency = "EUR";
            DonationDate = donationDate;
            CreditDate = creditDate;
            CampaignId = campaignId;
            BankAccountId = bankAccountId;
            ExternalId = externalId;
            Notes = notes;
            InternalNotes = internalNotes;

            Projects = new List<DonationProject>();

            VerifyInvariants();
        }

        // ======================================================================
        // QUERY METHODS
        // ======================================================================
        /// <summary>
        /// Check if donation is pending verification
        /// </summary>
        public bool IsPending() => Status == DonationStatus.Pending;

        /// <summary>
        /// Check if donation is verified
        /// </summary>
        public bool IsVerified() => Status == DonationStatus.Verified;

        /// <summary>
        /// Check if donation is rejected
        /// </summary>
        public bool IsRejected() => Status == DonationStatus.Rejected;

        /// <summary>
        /// Check if donation has project allocations
        /// </summary>
        public bool HasProjectAllocations() => Projects.Any();

        /// <summary>
        /// Get total allocated amount across all projects
        /// </summary>
        public decimal GetTotalAllocatedAmount()
        {
            return Projects.Sum(p => p.AllocatedAmount);
        }

        /// <summary>
        /// Get unallocated amount
        /// </summary>
        public decimal GetUnallocatedAmount()
        {
            return TotalAmount - GetTotalAllocatedAmount();
        }

        /// <summary>
        /// Check if allocation is complete (all amount allocated)
        /// </summary>
        public bool IsFullyAllocated()
        {
            return GetTotalAllocatedAmount() == TotalAmount;
        }

        // ======================================================================
        // INVARIANTS
        // ======================================================================
        /// <summary>
        /// Verify business invariants
        /// </summary>
        internal void VerifyInvariants()
        {
            Check.NotNullOrWhiteSpace(Reference, nameof(Reference));
            Check.NotNull(DonorId, nameof(DonorId));
            Check.Positive(TotalAmount, nameof(TotalAmount));

            if (CreditDate.HasValue && CreditDate.Value < DonationDate)
            {
                throw new BusinessException("DonaRog:DonationCreditDateBeforeDonationDate")
                    .WithData("donationDate", DonationDate)
                    .WithData("creditDate", CreditDate.Value);
            }

            // Validate project allocations don't exceed total amount
            if (HasProjectAllocations())
            {
                var totalAllocated = GetTotalAllocatedAmount();
                if (totalAllocated > TotalAmount)
                {
                    throw new BusinessException("DonaRog:DonationAllocationExceedsTotalAmount")
                        .WithData("totalAmount", TotalAmount)
                        .WithData("totalAllocated", totalAllocated);
                }
            }
        }
    }
}
