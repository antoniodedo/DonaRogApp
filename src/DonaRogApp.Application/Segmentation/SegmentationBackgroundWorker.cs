using DonaRogApp.Application.Contracts.Segmentation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Threading;

namespace DonaRogApp.Application.Segmentation
{
    /// <summary>
    /// Background worker for nightly donor segmentation batch
    /// Runs every 24 hours at 2:00 AM
    /// </summary>
    public class SegmentationBackgroundWorker : AsyncPeriodicBackgroundWorkerBase
    {
        private readonly ISegmentationRuleAppService _segmentationRuleAppService;

        public SegmentationBackgroundWorker(
            AbpAsyncTimer timer,
            IServiceScopeFactory serviceScopeFactory,
            ISegmentationRuleAppService segmentationRuleAppService)
            : base(timer, serviceScopeFactory)
        {
            _segmentationRuleAppService = segmentationRuleAppService;
            
            // Run every 24 hours
            Timer.Period = (int)TimeSpan.FromHours(24).TotalMilliseconds;
            
            // Don't run on start - wait for scheduled time
            Timer.RunOnStart = false;
        }

        protected override async Task DoWorkAsync(PeriodicBackgroundWorkerContext workerContext)
        {
            Logger.LogInformation("===== Starting nightly donor segmentation batch =====");
            
            try
            {
                var result = await _segmentationRuleAppService.RunSegmentationBatchAsync();

                if (result.Success)
                {
                    Logger.LogInformation(
                        "Nightly segmentation batch completed successfully: {DonorsProcessed} donors, " +
                        "{AssignmentsCreated} assignments created, {AssignmentsRemoved} removed in {Duration}s",
                        result.DonorsProcessed, result.AssignmentsCreated, result.AssignmentsRemoved, result.DurationSeconds);
                }
                else
                {
                    Logger.LogWarning(
                        "Nightly segmentation batch completed with errors: {Errors} errors, {Message}",
                        result.Errors, result.Message);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Nightly segmentation batch failed with exception");
                throw;
            }
        }

    }
}
