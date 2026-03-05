using DonaRogApp.Enums.Communications;
using DonaRogApp.Enums.Donations;
using DonaRogApp.Enums.Donors;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DonaRogApp.Application.Contracts.Communications.ThankYouRules.Dto
{
    /// <summary>
    /// DTO for creating/updating Thank You Rule
    /// </summary>
    public class CreateUpdateThankYouRuleDto
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = null!;

        [StringLength(1000)]
        public string? Description { get; set; }

        [Range(0, 1000)]
        public int Priority { get; set; } = 100;

        public bool IsActive { get; set; } = true;

        // Matching Conditions
        [Range(0, double.MaxValue)]
        public decimal? MinAmount { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? MaxAmount { get; set; }

        public bool? IsFirstDonation { get; set; }
        public Guid? ProjectId { get; set; }
        public Guid? CampaignId { get; set; }
        public DonorCategory? DonorCategory { get; set; }
        public SubjectType? SubjectType { get; set; }
        public Guid? RecurrenceId { get; set; }

        // Validity Period (for temporary rules/campaigns)
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidUntil { get; set; }

        // Actions
        public bool CreateThankYou { get; set; } = true;
        public PreferredThankYouChannel? SuggestedChannel { get; set; }
        public Guid? SuggestedTemplateId { get; set; }

        // Template Pool for LRU rotation
        public List<CreateUpdateTemplatePoolItemDto> TemplatePoolItems { get; set; } = new();

        [StringLength(1000)]
        public string? Notes { get; set; }
    }

    /// <summary>
    /// DTO for creating/updating template pool items
    /// </summary>
    public class CreateUpdateTemplatePoolItemDto
    {
        [Required]
        public Guid TemplateId { get; set; }

        [Range(1, 1000)]
        public int Priority { get; set; } = 1;

        public bool IsActive { get; set; } = true;
    }
}
