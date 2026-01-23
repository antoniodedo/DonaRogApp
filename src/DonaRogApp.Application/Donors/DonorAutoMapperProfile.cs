using AutoMapper;
using DonaRogApp.Domain.Donors.Entities;
using DonaRogApp.Donors.Dtos;

namespace DonaRogApp.Donors
{
    /// <summary>
    /// AutoMapper Profile per Donor Aggregate
    /// Configurazione mappature Entity <-> DTO
    /// </summary>
    public class DonorAutoMapperProfile : Profile
    {
        public DonorAutoMapperProfile()
        {
            // ======================================================================
            // DONOR MAPPINGS
            // ======================================================================

            // Entity -> DTO
            CreateMap<Donor, DonorDto>()
                .ForMember(d => d.FullName, opt => opt.MapFrom(s => s.GetFullName()))
                .ForMember(d => d.TitleName, opt => opt.Ignore()) // Populate in AppService if needed
                .ForMember(d => d.Age, opt => opt.MapFrom(s =>
                    s.BirthDate.HasValue ?
                        ((System.DateTime.UtcNow.Year - s.BirthDate.Value.Year)) :
                        (int?)null
                ));

            // DTO -> Entity (for updates)
            CreateMap<UpdateDonorDto, Donor>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<CreateDonorDto, Donor>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // ======================================================================
            // DONOR EMAIL MAPPINGS
            // ======================================================================

            CreateMap<DonorEmail, DonorEmailDto>();
            CreateMap<CreateDonorEmailDto, DonorEmail>()
                .ForMember(d => d.Id, opt => opt.MapFrom(_ => System.Guid.NewGuid()))
                .ForMember(d => d.DonorId, opt => opt.Ignore())
                .ForMember(d => d.Donor, opt => opt.Ignore())
                .ForMember(d => d.DateAdded, opt => opt.MapFrom(_ => System.DateTime.UtcNow))
                .ForMember(d => d.IsDefault, opt => opt.MapFrom(_ => false))
                .ForMember(d => d.IsVerified, opt => opt.MapFrom(_ => false))
                .ForMember(d => d.BounceCount, opt => opt.MapFrom(_ => 0))
                .ForMember(d => d.IsInvalid, opt => opt.MapFrom(_ => false));

            // ======================================================================
            // DONOR CONTACT MAPPINGS
            // ======================================================================

            CreateMap<DonorContact, DonorContactDto>();
            CreateMap<CreateDonorContactDto, DonorContact>()
                .ForMember(d => d.Id, opt => opt.MapFrom(_ => System.Guid.NewGuid()))
                .ForMember(d => d.DonorId, opt => opt.Ignore())
                .ForMember(d => d.Donor, opt => opt.Ignore())
                .ForMember(d => d.DateAdded, opt => opt.MapFrom(_ => System.DateTime.UtcNow))
                .ForMember(d => d.IsDefault, opt => opt.MapFrom(_ => false))
                .ForMember(d => d.IsVerified, opt => opt.MapFrom(_ => false));

            // ======================================================================
            // DONOR ADDRESS MAPPINGS
            // ======================================================================

            CreateMap<DonorAddress, DonorAddressDto>()
                .ForMember(d => d.IsActive, opt => opt.MapFrom(s => !s.EndDate.HasValue));

            CreateMap<CreateDonorAddressDto, DonorAddress>()
                .ForMember(d => d.Id, opt => opt.MapFrom(_ => System.Guid.NewGuid()))
                .ForMember(d => d.DonorId, opt => opt.Ignore())
                .ForMember(d => d.Donor, opt => opt.Ignore())
                .ForMember(d => d.StartDate, opt => opt.MapFrom(_ => System.DateTime.UtcNow))
                .ForMember(d => d.IsDefault, opt => opt.MapFrom(_ => false))
                .ForMember(d => d.IsVerified, opt => opt.MapFrom(_ => false));
        }
    }

    // Helper DTOs for contacts
    public class DonorContactDto
    {
        public System.Guid Id { get; set; }
        public string PhoneNumber { get; set; }
        public DonaRogApp.Enums.Shared.ContactType Type { get; set; }
        public bool IsDefault { get; set; }
        public bool IsVerified { get; set; }
        public System.DateTime? VerifiedDate { get; set; }
        public System.DateTime DateAdded { get; set; }
        public string? Notes { get; set; }
    }

    public class CreateDonorContactDto
    {
        public string PhoneNumber { get; set; }
        public DonaRogApp.Enums.Shared.ContactType Type { get; set; } = DonaRogApp.Enums.Shared.ContactType.Mobile;
    }
}
