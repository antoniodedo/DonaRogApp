using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DonaRogApp.Application.Contracts.Donations.Dto
{
    public class VerifyDonationDto
    {
        [Required]
        public Guid DonorId { get; set; }

        public Guid? CampaignId { get; set; }

        public Guid? BankAccountId { get; set; }

        public Guid? ThankYouTemplateId { get; set; }

        public List<DonationProjectAllocationDto> ProjectAllocations { get; set; } = new List<DonationProjectAllocationDto>();

        [StringLength(1000)]
        public string? Notes { get; set; }

        [StringLength(1000)]
        public string? InternalNotes { get; set; }
    }
}
