using System.Collections.Generic;

namespace DonaRogApp.Application.Contracts.Campaigns.Dto
{
    /// <summary>
    /// Preview of donor extraction results
    /// </summary>
    public class DonorExtractionPreviewDto
    {
        public int TotalCount { get; set; }
        public List<DonorPreviewItemDto> Donors { get; set; } = new();
        public ExtractionStatisticsDto Statistics { get; set; } = new();
        public List<FilterBreakdownDto> FilterBreakdown { get; set; } = new();
    }

    /// <summary>
    /// Donor preview item
    /// </summary>
    public class DonorPreviewItemDto
    {
        public System.Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? City { get; set; }
        public string? Region { get; set; }
        public decimal LastDonationAmount { get; set; }
        public System.DateTime? LastDonationDate { get; set; }
        public int TotalDonations { get; set; }
        public List<string> Tags { get; set; } = new();
    }

    /// <summary>
    /// Filter breakdown showing how many donors match each filter
    /// </summary>
    public class FilterBreakdownDto
    {
        public string FilterName { get; set; } = string.Empty;
        public int MatchCount { get; set; }
        public decimal Percentage { get; set; }
    }

    /// <summary>
    /// Extraction statistics
    /// </summary>
    public class ExtractionStatisticsDto
    {
        public int TotalDonors { get; set; }
        public decimal TotalPotentialRevenue { get; set; }
        public decimal AverageDonationAmount { get; set; }
        public int ActiveDonors { get; set; }
        public int LapsedDonors { get; set; }
        public Dictionary<string, int> RegionDistribution { get; set; } = new();
        public Dictionary<string, int> SegmentDistribution { get; set; } = new();
    }
}
