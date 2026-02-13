using DonaRogApp.Enums.Campaigns;
using System;
using Volo.Abp.Application.Dtos;

namespace DonaRogApp.Application.Contracts.Campaigns.Dto
{
    /// <summary>
    /// Campaign DTO - full details
    /// </summary>
    public class CampaignDto : FullAuditedEntityDto<Guid>
    {
        public string Name { get; set; } = string.Empty;
        public int Year { get; set; }
        public string Code { get; set; } = string.Empty;
        public string? Description { get; set; }
        
        // Classification
        public CampaignType CampaignType { get; set; }
        public CampaignChannel Channel { get; set; }
        public CampaignStatus Status { get; set; }
        
        // Recurrence association
        public Guid? RecurrenceId { get; set; }
        public string? RecurrenceName { get; set; }
        public DateTime? RecurrenceDate { get; set; }
        
        // Dates
        public DateTime CreatedDate { get; set; }
        public DateTime? ExtractionScheduledDate { get; set; }
        public DateTime? ExtractionDate { get; set; }
        public DateTime? DispatchScheduledDate { get; set; }
        public DateTime? DispatchDate { get; set; }
        
        // Postal tracking
        public int? YearlySequenceNumber { get; set; }
        public string? PostalCode674 { get; set; } // Formatted: YYYY-NNNNN
        
        // Email tracking
        public string? MailchimpCampaignId { get; set; }
        public string? MailchimpListId { get; set; }
        
        // SMS tracking
        public string? SmsProviderId { get; set; }
        
        // Financial
        public decimal TotalCost { get; set; }
        public decimal TotalRaised { get; set; }
        
        // Statistics
        public int TargetDonorCount { get; set; }
        public int ExtractedDonorCount { get; set; }
        public int DispatchedCount { get; set; }
        public int ResponseCount { get; set; }
        public decimal ResponseRate { get; set; }
        public int DonationCount { get; set; }
        public decimal AverageDonation { get; set; }
        public decimal ConversionRate { get; set; }
        public decimal ROI { get; set; }
        
        // Calculated properties
        public decimal NetAmount => TotalRaised - TotalCost;
        public decimal CostPerAcquisition => DonationCount > 0 ? TotalCost / DonationCount : 0;
    }
}
