using AutoMapper;
using DonaRogApp.Domain.Donors.Entities;
using DonaRogApp.Domain.Shared.Entities;
using DonaRogApp.Donors.Dto;
using DonaRogApp.Donors.Dtos;
using DonaRogApp.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;

namespace DonaRogApp.Donors
{
    /// <summary>
    /// Application Service per gestire i Donatori
    /// Implementa CRUD e operazioni su Email, Indirizzi, Privacy
    /// </summary>
    public class DonorAppService : CrudAppService<
        Donor,
        Dtos.DonorDto,
        Guid,
        GetDonorsInput,
        CreateDonorDto,
        UpdateDonorDto>,
        IDonorAppService
    {
        private readonly IRepository<Donor, Guid> _donorRepository;
        private readonly IRepository<Title, Guid> _titleRepository;
        private readonly IRepository<DonorStatusHistory, Guid> _statusHistoryRepository;
        private readonly IRepository<Tag, Guid> _tagRepository;
        private readonly IRepository<DonorTag> _donorTagRepository;
        private readonly IDataFilter _dataFilter;
        private readonly IMapper _mapper;

        public DonorAppService(
            IRepository<Donor, Guid> donorRepository,
            IRepository<Title, Guid> titleRepository,
            IRepository<DonorStatusHistory, Guid> statusHistoryRepository,
            IRepository<Tag, Guid> tagRepository,
            IRepository<DonorTag> donorTagRepository,
            IDataFilter dataFilter,
            IMapper mapper)
            : base(donorRepository)
        {
            _donorRepository = donorRepository;
            _titleRepository = titleRepository;
            _statusHistoryRepository = statusHistoryRepository;
            _tagRepository = tagRepository;
            _donorTagRepository = donorTagRepository;
            _dataFilter = dataFilter;
            _mapper = mapper;
        }

        // ======================================================================
        // OVERRIDE CRUD METHODS
        // ======================================================================

        /// <summary>
        /// Crea un nuovo donatore
        /// </summary>
        public override async Task<Dtos.DonorDto> CreateAsync(CreateDonorDto input)
        {
            // Validazione base
            ValidateCreateInput(input);

            Donor donor;

            // Crea donatore in base al tipo
            if (input.SubjectType == Enums.Donors.SubjectType.Individual)
            {
                var taxCode = input.TaxCode != null ? new TaxCode(input.TaxCode) : null;

                donor = Donor.CreateIndividual(
                    tenantId: CurrentTenant.Id,
                    firstName: input.FirstName!,
                    lastName: input.LastName!,
                    gender: input.Gender ?? Enums.Donors.Gender.Unspecified,
                    birthDate: input.BirthDate,
                    taxCode: taxCode,
                    titleId: input.TitleId,
                    email: input.Email,
                    phone: input.PhoneNumber
                );
            }
            else
            {
                var vatNumber = input.VatNumber != null ? new VatNumber(input.VatNumber) : null;
                var taxCode = input.TaxCode != null ? new TaxCode(input.TaxCode) : null;

                donor = Donor.CreateOrganization(
                    tenantId: CurrentTenant.Id,
                    companyName: input.CompanyName!,
                    organizationType: input.OrganizationType ?? Enums.Donors.OrganizationType.Other,
                    legalForm: input.LegalForm ?? Enums.Donors.LegalForm.Other,
                    vatNumber: vatNumber,
                    taxCode: taxCode != null ? taxCode.Value : null,
                    email: input.Email,
                    phone: input.PhoneNumber
                );
            }

            // Imposta proprietà aggiuntive
            donor.SetOrigin(input.Origin ?? Enums.Donors.DonorOrigin.Other);
            if (!string.IsNullOrWhiteSpace(input.PreferredLanguage))
            {
                donor.SetPreferredLanguage(input.PreferredLanguage);
            }
            if (!string.IsNullOrWhiteSpace(input.Notes))
            {
                donor.UpdateNotes(input.Notes);
            }

            // Salva
            var createdDonor = await _donorRepository.InsertAsync(donor, autoSave: true);

            return _mapper.Map<Donor, Dtos.DonorDto>(createdDonor);
        }

        /// <summary>
        /// Aggiorna un donatore
        /// </summary>
        public override async Task<Dtos.DonorDto> UpdateAsync(Guid id, UpdateDonorDto input)
        {
            var donor = await _donorRepository.GetAsync(id);

            if (donor.SubjectType == Enums.Donors.SubjectType.Individual)
            {
                if (!string.IsNullOrWhiteSpace(input.FirstName) || !string.IsNullOrWhiteSpace(input.LastName))
                {
                    donor.UpdateIndividualInfo(
                        firstName: input.FirstName ?? donor.FirstName!,
                        lastName: input.LastName ?? donor.LastName!,
                        titleId: input.TitleId ?? donor.TitleId,
                        gender: input.Gender ?? donor.Gender,
                        birthDate: input.BirthDate ?? donor.BirthDate,
                        birthPlace: input.BirthPlace ?? donor.BirthPlace
                    );
                }
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(input.CompanyName) ||
                    input.OrganizationType.HasValue ||
                    input.LegalForm.HasValue)
                {
                    donor.UpdateOrganizationInfo(
                        companyName: input.CompanyName ?? donor.CompanyName!,
                        organizationType: input.OrganizationType ?? donor.OrganizationType,
                        legalForm: input.LegalForm ?? donor.LegalForm,
                        businessSector: input.BusinessSector ?? donor.BusinessSector
                    );
                }
            }

            if (!string.IsNullOrWhiteSpace(input.PreferredLanguage))
            {
                donor.SetPreferredLanguage(input.PreferredLanguage);
            }

            if (!string.IsNullOrWhiteSpace(input.PreferredChannel))
            {
                donor.SetPreferredChannel(input.PreferredChannel);
            }

            if (input.Notes != null)
            {
                donor.UpdateNotes(input.Notes);
            }

            var updatedDonor = await _donorRepository.UpdateAsync(donor, autoSave: true);

            return _mapper.Map<Donor, Dtos.DonorDto>(updatedDonor);
        }

