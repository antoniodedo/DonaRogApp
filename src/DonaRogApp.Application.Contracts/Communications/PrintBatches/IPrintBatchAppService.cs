using DonaRogApp.Application.Contracts.Communications.PrintBatches.Dto;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace DonaRogApp.Application.Contracts.Communications.PrintBatches
{
    /// <summary>
    /// Application service for Print Batch management
    /// </summary>
    public interface IPrintBatchAppService : IApplicationService
    {
        // ======================================================================
        // QUERY
        // ======================================================================
        
        /// <summary>
        /// Get list of print batches
        /// </summary>
        Task<PagedResultDto<PrintBatchDto>> GetListAsync(GetPrintBatchesInput input);
        
        /// <summary>
        /// Get single print batch
        /// </summary>
        Task<PrintBatchDto> GetAsync(Guid id);
        
        // ======================================================================
        // PREVIEW
        // ======================================================================
        
        /// <summary>
        /// Preview letters that would be included in batch based on filters
        /// </summary>
        Task<PrintBatchPreviewDto> PreviewBatchAsync(PrintBatchFilterDto filters);
        
        // ======================================================================
        // CREATE & MANAGE
        // ======================================================================
        
        /// <summary>
        /// Create new print batch
        /// </summary>
        Task<PrintBatchDto> CreateAsync(CreatePrintBatchDto input);
        
        /// <summary>
        /// Update batch details (name, notes)
        /// Only works for Draft status
        /// </summary>
        Task<PrintBatchDto> UpdateAsync(Guid id, UpdatePrintBatchDto input);
        
        /// <summary>
        /// Delete batch (only if Draft)
        /// </summary>
        Task DeleteAsync(Guid id);
        
        // ======================================================================
        // PDF GENERATION
        // ======================================================================
        
        /// <summary>
        /// Generate PDF for batch
        /// </summary>
        Task<BatchPdfGenerationResultDto> GeneratePdfAsync(GenerateBatchPdfDto input);
        
        /// <summary>
        /// Download batch PDF
        /// </summary>
        Task<byte[]> DownloadPdfAsync(Guid batchId);
        
        /// <summary>
        /// Check PDF generation status (for background jobs)
        /// </summary>
        Task<BatchPdfGenerationResultDto> GetGenerationStatusAsync(Guid batchId);
        
        // ======================================================================
        // WORKFLOW
        // ======================================================================
        
        /// <summary>
        /// Mark batch as downloaded
        /// </summary>
        Task<PrintBatchDto> MarkAsDownloadedAsync(Guid batchId);
        
        /// <summary>
        /// Mark batch as printed (physical printing completed)
        /// </summary>
        Task<PrintBatchDto> MarkAsPrintedAsync(MarkBatchAsPrintedDto input);
        
        /// <summary>
        /// Cancel batch
        /// </summary>
        Task<PrintBatchDto> CancelAsync(CancelBatchDto input);
        
        // ======================================================================
        // STATISTICS
        // ======================================================================
        
        /// <summary>
        /// Get batch statistics
        /// </summary>
        Task<PrintBatchStatisticsDto> GetStatisticsAsync();
    }
    
    /// <summary>
    /// Update batch DTO
    /// </summary>
    public class UpdatePrintBatchDto
    {
        public string? Name { get; set; }
        public string? Notes { get; set; }
    }
    
    /// <summary>
    /// Batch statistics DTO
    /// </summary>
    public class PrintBatchStatisticsDto
    {
        public int TotalBatches { get; set; }
        public int PendingBatches { get; set; }
        public int GeneratedBatches { get; set; }
        public int PrintedBatches { get; set; }
        public int TotalLettersPrinted { get; set; }
        public int LettersPendingPrint { get; set; }
    }
}
