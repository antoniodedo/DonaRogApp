using AutoMapper;
using DonaRogApp.Donors.Dto;
using DonaRogApp.Donors.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using System.Linq.Dynamic.Core;
using DonaRogApp.LetterTemplates;
using DonaRogApp.LetterTemplates.Dto;

namespace DonaRogApp.LetterTemplates
{
    public class LetterTemplateAppService : CrudAppService<
     LetterTemplate,
     LetterTemplateDto,
     Guid,
     PagedAndSortedResultRequestDto,
     CreateUpdateLetterTemplateDto,
     CreateUpdateLetterTemplateDto>,
     ILetterTemplateAppService
    {
        private readonly IRepository<LetterTemplate, Guid> _repository;
        private readonly IMapper _mapper;

        public LetterTemplateAppService(IRepository<LetterTemplate, Guid> repository, IMapper mapper)
            : base(repository)
        {
            _repository = repository;
            _mapper = mapper;
        }
    }
}
