using AutoMapper;
using DonaRogApp.Domain.Donors.Entities;
using DonaRogApp.Donors.Dtos;
using DonaRogApp.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace DonaRogApp.Donors
{
    /// <summary>
    /// Application Service per gestire i Donatori
    /// Implementa CRUD e operazioni su Email, Indirizzi, Privacy
    /// </summary>
    public class DonorAppService : CrudAppService<
        Donor,
        DonorDto,
        Guid,
        PagedAndSortedResultRequestDto,
        CreateDonorDto,
        UpdateDonorDto>,
        IDonorAppService
    {
        private readonly IRepository<Donor, Guid> _donorRepository;
        private readonly IMapper _mapper;

        public DonorAppService(
            IRepository<Donor, Guid> donorRepository,
            IMapper mapper)
            : base(donorRepository)
        {
            _donorRepository = donorRepository;
            _mapper = mapper;
        }

        // ======================================================================
        // OVERRIDE CRUD METHODS
        // ======================================================================

        /// <summary>
        /// Crea un nuovo donatore
        /// </summary>
        public override async Task<DonorDto> CreateAsync(CreateDonorDto input)
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

            return _mapper.Map<Donor, DonorDto>(createdDonor);
        }

        /// <summary>
        /// Aggiorna un donatore
        /// </summary>
        public override async Task<DonorDto> UpdateAsync(Guid id, UpdateDonorDto input)
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

            return _mapper.Map<Donor, DonorDto>(updatedDonor);
        }

        /// <summary>
        /// Legge un donatore per ID
        /// </summary>
        public override async Task<DonorDto> GetAsync(Guid id)
        {
            var donor = await _donorRepository.GetAsync(id);
            return _mapper.Map<Donor, DonorDto>(donor);
        }

        /// <summary>
        /// Elenca donatori con paginazione
        /// </summary>
        public override async Task<PagedResultDto<DonorDto>> GetListAsync(PagedAndSortedResultRequestDto input)
        {
            var count = await _donorRepository.GetCountAsync();
            var donors = await _donorRepository.GetPagedListAsync(
                input.SkipCount,
                input.MaxResultCount,
                input.Sorting ?? "CreationTime DESC"
            );

            return new PagedResultDto<DonorDto>(
                count,
                donors.Select(d => _mapper.Map<Donor, DonorDto>(d)).ToList()
            );
        }

        // ======================================================================
        // EMAIL MANAGEMENT
        // ======================================================================

        /// <summary>
        /// Aggiunge un email a un donatore
        /// </summary>
        public async Task AddEmailAsync(Guid donorId, CreateDonorEmailDto input)
        {
            var donor = await _donorRepository.GetAsync(donorId);
            donor.AddEmail(input.EmailAddress, input.Type);
            await _donorRepository.UpdateAsync(donor, autoSave: true);
        }

        /// <summary>
        /// Rimuove un email da un donatore
        /// </summary>
        public async Task RemoveEmailAsync(Guid donorId, string emailAddress)
        {
            var donor = await _donorRepository.GetAsync(donorId);
            donor.RemoveEmail(emailAddress);
            await _donorRepository.UpdateAsync(donor, autoSave: true);
        }

        /// <summary>
        /// Imposta un email come default
        /// </summary>
        public async Task SetDefaultEmailAsync(Guid donorId, string emailAddress)
        {
            var donor = await _donorRepository.GetAsync(donorId);
            donor.SetDefaultEmail(emailAddress);
            await _donorRepository.UpdateAsync(donor, autoSave: true);
        }

        /// <summary>
        /// Verifica un email
        /// </summary>
        public async Task VerifyEmailAsync(Guid donorId, string emailAddress)
        {
            var donor = await _donorRepository.GetAsync(donorId);
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
            var donor = await _donorRepository.GetAsync(donorId);
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
            var donor = await _donorRepository.GetAsync(donorId);
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
            var donor = await _donorRepository.GetAsync(donorId);
            donor.EndAddress(addressId);
            await _donorRepository.UpdateAsync(donor, autoSave: true);
        }

        /// <summary>
        /// Imposta un indirizzo come default
        /// </summary>
        public async Task SetDefaultAddressAsync(Guid donorId, Guid addressId)
        {
            var donor = await _donorRepository.GetAsync(donorId);
            donor.SetDefaultAddress(addressId);
            await _donorRepository.UpdateAsync(donor, autoSave: true);
        }

        /// <summary>
        /// Ottiene gli indirizzi di un donatore
        /// </summary>
        public async Task<List<DonorAddressDto>> GetAddressesAsync(Guid donorId)
        {
            var donor = await _donorRepository.GetAsync(donorId);
            return donor.Addresses
                .Where(a => !a.IsDeleted)
                .Select(a => _mapper.Map<DonorAddress, DonorAddressDto>(a))
                .ToList();
        }

        // ======================================================================
        // PRIVACY MANAGEMENT
        // ======================================================================

        /// <summary>
        /// Concede consenso privacy
        /// </summary>
        public async Task GrantPrivacyConsentAsync(Guid donorId)
        {
            var donor = await _donorRepository.GetAsync(donorId);
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
        /// Anonimizza un donatore (GDPR)
        /// </summary>
        public async Task AnonymizeAsync(Guid donorId)
        {
            var donor = await _donorRepository.GetAsync(donorId);
            donor.Anonymize();
            await _donorRepository.UpdateAsync(donor, autoSave: true);
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
