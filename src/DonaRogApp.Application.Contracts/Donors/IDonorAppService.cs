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
        PagedAndSortedResultRequestDto, 
        CreateUpdateDonorDto>
    {
        Task<PagedResultDto<DonorListDto>> GetLightListAsync(PagedAndSortedResultRequestDto input);
    }
}





