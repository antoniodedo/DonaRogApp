using AutoMapper.Internal.Mappers;
using DonaRogApp.Donors.Dto;
using DonaRogApp.Donors.Entities;
using DonaRogApp.Notes.Dto;
using DonaRogApp.Notes.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using System.Linq.Dynamic.Core;
using Volo.Abp.MultiTenancy;

namespace DonaRogApp.Donors
{
    public class EmailAppService : ApplicationService, IEmailAppService
    {
        private readonly IRepository<Email, Guid> _emailRepository;
        private readonly IRepository<Donor, Guid> _donorRepository;

        public EmailAppService(
            IRepository<Email, Guid> emailRepository,
            IRepository<Donor, Guid> donorRepository)
        {
            _emailRepository = emailRepository;
            _donorRepository = donorRepository;
        }

        public async Task<EmailDto> GetAsync(Guid id)
        {
            var email = await _emailRepository.GetAsync(id);
            return ObjectMapper.Map<Email, EmailDto>(email);
        }

        public async Task<PagedResultDto<EmailDto>> GetListByDonorAsync(Guid donorId, PagedAndSortedResultRequestDto input)
        {
            var queryable = await _emailRepository.GetQueryableAsync();

            var query = queryable
                .Where(n => n.DonorId == donorId)
                .OrderBy(input.Sorting ?? nameof(Email.CreationTime))
                .Skip(input.SkipCount)
                .Take(input.MaxResultCount);

            var emails = await AsyncExecuter.ToListAsync(query);
            var totalCount = await _emailRepository.GetCountAsync();

            return new PagedResultDto<EmailDto>(
                totalCount,
                ObjectMapper.Map<List<Email>, List<EmailDto>>(emails)
            );
        }

        public async Task<EmailDto> CreateByDonorAsync(Guid donorId, CreateUpdateEmailDto input)
        {
            // Verifica che il Donor esista
            var donor = await _donorRepository.FindAsync(donorId);
            if (donor == null)
            {
                throw new UserFriendlyException("Donor not found.");
            }

            var email = new Email
            {
                DonorId = donorId,
                Address = input.Address,
                IsPrimary = input.IsPrimary,
                TenantId = CurrentTenant.Id
            };

            await _emailRepository.InsertAsync(email, autoSave: true);

            return ObjectMapper.Map<Email, EmailDto>(email);
        }

        public async Task<EmailDto> UpdateByDonorAsync(Guid donorId, Guid id, CreateUpdateEmailDto input)
        {
            var email = await _emailRepository.GetAsync(id);

            if (email.DonorId != donorId)
            {
                throw new UserFriendlyException("Note does not belong to the specified donor.");
            }

            //var isExistEmail = await _emailRepository.FirstOrDefaultAsync(e => String.Compare(e.Address, input.Address) > 0);
            //if (isExistEmail != null)
            //{
            //    throw new UserFriendlyException("Email address is already exists");
            //}

            email.Address = input.Address;
            email.IsPrimary = input.IsPrimary;

            await _emailRepository.UpdateAsync(email, autoSave: true);

            return ObjectMapper.Map<Email, EmailDto>(email);
        }

        public async Task DeleteAsync(Guid id)
        {
            await _emailRepository.DeleteAsync(id);
        }
    }
}

