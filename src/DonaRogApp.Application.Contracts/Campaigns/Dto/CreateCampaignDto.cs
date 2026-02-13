using DonaRogApp.Enums.Campaigns;
using System;
using System.ComponentModel.DataAnnotations;

namespace DonaRogApp.Application.Contracts.Campaigns.Dto
{
    /// <summary>
    /// DTO for creating a new campaign
    /// </summary>
    public class CreateCampaignDto
    {
        [Required]
        [StringLength(256)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Range(2000, 2100)]
        public int Year { get; set; }

        [Required]
        [StringLength(64)]
        public string Code { get; set; } = string.Empty;

        [StringLength(2000)]
        public string? Description { get; set; }

        [Required]
        public CampaignType CampaignType { get; set; }

        [Required]
        public CampaignChannel Channel { get; set; }

        public Guid? RecurrenceId { get; set; }

        public DateTime? ExtractionScheduledDate { get; set; }
        public DateTime? DispatchScheduledDate { get; set; }
        public DateTime? RecurrenceDate { get; set; }

        [Range(0, double.MaxValue)]
        public decimal TotalCost { get; set; }

        public int TargetDonorCount { get; set; }
    }
}
