using System;
using Volo.Abp.Application.Dtos;

namespace DonaRogApp.Application.Contracts.Segmentation.Dto
{
    /// <summary>
    /// Segmentation Rule DTO
    /// </summary>
    public class SegmentationRuleDto : FullAuditedEntityDto<Guid>
    {
        /// <summary>
        /// Rule name
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Rule description
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Is rule active?
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Priority (lower = higher priority)
        /// </summary>
        public int Priority { get; set; }

        // TARGET SEGMENT
        public Guid SegmentId { get; set; }
        public string? SegmentName { get; set; }
        public string? SegmentCode { get; set; }

        // RFM SCORE CONDITIONS
        public int? MinRecencyScore { get; set; }
        public int? MaxRecencyScore { get; set; }
        public int? MinFrequencyScore { get; set; }
        public int? MaxFrequencyScore { get; set; }
        public int? MinMonetaryScore { get; set; }
        public int? MaxMonetaryScore { get; set; }

        // RAW VALUE CONDITIONS
        public decimal? MinTotalDonated { get; set; }
        public decimal? MaxTotalDonated { get; set; }
        public int? MinDonationCount { get; set; }
        public int? MaxDonationCount { get; set; }
        public int? MinDaysSinceLastDonation { get; set; }
        public int? MaxDaysSinceLastDonation { get; set; }

        // DATE CONDITIONS
        public DateTime? FirstDonationAfter { get; set; }
        public DateTime? FirstDonationBefore { get; set; }
        public DateTime? LastDonationAfter { get; set; }
        public DateTime? LastDonationBefore { get; set; }

        /// <summary>
        /// Summary of conditions for display
        /// </summary>
        public string? ConditionsSummary { get; set; }
    }
}
