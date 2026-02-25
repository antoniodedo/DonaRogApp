using DonaRogApp.Application.Contracts.Communications;
using DonaRogApp.Application.Contracts.Communications.Dto;
using DonaRogApp.Domain.Communications.Entities;
using DonaRogApp.Domain.Donations.Entities;
using DonaRogApp.Domain.Donors.Entities;
using DonaRogApp.Enums.Communications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace DonaRogApp.Application.Communications
{
    /// <summary>
    /// Application service for Communication management
    /// </summary>
    public class CommunicationAppService : ApplicationService, ICommunicationAppService
    {
        private readonly IRepository<Communication, Guid> _communicationRepository;
        private readonly IRepository<PrintBatch, Guid> _batchRepository;
        private readonly IRepository<Donation, Guid> _donationRepository;

        public CommunicationAppService(
            IRepository<Communication, Guid> communicationRepository,
            IRepository<PrintBatch, Guid> batchRepository,
            IRepository<Donation, Guid> donationRepository)
        {
            _communicationRepository = communicationRepository;
            _batchRepository = batchRepository;
            _donationRepository = donationRepository;
        }

        // ======================================================================
        // DUPLICATE CHECKS
        // ======================================================================

        public virtual async Task<DuplicateCheckResultDto> CheckDuplicateLettersAsync(CheckDuplicateLettersDto input)
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-input.CheckLastDays);

            // Query recent communications
            var query = await _communicationRepository.GetQueryableAsync();
            
            var recentComms = await AsyncExecuter.ToListAsync(
                query.Where(c => c.DonorId == input.DonorId)
                     .Where(c => c.Type == CommunicationType.Letter)
                     .Where(c => c.SentDate >= cutoffDate || c.CreationTime >= cutoffDate)
                     .WhereIf(input.Category.HasValue, c => c.Category == input.Category!.Value)
                     .OrderByDescending(c => c.SentDate)
            );

            if (!recentComms.Any())
            {
                return new DuplicateCheckResultDto
                {
                    AlertLevel = AlertLevel.None,
                    Message = "Nessuna lettera recente trovata"
                };
            }

            // Load related data
            var donationIds = recentComms.Where(c => c.DonationId.HasValue).Select(c => c.DonationId!.Value).ToList();
            var donations = donationIds.Any() ? 
                await _donationRepository.GetListAsync(d => donationIds.Contains(d.Id)) : 
                new List<Donation>();

            var batchIds = recentComms.Where(c => c.PrintBatchId.HasValue).Select(c => c.PrintBatchId!.Value).Distinct().ToList();
            var batches = batchIds.Any() ? 
                await _batchRepository.GetListAsync(b => batchIds.Contains(b.Id)) : 
                new List<PrintBatch>();

            // Build result list
            var now = DateTime.UtcNow;
            var recentDtos = new List<RecentCommunicationDto>();
            AlertLevel maxAlertLevel = AlertLevel.None;

            foreach (var comm in recentComms)
            {
                var daysAgo = (int)(now - comm.SentDate).TotalDays;
                var alertLevel = DetermineAlertLevel(daysAgo, input.ErrorThresholdDays, input.WarningThresholdDays);
                
                if (alertLevel > maxAlertLevel)
                {
                    maxAlertLevel = alertLevel;
                }

                var donation = comm.DonationId.HasValue ? donations.FirstOrDefault(d => d.Id == comm.DonationId.Value) : null;
                var batch = comm.PrintBatchId.HasValue ? batches.FirstOrDefault(b => b.Id == comm.PrintBatchId.Value) : null;

                recentDtos.Add(new RecentCommunicationDto
                {
                    Id = comm.Id,
                    Type = comm.Type,
                    Category = comm.Category,
                    Subject = comm.Subject,
                    SentDate = comm.SentDate,
                    IsPrinted = comm.IsPrinted,
                    IsInBatch = comm.PrintBatchId.HasValue,
                    PrintBatchNumber = batch?.BatchNumber,
                    DonationId = comm.DonationId,
                    DonationReference = donation?.Reference,
                    DonationAmount = donation?.TotalAmount,
                    DaysAgo = daysAgo,
                    AlertLevel = alertLevel
                });
            }

            var message = maxAlertLevel switch
            {
                AlertLevel.Error => $"ATTENZIONE: Lettera inviata {recentDtos.First().DaysAgo} giorni fa!",
                AlertLevel.Warning => $"Attenzione: Lettera inviata {recentDtos.First().DaysAgo} giorni fa",
                AlertLevel.Info => $"Info: Lettera inviata {recentDtos.First().DaysAgo} giorni fa",
                _ => "Nessun duplicato recente"
            };

            return new DuplicateCheckResultDto
            {
                AlertLevel = maxAlertLevel,
                RecentCommunications = recentDtos,
                Message = message
            };
        }

        public virtual async Task<AlertLevel> GetDuplicateAlertLevelAsync(
            Guid donorId, 
            int errorThresholdDays = 7, 
            int warningThresholdDays = 15)
        {
            var result = await CheckDuplicateLettersAsync(new CheckDuplicateLettersDto
            {
                DonorId = donorId,
                ErrorThresholdDays = errorThresholdDays,
                WarningThresholdDays = warningThresholdDays,
                CheckLastDays = 30
            });

            return result.AlertLevel;
        }

        // ======================================================================
        // HISTORY
        // ======================================================================

        public virtual async Task<PagedResultDto<CommunicationHistoryDto>> GetHistoryAsync(GetCommunicationHistoryInput input)
        {
            var query = await _communicationRepository.GetQueryableAsync();

            query = query
                .WhereIf(input.DonorId.HasValue, c => c.DonorId == input.DonorId!.Value)
                .WhereIf(input.Type.HasValue, c => c.Type == input.Type!.Value)
                .WhereIf(input.Category.HasValue, c => c.Category == input.Category!.Value)
                .WhereIf(input.DateFrom.HasValue, c => c.SentDate >= input.DateFrom!.Value)
                .WhereIf(input.DateTo.HasValue, c => c.SentDate <= input.DateTo!.Value)
                .WhereIf(input.Status.HasValue, c => c.Status == input.Status!.Value)
                .WhereIf(input.OnlyPrinted == true, c => c.IsPrinted)
                .WhereIf(input.OnlyPrinted == false, c => !c.IsPrinted);

            var totalCount = await AsyncExecuter.CountAsync(query);

            query = query
                .OrderBy(input.Sorting ?? "SentDate DESC")
                .Skip(input.SkipCount)
                .Take(input.MaxResultCount);

            var communications = await AsyncExecuter.ToListAsync(query);

            // Load related data
            var donationIds = communications.Where(c => c.DonationId.HasValue).Select(c => c.DonationId!.Value).ToList();
            var donations = donationIds.Any() ? 
                await _donationRepository.GetListAsync(d => donationIds.Contains(d.Id)) : 
                new List<Donation>();

            var batchIds = communications.Where(c => c.PrintBatchId.HasValue).Select(c => c.PrintBatchId!.Value).Distinct().ToList();
            var batches = batchIds.Any() ? 
                await _batchRepository.GetListAsync(b => batchIds.Contains(b.Id)) : 
                new List<PrintBatch>();

            var dtos = communications.Select(c =>
            {
                var donation = c.DonationId.HasValue ? donations.FirstOrDefault(d => d.Id == c.DonationId.Value) : null;
                var batch = c.PrintBatchId.HasValue ? batches.FirstOrDefault(b => b.Id == c.PrintBatchId.Value) : null;

                return new CommunicationHistoryDto
                {
                    Id = c.Id,
                    DonorId = c.DonorId,
                    DonorName = "Unknown", // Will be loaded via Donor navigation if needed
                    Type = c.Type,
                    Category = c.Category,
                    Subject = c.Subject,
                    SentDate = c.SentDate,
                    Status = c.Status,
                    IsPrinted = c.IsPrinted,
                    PrintedAt = c.PrintedAt,
                    PrintBatchId = c.PrintBatchId,
                    PrintBatchNumber = batch?.BatchNumber,
                    DonationId = c.DonationId,
                    DonationReference = donation?.Reference,
                    CreationTime = c.CreationTime
                };
            }).ToList();

            return new PagedResultDto<CommunicationHistoryDto>(totalCount, dtos);
        }

        public virtual async Task<List<RecentCommunicationDto>> GetDonorRecentCommunicationsAsync(Guid donorId, int lastDays = 30)
        {
            var result = await CheckDuplicateLettersAsync(new CheckDuplicateLettersDto
            {
                DonorId = donorId,
                Category = null, // All categories
                CheckLastDays = lastDays
            });

            return result.RecentCommunications;
        }

        // ======================================================================
        // HELPER METHODS
        // ======================================================================

        private AlertLevel DetermineAlertLevel(int daysAgo, int errorThreshold, int warningThreshold)
        {
            if (daysAgo < errorThreshold)
                return AlertLevel.Error;
            if (daysAgo < warningThreshold)
                return AlertLevel.Warning;
            return AlertLevel.Info;
        }
    }
}
