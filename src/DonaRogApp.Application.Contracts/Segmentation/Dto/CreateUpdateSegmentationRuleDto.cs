using System;
using System.ComponentModel.DataAnnotations;

namespace DonaRogApp.Application.Contracts.Segmentation.Dto
{
    /// <summary>
    /// DTO for creating/updating a segmentation rule
    /// </summary>
    public class CreateUpdateSegmentationRuleDto
    {
        [Required]
        [StringLength(128)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        [Range(1, int.MaxValue)]
        public int Priority { get; set; } = 1;

        // TARGET SEGMENT
        [Required]
        public Guid SegmentId { get; set; }

        // RFM SCORE CONDITIONS (1-5)
        [Range(1, 5)]
        public int? MinRecencyScore { get; set; }

        [Range(1, 5)]
        public int? MaxRecencyScore { get; set; }

        [Range(1, 5)]
        public int? MinFrequencyScore { get; set; }

        [Range(1, 5)]
        public int? MaxFrequencyScore { get; set; }

        [Range(1, 5)]
        public int? MinMonetaryScore { get; set; }

        [Range(1, 5)]
        public int? MaxMonetaryScore { get; set; }

        // RAW VALUE CONDITIONS
        [Range(0, double.MaxValue)]
        public decimal? MinTotalDonated { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? MaxTotalDonated { get; set; }

        [Range(0, int.MaxValue)]
        public int? MinDonationCount { get; set; }

        [Range(0, int.MaxValue)]
        public int? MaxDonationCount { get; set; }

        [Range(0, int.MaxValue)]
        public int? MinDaysSinceLastDonation { get; set; }

        [Range(0, int.MaxValue)]
        public int? MaxDaysSinceLastDonation { get; set; }

        // DATE CONDITIONS
        public DateTime? FirstDonationAfter { get; set; }
        public DateTime? FirstDonationBefore { get; set; }
        public DateTime? LastDonationAfter { get; set; }
        public DateTime? LastDonationBefore { get; set; }
    }
}
