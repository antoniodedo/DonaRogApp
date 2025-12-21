using DonaRogApp.Donors.Entities;
using DonaRogApp.Notes.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using System.Linq.Dynamic.Core;
using DonaRogApp.Notes.Entities;


namespace DonaRogApp.Notes
{
    public class NoteAppService : ApplicationService, INoteAppService
    {
        private readonly IRepository<Note, Guid> _noteRepository;
        private readonly IRepository<Donor, Guid> _donorRepository;

        public NoteAppService(
            IRepository<Note, Guid> noteRepository,
            IRepository<Donor, Guid> donorRepository)
        {
            _noteRepository = noteRepository;
            _donorRepository = donorRepository;
        }

        public async Task<NoteDto> GetAsync(Guid id)
        {
            var note = await _noteRepository.GetAsync(id);
            return ObjectMapper.Map<Note, NoteDto>(note);
        }

        public async Task<PagedResultDto<NoteDto>> GetListByDonorAsync(Guid donorId, PagedAndSortedResultRequestDto input)
        {
            var queryable = await _noteRepository.GetQueryableAsync();

            var query = queryable
                .Where(n => n.DonorId == donorId)
                .OrderBy(input.Sorting ?? nameof(Note.CreationTime))
                .Skip(input.SkipCount)
                .Take(input.MaxResultCount);

            var notes = await AsyncExecuter.ToListAsync(query);
            var totalCount = await _noteRepository.GetCountAsync();

            return new PagedResultDto<NoteDto>(
                totalCount,
                ObjectMapper.Map<List<Note>, List<NoteDto>>(notes)
            );
        }

        public async Task<NoteDto> CreateByDonorAsync(Guid donorId, CreateUpdateNoteDto input)
        {
            // Verifica che il Donor esista
            var donor = await _donorRepository.FindAsync(donorId);
            if (donor == null)
            {
                throw new UserFriendlyException("Donor not found.");
            }

            var note = new Note
            {
                DonorId = donorId,
                Content = input.Content,
                IsImportant = input.IsImportant,
                TenantId = CurrentTenant.Id
            };

            await _noteRepository.InsertAsync(note, autoSave: true);

            return ObjectMapper.Map<Note, NoteDto>(note);
        }

        public async Task<NoteDto> UpdateByDonorAsync(Guid donorId, Guid id, CreateUpdateNoteDto input)
        {
            var note = await _noteRepository.GetAsync(id);

            if (note.DonorId != donorId)
            {
                throw new UserFriendlyException("Note does not belong to the specified donor.");
            }

            note.Content = input.Content;
            note.IsImportant = input.IsImportant;

            await _noteRepository.UpdateAsync(note, autoSave: true);

            return ObjectMapper.Map<Note, NoteDto>(note);
        }

        public async Task DeleteAsync(Guid id)
        {
            await _noteRepository.DeleteAsync(id);
        }
    }
}

