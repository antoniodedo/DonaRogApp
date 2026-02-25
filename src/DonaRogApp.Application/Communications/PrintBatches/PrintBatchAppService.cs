using DonaRogApp.Application.Communications;
using DonaRogApp.Application.Contracts.Communications.PrintBatches;
using DonaRogApp.Application.Contracts.Communications.PrintBatches.Dto;
using DonaRogApp.Domain.Communications.Entities;
using DonaRogApp.Domain.Donations.Entities;
using DonaRogApp.Domain.Donors.Entities;
using DonaRogApp.Domain.Storage;
using DonaRogApp.Enums.Communications;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text.Json;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;

namespace DonaRogApp.Application.Communications.PrintBatches
{
    /// <summary>
    /// Application service for Print Batch management
    /// </summary>
    public class PrintBatchAppService : ApplicationService, IPrintBatchAppService
    {
        private readonly IRepository<PrintBatch, Guid> _batchRepository;
        private readonly IRepository<Communication, Guid> _communicationRepository;
        private readonly IRepository<Donor, Guid> _donorRepository;
        private readonly IRepository<Donation, Guid> _donationRepository;
        private readonly PlaceholderService _placeholderService;
        private readonly TemplateMergeService _templateMergeService;
        private readonly IFileStorageService _fileStorageService;
        private readonly ICurrentUser _currentUser;

        public PrintBatchAppService(
            IRepository<PrintBatch, Guid> batchRepository,
            IRepository<Communication, Guid> communicationRepository,
            IRepository<Donor, Guid> donorRepository,
            IRepository<Donation, Guid> donationRepository,
            PlaceholderService placeholderService,
            TemplateMergeService templateMergeService,
            IFileStorageService fileStorageService,
            ICurrentUser currentUser)
        {
            _batchRepository = batchRepository;
            _communicationRepository = communicationRepository;
            _donorRepository = donorRepository;
            _donationRepository = donationRepository;
            _placeholderService = placeholderService;
            _templateMergeService = templateMergeService;
            _fileStorageService = fileStorageService;
            _currentUser = currentUser;
        }

        // ======================================================================
        // QUERY
        // ======================================================================

        public virtual async Task<PagedResultDto<PrintBatchDto>> GetListAsync(GetPrintBatchesInput input)
        {
            var query = await _batchRepository.GetQueryableAsync();

            query = query
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), b =>
                    b.BatchNumber.Contains(input.Filter!) ||
                    (b.Name != null && b.Name.Contains(input.Filter!)))
                .WhereIf(input.Status.HasValue, b => b.Status == input.Status!.Value)
                .WhereIf(input.GeneratedFrom.HasValue, b => b.GeneratedAt >= input.GeneratedFrom!.Value)
                .WhereIf(input.GeneratedTo.HasValue, b => b.GeneratedAt <= input.GeneratedTo!.Value)
                .WhereIf(input.GeneratedBy.HasValue, b => b.GeneratedBy == input.GeneratedBy!.Value)
                .WhereIf(input.IsPrinted.HasValue && input.IsPrinted.Value, b => b.PrintedAt != null)
                .WhereIf(input.IsPrinted.HasValue && !input.IsPrinted.Value, b => b.PrintedAt == null)
                .WhereIf(!input.IncludeCancelled, b => b.Status != PrintBatchStatus.Cancelled);

            var totalCount = await AsyncExecuter.CountAsync(query);

            query = query
                .OrderBy(input.Sorting ?? "CreationTime DESC")
                .Skip(input.SkipCount)
                .Take(input.MaxResultCount);

            var batches = await AsyncExecuter.ToListAsync(query);
            var dtos = batches.Select(MapToDto).ToList();

