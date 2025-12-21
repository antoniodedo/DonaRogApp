using AutoMapper;
using DonaRogApp.Donors.Dto;
using DonaRogApp.Donors.Entities;
using DonaRogApp.LetterTemplates;
using DonaRogApp.LetterTemplates.Dto;
using System.Linq;


namespace DonaRogApp.LetterTemplates
{
    public class LetterTemplateApplicationAutoMapperProfile : Profile
    {
        public LetterTemplateApplicationAutoMapperProfile()
        {
            CreateMap<LetterTemplate, LetterTemplateDto>();
            CreateMap<CreateUpdateLetterTemplateDto, LetterTemplate>();
        }
    }
}
