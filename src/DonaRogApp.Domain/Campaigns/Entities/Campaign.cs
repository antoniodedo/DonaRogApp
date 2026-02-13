using DonaRogApp.Domain.Recurrences.Entities;
using DonaRogApp.Enums.Campaigns;
using DonaRogApp.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace DonaRogApp.Domain.Campaigns.Entities
{
    /// <summary>
    /// Campaign Aggregate Root
    /// 
    /// RESPONSIBILITY:
    /// - Store campaign properties and configuration
    /// - Manage campaign lifecycle (draft → extracted → dispatched → completed)
    /// - Track campaign donors and their responses
    /// - Calculate statistics and ROI
    /// - Generate tracking codes for multi-channel campaigns
    /// 
    /// Business logic is split across partial classes:
    /// - Campaign.Factory.cs: Creation
    /// - Campaign.DonorExtraction.cs: Donor extraction logic
    /// - Campaign.Tracking.cs: Multi-channel tracking
    /// - Campaign.Statistics.cs: Statistical calculations and ROI
    /// - Campaign.Updates.cs: Update methods and workflow transitions
    /// </summary>
    public partial class Campaign : FullAuditedAggregateRoot<Guid>, IMultiTenant
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
        /// Campaign name (required)
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Campaign year
        /// </summary>
        public int Year { get; private set; }

        /// <summary>
        /// Campaign code/abbreviation (unique per year/tenant)
        /// Used in postal slips
        /// </summary>
        public string Code { get; private set; }

        /// <summary>
        /// Description
        /// </summary>
        public string? Description { get; private set; }

        // ======================================================================
        // CLASSIFICATION
        // ======================================================================
        /// <summary>
        /// Campaign type (Prospect/Archive)
        /// </summary>
        public CampaignType CampaignType { get; private set; }

        /// <summary>
        /// Communication channel
        /// </summary>
        public CampaignChannel Channel { get; private set; }

        /// <summary>
        /// Campaign status
        /// </summary>
        public CampaignStatus Status { get; private set; }

        // ======================================================================
        // EVENT ASSOCIATION
        // ======================================================================
        /// <summary>
        /// Associated recurrence ID (optional)
        /// </summary>
        public Guid? RecurrenceId { get; private set; }

        /// <summary>
        /// Navigation property to Recurrence
        /// </summary>
        public virtual Recurrence? Recurrence { get; private set; }

        // ======================================================================
        // DATES
        // ======================================================================
        /// <summary>
        /// Campaign creation date
        /// </summary>
        public DateTime CreatedDate { get; private set; }

        /// <summary>
        /// Scheduled extraction date
        /// </summary>
        public DateTime? ExtractionScheduledDate { get; private set; }

        /// <summary>
        /// Actual extraction date
        /// </summary>
        public DateTime? ExtractionDate { get; private set; }

        /// <summary>
        /// Scheduled dispatch date
        /// </summary>
        public DateTime? DispatchScheduledDate { get; private set; }

        /// <summary>
        /// Actual dispatch date
        /// </summary>
        public DateTime? DispatchDate { get; private set; }

        /// <summary>
        /// Recurrence date (if associated)
        /// </summary>
        public DateTime? RecurrenceDate { get; private set; }

        // ======================================================================
        // POSTAL TRACKING (for Channel = Postal)
        // ======================================================================
        /// <summary>
        /// Yearly sequence number for postal code 674
        /// </summary>
        public int? YearlySequenceNumber { get; private set; }

        /// <summary>
        /// Postal code 674 (YYYY-NNNNN)
        /// </summary>
        public PostalCode674? PostalCode { get; private set; }

        // ======================================================================
        // EMAIL TRACKING (for Channel = Email)
        // ======================================================================
        /// <summary>
        /// Mailchimp campaign ID
        /// </summary>
        public string? MailchimpCampaignId { get; private set; }

        /// <summary>
        /// Mailchimp list ID
        /// </summary>
        public string? MailchimpListId { get; private set; }

        // ======================================================================
        // SMS TRACKING (for Channel = SMS)
        // ======================================================================
        /// <summary>
        /// SMS provider identifier
        /// </summary>
        public string? SmsProviderId { get; private set; }

        // ======================================================================
        // FINANCIAL
        // ======================================================================
        /// <summary>
        /// Total campaign cost
        /// </summary>
        public decimal TotalCost { get; private set; }

        /// <summary>
        /// Total amount raised from this campaign
        /// </summary>
        public decimal TotalRaised { get; private set; }

        // ======================================================================
        // STATISTICS (Calculated)
        // ======================================================================
        /// <summary>
        /// Target donor count
        /// </summary>
        public int TargetDonorCount { get; private set; }

        /// <summary>
        /// Extracted donor count
        /// </summary>
        public int ExtractedDonorCount { get; private set; }

        /// <summary>
        /// Dispatched count (actually sent)
        /// </summary>
        public int DispatchedCount { get; private set; }

        /// <summary>
        /// Response count (opened/clicked/replied)
        /// </summary>
        public int ResponseCount { get; private set; }

        /// <summary>
        /// Response rate percentage
        /// </summary>
        public decimal ResponseRate { get; private set; }

        /// <summary>
        /// Number of donors who donated
        /// </summary>
        public int DonationCount { get; private set; }

        /// <summary>
        /// Average donation amount
        /// </summary>
        public decimal AverageDonation { get; private set; }

        /// <summary>
        /// Conversion rate percentage (donations / dispatched)
        /// </summary>
        public decimal ConversionRate { get; private set; }

        /// <summary>
        /// ROI percentage
        /// </summary>
        public decimal ROI { get; private set; }

        // ======================================================================
        // RELATIONSHIPS
        // ======================================================================
        /// <summary>
        /// Campaign donors (many-to-many with tracking)
        /// </summary>
        public virtual ICollection<CampaignDonor> CampaignDonors { get; private set; }

        // ======================================================================
        // CONSTRUCTOR
        // ======================================================================
        /// <summary>
        /// Private constructor for EF Core
        /// </summary>
        private Campaign()
        {
            CampaignDonors = new List<CampaignDonor>();
        }

        /// <summary>
        /// Constructor for creating new campaign
        /// </summary>
        internal Campaign(
            Guid id,
            Guid? tenantId,
            string name,
            int year,
            string code,
            CampaignType campaignType,
            CampaignChannel channel,
            Guid? recurrenceId = null)
            : base(id)
        {
            TenantId = tenantId;
            Name = Check.NotNullOrWhiteSpace(name, nameof(name), maxLength: 256);
            Year = year;
            Code = Check.NotNullOrWhiteSpace(code, nameof(code), maxLength: 64);
            CampaignType = campaignType;
            Channel = channel;
            RecurrenceId = recurrenceId;
            
            Status = CampaignStatus.Draft;
            CreatedDate = DateTime.UtcNow;
            
            CampaignDonors = new List<CampaignDonor>();
            
            VerifyInvariants();
        }

        // ======================================================================
        // QUERY METHODS
        // ======================================================================
        /// <summary>
        /// Get net amount (raised - cost)
        /// </summary>
        public decimal GetNetAmount()
        {
            return TotalRaised - TotalCost;
        }

        /// <summary>
        /// Calculate ROI percentage
        /// </summary>
        public decimal CalculateROI()
        {
            if (TotalCost <= 0)
                return 0;

            return ((TotalRaised - TotalCost) / TotalCost) * 100;
        }

        /// <summary>
        /// Calculate cost per acquisition
        /// </summary>
        public decimal GetCostPerAcquisition()
        {
            if (DonationCount <= 0)
                return 0;

            return TotalCost / DonationCount;
        }

        /// <summary>
        /// Check if campaign is postal
        /// </summary>
        public bool IsPostal()
        {
            return Channel == CampaignChannel.Postal || Channel == CampaignChannel.Mixed;
        }

        /// <summary>
        /// Check if campaign is email
        /// </summary>
        public bool IsEmail()
        {
            return Channel == CampaignChannel.Email || Channel == CampaignChannel.Mixed;
        }

        /// <summary>
        /// Check if campaign is SMS
        /// </summary>
        public bool IsSMS()
        {
            return Channel == CampaignChannel.SMS || Channel == CampaignChannel.Mixed;
        }

        /// <summary>
        /// Check if campaign is active
        /// </summary>
        public bool IsActive()
        {
            return Status == CampaignStatus.Dispatched || Status == CampaignStatus.InPreparation;
        }

        /// <summary>
        /// Check if campaign is completed
        /// </summary>
        public bool IsCompleted()
        {
            return Status == CampaignStatus.Completed;
        }

        // ======================================================================
        // INVARIANTS
        // ======================================================================
        /// <summary>
        /// Verify business invariants
        /// </summary>
        internal void VerifyInvariants()
        {
            Check.NotNullOrWhiteSpace(Name, nameof(Name));
            Check.NotNullOrWhiteSpace(Code, nameof(Code));

            if (Year < 2000 || Year > 2100)
            {
                throw new BusinessException("DonaRog:CampaignInvalidYear")
                    .WithData("year", Year);
            }

            if (ExtractionScheduledDate.HasValue && DispatchScheduledDate.HasValue &&
                ExtractionScheduledDate.Value > DispatchScheduledDate.Value)
            {
                throw new BusinessException("DonaRog:CampaignExtractionAfterDispatch")
                    .WithData("extractionDate", ExtractionScheduledDate.Value)
                    .WithData("dispatchDate", DispatchScheduledDate.Value);
            }
        }
    }
}
