using System;
using Volo.Abp.Application.Dtos;

namespace DonaRogApp.Application.Contracts.Recurrences.Dto
{
    /// <summary>
    /// Recurrence DTO - full details
    /// </summary>
    public class RecurrenceDto : FullAuditedEntityDto<Guid>
    {
        public string Name { get; set; } = string.Empty;
        public string? Code { get; set; }
        public string? Description { get; set; }
        
        // Recurrence Date and Validity Range
        public int? RecurrenceDay { get; set; }
        public int? RecurrenceMonth { get; set; }
        public int DaysBeforeRecurrence { get; set; }
        public int DaysAfterRecurrence { get; set; }
        
        // Notes
        public string? Notes { get; set; }
        
        // Activation Status
        public bool IsActive { get; set; }
        public DateTime? DeactivatedDate { get; set; }
        public string? DeactivationReason { get; set; }
        
        // Calculated properties
        public int TotalValidityDays => DaysBeforeRecurrence + DaysAfterRecurrence + 1;
        public string FullDisplayName => RecurrenceDay.HasValue && RecurrenceMonth.HasValue
            ? $"{Name} ({RecurrenceDay:00}/{RecurrenceMonth:00})" 
            : Name;
    }
}