        /// <summary>
        /// Legge un donatore per ID
        /// </summary>
        public override async Task<Dtos.DonorDto> GetAsync(Guid id)
        {
            // Carica con le collezioni per popolare PrimaryEmail e PrimaryAddress
            var query = await _donorRepository.WithDetailsAsync(d => d.Emails, d => d.Addresses);
            var donor = await AsyncExecuter.FirstOrDefaultAsync(query.Where(d => d.Id == id));
            
            if (donor == null)
            {
                throw new Volo.Abp.UserFriendlyException("Donatore non trovato");
            }
            
            var dto = _mapper.Map<Donor, Dtos.DonorDto>(donor);
            
            // Popola manualmente il TitleName caricando dal repository
            if (donor.TitleId.HasValue)
            {
                var title = await _titleRepository.FindAsync(donor.TitleId.Value);
                if (title != null)
                {
                    dto.TitleName = title.Abbreviation;
                }
            }
            
            return dto;
        }

        /// <summary>
        /// Elenca donatori con paginazione e filtri di ricerca
        /// Ottimizzato per gestire 30.000+ donatori per tenant
        /// </summary>
        public override async Task<PagedResultDto<Dtos.DonorDto>> GetListAsync(GetDonorsInput input)
        {
            // Ottiene query base con details per Email, Addresses, Contacts (necessari per i filtri di ricerca)
            var queryable = await _donorRepository.WithDetailsAsync(d => d.Emails, d => d.Addresses, d => d.Contacts);
            
            // Applica filtri di ricerca
            queryable = ApplyFilters(queryable, input);
            
            // Conta totale DOPO i filtri (importante per paginazione corretta)
            var totalCount = await AsyncExecuter.CountAsync(queryable);
            
            // Applica ordinamento
            queryable = ApplySorting(queryable, input.Sorting);
            
            // Applica paginazione
            queryable = queryable
                .Skip(input.SkipCount)
                .Take(input.MaxResultCount);
            
            // Esegue query
            var donors = await AsyncExecuter.ToListAsync(queryable);

            // Carica tutti i titoli necessari in un'unica query batch
            var titleIds = donors
                .Where(d => d.TitleId.HasValue)
                .Select(d => d.TitleId!.Value)
                .Distinct()
                .ToList();
                
            var titles = titleIds.Any() 
                ? (await _titleRepository.GetListAsync())
                    .Where(t => titleIds.Contains(t.Id))
                    .ToDictionary(t => t.Id)
                : new Dictionary<Guid, Title>();

            // Mappa a DTO e aggiungi nomi titoli
            var dtos = donors.Select(d => {
                var dto = _mapper.Map<Donor, Dtos.DonorDto>(d);
                if (d.TitleId.HasValue && titles.TryGetValue(d.TitleId.Value, out var title))
                {
                    dto.TitleName = title.Abbreviation;
                }
                return dto;
            }).ToList();

            return new PagedResultDto<Dtos.DonorDto>(totalCount, dtos);
        }

