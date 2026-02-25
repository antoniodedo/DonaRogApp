using System;
using System.Collections.Generic;

namespace DonaRogApp.Application.Contracts.Communications.PrintBatches.Dto
{
    /// <summary>
    /// Preview of letters that would be included in batch
    /// </summary>
    public class PrintBatchPreviewDto
    {
        /// <summary>
        /// Total letters matching filters
        /// </summary>
        public int TotalLetters { get; set; }

        /// <summary>
        /// Total donation amount
        /// </summary>
        public decimal TotalDonationAmount { get; set; }

        /// <summary>
        /// Estimated PDF size in MB
        /// </summary>
        public double EstimatedPdfSizeMB { get; set; }

        /// <summary>
        /// Sample letters (first 10)
        /// </summary>
        public List<LetterPreviewItemDto> SampleLetters { get; set; } = new();

        /// <summary>
        /// Breakdown by project
        /// </summary>
        public Dictionary<string, int> ByProject { get; set; } = new();

        /// <summary>
        /// Breakdown by region
        /// </summary>
        public Dictionary<string, int> ByRegion { get; set; } = new();

        /// <summary>
        /// Applied filters summary
        /// </summary>
        public PrintBatchFilterDto AppliedFilters { get; set; } = null!;
    }

    /// <summary>
    /// Preview item for a single letter
    /// </summary>
    public class LetterPreviewItemDto
    {
        public Guid CommunicationId { get; set; }
        public Guid DonorId { get; set; }
        public string DonorName { get; set; } = null!;
        public Guid DonationId { get; set; }
        public string DonationReference { get; set; } = null!;
        public decimal DonationAmount { get; set; }
        public DateTime DonationDate { get; set; }
        public string? Region { get; set; }
        public string? ProjectName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
