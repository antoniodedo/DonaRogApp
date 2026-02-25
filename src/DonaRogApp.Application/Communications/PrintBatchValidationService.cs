using DonaRogApp.Domain.Communications.Entities;
using DonaRogApp.Domain.Donors.Entities;
using DonaRogApp.Enums.Communications;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace DonaRogApp.Application.Communications
{
    /// <summary>
    /// Service for validating print batches and checking for conflicts
    /// </summary>
    public class PrintBatchValidationService : ITransientDependency
    {
        private readonly IRepository<PrintBatch, Guid> _batchRepository;
        private readonly IRepository<Communication, Guid> _communicationRepository;
        private readonly ILogger<PrintBatchValidationService> _logger;

        public PrintBatchValidationService(
            IRepository<PrintBatch, Guid> batchRepository,
            IRepository<Communication, Guid> communicationRepository,
            ILogger<PrintBatchValidationService> logger)
        {
            _batchRepository = batchRepository;
            _communicationRepository = communicationRepository;
            _logger = logger;
        }

        // ======================================================================
        // VALIDATION METHODS
        // ======================================================================

        /// <summary>
        /// Validate that batch can be generated
        /// </summary>
        public async Task<ValidationResultDto> ValidateBatchForGenerationAsync(Guid batchId)
        {
            var result = new ValidationResultDto { IsValid = true };

            var batch = await _batchRepository.GetAsync(batchId);

            // Check status
            if (!batch.CanGeneratePdf())
            {
                result.IsValid = false;
                result.Errors.Add($"Batch status is {batch.Status}, cannot generate PDF");
                return result;
            }

            // Check letter count
            if (batch.TotalLetters == 0)
            {
                result.IsValid = false;
                result.Errors.Add("Batch has no letters");
                return result;
            }

            // Check communications exist
            var commCount = await _communicationRepository.CountAsync(c => c.PrintBatchId == batchId);
            if (commCount == 0)
            {
                result.IsValid = false;
                result.Errors.Add("No communications found in batch");
                return result;
            }

            // Verify all communications have content
            var commsWithoutBody = await _communicationRepository
                .CountAsync(c => c.PrintBatchId == batchId && string.IsNullOrEmpty(c.Body));
            
            if (commsWithoutBody > 0)
            {
                result.Warnings.Add($"{commsWithoutBody} communication(s) have no body content");
            }

            return result;
        }

        /// <summary>
        /// Check for conflicts in batch (e.g., communications already in other batches)
        /// </summary>
        public async Task<List<BatchConflictDto>> CheckBatchConflictsAsync(
            List<Guid> communicationIds,
            Guid? excludeBatchId = null)
        {
            var conflicts = new List<BatchConflictDto>();

            var communications = await _communicationRepository
                .GetListAsync(c => communicationIds.Contains(c.Id) && c.PrintBatchId != null);

            if (!communications.Any())
                return conflicts;

            var batchIds = communications
                .Where(c => c.PrintBatchId.HasValue && c.PrintBatchId != excludeBatchId)
                .Select(c => c.PrintBatchId!.Value)
                .Distinct()
                .ToList();

            if (!batchIds.Any())
                return conflicts;

            var batches = await _batchRepository.GetListAsync(b => batchIds.Contains(b.Id));

            foreach (var comm in communications.Where(c => c.PrintBatchId != excludeBatchId))
            {
                var batch = batches.FirstOrDefault(b => b.Id == comm.PrintBatchId);
                if (batch != null)
                {
                    conflicts.Add(new BatchConflictDto
                    {
                        CommunicationId = comm.Id,
                        ExistingBatchId = batch.Id,
                        ExistingBatchNumber = batch.BatchNumber,
                        ExistingBatchStatus = batch.Status
                    });
                }
            }

            return conflicts;
        }

        /// <summary>
        /// Validate batch filters return at least some results
        /// </summary>
        public void ValidateFilterCriteria(decimal? minAmount, decimal? maxAmount)
        {
            if (minAmount.HasValue && minAmount.Value < 0)
            {
                throw new BusinessException("DonaRog:InvalidMinAmount")
                    .WithData("minAmount", minAmount.Value);
            }

            if (maxAmount.HasValue && maxAmount.Value < 0)
            {
                throw new BusinessException("DonaRog:InvalidMaxAmount")
                    .WithData("maxAmount", maxAmount.Value);
            }

            if (minAmount.HasValue && maxAmount.HasValue && minAmount.Value > maxAmount.Value)
            {
                throw new BusinessException("DonaRog:MinAmountGreaterThanMax")
                    .WithData("minAmount", minAmount.Value)
                    .WithData("maxAmount", maxAmount.Value);
            }
        }
    }

    /// <summary>
    /// Validation result DTO
    /// </summary>
    public class ValidationResultDto
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; } = new();
        public List<string> Warnings { get; set; } = new();
    }

    /// <summary>
    /// Batch conflict information
    /// </summary>
    public class BatchConflictDto
    {
        public Guid CommunicationId { get; set; }
        public Guid ExistingBatchId { get; set; }
        public string ExistingBatchNumber { get; set; } = null!;
        public PrintBatchStatus ExistingBatchStatus { get; set; }
    }
}
