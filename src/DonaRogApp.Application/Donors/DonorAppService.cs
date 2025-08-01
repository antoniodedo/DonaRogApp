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
    public class DonorAppService : CrudAppService<
     Donor,
     DonorDto,
     Guid,
     PagedAndSortedResultRequestDto,
     CreateUpdateDonorDto,
     CreateUpdateDonorDto>,
     IDonorAppService
    {
        private readonly IRepository<Donor, Guid> _repository;
        private readonly IMapper _mapper;

        public DonorAppService(IRepository<Donor, Guid> repository, IMapper mapper)
            : base(repository)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<PagedResultDto<DonorListDto>> GetLightListAsync(PagedAndSortedResultRequestDto input)
        {
            var queryable = await Repository.GetQueryableAsync();

            var query = queryable
                .OrderBy(input.Sorting ?? nameof(Donor.LastName))
                .Skip(input.SkipCount)
                .Take(input.MaxResultCount);

            var donors = await AsyncExecuter.ToListAsync(query);
            var totalCount = await Repository.GetCountAsync();

            var items = ObjectMapper.Map<List<Donor>, List<DonorListDto>>(donors);

            return new PagedResultDto<DonorListDto>(totalCount, items);
        }
    }
}
