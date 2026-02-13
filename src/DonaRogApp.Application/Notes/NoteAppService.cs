using DonaRogApp.Domain.Donors.Entities;
using DonaRogApp.Notes.Dto;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace DonaRogApp.Notes
{
    public class NoteAppService : ApplicationService, INoteAppService
    {
        private readonly IRepository<Donor, Guid> _donorRepository;
        private readonly IRepository<DonorNote, Guid> _noteRepository;

        public NoteAppService(
            IRepository<Donor, Guid> donorRepository,
            IRepository<DonorNote, Guid> noteRepository)
        {
            _donorRepository = donorRepository;
            _noteRepository = noteRepository;
        }

        public async Task<NoteDto> GetAsync(Guid id)
        {
            var note = await _noteRepository.GetAsync(id);
            return MapToDto(note);
        }

        public async Task<PagedResultDto<NoteDto>> GetListByDonorAsync(Guid donorId, PagedAndSortedResultRequestDto input)
        {
            var query = await _noteRepository.GetQueryableAsync();
            
            // Count totale
            var totalCount = query.Count(n => n.DonorId == donorId && !n.IsDeleted);

            // Paginazione con ordinamento: importanti prima, poi per data decrescente
            var pagedNotes = query
                .Where(n => n.DonorId == donorId && !n.IsDeleted)
                .OrderByDescending(n => n.IsImportant)
                .ThenByDescending(n => n.InteractionDate)
                .Skip(input.SkipCount)
                .Take(input.MaxResultCount)
                .ToList()
                .Select(MapToDto)
                .ToList();

            return new PagedResultDto<NoteDto>(totalCount, pagedNotes);
        }

        public async Task<NoteDto> CreateForDonorAsync(Guid donorId, CreateUpdateNoteDto input)
        {
            // Verifica che il donor esista
            await _donorRepository.GetAsync(donorId);

            // Usa reflection per chiamare il factory method internal
            var createMethod = typeof(DonorNote).GetMethod("Create", BindingFlags.Static | BindingFlags.NonPublic);
            
            var note = (DonorNote)createMethod!.Invoke(null, new object?[] {
                donorId,
                string.Empty, // title non più usato
                input.Content,
                CurrentTenant.Id,
                input.InteractionDate,
                input.Category,
                CurrentUser.Id,
                input.IsImportant,
                input.IsPrivate
            })!;

            await _noteRepository.InsertAsync(note, autoSave: true);

            return MapToDto(note);
        }

        public async Task<NoteDto> UpdateAsync(Guid id, CreateUpdateNoteDto input)
        {
            var note = await _noteRepository.GetAsync(id);

            // Usa reflection per chiamare i metodi internal
            var updateMethod = typeof(DonorNote).GetMethod("Update", BindingFlags.Instance | BindingFlags.NonPublic);
            updateMethod!.Invoke(note, new object?[] { string.Empty, input.Content, input.Category, input.InteractionDate });

            if (input.IsImportant != note.IsImportant)
            {
                var method = input.IsImportant 
                    ? typeof(DonorNote).GetMethod("MarkAsImportant", BindingFlags.Instance | BindingFlags.NonPublic)
                    : typeof(DonorNote).GetMethod("RemoveImportantFlag", BindingFlags.Instance | BindingFlags.NonPublic);
                method!.Invoke(note, null);
            }

            if (input.IsPrivate != note.IsPrivate)
            {
                var method = input.IsPrivate 
                    ? typeof(DonorNote).GetMethod("MarkAsPrivate", BindingFlags.Instance | BindingFlags.NonPublic)
                    : typeof(DonorNote).GetMethod("MakePublic", BindingFlags.Instance | BindingFlags.NonPublic);
                method!.Invoke(note, null);
            }

            await _noteRepository.UpdateAsync(note, autoSave: true);

            return MapToDto(note);
        }

        public async Task DeleteAsync(Guid id)
        {
            var note = await _noteRepository.GetAsync(id);
            
            var deleteMethod = typeof(DonorNote).GetMethod("Delete", BindingFlags.Instance | BindingFlags.NonPublic);
            deleteMethod!.Invoke(note, null);
            
            await _noteRepository.UpdateAsync(note, autoSave: true);
        }

        public async Task ToggleImportantAsync(Guid id)
        {
            var note = await _noteRepository.GetAsync(id);
            
            var method = note.IsImportant 
                ? typeof(DonorNote).GetMethod("RemoveImportantFlag", BindingFlags.Instance | BindingFlags.NonPublic)
                : typeof(DonorNote).GetMethod("MarkAsImportant", BindingFlags.Instance | BindingFlags.NonPublic);
            method!.Invoke(note, null);

            await _noteRepository.UpdateAsync(note, autoSave: true);
        }

        private NoteDto MapToDto(DonorNote note)
        {
            return new NoteDto
            {
                Id = note.Id,
                DonorId = note.DonorId,
                Content = note.Content,
                Category = note.Category,
                InteractionDate = note.InteractionDate,
                IsImportant = note.IsImportant,
                IsPrivate = note.IsPrivate,
                CreationTime = note.CreationTime,
                LastModificationTime = note.LastModificationTime,
                CreatorId = note.CreatorId,
                LastModifierId = note.LastModifierId
            };
        }
    }
}
