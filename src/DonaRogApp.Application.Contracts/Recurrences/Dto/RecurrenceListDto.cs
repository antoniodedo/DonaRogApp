using System;
using Volo.Abp.Application.Dtos;

namespace DonaRogApp.Application.Contracts.Recurrences.Dto
{
    /// <summary>
    /// Recurrence DTO for lists - simplified
    /// </summary>
    public class RecurrenceListDto : EntityDto<Guid>
    {
        public string Name { get; set; } = string.Empty;
        public string? Code { get; set; }
        public int? RecurrenceDay { get; set; }
        public int? RecurrenceMonth { get; set; }
        public int DaysBeforeRecurrence { get; set; }
        public int DaysAfterRecurrence { get; set; }
        public bool IsActive { get; set; }
    }
}