        /// <summary>
        /// Applica filtri di ricerca alla query
        /// Ricerca su: Nome, Cognome, Ragione Sociale, Email, Codice Fiscale, P.IVA
        /// </summary>
        private IQueryable<Donor> ApplyFilters(IQueryable<Donor> query, GetDonorsInput input)
        {
            // Filtro testo generico
            if (!string.IsNullOrWhiteSpace(input.Filter))
            {
                var filter = input.Filter.Trim().ToLower();
                
                // Splitta il filtro in parole per ricerca multi-parola (es: "Mario Rossi")
                var words = filter.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                
                if (words.Length > 1)
                {
                    // Ricerca multi-parola: tutte le parole devono essere presenti
                    query = query.Where(d =>
                        // Concatena nome e cognome e cerca
                        ((d.FirstName != null && d.LastName != null && 
                          (d.FirstName + " " + d.LastName).ToLower().Contains(filter)) ||
                         (d.FirstName != null && d.MiddleName != null && d.LastName != null && 
                          (d.FirstName + " " + d.MiddleName + " " + d.LastName).ToLower().Contains(filter)) ||
                         // Oppure tutte le parole devono essere nei campi nome/cognome
                         words.All(word => 
                            (d.FirstName != null && d.FirstName.ToLower().Contains(word)) ||
                            (d.LastName != null && d.LastName.ToLower().Contains(word)) ||
                            (d.MiddleName != null && d.MiddleName.ToLower().Contains(word))
                         )) ||
                        // Organization: Ragione Sociale
                        (d.CompanyName != null && d.CompanyName.ToLower().Contains(filter)) ||
                        // Codici fiscali
                        (d.TaxCode != null && d.TaxCode.Value.ToLower().Contains(filter)) ||
                        (d.VatNumber != null && d.VatNumber.Value.ToLower().Contains(filter)) ||
                        // Email (SQL Server è case-insensitive di default)
                        d.Emails.Any(e => e.EmailAddress.Contains(filter)) ||
                        // Telefoni (cerca in NationalNumber, il campo mappato)
                        d.Contacts.Any(c => c.PhoneNumber != null && c.PhoneNumber.NationalNumber.Contains(filter)) ||
                        // Indirizzi (via, città, CAP, provincia, nazione)
                        d.Addresses.Any(a => 
                            (a.Street != null && a.Street.Contains(filter)) ||
                            (a.City != null && a.City.Contains(filter)) ||
                            (a.PostalCode != null && a.PostalCode.Contains(filter)) ||
                            (a.Province != null && a.Province.Contains(filter)) ||
                            (a.Country != null && a.Country.Contains(filter))
                        ) ||
                        // Codice donatore
                        d.DonorCode.ToLower().Contains(filter)
                    );
                }
                else
                {
                    // Ricerca singola parola
                    query = query.Where(d =>
                        // Individual: Nome + Cognome
                        (d.FirstName != null && d.FirstName.ToLower().Contains(filter)) ||
                        (d.LastName != null && d.LastName.ToLower().Contains(filter)) ||
                        (d.MiddleName != null && d.MiddleName.ToLower().Contains(filter)) ||
                        // Organization: Ragione Sociale
                        (d.CompanyName != null && d.CompanyName.ToLower().Contains(filter)) ||
                        // Codici fiscali
                        (d.TaxCode != null && d.TaxCode.Value.ToLower().Contains(filter)) ||
                        (d.VatNumber != null && d.VatNumber.Value.ToLower().Contains(filter)) ||
                        // Email
                        d.Emails.Any(e => e.EmailAddress.Contains(filter)) ||
                        // Telefoni (cerca in NationalNumber, il campo mappato)
                        d.Contacts.Any(c => c.PhoneNumber != null && c.PhoneNumber.NationalNumber.Contains(filter)) ||
                        // Indirizzi
                        d.Addresses.Any(a => 
                            (a.Street != null && a.Street.Contains(filter)) ||
                            (a.City != null && a.City.Contains(filter)) ||
                            (a.PostalCode != null && a.PostalCode.Contains(filter)) ||
                            (a.Province != null && a.Province.Contains(filter)) ||
                            (a.Country != null && a.Country.Contains(filter))
                        ) ||
                        // Codice donatore
                        d.DonorCode.ToLower().Contains(filter)
                    );
                }
            }

            // ======================================================================
            // FILTRI GENERALI
            // ======================================================================

            // Filtro tipo donatore
            if (input.SubjectType.HasValue)
            {
                query = query.Where(d => d.SubjectType == input.SubjectType.Value);
            }

            // Filtro stato
            if (input.Status.HasValue)
            {
                query = query.Where(d => d.Status == input.Status.Value);
            }

            // Filtro categoria
            if (input.Category.HasValue)
            {
                query = query.Where(d => d.Category == input.Category.Value);
            }

            // Filtro titolo
            if (input.TitleId.HasValue)
            {
                query = query.Where(d => d.TitleId == input.TitleId.Value);
            }

            // ======================================================================
            // FILTRI SPECIFICI - RICERCA AVANZATA
            // ======================================================================

            // Filtro codice donatore
            if (!string.IsNullOrWhiteSpace(input.DonorCode))
            {
                var donorCode = input.DonorCode.Trim().ToLower();
                query = query.Where(d => d.DonorCode.ToLower().Contains(donorCode));
            }

            // Filtro email (SQL Server è case-insensitive di default)
            if (!string.IsNullOrWhiteSpace(input.Email))
            {
                var email = input.Email.Trim().ToLower();
                query = query.Where(d => d.Emails.Any(e => e.EmailAddress.ToLower().Contains(email)));
            }

            // Filtro telefono (cerca in NationalNumber, il campo mappato)
            if (!string.IsNullOrWhiteSpace(input.PhoneNumber))
            {
                var phone = input.PhoneNumber.Trim();
                query = query.Where(d => d.Contacts.Any(c => c.PhoneNumber != null && c.PhoneNumber.NationalNumber.Contains(phone)));
            }

            // Filtro città
            if (!string.IsNullOrWhiteSpace(input.City))
            {
                var city = input.City.Trim().ToLower();
                query = query.Where(d => d.Addresses.Any(a => a.City != null && a.City.ToLower().Contains(city)));
            }

            // Filtro CAP
            if (!string.IsNullOrWhiteSpace(input.PostalCode))
            {
                var postalCode = input.PostalCode.Trim();
                query = query.Where(d => d.Addresses.Any(a => a.PostalCode != null && a.PostalCode.Contains(postalCode)));
            }

            // Filtro provincia
            if (!string.IsNullOrWhiteSpace(input.Province))
            {
                var province = input.Province.Trim().ToLower();
                query = query.Where(d => d.Addresses.Any(a => a.Province != null && a.Province.ToLower().Contains(province)));
            }

            // Filtro nazione
            if (!string.IsNullOrWhiteSpace(input.Country))
            {
                var country = input.Country.Trim().ToLower();
                query = query.Where(d => d.Addresses.Any(a => a.Country != null && a.Country.ToLower().Contains(country)));
            }

            return query;
        }

