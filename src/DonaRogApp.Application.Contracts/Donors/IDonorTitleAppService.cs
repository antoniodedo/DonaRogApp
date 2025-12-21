using DonaRogApp.Donors.Dto;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace DonaRogApp.Donors
{
    public interface IDonorTitleAppService :
    ICrudAppService< //Defines CRUD methods
        DonorTitleDto,
        Guid,
        PagedAndSortedResultRequestDto, 
        CreateUpdateDonorTitleDto>
    {
    }
}





