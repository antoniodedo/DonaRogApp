using AutoMapper;
using DonaRogApp.Donors.Dto;
using DonaRogApp.Donors.Entities;
using System.Linq;


namespace YourProject.Donors
{
    public class DonorApplicationAutoMapperProfile : Profile
    {
        public DonorApplicationAutoMapperProfile()
        {
            //CreateMap<Donor, DonorListDto>()
            //    .ForMember(dest => dest.AddressSummary, opt => opt.MapFrom(
            //        src => src.Addresses
            //            .Where(a => a.IsPrimary)
            //            .Select(a => $"{a.Dug} {a.Street} {a.CivicNumber}")
            //            .FirstOrDefault()
            //    ))
            //    .ForMember(dest => dest.CityName, opt => opt.MapFrom(
            //        src => src.Addresses
            //            .Where(a => a.IsPrimary)
            //            .Select(a => a.City)
            //            .FirstOrDefault()
            //    ))
            //    .ForMember(dest => dest.PostalCode, opt => opt.MapFrom(
            //        src => src.Addresses
            //            .Where(a => a.IsPrimary)
            //            .Select(a => a.PostalCode)
            //            .FirstOrDefault()
            //    ));
            CreateMap<Donor, DonorListDto>();
            CreateMap<Donor, DonorDto>();
            CreateMap<CreateUpdateDonorDto, Donor>();
        }
    }
}
