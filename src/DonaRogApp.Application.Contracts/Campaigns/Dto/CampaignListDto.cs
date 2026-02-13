using DonaRogApp.Enums.Campaigns;
using System;
using Volo.Abp.Application.Dtos;

namespace DonaRogApp.Application.Contracts.Campaigns.Dto
{
    /// <summary>
    /// Campaign DTO for lists - simplified
    /// </summary>
    public class CampaignListDto : EntityDto<Guid>
    {
        public string Name { get; set; } = string.Empty;
        public int Year { get; set; }
        public string Code { get; set; } = string.Empty;
        public CampaignType CampaignType { get; set; }
        public CampaignChannel Channel { get; set; }
        public CampaignStatus Status { get; set; }
        public Guid? RecurrenceId { get; set; }
        public string? RecurrenceName { get; set; }
        public DateTime? DispatchDate { get; set; }
        public int ExtractedDonorCount { get; set; }
        public int ResponseCount { get; set; }
        public decimal ResponseRate { get; set; }
        public decimal TotalRaised { get; set; }
        public decimal ROI { get; set; }
    }
}
