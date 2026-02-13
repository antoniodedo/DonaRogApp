using DonaRogApp.Enums.Campaigns;
using System;
using Volo.Abp.Application.Dtos;

namespace DonaRogApp.Application.Contracts.Campaigns.Dto
{
    /// <summary>
    /// Campaign donor DTO (donor in campaign with tracking)
    /// </summary>
    public class CampaignDonorDto : EntityDto
    {
        public Guid CampaignId { get; set; }
        public Guid DonorId { get; set; }
        
        // Donor info
        public string DonorName { get; set; } = string.Empty;
        public string? DonorEmail { get; set; }
        public string? DonorPhone { get; set; }
        public string? DonorCity { get; set; }
        public string? DonorRegion { get; set; }
        
        // Tracking
        public string? TrackingCode { get; set; }
        public DateTime ExtractedAt { get; set; }
        public DateTime? DispatchedAt { get; set; }
        public DateTime? OpenedAt { get; set; }
        public DateTime? ClickedAt { get; set; }
        
        // Response
        public ResponseType ResponseType { get; set; }
        
        // Donation
        public decimal? DonationAmount { get; set; }
        public DateTime? DonationDate { get; set; }
        
        // Metadata
        public string? Notes { get; set; }
    }
}
