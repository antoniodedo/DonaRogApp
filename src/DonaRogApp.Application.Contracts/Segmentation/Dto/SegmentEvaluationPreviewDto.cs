using System.Collections.Generic;

namespace DonaRogApp.Application.Contracts.Segmentation.Dto
{
    /// <summary>
    /// Preview of donors matching a segmentation rule
    /// </summary>
    public class SegmentEvaluationPreviewDto
    {
        public int TotalMatchingDonors { get; set; }
        public List<DonorPreviewDto> PreviewDonors { get; set; } = new();
    }

    /// <summary>
    /// Simplified donor info for preview
    /// </summary>
    public class DonorPreviewDto
    {
        public System.Guid Id { get; set; }
        public string DonorCode { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        
        // RFM Scores
        public int RecencyScore { get; set; }
        public int FrequencyScore { get; set; }
        public int MonetaryScore { get; set; }
        
        // Stats
        public decimal TotalDonated { get; set; }
        public int DonationCount { get; set; }
        public System.DateTime? LastDonationDate { get; set; }
        public int? DaysSinceLastDonation { get; set; }
    }
}
