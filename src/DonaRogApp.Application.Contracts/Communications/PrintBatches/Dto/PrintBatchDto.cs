using DonaRogApp.Enums.Communications;
using System;
using Volo.Abp.Application.Dtos;

namespace DonaRogApp.Application.Contracts.Communications.PrintBatches.Dto
{
    /// <summary>
    /// Print Batch DTO
    /// </summary>
    public class PrintBatchDto : FullAuditedEntityDto<Guid>
    {
        /// <summary>
        /// Batch number (BATCH-2026-001)
        /// </summary>
        public string BatchNumber { get; set; } = null!;

        /// <summary>
        /// Optional batch name
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Current status
        /// </summary>
        public PrintBatchStatus Status { get; set; }

        /// <summary>
        /// Total letters in batch
        /// </summary>
        public int TotalLetters { get; set; }

        /// <summary>
        /// Total donation amount
        /// </summary>
        public decimal TotalDonationAmount { get; set; }

        /// <summary>
        /// PDF file size in bytes
        /// </summary>
        public long? PdfFileSizeBytes { get; set; }

        /// <summary>
        /// When batch was generated
        /// </summary>
        public DateTime? GeneratedAt { get; set; }

        /// <summary>
        /// User who generated the batch
        /// </summary>
        public Guid? GeneratedBy { get; set; }

        /// <summary>
        /// When batch was printed
        /// </summary>
        public DateTime? PrintedAt { get; set; }

        /// <summary>
        /// User who marked as printed
        /// </summary>
        public Guid? PrintedBy { get; set; }

        /// <summary>
        /// Applied filters (for display)
        /// </summary>
        public BatchFilterSummaryDto? FilterSummary { get; set; }

        /// <summary>
        /// Notes
        /// </summary>
        public string? Notes { get; set; }

        /// <summary>
        /// Can this batch be edited?
        /// </summary>
        public bool CanEdit => Status == PrintBatchStatus.Draft;

        /// <summary>
        /// Can generate PDF?
        /// </summary>
        public bool CanGeneratePdf => (Status == PrintBatchStatus.Draft || Status == PrintBatchStatus.Ready) && TotalLetters > 0;

        /// <summary>
        /// Can be cancelled?
        /// </summary>
        public bool CanCancel => Status != PrintBatchStatus.Printed && Status != PrintBatchStatus.Cancelled;
    }

    /// <summary>
    /// Summary of filters applied to batch
    /// </summary>
    public class BatchFilterSummaryDto
    {
        public decimal? MinAmount { get; set; }
        public decimal? MaxAmount { get; set; }
        public string? Region { get; set; }
        public string? ProjectNames { get; set; }
        public string? CampaignNames { get; set; }
        public int FilterCount { get; set; }
    }
}
