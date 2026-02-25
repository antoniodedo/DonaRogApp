using DonaRogApp.Application.Contracts.Communications.PrintBatches;
using DonaRogApp.Application.Contracts.Communications.PrintBatches.Dto;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.DependencyInjection;

namespace DonaRogApp.Application.Communications.PrintBatches
{
    /// <summary>
    /// Background job for generating large batch PDFs
    /// </summary>
    public class GenerateBatchPdfJob : AsyncBackgroundJob<GenerateBatchPdfJobArgs>, ITransientDependency
    {
        private readonly IPrintBatchAppService _printBatchAppService;

        public GenerateBatchPdfJob(IPrintBatchAppService printBatchAppService)
        {
            _printBatchAppService = printBatchAppService;
        }

        public override async Task ExecuteAsync(GenerateBatchPdfJobArgs args)
        {
            Logger.LogInformation("Starting background PDF generation for batch {BatchId}", args.BatchId);

            try
            {
                var result = await _printBatchAppService.GeneratePdfAsync(new GenerateBatchPdfDto
                {
                    BatchId = args.BatchId,
                    RunInBackground = false // Already in background
                });

                if (result.Success)
                {
                    Logger.LogInformation("Background PDF generation completed for batch {BatchId}, size: {Size} bytes",
                        args.BatchId, result.PdfFileSizeBytes);
                }
                else
                {
                    Logger.LogError("Background PDF generation failed for batch {BatchId}: {Error}",
                        args.BatchId, result.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Background PDF generation failed for batch {BatchId}", args.BatchId);
                throw;
            }
        }
    }

    /// <summary>
    /// Arguments for GenerateBatchPdfJob
    /// </summary>
    public class GenerateBatchPdfJobArgs
    {
        public Guid BatchId { get; set; }
        public Guid RequestedBy { get; set; }
    }
}
