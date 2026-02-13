using DonaRogApp.Domain.Donors.Entities;
using DonaRogApp.Domain.Shared.Entities;
using DonaRogApp.Enums.Donors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace DonaRogApp.Titles
{
    /// <summary>
    /// Service per gestire i titoli di cortesia (Sig., Dott., etc.)
    /// </summary>
    public class TitleAppService : ApplicationService, ITitleAppService
    {
        private readonly IRepository<Title, Guid> _titleRepository;
        private readonly IRepository<Donor, Guid> _donorRepository;

        public TitleAppService(
            IRepository<Title, Guid> titleRepository,
            IRepository<Donor, Guid> donorRepository)
        {
            _titleRepository = titleRepository;
            _donorRepository = donorRepository;
        }

        /// <summary>
        /// Ottiene la lista di tutti i titoli attivi (per dropdown)
        /// </summary>
        public async Task<ListResultDto<TitleDto>> GetAllAsync()
        {
            var titles = await _titleRepository.GetListAsync();
            
            var activeTitles = titles
                .Where(t => t.IsActive)
                .OrderBy(t => t.DisplayOrder)
                .ThenBy(t => t.Name)
                .Select(t => new TitleDto
                {
                    Id = t.Id,
                    Code = t.Code,
                    Name = t.Name,
                    Abbreviation = t.Abbreviation,
                    AssociatedGender = t.AssociatedGender,
                    DisplayOrder = t.DisplayOrder,
                    IsDefault = t.IsDefault,
                    IsActive = t.IsActive
                })
                .ToList();

            return new ListResultDto<TitleDto>(activeTitles);
        }

        /// <summary>
        /// Ottiene la lista di tutti i titoli con statistiche (per admin)
        /// Include anche i titoli disattivati
        /// </summary>
        public async Task<TitleListWithStatsDto> GetAllWithStatsAsync()
        {
            var titles = await _titleRepository.GetListAsync();
            var donors = await _donorRepository.GetListAsync();

            // Conteggio donatori per titolo
            var donorCountByTitle = donors
                .Where(d => d.TitleId.HasValue)
                .GroupBy(d => d.TitleId!.Value)
                .ToDictionary(g => g.Key, g => g.Count());

            // Conteggio donatori senza titolo
            var donorsWithoutTitle = donors.Count(d => !d.TitleId.HasValue);

            var titleDtos = titles
                .OrderBy(t => t.DisplayOrder)
                .ThenBy(t => t.Name)
                .Select(t => new TitleWithStatsDto
                {
                    Id = t.Id,
                    Code = t.Code,
                    Name = t.Name,
                    Abbreviation = t.Abbreviation,
                    AssociatedGender = t.AssociatedGender,
                    DisplayOrder = t.DisplayOrder,
                    IsDefault = t.IsDefault,
                    IsActive = t.IsActive,
                    Notes = t.Notes,
                    DonorCount = donorCountByTitle.TryGetValue(t.Id, out var count) ? count : 0
                })
                .ToList();

            return new TitleListWithStatsDto
            {
                Items = titleDtos,
                DonorsWithoutTitle = donorsWithoutTitle,
                TotalDonors = donors.Count
            };
        }

        /// <summary>
        /// Ottiene un titolo per ID
        /// </summary>
        public async Task<TitleDto?> GetByIdAsync(Guid id)
        {
            var title = await _titleRepository.FindAsync(id);
            
            if (title == null)
                return null;

            return new TitleDto
            {
                Id = title.Id,
                Code = title.Code,
                Name = title.Name,
                Abbreviation = title.Abbreviation,
                AssociatedGender = title.AssociatedGender,
                DisplayOrder = title.DisplayOrder,
                IsDefault = title.IsDefault,
                IsActive = title.IsActive,
                Notes = title.Notes
            };
        }

        /// <summary>
        /// Crea un nuovo titolo
        /// </summary>
        public async Task<TitleDto> CreateAsync(CreateUpdateTitleDto input)
        {
            // Verifica codice univoco
            var existing = (await _titleRepository.GetListAsync())
                .FirstOrDefault(t => t.Code.Equals(input.Code, StringComparison.OrdinalIgnoreCase));
            
            if (existing != null)
            {
                throw new UserFriendlyException($"Esiste già un titolo con codice '{input.Code}'");
            }

            var title = Title.Create(
                input.Code,
                input.Name,
                input.Abbreviation,
                input.AssociatedGender,
                input.DisplayOrder
            );

            if (!string.IsNullOrEmpty(input.Notes))
            {
                title.UpdateNotes(input.Notes);
            }

            await _titleRepository.InsertAsync(title, autoSave: true);

            return new TitleDto
            {
                Id = title.Id,
                Code = title.Code,
                Name = title.Name,
                Abbreviation = title.Abbreviation,
                AssociatedGender = title.AssociatedGender,
                DisplayOrder = title.DisplayOrder,
                IsDefault = title.IsDefault,
                IsActive = title.IsActive,
                Notes = title.Notes
            };
        }

        /// <summary>
        /// Aggiorna un titolo esistente
        /// </summary>
        public async Task<TitleDto> UpdateAsync(Guid id, CreateUpdateTitleDto input)
        {
            var title = await _titleRepository.GetAsync(id);

            // Verifica codice univoco (escludendo se stesso)
            var existing = (await _titleRepository.GetListAsync())
                .FirstOrDefault(t => t.Code.Equals(input.Code, StringComparison.OrdinalIgnoreCase) && t.Id != id);
            
            if (existing != null)
            {
                throw new UserFriendlyException($"Esiste già un titolo con codice '{input.Code}'");
            }

            title.Update(
                input.Code,
                input.Name,
                input.Abbreviation,
                input.AssociatedGender,
                input.DisplayOrder,
                input.Notes
            );

            await _titleRepository.UpdateAsync(title, autoSave: true);

            return new TitleDto
            {
                Id = title.Id,
                Code = title.Code,
                Name = title.Name,
                Abbreviation = title.Abbreviation,
                AssociatedGender = title.AssociatedGender,
                DisplayOrder = title.DisplayOrder,
                IsDefault = title.IsDefault,
                IsActive = title.IsActive,
                Notes = title.Notes
            };
        }

        /// <summary>
        /// Disattiva un titolo (soft delete) e riassegna i donatori
        /// </summary>
        public async Task<DeleteTitleResultDto> DeleteAsync(Guid id, Guid? replacementTitleId = null)
        {
            var title = await _titleRepository.GetAsync(id);
            var donors = await _donorRepository.GetListAsync();
            
            // Trova donatori con questo titolo
            var affectedDonors = donors.Where(d => d.TitleId == id).ToList();
            var affectedCount = affectedDonors.Count;

            if (affectedCount > 0)
            {
                Guid? newTitleId = replacementTitleId;

                // Se non è specificato un titolo sostitutivo, usa il predefinito del genere
                if (!newTitleId.HasValue)
                {
                    var allTitles = await _titleRepository.GetListAsync();
                    
                    foreach (var donor in affectedDonors)
                    {
                        // Trova il titolo predefinito per il genere del donatore
                        var defaultTitle = allTitles.FirstOrDefault(t => 
                            t.IsDefault && 
                            t.IsActive && 
                            t.AssociatedGender == donor.Gender &&
                            t.Id != id);

                        if (defaultTitle != null)
                        {
                            donor.UpdateIndividualInfo(
                                firstName: donor.FirstName!,
                                lastName: donor.LastName!,
                                titleId: defaultTitle.Id,
                                gender: donor.Gender,
                                birthDate: donor.BirthDate,
                                birthPlace: donor.BirthPlace
                            );
                        }
                        else
                        {
                            // Nessun predefinito trovato, rimuovi il titolo
                            donor.UpdateIndividualInfo(
                                firstName: donor.FirstName!,
                                lastName: donor.LastName!,
                                titleId: null,
                                gender: donor.Gender,
                                birthDate: donor.BirthDate,
                                birthPlace: donor.BirthPlace
                            );
                        }
                        
                        await _donorRepository.UpdateAsync(donor);
                    }
                }
                else
                {
                    // Usa il titolo sostitutivo specificato
                    foreach (var donor in affectedDonors)
                    {
                        donor.UpdateIndividualInfo(
                            firstName: donor.FirstName!,
                            lastName: donor.LastName!,
                            titleId: newTitleId,
                            gender: donor.Gender,
                            birthDate: donor.BirthDate,
                            birthPlace: donor.BirthPlace
                        );
                        await _donorRepository.UpdateAsync(donor);
                    }
                }
            }

            // Se era il predefinito, rimuovi lo stato
            if (title.IsDefault)
            {
                title.RemoveDefault();
            }

            // Soft delete
            title.Deactivate();
            await _titleRepository.UpdateAsync(title, autoSave: true);

            return new DeleteTitleResultDto
            {
                Success = true,
                AffectedDonors = affectedCount,
                Message = affectedCount > 0 
                    ? $"Titolo disattivato. {affectedCount} donatori sono stati aggiornati."
                    : "Titolo disattivato."
            };
        }

        /// <summary>
        /// Riattiva un titolo
        /// </summary>
        public async Task ReactivateAsync(Guid id)
        {
            var title = await _titleRepository.GetAsync(id);
            title.Activate();
            await _titleRepository.UpdateAsync(title, autoSave: true);
        }

        /// <summary>
        /// Imposta un titolo come predefinito per il suo genere
        /// </summary>
        public async Task SetAsDefaultAsync(Guid id)
        {
            var title = await _titleRepository.GetAsync(id);
            
            if (!title.AssociatedGender.HasValue)
            {
                throw new UserFriendlyException("Solo i titoli con un genere associato possono essere predefiniti");
            }

            // Rimuovi predefinito da altri titoli dello stesso genere
            var allTitles = await _titleRepository.GetListAsync();
            var sameGenderDefaults = allTitles.Where(t => 
                t.AssociatedGender == title.AssociatedGender && 
                t.IsDefault && 
                t.Id != id);

            foreach (var t in sameGenderDefaults)
            {
                t.RemoveDefault();
                await _titleRepository.UpdateAsync(t);
            }

            // Imposta il nuovo predefinito
            title.SetAsDefault();
            await _titleRepository.UpdateAsync(title, autoSave: true);
        }

        /// <summary>
        /// Rimuove lo stato di predefinito da un titolo
        /// </summary>
        public async Task RemoveDefaultAsync(Guid id)
        {
            var title = await _titleRepository.GetAsync(id);
            title.RemoveDefault();
            await _titleRepository.UpdateAsync(title, autoSave: true);
        }

        /// <summary>
        /// Ottiene info per preview eliminazione
        /// </summary>
        public async Task<DeleteTitlePreviewDto> GetDeletePreviewAsync(Guid id)
        {
            var title = await _titleRepository.GetAsync(id);
            var donors = await _donorRepository.GetListAsync();
            var allTitles = await _titleRepository.GetListAsync();

            var affectedDonors = donors.Where(d => d.TitleId == id).ToList();
            
            // Raggruppa per genere
            var byGender = affectedDonors
                .GroupBy(d => d.Gender)
                .Select(g => new AffectedDonorsByGenderDto
                {
                    Gender = g.Key,
                    Count = g.Count(),
                    DefaultTitleId = allTitles
                        .FirstOrDefault(t => t.IsDefault && t.IsActive && t.AssociatedGender == g.Key && t.Id != id)?.Id,
                    DefaultTitleName = allTitles
                        .FirstOrDefault(t => t.IsDefault && t.IsActive && t.AssociatedGender == g.Key && t.Id != id)?.Abbreviation
                })
                .ToList();

            // Titoli alternativi disponibili
            var alternatives = allTitles
                .Where(t => t.IsActive && t.Id != id)
                .OrderBy(t => t.DisplayOrder)
                .Select(t => new TitleDto
                {
                    Id = t.Id,
                    Code = t.Code,
                    Name = t.Name,
                    Abbreviation = t.Abbreviation,
                    AssociatedGender = t.AssociatedGender
                })
                .ToList();

            return new DeleteTitlePreviewDto
            {
                TitleId = id,
                TitleName = title.Abbreviation,
                TotalAffectedDonors = affectedDonors.Count,
                AffectedByGender = byGender,
                AlternativeTitles = alternatives
            };
        }
    }

    #region DTOs

    public class TitleDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Abbreviation { get; set; } = string.Empty;
        public Gender? AssociatedGender { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; }
        public string? Notes { get; set; }
    }

    public class TitleWithStatsDto : TitleDto
    {
        public int DonorCount { get; set; }
    }

    public class TitleListWithStatsDto
    {
        public List<TitleWithStatsDto> Items { get; set; } = new();
        public int DonorsWithoutTitle { get; set; }
        public int TotalDonors { get; set; }
    }

    public class CreateUpdateTitleDto
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Abbreviation { get; set; } = string.Empty;
        public Gender? AssociatedGender { get; set; }
        public int DisplayOrder { get; set; }
        public string? Notes { get; set; }
    }

    public class DeleteTitleResultDto
    {
        public bool Success { get; set; }
        public int AffectedDonors { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class DeleteTitlePreviewDto
    {
        public Guid TitleId { get; set; }
        public string TitleName { get; set; } = string.Empty;
        public int TotalAffectedDonors { get; set; }
        public List<AffectedDonorsByGenderDto> AffectedByGender { get; set; } = new();
        public List<TitleDto> AlternativeTitles { get; set; } = new();
    }

    public class AffectedDonorsByGenderDto
    {
        public Gender? Gender { get; set; }
        public int Count { get; set; }
        public Guid? DefaultTitleId { get; set; }
        public string? DefaultTitleName { get; set; }
    }

    public interface ITitleAppService
    {
        Task<ListResultDto<TitleDto>> GetAllAsync();
        Task<TitleListWithStatsDto> GetAllWithStatsAsync();
        Task<TitleDto?> GetByIdAsync(Guid id);
        Task<TitleDto> CreateAsync(CreateUpdateTitleDto input);
        Task<TitleDto> UpdateAsync(Guid id, CreateUpdateTitleDto input);
        Task<DeleteTitleResultDto> DeleteAsync(Guid id, Guid? replacementTitleId = null);
        Task ReactivateAsync(Guid id);
        Task SetAsDefaultAsync(Guid id);
        Task RemoveDefaultAsync(Guid id);
        Task<DeleteTitlePreviewDto> GetDeletePreviewAsync(Guid id);
    }

    #endregion
}
