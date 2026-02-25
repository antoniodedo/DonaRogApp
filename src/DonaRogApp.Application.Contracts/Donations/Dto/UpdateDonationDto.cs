using DonaRogApp.Enums.Donations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DonaRogApp.Application.Contracts.Donations.Dto
{
    public class UpdateDonationDto
    {
        // Core data (only for manually registered donations without ExternalId)
        public DonationChannel? Channel { get; set; }
        
        [Range(0.01, double.MaxValue)]
        public decimal? TotalAmount { get; set; }
        
        public DateTime? DonationDate { get; set; }
        
        public DateTime? CreditDate { get; set; }

        // Metadata (can be updated for all donations)
        public Guid? CampaignId { get; set; }

        public Guid? BankAccountId { get; set; }

        public Guid? ThankYouTemplateId { get; set; }

        [StringLength(1000)]
        public string? Notes { get; set; }

        [StringLength(1000)]
        public string? InternalNotes { get; set; }

        // Project allocations
        public List<DonationProjectAllocationDto> ProjectAllocations { get; set; } = new List<DonationProjectAllocationDto>();
    }
}
