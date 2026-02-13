using System;
using System.Collections.Generic;

namespace DonaRogApp.Application.Contracts.Campaigns.Dto
{
    /// <summary>
    /// Campaign statistics DTO
    /// </summary>
    public class CampaignStatisticsDto
    {
        public Guid CampaignId { get; set; }
        public string CampaignName { get; set; } = string.Empty;
        public int Year { get; set; }

        // Counts
        public int ExtractedDonorCount { get; set; }
        public int DispatchedCount { get; set; }
        public int ResponseCount { get; set; }
        public int OpenedCount { get; set; }
        public int ClickedCount { get; set; }
        public int DonationCount { get; set; }
        public int UnsubscribedCount { get; set; }
        public int BouncedCount { get; set; }

        // Rates
        public decimal ResponseRate { get; set; }
        public decimal OpenRate { get; set; }
        public decimal ClickRate { get; set; }
        public decimal ConversionRate { get; set; }

        // Financial
        public decimal TotalCost { get; set; }
        public decimal TotalRaised { get; set; }
        public decimal NetAmount { get; set; }
        public decimal ROI { get; set; }
        public decimal AverageDonation { get; set; }
        public decimal CostPerAcquisition { get; set; }
        public decimal CostPerClick { get; set; }

        // Breakdowns
        public Dictionary<string, int> ResponseTypeBreakdown { get; set; } = new();
        public Dictionary<string, decimal> RegionBreakdown { get; set; } = new();
        public Dictionary<string, decimal> SegmentBreakdown { get; set; } = new();

        // Timeline
        public List<DailyResponseDto> ResponseTimeline { get; set; } = new();
        public List<DailyDonationDto> DonationTimeline { get; set; } = new();
    }

    public class DailyResponseDto
    {
        public DateTime Date { get; set; }
        public int OpenedCount { get; set; }
        public int ClickedCount { get; set; }
        public int DonationCount { get; set; }
    }

    public class DailyDonationDto
    {
        public DateTime Date { get; set; }
        public int Count { get; set; }
        public decimal Amount { get; set; }
    }
}
