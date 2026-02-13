using Volo.Abp.Application.Dtos;

namespace DonaRogApp.Application.Contracts.Recurrences.Dto
{
    /// <summary>
    /// Input DTO for querying recurrences
    /// </summary>
    public class GetRecurrencesInput : PagedAndSortedResultRequestDto
    {
        /// <summary>
        /// General search filter (name, code, description)
        /// </summary>
        public string? Filter { get; set; }

        /// <summary>
        /// Filter by activation status
        /// </summary>
        public bool? IsActive { get; set; }

        /// <summary>
        /// Filter recurrences that are currently in validity period
        /// </summary>
        public bool? IsCurrentlyInValidityPeriod { get; set; }
    }
}
