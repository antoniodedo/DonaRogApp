using DonaRogApp.Enums.Donations;
using System.ComponentModel.DataAnnotations;

namespace DonaRogApp.Application.Contracts.Donations.Dto
{
    public class RejectDonationDto
    {
        [Required]
        public RejectionReason Reason { get; set; }

        [StringLength(1000)]
        public string? Notes { get; set; }
    }
}
