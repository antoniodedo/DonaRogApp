using DonaRogApp.Application.Contracts.Communications.Dto;
using DonaRogApp.Enums.Communications;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace DonaRogApp.Application.Contracts.Communications
{
    /// <summary>
    /// Application service for Communication management
    /// </summary>
    public interface ICommunicationAppService : IApplicationService
    {
        // ======================================================================
        // DUPLICATE CHECKS
        // ======================================================================
        
        /// <summary>
        /// Check for duplicate/recent letters to donor
        /// Returns alert level and list of recent communications
        /// </summary>
        Task<DuplicateCheckResultDto> CheckDuplicateLettersAsync(CheckDuplicateLettersDto input);
        
        /// <summary>
        /// Quick check - returns only alert level
        /// </summary>
        Task<AlertLevel> GetDuplicateAlertLevelAsync(Guid donorId, int errorThresholdDays = 7, int warningThresholdDays = 15);
        
        // ======================================================================
        // HISTORY
        // ======================================================================
        
        /// <summary>
        /// Get communication history (with filters)
        /// </summary>
        Task<PagedResultDto<CommunicationHistoryDto>> GetHistoryAsync(GetCommunicationHistoryInput input);
        
        /// <summary>
        /// Get donor's recent communications (last 30 days)
        /// </summary>
        Task<List<RecentCommunicationDto>> GetDonorRecentCommunicationsAsync(Guid donorId, int lastDays = 30);
    }
}
