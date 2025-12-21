using DonaRogApp.Notes.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace DonaRogApp.Donors.Dto
{
    public interface IEmailAppService : IApplicationService
    {
        Task<EmailDto> GetAsync(Guid id);
        Task<PagedResultDto<EmailDto>> GetListByDonorAsync(Guid donorId, PagedAndSortedResultRequestDto input);
        Task<EmailDto> CreateByDonorAsync(Guid donorId, CreateUpdateEmailDto input);
        Task<EmailDto> UpdateByDonorAsync(Guid donorId, Guid id, CreateUpdateEmailDto input);
        Task DeleteAsync(Guid id);
    }
}
