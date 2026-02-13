using DonaRogApp.Enums.Campaigns;
using System;
using System.ComponentModel.DataAnnotations;

namespace DonaRogApp.Application.Contracts.Campaigns.Dto
{
    /// <summary>
    /// DTO for updating an existing campaign
    /// </summary>
    public class UpdateCampaignDto
    {
        [Required]
        [StringLength(256)]
        public string Name { get; set; } = string.Empty;

        [StringLength(2000)]
        public string? Description { get; set; }

        public Guid? RecurrenceId { get; set; }

        public DateTime? ExtractionScheduledDate { get; set; }
        public DateTime? DispatchScheduledDate { get; set; }
        public DateTime? RecurrenceDate { get; set; }

        [Range(0, double.MaxValue)]
        public decimal TotalCost { get; set; }

        public int TargetDonorCount { get; set; }
    }
}
