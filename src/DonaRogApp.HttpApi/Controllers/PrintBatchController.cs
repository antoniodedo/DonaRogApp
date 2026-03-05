using DonaRogApp.Application.Contracts.Communications.PrintBatches;
using DonaRogApp.Application.Contracts.Communications.PrintBatches.Dto;
using DonaRogApp.Controllers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace DonaRogApp.HttpApi.Controllers
{
    /// <summary>
    /// Controller for Print Batch management
    /// </summary>
    [Route("api/print-batches")]
    [ApiController]
    public class PrintBatchController : DonaRogAppController
    {
        private readonly IPrintBatchAppService _service;

        public PrintBatchController(IPrintBatchAppService service)
        {
            _service = service;
        }

        // ======================================================================
        // QUERY
        // ======================================================================

        [HttpGet]
        public virtual Task<PagedResultDto<PrintBatchDto>> GetListAsync([FromQuery] GetPrintBatchesInput input)
        {
            return _service.GetListAsync(input);
        }

        [HttpGet]
        [Route("{id}")]
        public virtual Task<PrintBatchDto> GetAsync(Guid id)
        {
            return _service.GetAsync(id);
        }

        // ======================================================================
        // PREVIEW
        // ======================================================================

        [HttpPost]
        [Route("preview")]
        public virtual Task<PrintBatchPreviewDto> PreviewBatchAsync([FromBody] PrintBatchFilterDto filters)
        {
            return _service.PreviewBatchAsync(filters);
        }

        // ======================================================================
        // CREATE & MANAGE
        // ======================================================================

        [HttpPost]
        public virtual Task<PrintBatchDto> CreateAsync([FromBody] CreatePrintBatchDto input)
        {
            return _service.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public virtual Task<PrintBatchDto> UpdateAsync(Guid id, [FromBody] UpdatePrintBatchDto input)
        {
            return _service.UpdateAsync(id, input);
        }

        [HttpDelete]
        [Route("{id}")]
        public virtual Task DeleteAsync(Guid id)
        {
            return _service.DeleteAsync(id);
        }

        // ======================================================================
        // PDF GENERATION
        // ======================================================================

        [HttpPost]
        [Route("generate-pdf")]
        public virtual Task<BatchPdfGenerationResultDto> GeneratePdfAsync([FromBody] GenerateBatchPdfDto input)
        {
            return _service.GeneratePdfAsync(input);
        }

        [HttpGet]
        [Route("{batchId}/download-pdf")]
        public virtual async Task<IActionResult> DownloadPdfAsync(Guid batchId)
        {
            var pdfBytes = await _service.DownloadPdfAsync(batchId);
            var batch = await _service.GetAsync(batchId);
            
            return File(pdfBytes, "application/pdf", $"{batch.BatchNumber}.pdf");
        }

        [HttpGet]
        [Route("{batchId}/generation-status")]
        public virtual Task<BatchPdfGenerationResultDto> GetGenerationStatusAsync(Guid batchId)
        {
            return _service.GetGenerationStatusAsync(batchId);
        }

        // ======================================================================
        // WORKFLOW
        // ======================================================================

        [HttpPost]
        [Route("{batchId}/mark-downloaded")]
        public virtual Task<PrintBatchDto> MarkAsDownloadedAsync(Guid batchId)
        {
            return _service.MarkAsDownloadedAsync(batchId);
        }

        [HttpPost]
        [Route("mark-printed")]
        public virtual Task<PrintBatchDto> MarkAsPrintedAsync([FromBody] MarkBatchAsPrintedDto input)
        {
            return _service.MarkAsPrintedAsync(input);
        }

        [HttpPost]
        [Route("cancel")]
        public virtual Task<PrintBatchDto> CancelAsync([FromBody] CancelBatchDto input)
        {
            return _service.CancelAsync(input);
        }

        // ======================================================================
        // STATISTICS
        // ======================================================================

        [HttpGet]
        [Route("statistics")]
        public virtual Task<PrintBatchStatisticsDto> GetStatisticsAsync()
        {
            return _service.GetStatisticsAsync();
        }
    }
}
