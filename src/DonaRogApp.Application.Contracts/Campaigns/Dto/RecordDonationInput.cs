using System;
using System.ComponentModel.DataAnnotations;

namespace DonaRogApp.Application.Contracts.Campaigns.Dto
{
    /// <summary>
    /// Input for recording a donation in a campaign
    /// </summary>
    public class RecordDonationInput
    {
        [Required]
        public Guid DonorId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }

        public DateTime? DonationDate { get; set; }

        [StringLength(1000)]
        public string? Notes { get; set; }
    }
}
