using DonaRogApp.Notes.Dto;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace DonaRogApp.Notes
{
    public interface INoteAppService : IApplicationService
    {
        Task<NoteDto> GetAsync(Guid id);
        Task<PagedResultDto<NoteDto>> GetListByDonorAsync(Guid donorId, PagedAndSortedResultRequestDto input);
        Task<NoteDto> CreateForDonorAsync(Guid donorId, CreateUpdateNoteDto input);
        Task<NoteDto> UpdateAsync(Guid id, CreateUpdateNoteDto input);
        Task DeleteAsync(Guid id);
        Task ToggleImportantAsync(Guid id);
    }
}
