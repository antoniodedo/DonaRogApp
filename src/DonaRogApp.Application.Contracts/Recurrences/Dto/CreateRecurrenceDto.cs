using System;
using System.ComponentModel.DataAnnotations;

namespace DonaRogApp.Application.Contracts.Recurrences.Dto
{
    /// <summary>
    /// DTO for creating a new recurrence
    /// </summary>
    public class CreateRecurrenceDto
    {
        [Required]
        [StringLength(256)]
        public string Name { get; set; } = string.Empty;

        [StringLength(64)]
        public string? Code { get; set; }

        [StringLength(2000)]
        public string? Description { get; set; }

        /// <summary>
        /// Day of the recurrence (1-31, optional - e.g., 25 for Christmas)
        /// </summary>
        [Range(1, 31)]
        public int? RecurrenceDay { get; set; }

        /// <summary>
        /// Month of the recurrence (1-12, optional - e.g., 12 for December)
        /// </summary>
        [Range(1, 12)]
        public int? RecurrenceMonth { get; set; }

        /// <summary>
        /// Number of days before the recurrence when campaigns are valid
        /// </summary>
        [Range(0, 365)]
        public int DaysBeforeRecurrence { get; set; } = 30;

        /// <summary>
        /// Number of days after the recurrence when campaigns are valid
        /// </summary>
        [Range(0, 365)]
        public int DaysAfterRecurrence { get; set; } = 7;

        [StringLength(2000)]
        public string? Notes { get; set; }
    }
}
