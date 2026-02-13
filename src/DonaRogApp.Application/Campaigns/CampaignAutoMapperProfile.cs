using AutoMapper;
using DonaRogApp.Application.Contracts.Campaigns.Dto;
using DonaRogApp.Domain.Campaigns.Entities;
using DonaRogApp.Domain.Donors.Entities;
using System.Linq;

namespace DonaRogApp.Application.Campaigns
{
    /// <summary>
    /// AutoMapper Profile for Campaign Aggregate
    /// Configuration for Entity <-> DTO mappings
    /// </summary>
    public class CampaignAutoMapperProfile : Profile
    {
        public CampaignAutoMapperProfile()
        {
            // ======================================================================
            // CAMPAIGN MAPPINGS
            // ======================================================================

            // Entity -> CampaignDto (full)
            CreateMap<Campaign, CampaignDto>()
                .ForMember(d => d.RecurrenceName, opt => opt.MapFrom(s => s.Recurrence != null ? s.Recurrence.Name : null))
                .ForMember(d => d.PostalCode674, opt => opt.MapFrom(s =>
                    s.PostalCode != null ? s.PostalCode.Value : null));

            // Entity -> CampaignListDto (simplified)
            CreateMap<Campaign, CampaignListDto>()
                .ForMember(d => d.RecurrenceName, opt => opt.MapFrom(s => s.Recurrence != null ? s.Recurrence.Name : null));

            // ======================================================================
            // CAMPAIGN DONOR MAPPINGS
            // ======================================================================

            // CampaignDonor -> CampaignDonorDto
            CreateMap<CampaignDonor, CampaignDonorDto>()
                .ForMember(d => d.DonorName, opt => opt.MapFrom(s => s.Donor.GetFullName()))
                .ForMember(d => d.DonorEmail, opt => opt.MapFrom(s =>
                    s.Donor.Emails.FirstOrDefault(e => e.IsDefault) != null ?
                        s.Donor.Emails.FirstOrDefault(e => e.IsDefault)!.EmailAddress :
                        s.Donor.Emails.FirstOrDefault() != null ?
                            s.Donor.Emails.FirstOrDefault()!.EmailAddress :
                            null))
                .ForMember(d => d.DonorPhone, opt => opt.MapFrom(s =>
                    s.Donor.Contacts.FirstOrDefault(c => c.IsDefault) != null ?
                        s.Donor.Contacts.FirstOrDefault(c => c.IsDefault)!.PhoneNumber.ToString() :
                        s.Donor.Contacts.FirstOrDefault() != null ?
                            s.Donor.Contacts.FirstOrDefault()!.PhoneNumber.ToString() :
                            null))
                .ForMember(d => d.DonorCity, opt => opt.MapFrom(s =>
                    s.Donor.Addresses.FirstOrDefault(a => a.IsDefault) != null ?
                        s.Donor.Addresses.FirstOrDefault(a => a.IsDefault)!.City :
                        s.Donor.Addresses.FirstOrDefault() != null ?
                            s.Donor.Addresses.FirstOrDefault()!.City :
                            null))
                .ForMember(d => d.DonorRegion, opt => opt.MapFrom(s =>
                    s.Donor.Addresses.FirstOrDefault(a => a.IsDefault) != null ?
                        s.Donor.Addresses.FirstOrDefault(a => a.IsDefault)!.Region :
                        s.Donor.Addresses.FirstOrDefault() != null ?
                            s.Donor.Addresses.FirstOrDefault()!.Region :
                            null))
                .ForMember(d => d.TrackingCode, opt => opt.MapFrom(s =>
                    s.TrackingCode != null ? s.TrackingCode.Value : null));

            // ======================================================================
            // DONOR PREVIEW MAPPINGS
            // ======================================================================

            // Donor -> DonorPreviewItemDto (for extraction preview)
            CreateMap<Donor, DonorPreviewItemDto>()
                .ForMember(d => d.Name, opt => opt.MapFrom(s => s.GetFullName()))
                .ForMember(d => d.Email, opt => opt.MapFrom(s =>
                    s.Emails.FirstOrDefault(e => e.IsDefault) != null ?
                        s.Emails.FirstOrDefault(e => e.IsDefault)!.EmailAddress :
                        s.Emails.FirstOrDefault() != null ?
                            s.Emails.FirstOrDefault()!.EmailAddress :
                            null))
                .ForMember(d => d.Phone, opt => opt.MapFrom(s =>
                    s.Contacts.FirstOrDefault(c => c.IsDefault) != null ?
                        s.Contacts.FirstOrDefault(c => c.IsDefault)!.PhoneNumber.ToString() :
                        s.Contacts.FirstOrDefault() != null ?
                            s.Contacts.FirstOrDefault()!.PhoneNumber.ToString() :
                            null))
                .ForMember(d => d.City, opt => opt.MapFrom(s =>
                    s.Addresses.FirstOrDefault(a => a.IsDefault) != null ?
                        s.Addresses.FirstOrDefault(a => a.IsDefault)!.City :
                        s.Addresses.FirstOrDefault() != null ?
                            s.Addresses.FirstOrDefault()!.City :
                            null))
                .ForMember(d => d.Region, opt => opt.MapFrom(s =>
                    s.Addresses.FirstOrDefault(a => a.IsDefault) != null ?
                        s.Addresses.FirstOrDefault(a => a.IsDefault)!.Region :
                        s.Addresses.FirstOrDefault() != null ?
                            s.Addresses.FirstOrDefault()!.Region :
                            null))
                .ForMember(d => d.LastDonationAmount, opt => opt.Ignore()) // TODO: Map from donation history
                .ForMember(d => d.LastDonationDate, opt => opt.Ignore()) // TODO: Map from donation history
                .ForMember(d => d.TotalDonations, opt => opt.Ignore()) // TODO: Map from donation history
                .ForMember(d => d.Tags, opt => opt.MapFrom(s =>
                    s.Tags.Where(t => t.RemovedAt == null)
                        .Select(t => t.Tag.Name)
                        .ToList()));
        }
    }
}
