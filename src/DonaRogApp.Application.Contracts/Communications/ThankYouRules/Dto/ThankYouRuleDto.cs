using DonaRogApp.Enums.Communications;
using DonaRogApp.Enums.Donations;
using DonaRogApp.Enums.Donors;
using System;
using Volo.Abp.Application.Dtos;

namespace DonaRogApp.Application.Contracts.Communications.ThankYouRules.Dto
{
    /// <summary>
    /// Thank You Rule DTO
    /// </summary>
    public class ThankYouRuleDto : FullAuditedEntityDto<Guid>
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public int Priority { get; set; }
        public bool IsActive { get; set; }

        // Matching Conditions
        public decimal? MinAmount { get; set; }
        public decimal? MaxAmount { get; set; }
        public bool? IsFirstDonation { get; set; }
        public Guid? ProjectId { get; set; }
        public string? ProjectName { get; set; }
        public Guid? CampaignId { get; set; }
        public string? CampaignName { get; set; }
        public DonorCategory? DonorCategory { get; set; }
        public SubjectType? SubjectType { get; set; }

        // Actions
        public bool CreateThankYou { get; set; }
        public PreferredThankYouChannel? SuggestedChannel { get; set; }
        public Guid? SuggestedTemplateId { get; set; }
        public string? SuggestedTemplateName { get; set; }

        public string? Notes { get; set; }
    }
}