        /// <summary>
        /// Applica ordinamento dinamico alla query
        /// </summary>
        private IQueryable<Donor> ApplySorting(IQueryable<Donor> query, string? sorting)
        {
            if (string.IsNullOrWhiteSpace(sorting))
            {
                return query.OrderByDescending(d => d.CreationTime);
            }

            // Parse sorting (es: "FullName ASC", "CreationTime DESC")
            var sortParts = sorting.Trim().Split(' ');
            var sortField = sortParts[0];
            var sortDirection = sortParts.Length > 1 
                ? sortParts[1].ToUpper() 
                : "ASC";

            // Applica ordinamento in base al campo
            query = sortField switch
            {
                "FullName" or "fullName" => sortDirection == "DESC" 
                    ? query.OrderByDescending(d => d.FirstName).ThenByDescending(d => d.LastName)
                    : query.OrderBy(d => d.FirstName).ThenBy(d => d.LastName),
                    
                "CompanyName" or "companyName" => sortDirection == "DESC"
                    ? query.OrderByDescending(d => d.CompanyName)
                    : query.OrderBy(d => d.CompanyName),
                    
                "Status" or "status" => sortDirection == "DESC"
                    ? query.OrderByDescending(d => d.Status)
                    : query.OrderBy(d => d.Status),
                    
                "Category" or "category" => sortDirection == "DESC"
                    ? query.OrderByDescending(d => d.Category)
                    : query.OrderBy(d => d.Category),
                    
                "TotalDonated" or "totalDonated" => sortDirection == "DESC"
                    ? query.OrderByDescending(d => d.TotalDonated)
                    : query.OrderBy(d => d.TotalDonated),
                    
                "LastDonationDate" or "lastDonationDate" => sortDirection == "DESC"
                    ? query.OrderByDescending(d => d.LastDonationDate)
                    : query.OrderBy(d => d.LastDonationDate),
                    
                "CreationTime" or "creationTime" => sortDirection == "DESC"
                    ? query.OrderByDescending(d => d.CreationTime)
                    : query.OrderBy(d => d.CreationTime),
                    
                _ => query.OrderByDescending(d => d.CreationTime) // Default
            };

            return query;
        }

        // ======================================================================
        // EMAIL MANAGEMENT
        // ======================================================================

