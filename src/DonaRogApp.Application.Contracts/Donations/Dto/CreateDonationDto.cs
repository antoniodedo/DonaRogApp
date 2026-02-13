using DonaRogApp.Enums.Donations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DonaRogApp.Application.Contracts.Donations.Dto
{
    public class CreateDonationDto
    {
        [Required]
        public Guid DonorId { get; set; }

        [Required]
        public DonationChannel Channel { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal TotalAmount { get; set; }

        [Required]
        public DateTime DonationDate { get; set; }

        public DateTime? CreditDate { get; set; }

        public Guid? CampaignId { get; set; }

        public Guid? BankAccountId { get; set; }

        public Guid? ThankYouTemplateId { get; set; }

        public List<DonationProjectAllocationDto> ProjectAllocations { get; set; } = new List<DonationProjectAllocationDto>();

        [StringLength(1000)]
        public string? Notes { get; set; }

        [StringLength(1000)]
        public string? InternalNotes { get; set; }

        /// <summary>
        /// External ID from external systems (PayPal, bank, etc.)
        /// </summary>
        [StringLength(200)]
        public string? ExternalId { get; set; }

        /// <summary>
        /// Optional: Set to Pending for donations that need verification, or leave null to create as Verified
        /// </summary>
        public DonationStatus? Status { get; set; }
    }

    public class DonationProjectAllocationDto
    {
        [Required]
        public Guid ProjectId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal AllocatedAmount { get; set; }
    }
}
