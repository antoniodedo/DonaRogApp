using DonaRogApp.Application.Contracts.Donors.Dto;
using DonaRogApp.Donors.Dto;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace DonaRogApp.Donors
{
    public interface IDonorAppService :
    ICrudAppService< //Defines CRUD methods
        DonorDto,
        Guid,
        GetDonorsInput, 
        CreateUpdateDonorDto>
    {
        Task<PagedResultDto<DonorListDto>> GetLightListAsync(GetDonorsInput input);
        
        Task<DonorRfmStatisticsDto> GetRfmStatisticsAsync();
    }
}