        /// <summary>
        /// Aggiunge un email a un donatore
        /// Nota: Include le email soft-deleted per verificare duplicati e riattivare
        /// </summary>
        public async Task AddEmailAsync(Guid donorId, CreateDonorEmailDto input)
        {
            // Disabilita il filtro soft delete per includere email eliminate
            using (DataFilter.Disable<ISoftDelete>())
            {
                var query = await _donorRepository.WithDetailsAsync(d => d.Emails);
                var donor = await AsyncExecuter.FirstOrDefaultAsync(query.Where(d => d.Id == donorId));
                
                if (donor == null)
                {
                    throw new Volo.Abp.UserFriendlyException("Donatore non trovato");
                }

                donor.AddEmail(input.EmailAddress, input.Type);
                await _donorRepository.UpdateAsync(donor, autoSave: true);
            }
        }

        /// <summary>
        /// Rimuove un email da un donatore
        /// </summary>
        public async Task RemoveEmailAsync(Guid donorId, string emailAddress)
        {
            var query = await _donorRepository.WithDetailsAsync(d => d.Emails);
            var donor = await AsyncExecuter.FirstOrDefaultAsync(query.Where(d => d.Id == donorId));
            
            if (donor == null)
                throw new Volo.Abp.UserFriendlyException("Donatore non trovato");
                
            donor.RemoveEmail(emailAddress);
            await _donorRepository.UpdateAsync(donor, autoSave: true);
        }

        /// <summary>
        /// Imposta un email come default
        /// </summary>
        public async Task SetDefaultEmailAsync(Guid donorId, string emailAddress)
        {
            var query = await _donorRepository.WithDetailsAsync(d => d.Emails);
            var donor = await AsyncExecuter.FirstOrDefaultAsync(query.Where(d => d.Id == donorId));
            
            if (donor == null)
                throw new Volo.Abp.UserFriendlyException("Donatore non trovato");
                
            donor.SetDefaultEmail(emailAddress);
            await _donorRepository.UpdateAsync(donor, autoSave: true);
        }

        /// <summary>
        /// Verifica un email
        /// </summary>
        public async Task VerifyEmailAsync(Guid donorId, string emailAddress)
        {
            var query = await _donorRepository.WithDetailsAsync(d => d.Emails);
            var donor = await AsyncExecuter.FirstOrDefaultAsync(query.Where(d => d.Id == donorId));
            
            if (donor == null)
                throw new Volo.Abp.UserFriendlyException("Donatore non trovato");
                
            donor.VerifyEmail(emailAddress);
            await _donorRepository.UpdateAsync(donor, autoSave: true);
        }

        /// <summary>
        /// Registra un bounce su un email
        /// </summary>
        public async Task RecordEmailBounceAsync(Guid donorId, string emailAddress, string? reason = null)
        {
            var donor = await _donorRepository.GetAsync(donorId);
            donor.RecordEmailBounce(emailAddress, reason);
            await _donorRepository.UpdateAsync(donor, autoSave: true);
        }

        /// <summary>
        /// Ottiene gli email di un donatore
        /// </summary>
        public async Task<List<DonorEmailDto>> GetEmailsAsync(Guid donorId)
        {
            var query = await _donorRepository.WithDetailsAsync(d => d.Emails);
            var donor = await AsyncExecuter.FirstOrDefaultAsync(query.Where(d => d.Id == donorId));
            
            if (donor == null)
            {
                return new List<DonorEmailDto>();
            }

            return donor.Emails.Select(e => _mapper.Map<DonorEmail, DonorEmailDto>(e)).ToList();
        }

        // ======================================================================
        // ADDRESS MANAGEMENT
        // ======================================================================

        /// <summary>
        /// Aggiunge un indirizzo a un donatore
        /// </summary>
        public async Task AddAddressAsync(Guid donorId, CreateDonorAddressDto input)
        {
            var query = await _donorRepository.WithDetailsAsync(d => d.Addresses);
            var donor = await AsyncExecuter.FirstOrDefaultAsync(query.Where(d => d.Id == donorId));
            
            if (donor == null)
            {
                throw new Volo.Abp.UserFriendlyException("Donatore non trovato");
            }

            donor.AddAddress(
                street: input.Street,
                city: input.City,
                postalCode: input.PostalCode,
                country: input.Country ?? "Italy",
                addressType: input.Type,
                province: input.Province,
                region: input.Region,
                notes: input.Notes
            );
            await _donorRepository.UpdateAsync(donor, autoSave: true);
        }

        /// <summary>
        /// Termina un indirizzo di un donatore
        /// </summary>
        public async Task EndAddressAsync(Guid donorId, Guid addressId)
        {
            var query = await _donorRepository.WithDetailsAsync(d => d.Addresses);
            var donor = await AsyncExecuter.FirstOrDefaultAsync(query.Where(d => d.Id == donorId));
            
            if (donor == null)
                throw new Volo.Abp.UserFriendlyException("Donatore non trovato");
                
            donor.EndAddress(addressId);
            await _donorRepository.UpdateAsync(donor, autoSave: true);
        }

