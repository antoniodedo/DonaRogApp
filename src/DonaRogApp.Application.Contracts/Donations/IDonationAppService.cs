using DonaRogApp.Application.Contracts.Donations.Dto;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace DonaRogApp.Application.Contracts.Donations
{
    public interface IDonationAppService : IApplicationService
    {
        // ======================================================================
        // CRUD OPERATIONS
        // ======================================================================
        Task<DonationDto> GetAsync(Guid id);
        
        Task<PagedResultDto<DonationListDto>> GetListAsync(GetDonationsInput input);
        
        Task<DonationDto> CreateAsync(CreateDonationDto input);
        
        Task DeleteAsync(Guid id);

        // ======================================================================
        // WORKFLOW OPERATIONS
        // ======================================================================
        Task<DonationDto> VerifyAsync(Guid id, VerifyDonationDto input);
        
        Task<DonationDto> RejectAsync(Guid id, RejectDonationDto input);

        // ======================================================================
        // PROJECT ALLOCATION OPERATIONS
        // ======================================================================
        Task AllocateToProjectAsync(Guid id, Guid projectId, decimal amount);
        
        Task RemoveProjectAllocationAsync(Guid id, Guid projectId);

        // ======================================================================
        // EXTERNAL IMPORT (for demo purposes)
        // ======================================================================
        Task<DonationDto> AddExternalAsync(ExternalDonationDto input);

        // ======================================================================
        // STATISTICS
        // ======================================================================
        Task<DonationStatisticsDto> GetStatisticsAsync(GetDonationsInput filter);
    }
}
