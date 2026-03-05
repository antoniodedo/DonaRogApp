using DonaRogApp.Enums.Communications;
using DonaRogApp.Enums.Donors;
using System;
using System.Collections.Generic;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace DonaRogApp.Domain.Communications.Entities
{
    /// <summary>
    /// Thank You Rule Aggregate Root
    /// 
    /// RESPONSIBILITY:
    /// - Define automatic rules for thank you letter creation
    /// - Match donations against criteria (amount, donor type, project, etc.)
    /// - Suggest template and communication channel
    /// - Support priority-based evaluation (higher priority rules evaluated first)
    /// </summary>
    public class ThankYouRule : FullAuditedAggregateRoot<Guid>, IMultiTenant
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
        /// Rule name (e.g., "Large Donors Premium", "First Time Donors")
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Rule description
        /// </summary>
        public string? Description { get; private set; }

        /// <summary>
        /// Priority (higher = evaluated first, default = 0)
        /// </summary>
        public int Priority { get; private set; }

        /// <summary>
        /// Is this rule active?
        /// </summary>
        public bool IsActive { get; private set; }

        // ======================================================================
        // MATCH CONDITIONS
        // ======================================================================
        /// <summary>
        /// Minimum donation amount (null = no minimum)
        /// </summary>
        public decimal? MinAmount { get; private set; }

        /// <summary>
        /// Maximum donation amount (null = no maximum)
        /// </summary>
        public decimal? MaxAmount { get; private set; }

        /// <summary>
        /// Is this rule for first-time donors only? (null = any)
        /// </summary>
        public bool? IsFirstDonation { get; private set; }

        /// <summary>
        /// Specific project ID (null = any project)
        /// </summary>
        public Guid? ProjectId { get; private set; }

        /// <summary>
        /// Specific campaign ID (null = any campaign)
        /// </summary>
        public Guid? CampaignId { get; private set; }

        /// <summary>
        /// Donor category (null = any category)
        /// </summary>
        public DonorCategory? DonorCategory { get; private set; }

        /// <summary>
        /// Subject type (Individual/Organization, null = any)
        /// </summary>
        public SubjectType? SubjectType { get; private set; }

        /// <summary>
        /// Recurrence ID for seasonal communications (null = any time)
        /// </summary>
        public Guid? RecurrenceId { get; private set; }

        // ======================================================================
        // VALIDITY PERIOD (For temporary rules/campaigns)
        // ======================================================================
        /// <summary>
        /// Rule is valid from this date (null = always valid from start)
        /// </summary>
        public DateTime? ValidFrom { get; private set; }

        /// <summary>
        /// Rule is valid until this date (null = always valid until end)
        /// </summary>
        public DateTime? ValidUntil { get; private set; }

        // ======================================================================
        // ACTIONS (What to do when rule matches)
        // ======================================================================
        /// <summary>
        /// Should create thank you communication?
        /// </summary>
        public bool CreateThankYou { get; private set; }

        /// <summary>
        /// Suggested communication channel (null = use donor preference)
        /// </summary>
        public PreferredThankYouChannel? SuggestedChannel { get; private set; }

        /// <summary>
        /// Suggested template ID (null = use default template for category)
        /// DEPRECATED: Use TemplateAssociations instead
        /// </summary>
        public Guid? SuggestedTemplateId { get; private set; }

        // ======================================================================
        // NAVIGATION PROPERTIES
        // ======================================================================
        /// <summary>
        /// Template pool for LRU rotation
        /// </summary>
        public virtual ICollection<RuleTemplateAssociation> TemplateAssociations { get; set; } = new List<RuleTemplateAssociation>();

        /// <summary>
        /// Associated recurrence (if RecurrenceId is set)
        /// </summary>
        public virtual Domain.Recurrences.Entities.Recurrence? Recurrence { get; set; }

        // ======================================================================
        // CONSTRUCTOR
        // ======================================================================
        /// <summary>
        /// Private constructor for EF Core
        /// </summary>
        private ThankYouRule()
        {
            Name = string.Empty;
        }

        /// <summary>
        /// Constructor for creating new rule
        /// </summary>
        internal ThankYouRule(
            Guid id,
            Guid? tenantId,
            string name,
            int priority,
            bool createThankYou,
            string? description = null)
            : base(id)
        {
            TenantId = tenantId;
            Name = Check.NotNullOrWhiteSpace(name, nameof(name), maxLength: 200);
            Description = description;
            Priority = priority;
            CreateThankYou = createThankYou;
            IsActive = true;

            VerifyInvariants();
        }

        // ======================================================================
        // FACTORY METHOD
        // ======================================================================
        /// <summary>
        /// Creates a new thank you rule
        /// </summary>
        public static ThankYouRule Create(
            Guid id,
            Guid? tenantId,
            string name,
            int priority,
            bool createThankYou,
            string? description = null)
        {
            return new ThankYouRule(id, tenantId, name, priority, createThankYou, description);
        }

        // ======================================================================
        // BUSINESS METHODS - Configuration
        // ======================================================================
        /// <summary>
        /// Update rule conditions
        /// </summary>
        public void UpdateConditions(
            decimal? minAmount = null,
            decimal? maxAmount = null,
            bool? isFirstDonation = null,
            Guid? projectId = null,
            Guid? campaignId = null,
            DonorCategory? donorCategory = null,
            SubjectType? subjectType = null,
            Guid? recurrenceId = null)
        {
            MinAmount = minAmount;
            MaxAmount = maxAmount;
            IsFirstDonation = isFirstDonation;
            ProjectId = projectId;
            CampaignId = campaignId;
            DonorCategory = donorCategory;
            SubjectType = subjectType;
            RecurrenceId = recurrenceId;

            VerifyInvariants();
        }

        /// <summary>
        /// Update rule actions
        /// </summary>
        public void UpdateActions(
            bool createThankYou,
            PreferredThankYouChannel? suggestedChannel = null,
            Guid? suggestedTemplateId = null)
        {
            CreateThankYou = createThankYou;
            SuggestedChannel = suggestedChannel;
            SuggestedTemplateId = suggestedTemplateId;
        }

        /// <summary>
        /// Update rule details
        /// </summary>
        public void UpdateDetails(string name, string? description = null, int? priority = null)
        {
            Name = Check.NotNullOrWhiteSpace(name, nameof(name), maxLength: 200);
            Description = description;
            
            if (priority.HasValue)
            {
                Priority = priority.Value;
            }

            VerifyInvariants();
        }

        /// <summary>
        /// Activate rule
        /// </summary>
        public void Activate()
        {
            IsActive = true;
        }

        /// <summary>
        /// Deactivate rule
        /// </summary>
        public void Deactivate()
        {
            IsActive = false;
        }

        /// <summary>
        /// Update priority
        /// </summary>
        public void UpdatePriority(int priority)
        {
            Priority = priority;
        }

        // ======================================================================
        // BUSINESS METHODS - Matching
        // ======================================================================
        /// <summary>
        /// Check if this rule matches the given donation and donor
        /// </summary>
        public bool Matches(
            decimal donationAmount,
            bool isFirstDonation,
            DonorCategory donorCategory,
            SubjectType subjectType,
            Guid? projectId = null,
            Guid? campaignId = null)
        {
            if (!IsActive) return false;

            // Check amount range
            if (MinAmount.HasValue && donationAmount < MinAmount.Value) return false;
            if (MaxAmount.HasValue && donationAmount > MaxAmount.Value) return false;

            // Check first donation flag
            if (IsFirstDonation.HasValue && IsFirstDonation.Value != isFirstDonation) return false;

            // Check project (if rule is project-specific)
            if (ProjectId.HasValue && ProjectId.Value != projectId) return false;

            // Check campaign (if rule is campaign-specific)
            if (CampaignId.HasValue && CampaignId.Value != campaignId) return false;

            // Check donor category
            if (DonorCategory.HasValue && DonorCategory.Value != donorCategory) return false;

            // Check subject type
            if (SubjectType.HasValue && SubjectType.Value != subjectType) return false;

            return true;
        }

        /// <summary>
        /// Calculate match score for ranking (higher = more specific rule)
        /// Used to determine which rule to apply when multiple rules match
        /// </summary>
        public int GetMatchScore()
        {
            int score = Priority * 100; // Base score from priority

            // More specific rules get higher scores
            if (MinAmount.HasValue || MaxAmount.HasValue) score += 10;
            if (ProjectId.HasValue) score += 15;
            if (CampaignId.HasValue) score += 15;
            if (IsFirstDonation.HasValue) score += 8;
            if (DonorCategory.HasValue) score += 12;
            if (SubjectType.HasValue) score += 5;

            return score;
        }

        // ======================================================================
        // VALIDITY AND TEMPORAL METHODS
        // ======================================================================
        /// <summary>
        /// Set validity period for temporary rules
        /// </summary>
        public void SetValidityPeriod(DateTime? validFrom, DateTime? validUntil)
        {
            if (validFrom.HasValue && validUntil.HasValue && validFrom.Value > validUntil.Value)
            {
                throw new BusinessException("DonaRog:InvalidValidityPeriod")
                    .WithData("validFrom", validFrom.Value)
                    .WithData("validUntil", validUntil.Value);
            }

            ValidFrom = validFrom;
            ValidUntil = validUntil;
        }

        /// <summary>
        /// Check if rule is valid on a specific date
        /// </summary>
        public bool IsValidOnDate(DateTime date)
        {
            if (ValidFrom.HasValue && date < ValidFrom.Value) return false;
            if (ValidUntil.HasValue && date > ValidUntil.Value) return false;
            return true;
        }

        /// <summary>
        /// Check if this is a temporary rule (has validity dates)
        /// </summary>
        public bool IsTemporaryRule()
        {
            return ValidFrom.HasValue || ValidUntil.HasValue;
        }

        /// <summary>
        /// Clone this rule with all its properties
        /// </summary>
        public ThankYouRule Clone(string newName)
        {
            var cloned = new ThankYouRule(
                Guid.NewGuid(),
                TenantId,
                newName,
                Priority,
                CreateThankYou,
                Description);

            cloned.MinAmount = MinAmount;
            cloned.MaxAmount = MaxAmount;
            cloned.IsFirstDonation = IsFirstDonation;
            cloned.ProjectId = ProjectId;
            cloned.CampaignId = CampaignId;
            cloned.DonorCategory = DonorCategory;
            cloned.SubjectType = SubjectType;
            cloned.RecurrenceId = RecurrenceId;
            cloned.ValidFrom = ValidFrom;
            cloned.ValidUntil = ValidUntil;
            cloned.SuggestedChannel = SuggestedChannel;
            cloned.SuggestedTemplateId = SuggestedTemplateId;
            cloned.IsActive = false;

            return cloned;
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

            if (MinAmount.HasValue && MinAmount.Value < 0)
            {
                throw new BusinessException("DonaRog:RuleNegativeMinAmount")
                    .WithData("minAmount", MinAmount.Value);
            }

            if (MaxAmount.HasValue && MaxAmount.Value < 0)
            {
                throw new BusinessException("DonaRog:RuleNegativeMaxAmount")
                    .WithData("maxAmount", MaxAmount.Value);
            }

            if (MinAmount.HasValue && MaxAmount.HasValue && MinAmount.Value > MaxAmount.Value)
            {
                throw new BusinessException("DonaRog:RuleMinAmountGreaterThanMax")
                    .WithData("minAmount", MinAmount.Value)
                    .WithData("maxAmount", MaxAmount.Value);
            }

            if (ValidFrom.HasValue && ValidUntil.HasValue && ValidFrom.Value > ValidUntil.Value)
            {
                throw new BusinessException("DonaRog:InvalidValidityPeriod")
                    .WithData("validFrom", ValidFrom.Value)
                    .WithData("validUntil", ValidUntil.Value);
            }
        }
    }
}
