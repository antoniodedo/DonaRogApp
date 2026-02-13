using DonaRogApp.Enums.Donations;
using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace DonaRogApp.Application.Contracts.Donations.Dto
{
    /// <summary>
    /// Lightweight DTO for donation lists
    /// </summary>
    public class DonationListDto : EntityDto<Guid>
    {
        public string Reference { get; set; } = string.Empty;
        public Guid DonorId { get; set; }
        public string DonorFullName { get; set; } = string.Empty;
        public DonationChannel Channel { get; set; }
        public DonationStatus Status { get; set; }
        public decimal TotalAmount { get; set; }
        public string Currency { get; set; } = "EUR";
        public DateTime DonationDate { get; set; }
        public DateTime? CreditDate { get; set; }
        public Guid? CampaignId { get; set; }
        public string? CampaignName { get; set; }
        public List<string> ProjectNames { get; set; } = new List<string>();
        public bool IsFullyAllocated { get; set; }
    }
}
