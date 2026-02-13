using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DonaRogApp.Tags.Dto;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace DonaRogApp.Tags
{
    public interface ITagAppService : IApplicationService
    {
        Task<TagDto> GetAsync(Guid id);
        Task<PagedResultDto<TagDto>> GetListAsync(PagedAndSortedResultRequestDto input);
        Task<List<TagDto>> GetActiveListAsync();
        Task<List<string>> GetCategoriesAsync();
        Task<TagDto> CreateAsync(CreateUpdateTagDto input);
        Task<TagDto> UpdateAsync(Guid id, CreateUpdateTagDto input);
        Task DeleteAsync(Guid id);
        Task ActivateAsync(Guid id);
        Task DeactivateAsync(Guid id);
    }
}
