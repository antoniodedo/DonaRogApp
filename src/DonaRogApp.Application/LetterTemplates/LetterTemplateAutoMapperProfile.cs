using AutoMapper;
using DonaRogApp.LetterTemplates.Dto;

namespace DonaRogApp.Application.LetterTemplates
{
    public class LetterTemplateAutoMapperProfile : Profile
    {
        public LetterTemplateAutoMapperProfile()
        {
            // Entity -> DTO
            CreateMap<DonaRogApp.LetterTemplates.LetterTemplate, LetterTemplateDto>()
                .ForMember(dest => dest.AssociatedRules, opt => opt.Ignore()); // Populated separately

            CreateMap<DonaRogApp.LetterTemplates.TemplateAttachment, TemplateAttachmentDto>();

            // DTO -> Entity
            CreateMap<CreateUpdateLetterTemplateDto, DonaRogApp.LetterTemplates.LetterTemplate>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.TenantId, opt => opt.Ignore())
                .ForMember(dest => dest.UsageCount, opt => opt.Ignore())
                .ForMember(dest => dest.LastUsedDate, opt => opt.Ignore())
                .ForMember(dest => dest.Version, opt => opt.Ignore())
                .ForMember(dest => dest.PreviousVersionId, opt => opt.Ignore())
                .ForMember(dest => dest.Attachments, opt => opt.Ignore());
        }
    }
}
