using DonaRogApp.LetterTemplates.Dto;
using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace DonaRogApp.LetterTemplates
{
    public interface ILetterTemplateAppService :
    ICrudAppService< //Defines CRUD methods
        LetterTemplateDto,
        Guid,
        PagedAndSortedResultRequestDto,
        CreateUpdateLetterTemplateDto>
    {
    }
}