        /// <summary>
        /// Imposta un indirizzo come default
        /// </summary>
        public async Task SetDefaultAddressAsync(Guid donorId, Guid addressId)
        {
            var query = await _donorRepository.WithDetailsAsync(d => d.Addresses);
            var donor = await AsyncExecuter.FirstOrDefaultAsync(query.Where(d => d.Id == donorId));
            
            if (donor == null)
                throw new Volo.Abp.UserFriendlyException("Donatore non trovato");
                
            donor.SetDefaultAddress(addressId);
            await _donorRepository.UpdateAsync(donor, autoSave: true);
        }

        /// <summary>
        /// Ottiene gli indirizzi di un donatore
        /// </summary>
        public async Task<List<DonorAddressDto>> GetAddressesAsync(Guid donorId)
        {
            var query = await _donorRepository.WithDetailsAsync(d => d.Addresses);
            var donor = await AsyncExecuter.FirstOrDefaultAsync(query.Where(d => d.Id == donorId));
            
            if (donor == null)
            {
                return new List<DonorAddressDto>();
            }

            return donor.Addresses
                .Where(a => !a.IsDeleted)
                .OrderByDescending(a => a.IsActive())
                .ThenByDescending(a => a.StartDate)
                .Select(a => _mapper.Map<DonorAddress, DonorAddressDto>(a))
                .ToList();
        }

        // ======================================================================
        // CONTACT MANAGEMENT
        // ======================================================================

        /// <summary>
        /// Aggiunge un contatto telefonico a un donatore
        /// </summary>
        public async Task AddContactAsync(Guid donorId, CreateDonorContactDto input)
        {
            var query = await _donorRepository.WithDetailsAsync(d => d.Contacts);
            var donor = await AsyncExecuter.FirstOrDefaultAsync(query.Where(d => d.Id == donorId));
            
            if (donor == null)
            {
                throw new Volo.Abp.UserFriendlyException("Donatore non trovato");
            }

            var phoneNumber = new PhoneNumber(input.PhoneNumber);
            donor.AddContact(phoneNumber, input.Type);
            await _donorRepository.UpdateAsync(donor, autoSave: true);
        }

        /// <summary>
        /// Rimuove un contatto telefonico da un donatore
        /// </summary>
        public async Task RemoveContactAsync(Guid donorId, string phoneNumber)
        {
            var query = await _donorRepository.WithDetailsAsync(d => d.Contacts);
            var donor = await AsyncExecuter.FirstOrDefaultAsync(query.Where(d => d.Id == donorId));
            
            if (donor == null)
                throw new Volo.Abp.UserFriendlyException("Donatore non trovato");
                
            donor.RemoveContact(phoneNumber);
            await _donorRepository.UpdateAsync(donor, autoSave: true);
        }

        /// <summary>
        /// Imposta un contatto come default
        /// </summary>
        public async Task SetDefaultContactAsync(Guid donorId, string phoneNumber)
        {
            var query = await _donorRepository.WithDetailsAsync(d => d.Contacts);
            var donor = await AsyncExecuter.FirstOrDefaultAsync(query.Where(d => d.Id == donorId));
            
            if (donor == null)
                throw new Volo.Abp.UserFriendlyException("Donatore non trovato");
                
            donor.SetDefaultContact(phoneNumber);
            await _donorRepository.UpdateAsync(donor, autoSave: true);
        }

        /// <summary>
        /// Ottiene i contatti di un donatore
        /// </summary>
        public async Task<List<DonorContactDto>> GetContactsAsync(Guid donorId)
        {
            var query = await _donorRepository.WithDetailsAsync(d => d.Contacts);
            var donor = await AsyncExecuter.FirstOrDefaultAsync(query.Where(d => d.Id == donorId));
            
            if (donor == null)
            {
                return new List<DonorContactDto>();
            }

            return donor.Contacts
                .Where(c => !c.IsDeleted)
                .Select(c => _mapper.Map<DonorContact, DonorContactDto>(c))
                .ToList();
        }

        // ======================================================================
        // PRIVACY MANAGEMENT
        // ======================================================================

