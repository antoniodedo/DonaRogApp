using System;
using System.ComponentModel.DataAnnotations;

namespace DonaRogApp.Application.Contracts.Communications.PrintBatches.Dto
{
    /// <summary>
    /// DTO for generating batch PDF
    /// </summary>
    public class GenerateBatchPdfDto
    {
        /// <summary>
        /// Batch ID to generate
        /// </summary>
        [Required]
        public Guid BatchId { get; set; }

        /// <summary>
        /// Run in background? (for large batches)
        /// </summary>
        public bool RunInBackground { get; set; } = false;
    }

    /// <summary>
    /// Result of PDF generation
    /// </summary>
    public class BatchPdfGenerationResultDto
    {
        /// <summary>
        /// Was generation successful?
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Is generation running in background?
        /// </summary>
        public bool IsBackgroundJob { get; set; }

        /// <summary>
        /// Background job ID (if running in background)
        /// </summary>
        public string? JobId { get; set; }

        /// <summary>
        /// PDF file size in bytes (if completed)
        /// </summary>
        public long? PdfFileSizeBytes { get; set; }

        /// <summary>
        /// Generation duration in milliseconds
        /// </summary>
        public long? DurationMs { get; set; }

        /// <summary>
        /// Error message (if failed)
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// Batch details after generation
        /// </summary>
        public PrintBatchDto? Batch { get; set; }
    }
}
