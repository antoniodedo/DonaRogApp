using DonaRogApp.Enums.Communications;
using DonaRogApp.Enums.Donations;
using DonaRogApp.Enums.Donors;
using System;
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

        // Actions
        public bool CreateThankYou { get; set; } = true;
        public PreferredThankYouChannel? SuggestedChannel { get; set; }
        public Guid? SuggestedTemplateId { get; set; }

        [StringLength(1000)]
        public string? Notes { get; set; }
    }
}