        /// <summary>
        /// Concede consenso privacy
        /// Business rule: non consentito se stato = DoNotContact
        /// </summary>
        public async Task GrantPrivacyConsentAsync(Guid donorId)
        {
            var donor = await _donorRepository.GetAsync(donorId);
            
            // Business rule: non è possibile concedere privacy se stato è DoNotContact
            if (donor.Status == Enums.Donors.DonorStatus.DoNotContact)
            {
                throw new UserFriendlyException("Non è possibile concedere il consenso privacy quando lo stato è 'Non contattare'. Cambiare prima lo stato del donatore.");
            }
            
            donor.GrantPrivacyConsent();
            await _donorRepository.UpdateAsync(donor, autoSave: true);
        }

        /// <summary>
        /// Revoca consenso privacy
        /// </summary>
        public async Task RevokePrivacyConsentAsync(Guid donorId)
        {
            var donor = await _donorRepository.GetAsync(donorId);
            donor.RevokePrivacyConsent();
            await _donorRepository.UpdateAsync(donor, autoSave: true);
        }

        /// <summary>
        /// Concede consenso newsletter
        /// </summary>
        public async Task GrantNewsletterConsentAsync(Guid donorId)
        {
            var donor = await _donorRepository.GetAsync(donorId);
            donor.GrantNewsletterConsent();
            await _donorRepository.UpdateAsync(donor, autoSave: true);
        }

        /// <summary>
        /// Revoca consenso newsletter
        /// </summary>
        public async Task RevokeNewsletterConsentAsync(Guid donorId)
        {
            var donor = await _donorRepository.GetAsync(donorId);
            donor.RevokeNewsletterConsent();
            await _donorRepository.UpdateAsync(donor, autoSave: true);
        }

        /// <summary>
        /// Concede consenso spedizioni cartacee
        /// </summary>
        public async Task GrantMailConsentAsync(Guid donorId)
        {
            var donor = await _donorRepository.GetAsync(donorId);
            donor.GrantMailConsent();
            await _donorRepository.UpdateAsync(donor, autoSave: true);
        }

        /// <summary>
        /// Revoca consenso spedizioni cartacee
        /// </summary>
        public async Task RevokeMailConsentAsync(Guid donorId)
        {
            var donor = await _donorRepository.GetAsync(donorId);
            donor.RevokeMailConsent();
            await _donorRepository.UpdateAsync(donor, autoSave: true);
        }

        /// <summary>
        /// Anonimizza un donatore (GDPR)
        /// </summary>
        public async Task AnonymizeAsync(Guid donorId)
        {
            var donor = await _donorRepository.GetAsync(donorId);
            donor.Anonymize();
            await _donorRepository.UpdateAsync(donor, autoSave: true);
        }

        // ======================================================================
        // STATUS MANAGEMENT
        // ======================================================================

        /// <summary>
        /// Cambia lo stato del donatore
        /// Business rules:
        /// - Se lo stato diventa DoNotContact → revoca automaticamente il consenso privacy
        /// - Se lo stato esce da DoNotContact → riattiva automaticamente il consenso privacy
        /// </summary>
        public async Task ChangeStatusAsync(Guid donorId, int status, string? note = null)
        {
            var donor = await _donorRepository.GetAsync(donorId);
            var oldStatus = donor.Status;
            var newStatus = (Enums.Donors.DonorStatus)status;
            
            // Se lo stato non cambia, non fare nulla
            if (oldStatus == newStatus) return;
            
            // Business rule: uscendo da DoNotContact, riattiva il consenso privacy
            if (oldStatus == Enums.Donors.DonorStatus.DoNotContact && 
                newStatus != Enums.Donors.DonorStatus.DoNotContact &&
                !donor.PrivacyConsent)
            {
                donor.GrantPrivacyConsent();
            }
            
            donor.SetStatus(newStatus);
            
            // Business rule: entrando in DoNotContact, revoca il consenso privacy
            if (newStatus == Enums.Donors.DonorStatus.DoNotContact && donor.PrivacyConsent)
            {
                donor.RevokePrivacyConsent();
            }
            
            // Salva lo storico del cambio stato
            var history = DonorStatusHistory.Create(
                donorId,
                oldStatus,
                newStatus,
                note,
                CurrentTenant.Id
            );
            await _statusHistoryRepository.InsertAsync(history, autoSave: true);
            
            await _donorRepository.UpdateAsync(donor, autoSave: true);
        }

        /// <summary>
        /// Recupera lo storico dei cambi di stato
        /// </summary>
        public async Task<List<DonorStatusHistoryDto>> GetStatusHistoryAsync(Guid donorId)
        {
            var query = await _statusHistoryRepository.GetQueryableAsync();
            
            var histories = query
                .Where(h => h.DonorId == donorId)
                .OrderByDescending(h => h.ChangedAt)
                .ToList();

            return histories.Select(h => new DonorStatusHistoryDto
            {
                Id = h.Id,
                DonorId = h.DonorId,
                OldStatus = (int)h.OldStatus,
                NewStatus = (int)h.NewStatus,
                Note = h.Note,
                ChangedAt = h.ChangedAt,
                CreationTime = h.CreationTime,
                CreatorId = h.CreatorId
            }).ToList();
        }

