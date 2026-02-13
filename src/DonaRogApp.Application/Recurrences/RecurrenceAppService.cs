using DonaRogApp.Application.Contracts.Recurrences;
using DonaRogApp.Application.Contracts.Recurrences.Dto;
using DonaRogApp.Domain.Recurrences.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace DonaRogApp.Application.Recurrences
{
    /// <summary>
    /// Application Service for managing Recurrences (annual recurring periods)
    /// </summary>
    public class RecurrenceAppService : CrudAppService<
        Recurrence,
        RecurrenceDto,
        Guid,
        GetRecurrencesInput,
        CreateRecurrenceDto,
        UpdateRecurrenceDto>,
        IRecurrenceAppService
    {
        private readonly IRepository<Recurrence, Guid> _recurrenceRepository;

        public RecurrenceAppService(IRepository<Recurrence, Guid> recurrenceRepository)
            : base(recurrenceRepository)
        {
            _recurrenceRepository = recurrenceRepository;
        }

        // ======================================================================
        // OVERRIDE CRUD METHODS
        // ======================================================================

        protected override async Task<IQueryable<Recurrence>> CreateFilteredQueryAsync(GetRecurrencesInput input)
        {
            var query = await base.CreateFilteredQueryAsync(input);

            // Apply filters
            query = ApplyFilters(query, input);

            return query;
        }

        private IQueryable<Recurrence> ApplyFilters(IQueryable<Recurrence> query, GetRecurrencesInput input)
        {
            // General search filter
            if (!string.IsNullOrWhiteSpace(input.Filter))
            {
                query = query.Where(e =>
                    e.Name.Contains(input.Filter) ||
                    e.Code.Contains(input.Filter) ||
                    (e.Description != null && e.Description.Contains(input.Filter)));
            }

            // IsActive filter
            if (input.IsActive.HasValue)
            {
                query = query.Where(e => e.IsActive == input.IsActive.Value);
            }

            // Note: IsCurrentlyInValidityPeriod filter removed - complex to query with day/month only
            // Can be filtered in memory if needed

            return query;
        }

        public override async Task<RecurrenceDto> CreateAsync(CreateRecurrenceDto input)
        {
            // Create recurrence
            var recurrence = Recurrence.Create(
                id: GuidGenerator.Create(),
                tenantId: CurrentTenant.Id,
                name: input.Name,
                code: input.Code,
                recurrenceDay: input.RecurrenceDay,
                recurrenceMonth: input.RecurrenceMonth,
                daysBeforeRecurrence: input.DaysBeforeRecurrence,
                daysAfterRecurrence: input.DaysAfterRecurrence,
                description: input.Description);

            if (!string.IsNullOrWhiteSpace(input.Notes))
            {
                recurrence.UpdateDetails(recurrence.Name, recurrence.Description, input.Notes);
            }

            await _recurrenceRepository.InsertAsync(recurrence);
            if (CurrentUnitOfWork != null)
            {
                await CurrentUnitOfWork.SaveChangesAsync();
            }

            return ObjectMapper.Map<Recurrence, RecurrenceDto>(recurrence);
        }

        public override async Task<RecurrenceDto> UpdateAsync(Guid id, UpdateRecurrenceDto input)
        {
            var recurrence = await _recurrenceRepository.GetAsync(id);

            // Update details
            recurrence.UpdateDetails(input.Name, input.Description, input.Notes);
            recurrence.UpdateValidityPeriod(input.RecurrenceDay, input.RecurrenceMonth, input.DaysBeforeRecurrence, input.DaysAfterRecurrence);

            await _recurrenceRepository.UpdateAsync(recurrence);
            if (CurrentUnitOfWork != null)
            {
                await CurrentUnitOfWork.SaveChangesAsync();
            }

            return ObjectMapper.Map<Recurrence, RecurrenceDto>(recurrence);
        }

        // ======================================================================
        // CUSTOM METHODS
        // ======================================================================

        public async Task<PagedResultDto<RecurrenceListDto>> GetRecurrenceListAsync(GetRecurrencesInput input)
        {
            var query = await CreateFilteredQueryAsync(input);

            var totalCount = await AsyncExecuter.CountAsync(query);

            query = ApplySorting(query, input);
            query = ApplyPaging(query, input);

            var recurrences = await AsyncExecuter.ToListAsync(query);

            return new PagedResultDto<RecurrenceListDto>(
                totalCount,
                ObjectMapper.Map<List<Recurrence>, List<RecurrenceListDto>>(recurrences));
        }

        public async Task DeactivateAsync(Guid id, string reason)
        {
            var recurrence = await _recurrenceRepository.GetAsync(id);
            recurrence.Deactivate(reason);
            await _recurrenceRepository.UpdateAsync(recurrence);
        }

        public async Task ReactivateAsync(Guid id)
        {
            var recurrence = await _recurrenceRepository.GetAsync(id);
            recurrence.Reactivate();
            await _recurrenceRepository.UpdateAsync(recurrence);
        }

        public async Task<ListResultDto<RecurrenceListDto>> GetActiveRecurrencesAsync()
        {
            var query = await _recurrenceRepository.GetQueryableAsync();
            var recurrences = await AsyncExecuter.ToListAsync(
                query.Where(e => e.IsActive)
                     .OrderBy(e => e.RecurrenceMonth ?? 13)
                     .ThenBy(e => e.RecurrenceDay ?? 32)
                     .ThenBy(e => e.Name));

            return new ListResultDto<RecurrenceListDto>(
                ObjectMapper.Map<List<Recurrence>, List<RecurrenceListDto>>(recurrences));
        }
    }
}