            return new PagedResultDto<PrintBatchDto>(totalCount, dtos);
        }

        public virtual async Task<PrintBatchDto> GetAsync(Guid id)
        {
            var batch = await _batchRepository.GetAsync(id);
            return MapToDto(batch);
        }

        // ======================================================================
        // PREVIEW
        // ======================================================================

        public virtual async Task<PrintBatchPreviewDto> PreviewBatchAsync(PrintBatchFilterDto filters)
        {
            var query = await BuildFilteredCommunicationsQueryAsync(filters);
            
            var communications = await AsyncExecuter.ToListAsync(
                query.OrderBy(c => c.CreationTime)
                     .Take(10)); // Sample first 10

            var totalCount = await AsyncExecuter.CountAsync(query);
            
            // Get donation IDs to load donations
            var donationIds = communications.Where(c => c.DonationId.HasValue).Select(c => c.DonationId!.Value).ToList();
            var donations = donationIds.Any() ? 
                await _donationRepository.GetListAsync(d => donationIds.Contains(d.Id)) : 
                new List<Donation>();
            
            // Calculate total donation amount
            var allCommunicationsWithDonations = await AsyncExecuter.ToListAsync(
                query.Where(c => c.DonationId != null));
            
            var allDonationIds = allCommunicationsWithDonations.Select(c => c.DonationId!.Value).Distinct().ToList();
            var allDonations = allDonationIds.Any() ?
                await _donationRepository.GetListAsync(d => allDonationIds.Contains(d.Id)) :
                new List<Donation>();
            
            var totalAmount = allDonations.Sum(d => d.TotalAmount);

            // Breakdown by region
            var byRegion = communications
                .Where(c => c.Donor != null)
                .GroupBy(c => c.Donor!.Addresses.FirstOrDefault(a => a.EndDate == null)?.Region ?? "Unknown")
                .ToDictionary(g => g.Key, g => g.Count());

            var preview = new PrintBatchPreviewDto
            {
                TotalLetters = totalCount,
                TotalDonationAmount = totalAmount,
                EstimatedPdfSizeMB = (totalCount * 50 * 1024) / (1024.0 * 1024.0), // ~50KB per letter
                AppliedFilters = filters,
                SampleLetters = communications.Select(c =>
                {
                    var donation = c.DonationId.HasValue ? donations.FirstOrDefault(d => d.Id == c.DonationId.Value) : null;
                    return new LetterPreviewItemDto
                    {
                        CommunicationId = c.Id,
                        DonorId = c.DonorId,
                        DonorName = c.Donor?.FirstName + " " + c.Donor?.LastName ?? c.Donor?.CompanyName ?? "Unknown",
                        DonationId = c.DonationId ?? Guid.Empty,
                        DonationReference = donation?.Reference ?? "N/A",
                        DonationAmount = donation?.TotalAmount ?? 0,
                        DonationDate = donation?.DonationDate ?? DateTime.MinValue,
                        Region = c.Donor?.Addresses.FirstOrDefault(a => a.EndDate == null)?.Region,
                        CreatedAt = c.CreationTime
                    };
                }).ToList(),
                ByRegion = byRegion
            };

            return preview;
        }

        // ======================================================================
        // CREATE & MANAGE
        // ======================================================================

        public virtual async Task<PrintBatchDto> CreateAsync(CreatePrintBatchDto input)
        {
            // Generate batch number
            var queryableRepo = await _batchRepository.GetQueryableAsync();
            var lastBatch = await AsyncExecuter.FirstOrDefaultAsync(
                queryableRepo.OrderByDescending(b => b.CreationTime));
            
            var sequenceNumber = (lastBatch != null ? 
                int.Parse(lastBatch.BatchNumber.Split('-').Last()) : 0) + 1;
            
            var batchNumber = PrintBatch.GenerateBatchNumber(sequenceNumber);

            // Create batch entity
            var batch = PrintBatch.Create(
                GuidGenerator.Create(),
                CurrentTenant.Id,
                batchNumber,
                input.Name,
                input.Notes);

            // Get matching communications
            var query = await BuildFilteredCommunicationsQueryAsync(input.Filters);
            var communicationIds = await AsyncExecuter.ToListAsync(query.Select(c => c.Id));
            
            // Calculate total amount from donations
            var donationIdsQuery = query.Where(c => c.DonationId != null).Select(c => c.DonationId!.Value);
            var donationIds = await AsyncExecuter.ToListAsync(donationIdsQuery);
            var donations = donationIds.Any() ? 
                await _donationRepository.GetListAsync(d => donationIds.Contains(d.Id)) : 
                new List<Donation>();
            var totalAmount = donations.Sum(d => d.TotalAmount);

            // Store filters
            var filterJson = JsonSerializer.Serialize(input.Filters);
            batch.SetFilters(
                filterJson,
                input.Filters.MinAmount,
                input.Filters.MaxAmount,
                input.Filters.Region,
                input.Filters.ProjectIds != null ? string.Join(",", input.Filters.ProjectIds) : null,
                input.Filters.CampaignIds != null ? string.Join(",", input.Filters.CampaignIds) : null);

            batch.UpdateStatistics(communicationIds.Count, totalAmount);

            // Save batch
            await _batchRepository.InsertAsync(batch);
            await CurrentUnitOfWork!.SaveChangesAsync();

            // Assign communications to batch
            var communications = await _communicationRepository.GetListAsync(c => communicationIds.Contains(c.Id));
            foreach (var comm in communications)
            {
                comm.AssignToBatch(batch.Id);
            }
            await CurrentUnitOfWork.SaveChangesAsync();

            // Auto-generate PDF if requested
            if (input.AutoGeneratePdf)
            {
                await GeneratePdfAsync(new GenerateBatchPdfDto { BatchId = batch.Id });
            }

            return MapToDto(batch);
        }

        public virtual async Task<PrintBatchDto> UpdateAsync(Guid id, UpdatePrintBatchDto input)
        {
            var batch = await _batchRepository.GetAsync(id);

            if (!batch.CanBeEdited())
            {
                throw new BusinessException("DonaRog:BatchCannotBeEdited")
                    .WithData("batchId", id)
                    .WithData("status", batch.Status);
            }

            batch.UpdateDetails(input.Name, input.Notes);
            await _batchRepository.UpdateAsync(batch);

            return MapToDto(batch);
        }

        public virtual async Task DeleteAsync(Guid id)
        {
            var batch = await _batchRepository.GetAsync(id);

            if (!batch.IsDraft())
            {
                throw new BusinessException("DonaRog:CanOnlyDeleteDraftBatch")
                    .WithData("batchId", id)
                    .WithData("status", batch.Status);
            }

            // Remove communications from batch
            var communications = await _communicationRepository.GetListAsync(c => c.PrintBatchId == id);
            foreach (var comm in communications)
            {
                comm.RemoveFromBatch();
            }

            // Delete PDF file if exists
            if (!string.IsNullOrEmpty(batch.PdfFilePath))
            {
                await _fileStorageService.DeleteFileAsync(batch.PdfFilePath);
            }

            await _batchRepository.DeleteAsync(id);
        }

        // ======================================================================
        // PDF GENERATION
        // ======================================================================

        public virtual async Task<BatchPdfGenerationResultDto> GeneratePdfAsync(GenerateBatchPdfDto input)
        {
            var stopwatch = Stopwatch.StartNew();
            var batch = await _batchRepository.GetAsync(input.BatchId);

            try
            {
                if (!batch.CanGeneratePdf())
                {
                    throw new BusinessException("DonaRog:BatchCannotGeneratePdf")
                        .WithData("batchId", input.BatchId)
                        .WithData("status", batch.Status);
                }

                // Mark as generating
                batch.MarkAsGenerating();
                await _batchRepository.UpdateAsync(batch);
                await CurrentUnitOfWork!.SaveChangesAsync();

                // Get all communications in batch
                var queryWithDetails = await _communicationRepository.WithDetailsAsync(c => c.Donor);
                var communications = await AsyncExecuter.ToListAsync(
                    queryWithDetails.Where(c => c.PrintBatchId == input.BatchId)
                                    .OrderBy(c => c.Donor!.LastName)
                                    .ThenBy(c => c.Donor!.FirstName));
                
                // Load donations separately
                var donationIds = communications.Where(c => c.DonationId.HasValue).Select(c => c.DonationId!.Value).Distinct().ToList();
                var donations = donationIds.Any() ? 
                    await _donationRepository.GetListAsync(d => donationIds.Contains(d.Id), includeDetails: true) : 
                    new List<Donation>();

                Logger.LogInformation("Generating PDF for batch {BatchId} with {Count} letters", 
                    input.BatchId, communications.Count);

                // Generate individual PDFs
                var pdfBytesList = new List<byte[]>();
                foreach (var comm in communications)
                {
                    var donor = comm.Donor ?? await _donorRepository.GetAsync(comm.DonorId, includeDetails: true);
                    var donation = comm.DonationId.HasValue ? 
                        donations.FirstOrDefault(d => d.Id == comm.DonationId.Value) : null;

                    if (donation == null) continue;

                    // Build merge data
                    var mergeData = _placeholderService.BuildMergeData(donor, donation);
                    
                    // Replace placeholders in communication body
                    var htmlContent = _placeholderService.ReplacePlaceholders(comm.Body ?? "", mergeData);
                    
                    // Generate PDF
                    var pdfBytes = _templateMergeService.ConvertHtmlToPdf(htmlContent, "Thank You Letter");
                    pdfBytesList.Add(pdfBytes);
                }

                // Merge all PDFs
                Logger.LogInformation("Merging {Count} PDFs", pdfBytesList.Count);
                var mergedPdf = _templateMergeService.MergePdfs(pdfBytesList);

                // Save to storage
                var fileName = $"{batch.BatchNumber}.pdf";
                using var pdfStream = new MemoryStream(mergedPdf);
                var storagePath = await _fileStorageService.SaveFileAsync(
                    pdfStream, 
                    fileName, 
                    $"batches/{batch.Id}");

                // Update batch
                batch.MarkAsGenerated(storagePath, mergedPdf.Length, _currentUser.Id ?? Guid.Empty);
                await _batchRepository.UpdateAsync(batch);

                stopwatch.Stop();

                Logger.LogInformation("PDF generation completed for batch {BatchId} in {Duration}ms", 
                    input.BatchId, stopwatch.ElapsedMilliseconds);

                return new BatchPdfGenerationResultDto
                {
                    Success = true,
                    IsBackgroundJob = false,
                    PdfFileSizeBytes = mergedPdf.Length,
                    DurationMs = stopwatch.ElapsedMilliseconds,
                    Batch = MapToDto(batch)
                };
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "PDF generation failed for batch {BatchId}", input.BatchId);
                
                return new BatchPdfGenerationResultDto
                {
                    Success = false,
                    ErrorMessage = ex.Message,
                    DurationMs = stopwatch.ElapsedMilliseconds
                };
            }
        }

        public virtual async Task<byte[]> DownloadPdfAsync(Guid batchId)
        {
            var batch = await _batchRepository.GetAsync(batchId);

            if (string.IsNullOrEmpty(batch.PdfFilePath))
            {
                throw new BusinessException("DonaRog:BatchPdfNotGenerated")
                    .WithData("batchId", batchId);
            }

            using var stream = await _fileStorageService.GetFileAsync(batch.PdfFilePath);
            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            
            // Mark as downloaded
            if (batch.Status == PrintBatchStatus.Generated)
            {
                batch.MarkAsDownloaded(_currentUser.Id ?? Guid.Empty);
                await _batchRepository.UpdateAsync(batch);
            }

            return memoryStream.ToArray();
        }

        public virtual async Task<BatchPdfGenerationResultDto> GetGenerationStatusAsync(Guid batchId)
        {
            var batch = await _batchRepository.GetAsync(batchId);

            return new BatchPdfGenerationResultDto
            {
                Success = batch.Status >= PrintBatchStatus.Generated,
                IsBackgroundJob = false,
                PdfFileSizeBytes = batch.PdfFileSizeBytes,
                Batch = MapToDto(batch)
            };
        }

        // ======================================================================
        // WORKFLOW
        // ======================================================================

        public virtual async Task<PrintBatchDto> MarkAsDownloadedAsync(Guid batchId)
        {
            var batch = await _batchRepository.GetAsync(batchId);
            
            batch.MarkAsDownloaded(_currentUser.Id ?? Guid.Empty);
            await _batchRepository.UpdateAsync(batch);

            return MapToDto(batch);
        }

        public virtual async Task<PrintBatchDto> MarkAsPrintedAsync(MarkBatchAsPrintedDto input)
        {
            var batch = await _batchRepository.GetAsync(input.BatchId);
            
            batch.MarkAsPrinted(_currentUser.Id ?? Guid.Empty);
            
            if (!string.IsNullOrWhiteSpace(input.Notes))
            {
                batch.UpdateDetails(notes: input.Notes);
            }
            
            await _batchRepository.UpdateAsync(batch);

            // Mark all communications as printed
            var communications = await _communicationRepository.GetListAsync(c => c.PrintBatchId == input.BatchId);
            foreach (var comm in communications)
            {
                comm.MarkAsPrinted(input.PrintedAt ?? DateTime.UtcNow);
            }
            
            await CurrentUnitOfWork!.SaveChangesAsync();

            return MapToDto(batch);
        }

        public virtual async Task<PrintBatchDto> CancelAsync(CancelBatchDto input)
        {
            var batch = await _batchRepository.GetAsync(input.BatchId);
            
            if (!batch.CanBeCancelled())
            {
                throw new BusinessException("DonaRog:BatchCannotBeCancelled")
                    .WithData("batchId", input.BatchId)
                    .WithData("status", batch.Status);
            }

            batch.Cancel(input.Reason);
            await _batchRepository.UpdateAsync(batch);

            // Remove communications from batch
            var communications = await _communicationRepository.GetListAsync(c => c.PrintBatchId == input.BatchId);
            foreach (var comm in communications)
            {
                comm.RemoveFromBatch();
            }
            await CurrentUnitOfWork!.SaveChangesAsync();

            return MapToDto(batch);
        }

        // ======================================================================
        // STATISTICS
        // ======================================================================

        public virtual async Task<PrintBatchStatisticsDto> GetStatisticsAsync()
        {
            var batches = await _batchRepository.GetListAsync();
            
            var totalLettersPrinted = await _communicationRepository
                .CountAsync(c => c.IsPrinted && c.Type == CommunicationType.Letter);
            
            var lettersPendingPrint = await _communicationRepository
                .CountAsync(c => !c.IsPrinted && c.Status == CommunicationStatus.PendingPrint && c.Type == CommunicationType.Letter);

            return new PrintBatchStatisticsDto
            {
                TotalBatches = batches.Count,
                PendingBatches = batches.Count(b => b.Status == PrintBatchStatus.Draft || b.Status == PrintBatchStatus.Ready),
                GeneratedBatches = batches.Count(b => b.Status == PrintBatchStatus.Generated || b.Status == PrintBatchStatus.Downloaded),
                PrintedBatches = batches.Count(b => b.Status == PrintBatchStatus.Printed),
                TotalLettersPrinted = totalLettersPrinted,
                LettersPendingPrint = lettersPendingPrint
            };
        }

        // ======================================================================
        // HELPER METHODS
        // ======================================================================

        private async Task<IQueryable<Communication>> BuildFilteredCommunicationsQueryAsync(PrintBatchFilterDto filters)
        {
            var query = await _communicationRepository.WithDetailsAsync(c => c.Donor);

            // Base filters: only letters, pending print
            query = query.Where(c => c.Type == CommunicationType.Letter)
                         .Where(c => c.Status == CommunicationStatus.PendingPrint || c.Status == CommunicationStatus.Draft);

            // Exclude already printed
            if (filters.ExcludePrinted)
            {
                query = query.Where(c => !c.IsPrinted);
            }

            // Exclude in other batches
            if (filters.ExcludeInOtherBatches)
            {
                query = query.Where(c => c.PrintBatchId == null);
            }

            // For filters on donations, we need to join with donations table
            // Get donation IDs that match filters first
            var donationQuery = await _donationRepository.GetQueryableAsync();
            
            if (filters.MinAmount.HasValue)
            {
                donationQuery = donationQuery.Where(d => d.TotalAmount >= filters.MinAmount.Value);
            }
            if (filters.MaxAmount.HasValue)
            {
                donationQuery = donationQuery.Where(d => d.TotalAmount <= filters.MaxAmount.Value);
            }
            if (filters.DonationDateFrom.HasValue)
            {
                donationQuery = donationQuery.Where(d => d.DonationDate >= filters.DonationDateFrom.Value);
            }
            if (filters.DonationDateTo.HasValue)
            {
                donationQuery = donationQuery.Where(d => d.DonationDate <= filters.DonationDateTo.Value);
            }
            if (filters.OnlyVerified)
            {
                donationQuery = donationQuery.Where(d => d.Status == Enums.Donations.DonationStatus.Verified);
            }
            if (filters.ProjectIds != null && filters.ProjectIds.Any())
            {
                donationQuery = donationQuery.Where(d => d.Projects.Any(p => filters.ProjectIds.Contains(p.ProjectId)));
            }
            if (filters.CampaignIds != null && filters.CampaignIds.Any())
            {
                donationQuery = donationQuery.Where(d => filters.CampaignIds.Contains(d.CampaignId!.Value));
            }
            
            var matchingDonationIds = await AsyncExecuter.ToListAsync(donationQuery.Select(d => d.Id));
            
            if (matchingDonationIds.Any())
            {
                query = query.Where(c => c.DonationId != null && matchingDonationIds.Contains(c.DonationId.Value));
            }

            // Region filter
            if (!string.IsNullOrWhiteSpace(filters.Region))
            {
                query = query.Where(c => c.Donor != null && 
                                         c.Donor.Addresses.Any(a => a.EndDate == null && a.Region == filters.Region));
            }
            
            // Donor category filter
            if (filters.DonorCategory.HasValue)
            {
                query = query.Where(c => c.Donor != null && c.Donor.Category == filters.DonorCategory.Value);
            }

            return query;
        }

        private PrintBatchDto MapToDto(PrintBatch batch)
        {
            return new PrintBatchDto
            {
                Id = batch.Id,
                BatchNumber = batch.BatchNumber,
                Name = batch.Name,
                Status = batch.Status,
                TotalLetters = batch.TotalLetters,
                TotalDonationAmount = batch.TotalDonationAmount,
                PdfFileSizeBytes = batch.PdfFileSizeBytes,
                GeneratedAt = batch.GeneratedAt,
                GeneratedBy = batch.GeneratedBy,
                PrintedAt = batch.PrintedAt,
                PrintedBy = batch.PrintedBy,
                Notes = batch.Notes,
                CreationTime = batch.CreationTime,
                CreatorId = batch.CreatorId,
                LastModificationTime = batch.LastModificationTime,
                LastModifierId = batch.LastModifierId,
                FilterSummary = new BatchFilterSummaryDto
                {
                    MinAmount = batch.MinAmount,
                    MaxAmount = batch.MaxAmount,
                    Region = batch.Region,
                    FilterCount = 
                        (batch.MinAmount.HasValue ? 1 : 0) +
                        (batch.MaxAmount.HasValue ? 1 : 0) +
                        (!string.IsNullOrEmpty(batch.Region) ? 1 : 0) +
                        (!string.IsNullOrEmpty(batch.ProjectIds) ? 1 : 0) +
                        (!string.IsNullOrEmpty(batch.CampaignIds) ? 1 : 0)
                }
            };
        }
    }
}