        // ======================================================================
        // TAG MANAGEMENT
        // ======================================================================

        /// <summary>
        /// Recupera i tag assegnati a un donatore
        /// </summary>
        public async Task<List<DonorTagDto>> GetTagsAsync(Guid donorId)
        {
            // Query diretta sulla tabella DonorTag
            var donorTagQuery = await _donorTagRepository.GetQueryableAsync();
            var activeTags = donorTagQuery
                .Where(dt => dt.DonorId == donorId && dt.RemovedAt == null)
                .ToList();
            
            if (!activeTags.Any())
            {
                return new List<DonorTagDto>();
            }

            var tagIds = activeTags.Select(t => t.TagId).ToList();
            
            var tagQuery = await _tagRepository.GetQueryableAsync();
            var tags = tagQuery.Where(t => tagIds.Contains(t.Id)).ToDictionary(t => t.Id);

            return activeTags.Select(dt => new DonorTagDto
            {
                TagId = dt.TagId,
                TagName = tags.TryGetValue(dt.TagId, out var tag) ? tag.Name : "",
                TagCode = tags.TryGetValue(dt.TagId, out var t2) ? t2.Code : "",
                ColorCode = tags.TryGetValue(dt.TagId, out var t3) ? t3.ColorCode : null,
                Category = tags.TryGetValue(dt.TagId, out var t4) ? t4.Category : null,
                TaggedAt = dt.TaggedAt,
                IsAutomatic = dt.IsAutomatic,
                TaggingReason = dt.TaggingReason,
                Priority = dt.Priority
            }).OrderBy(t => t.TagName).ToList();
        }

        /// <summary>
        /// Assegna un tag a un donatore
        /// </summary>
        public async Task AddTagAsync(Guid donorId, AssignTagDto input)
        {
            var tagId = input.TagId;
            var donor = await _donorRepository.GetAsync(donorId, includeDetails: true);
            var tag = await _tagRepository.GetAsync(tagId);

            // Verifica che il tag non sia già assegnato
            var existingTag = donor.Tags.FirstOrDefault(t => t.TagId == tagId && t.IsActive);
            if (existingTag != null)
            {
                throw new UserFriendlyException($"Il tag '{tag.Name}' è già assegnato a questo donatore");
            }

            // Crea l'assegnazione usando reflection per il metodo internal
            var createMethod = typeof(DonorTag).GetMethod("CreateManual", 
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            
            var donorTag = (DonorTag)createMethod!.Invoke(null, new object?[] 
            { 
                donorId, 
                tagId, 
                CurrentTenant.Id, 
                3, // priority default
                "Assegnato manualmente", 
                CurrentUser.Id 
            })!;

            donor.Tags.Add(donorTag);
            
            // Incrementa il contatore di utilizzo del tag
            tag.IncrementUsageCount();
            
            await _donorRepository.UpdateAsync(donor, autoSave: true);
            await _tagRepository.UpdateAsync(tag, autoSave: true);
        }

        /// <summary>
        /// Rimuove un tag da un donatore
        /// </summary>
        public async Task DeleteTagAsync(Guid donorId, Guid tagId)
        {
            var donor = await _donorRepository.GetAsync(donorId, includeDetails: true);
            
            var donorTag = donor.Tags.FirstOrDefault(t => t.TagId == tagId && t.IsActive);
            if (donorTag == null)
            {
                throw new UserFriendlyException("Tag non trovato per questo donatore");
            }

            donorTag.Remove();
            
            // Decrementa il contatore di utilizzo del tag
            var tag = await _tagRepository.GetAsync(tagId);
            tag.DecrementUsageCount();
            
            await _donorRepository.UpdateAsync(donor, autoSave: true);
            await _tagRepository.UpdateAsync(tag, autoSave: true);
        }

        // ======================================================================
        // HELPERS
        // ======================================================================

        private void ValidateCreateInput(CreateDonorDto input)
        {
            if (input.SubjectType == Enums.Donors.SubjectType.Individual)
            {
                if (string.IsNullOrWhiteSpace(input.FirstName) || string.IsNullOrWhiteSpace(input.LastName))
                {
                    throw new Volo.Abp.UserFriendlyException("Nome e Cognome sono obbligatori per le persone fisiche");
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(input.CompanyName))
                {
                    throw new Volo.Abp.UserFriendlyException("Nome azienda è obbligatorio per le organizzazioni");
                }
            }
        }
    }
}
