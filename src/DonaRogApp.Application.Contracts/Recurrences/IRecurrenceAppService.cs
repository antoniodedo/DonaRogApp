using DonaRogApp.Application.Contracts.Recurrences.Dto;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace DonaRogApp.Application.Contracts.Recurrences
{
    /// <summary>
    /// Recurrence App Service Interface - manages recurrences with validity periods
    /// </summary>
    public interface IRecurrenceAppService :
        ICrudAppService<
            RecurrenceDto,
            Guid,
            GetRecurrencesInput,
            CreateRecurrenceDto,
            UpdateRecurrenceDto>
    {
        /// <summary>
        /// Get list of recurrences (simplified with RecurrenceListDto)
        /// </summary>
        Task<PagedResultDto<RecurrenceListDto>> GetRecurrenceListAsync(GetRecurrencesInput input);

        /// <summary>
        /// Deactivate a recurrence (disable it with reason)
        /// </summary>
        Task DeactivateAsync(Guid id, string reason);

        /// <summary>
        /// Reactivate a deactivated recurrence
        /// </summary>
        Task ReactivateAsync(Guid id);

        /// <summary>
        /// Get currently active recurrences (IsActive = true)
        /// </summary>
        Task<ListResultDto<RecurrenceListDto>> GetActiveRecurrencesAsync();
    }
}
