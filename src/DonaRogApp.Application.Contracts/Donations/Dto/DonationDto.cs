using DonaRogApp.Enums.Donations;
using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace DonaRogApp.Application.Contracts.Donations.Dto
{
    public class DonationDto : FullAuditedEntityDto<Guid>
    {
        public string Reference { get; set; } = string.Empty;
        public string? ExternalId { get; set; }
        
        // Relationships
        public Guid DonorId { get; set; }
        public string DonorFullName { get; set; } = string.Empty;
        public Guid? CampaignId { get; set; }
        public string? CampaignName { get; set; }
        public Guid? BankAccountId { get; set; }
        public string? BankAccountName { get; set; }
        public Guid? ThankYouTemplateId { get; set; }
        public string? ThankYouTemplateName { get; set; }
        
        // Donation details
        public DonationChannel Channel { get; set; }
        public DonationStatus Status { get; set; }
        public decimal TotalAmount { get; set; }
        public string Currency { get; set; } = "EUR";
        public DateTime DonationDate { get; set; }
        public DateTime? CreditDate { get; set; }
        
        // Rejection info
        public RejectionReason? RejectionReason { get; set; }
        public string? RejectionNotes { get; set; }
        public DateTime? RejectedAt { get; set; }
        public Guid? RejectedBy { get; set; }
        
        // Verification info
        public DateTime? VerifiedAt { get; set; }
        public Guid? VerifiedBy { get; set; }
        
        // Notes
        public string? Notes { get; set; }
        public string? InternalNotes { get; set; }
        
        // Project allocations
        public List<DonationProjectDto> Projects { get; set; } = new List<DonationProjectDto>();
        
        // Computed properties
        public decimal TotalAllocatedAmount { get; set; }
        public decimal UnallocatedAmount { get; set; }
        public bool IsFullyAllocated { get; set; }
    }
}
