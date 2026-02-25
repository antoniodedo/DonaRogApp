using DonaRogApp.Enums.Communications;
using System;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace DonaRogApp.Domain.Communications.Entities
{
    /// <summary>
    /// Print Batch Aggregate Root
    /// 
    /// RESPONSIBILITY:
    /// - Group multiple thank you letters for batch printing
    /// - Track batch generation workflow (Draft → Generated → Printed)
    /// - Store applied filters for audit trail
    /// - Generate unified PDF for physical printing
    /// - Track printing completion and statistics
    /// </summary>
    public class PrintBatch : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {
        // ======================================================================
        // MULTI-TENANCY
        // ======================================================================
        /// <summary>
        /// Tenant ID
        /// </summary>
        public Guid? TenantId { get; private set; }

        // ======================================================================
        // IDENTIFICATION
        // ======================================================================
        /// <summary>
        /// Batch number (auto-generated: BATCH-2026-001)
        /// </summary>
        public string BatchNumber { get; private set; }

        /// <summary>
        /// Optional name/description for the batch
        /// </summary>
        public string? Name { get; private set; }

        // ======================================================================
        // STATUS & WORKFLOW
        // ======================================================================
        /// <summary>
        /// Current batch status
        /// </summary>
        public PrintBatchStatus Status { get; private set; }

        /// <summary>
        /// When batch was marked as ready for generation
        /// </summary>
        public DateTime? ReadyAt { get; private set; }

        /// <summary>
        /// When PDF generation started
        /// </summary>
        public DateTime? GenerationStartedAt { get; private set; }

        /// <summary>
        /// When PDF was successfully generated
        /// </summary>
        public DateTime? GeneratedAt { get; private set; }

        /// <summary>
        /// User who generated the batch
        /// </summary>
        public Guid? GeneratedBy { get; private set; }

        /// <summary>
        /// When PDF was downloaded by operator
        /// </summary>
        public DateTime? DownloadedAt { get; private set; }

        /// <summary>
        /// User who downloaded the batch
        /// </summary>
        public Guid? DownloadedBy { get; private set; }

        /// <summary>
        /// When batch was physically printed
        /// </summary>
        public DateTime? PrintedAt { get; private set; }

        /// <summary>
        /// User who marked batch as printed
        /// </summary>
        public Guid? PrintedBy { get; private set; }

        /// <summary>
        /// When batch was cancelled
        /// </summary>
        public DateTime? CancelledAt { get; private set; }

        /// <summary>
        /// Reason for cancellation
        /// </summary>
        public string? CancellationReason { get; private set; }

        // ======================================================================
        // FILE STORAGE
        // ======================================================================
        /// <summary>
        /// Path to generated PDF file in storage
        /// </summary>
        public string? PdfFilePath { get; private set; }

        /// <summary>
        /// PDF file size in bytes
        /// </summary>
        public long? PdfFileSizeBytes { get; private set; }

        // ======================================================================
        // STATISTICS
        // ======================================================================
        /// <summary>
        /// Total number of letters in this batch
        /// </summary>
        public int TotalLetters { get; private set; }

        /// <summary>
        /// Total donation amount for all letters in batch
        /// </summary>
        public decimal TotalDonationAmount { get; private set; }

        // ======================================================================
        // FILTERS (Audit Trail)
        // ======================================================================
        /// <summary>
        /// Serialized JSON of filters applied when creating batch
        /// For audit and recreate capability
        /// </summary>
        public string? FilterJson { get; private set; }

        /// <summary>
        /// Minimum donation amount filter (extracted for quick queries)
        /// </summary>
        public decimal? MinAmount { get; private set; }

        /// <summary>
        /// Maximum donation amount filter
        /// </summary>
        public decimal? MaxAmount { get; private set; }

        /// <summary>
        /// Region filter (from donor address)
        /// </summary>
        public string? Region { get; private set; }

        /// <summary>
        /// Comma-separated project IDs filter
        /// </summary>
        public string? ProjectIds { get; private set; }

        /// <summary>
        /// Comma-separated campaign IDs filter
        /// </summary>
        public string? CampaignIds { get; private set; }

        // ======================================================================
        // RELATIONSHIPS
        // ======================================================================
        /// <summary>
        /// Communications (letters) included in this batch
        /// </summary>
        public virtual ICollection<Donors.Entities.Communication> Communications { get; private set; }

        // ======================================================================
        // NOTES
        // ======================================================================
        /// <summary>
        /// Internal notes about the batch
        /// </summary>
        public string? Notes { get; private set; }

        // ======================================================================
        // CONSTRUCTOR
        // ======================================================================
        /// <summary>
        /// Private constructor for EF Core
        /// </summary>
        private PrintBatch()
        {
            BatchNumber = string.Empty;
            Communications = new List<Donors.Entities.Communication>();
        }

        /// <summary>
        /// Constructor for creating new print batch
        /// </summary>
        internal PrintBatch(
            Guid id,
            Guid? tenantId,
            string batchNumber,
            string? name = null,
            string? notes = null)
            : base(id)
        {
            TenantId = tenantId;
            BatchNumber = Check.NotNullOrWhiteSpace(batchNumber, nameof(batchNumber), maxLength: 50);
            Name = name;
            Notes = notes;
            Status = PrintBatchStatus.Draft;
            TotalLetters = 0;
            TotalDonationAmount = 0;
            Communications = new List<Donors.Entities.Communication>();

            VerifyInvariants();
        }

        // ======================================================================
        // FACTORY METHOD
        // ======================================================================
        /// <summary>
        /// Creates a new print batch
        /// </summary>
        public static PrintBatch Create(
            Guid id,
            Guid? tenantId,
            string batchNumber,
            string? name = null,
            string? notes = null)
        {
            return new PrintBatch(id, tenantId, batchNumber, name, notes);
        }

        /// <summary>
        /// Generate batch number (format: BATCH-YYYY-NNN)
        /// </summary>
        public static string GenerateBatchNumber(int sequenceNumber)
        {
            var year = DateTime.UtcNow.Year;
            return $"BATCH-{year}-{sequenceNumber:D3}";
        }

        // ======================================================================
        // BUSINESS METHODS - Workflow
        // ======================================================================
        /// <summary>
        /// Mark batch as ready for PDF generation
        /// </summary>
        public void MarkAsReady()
        {
            if (Status != PrintBatchStatus.Draft)
            {
                throw new BusinessException("DonaRog:BatchCanOnlyMarkReadyFromDraft")
                    .WithData("batchId", Id)
                    .WithData("currentStatus", Status);
            }

            if (TotalLetters == 0)
            {
                throw new BusinessException("DonaRog:BatchCannotBeReadyWithNoLetters")
                    .WithData("batchId", Id);
            }

            Status = PrintBatchStatus.Ready;
            ReadyAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Mark PDF generation started
        /// </summary>
        public void MarkAsGenerating()
        {
            if (Status != PrintBatchStatus.Ready && Status != PrintBatchStatus.Draft)
            {
                throw new BusinessException("DonaRog:BatchCanOnlyStartGeneratingFromReady")
                    .WithData("batchId", Id)
                    .WithData("currentStatus", Status);
            }

            Status = PrintBatchStatus.Generating;
            GenerationStartedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Mark PDF generated successfully
        /// </summary>
        public void MarkAsGenerated(string pdfFilePath, long pdfFileSizeBytes, Guid generatedBy)
        {
            Check.NotNullOrWhiteSpace(pdfFilePath, nameof(pdfFilePath));

            if (Status != PrintBatchStatus.Generating && Status != PrintBatchStatus.Ready)
            {
                throw new BusinessException("DonaRog:BatchCanOnlyMarkGeneratedFromGenerating")
                    .WithData("batchId", Id)
                    .WithData("currentStatus", Status);
            }

            Status = PrintBatchStatus.Generated;
            PdfFilePath = pdfFilePath;
            PdfFileSizeBytes = pdfFileSizeBytes;
            GeneratedAt = DateTime.UtcNow;
            GeneratedBy = generatedBy;
        }

        /// <summary>
        /// Mark PDF downloaded by operator
        /// </summary>
        public void MarkAsDownloaded(Guid downloadedBy)
        {
            if (Status != PrintBatchStatus.Generated && Status != PrintBatchStatus.Downloaded)
            {
                throw new BusinessException("DonaRog:BatchCanOnlyMarkDownloadedFromGenerated")
                    .WithData("batchId", Id)
                    .WithData("currentStatus", Status);
            }

            Status = PrintBatchStatus.Downloaded;
            DownloadedAt = DateTime.UtcNow;
            DownloadedBy = downloadedBy;
        }

        /// <summary>
        /// Mark batch as physically printed
        /// </summary>
        public void MarkAsPrinted(Guid printedBy)
        {
            if (Status != PrintBatchStatus.Downloaded && Status != PrintBatchStatus.Generated)
            {
                throw new BusinessException("DonaRog:BatchCanOnlyMarkPrintedFromDownloaded")
                    .WithData("batchId", Id)
                    .WithData("currentStatus", Status);
            }

            Status = PrintBatchStatus.Printed;
            PrintedAt = DateTime.UtcNow;
            PrintedBy = printedBy;
        }

        /// <summary>
        /// Cancel batch before printing
        /// </summary>
        public void Cancel(string? reason = null)
        {
            if (Status == PrintBatchStatus.Printed)
            {
                throw new BusinessException("DonaRog:CannotCancelPrintedBatch")
                    .WithData("batchId", Id);
            }

            Status = PrintBatchStatus.Cancelled;
            CancelledAt = DateTime.UtcNow;
            CancellationReason = reason;
        }

        // ======================================================================
        // BUSINESS METHODS - Statistics & Filters
        // ======================================================================
        /// <summary>
        /// Update batch statistics (called when communications are added/removed)
        /// </summary>
        public void UpdateStatistics(int totalLetters, decimal totalDonationAmount)
        {
            TotalLetters = totalLetters;
            TotalDonationAmount = totalDonationAmount;
        }

        /// <summary>
        /// Store filters used to create this batch (for audit)
        /// </summary>
        public void SetFilters(
            string filterJson,
            decimal? minAmount = null,
            decimal? maxAmount = null,
            string? region = null,
            string? projectIds = null,
            string? campaignIds = null)
        {
            FilterJson = filterJson;
            MinAmount = minAmount;
            MaxAmount = maxAmount;
            Region = region;
            ProjectIds = projectIds;
            CampaignIds = campaignIds;
        }

        /// <summary>
        /// Update batch name and notes
        /// </summary>
        public void UpdateDetails(string? name = null, string? notes = null)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                Name = name;
            }

            if (!string.IsNullOrWhiteSpace(notes))
            {
                Notes = notes;
            }
        }

        // ======================================================================
        // QUERY METHODS
        // ======================================================================
        /// <summary>
        /// Check if batch is in draft state
        /// </summary>
        public bool IsDraft() => Status == PrintBatchStatus.Draft;

        /// <summary>
        /// Check if batch is ready for generation
        /// </summary>
        public bool IsReady() => Status == PrintBatchStatus.Ready;

        /// <summary>
        /// Check if batch is generating
        /// </summary>
        public bool IsGenerating() => Status == PrintBatchStatus.Generating;

        /// <summary>
        /// Check if batch PDF is generated
        /// </summary>
        public bool IsGenerated() => Status == PrintBatchStatus.Generated;

        /// <summary>
        /// Check if batch is printed
        /// </summary>
        public bool IsPrinted() => Status == PrintBatchStatus.Printed;

        /// <summary>
        /// Check if batch is cancelled
        /// </summary>
        public bool IsCancelled() => Status == PrintBatchStatus.Cancelled;

        /// <summary>
        /// Check if batch can be edited (communications added/removed)
        /// </summary>
        public bool CanBeEdited() => Status == PrintBatchStatus.Draft;

        /// <summary>
        /// Check if batch can be cancelled
        /// </summary>
        public bool CanBeCancelled() => Status != PrintBatchStatus.Printed && Status != PrintBatchStatus.Cancelled;

        /// <summary>
        /// Check if PDF can be generated
        /// </summary>
        public bool CanGeneratePdf() => (Status == PrintBatchStatus.Draft || Status == PrintBatchStatus.Ready) && TotalLetters > 0;

        // ======================================================================
        // INVARIANTS
        // ======================================================================
        /// <summary>
        /// Verify business invariants
        /// </summary>
        internal void VerifyInvariants()
        {
            Check.NotNullOrWhiteSpace(BatchNumber, nameof(BatchNumber));

            if (TotalLetters < 0)
            {
                throw new BusinessException("DonaRog:BatchNegativeTotalLetters")
                    .WithData("batchId", Id)
                    .WithData("totalLetters", TotalLetters);
            }

            if (TotalDonationAmount < 0)
            {
                throw new BusinessException("DonaRog:BatchNegativeTotalAmount")
                    .WithData("batchId", Id)
                    .WithData("totalAmount", TotalDonationAmount);
            }
        }
    }
}
