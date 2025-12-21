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

namespace DonaRogApp.Donors
{
    public class DonorTitleAppService : CrudAppService<
     DonorTitle,
     DonorTitleDto,
     Guid,
     PagedAndSortedResultRequestDto,
     CreateUpdateDonorTitleDto,
     CreateUpdateDonorTitleDto>,
     IDonorTitleAppService
    {
        private readonly IRepository<DonorTitle, Guid> _repository;
        private readonly IMapper _mapper;

        public DonorTitleAppService(IRepository<DonorTitle, Guid> repository, IMapper mapper)
            : base(repository)
        {
            _repository = repository;
            _mapper = mapper;
        }
    }
}
