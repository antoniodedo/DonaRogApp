using System;
using Volo.Abp;

namespace DonaRogApp.Domain.Recurrences.Entities
{
    public partial class Recurrence
    {
        /// <summary>
        /// Update basic recurrence details
        /// </summary>
        public void UpdateDetails(
            string name,
            string? description = null,
            string? notes = null)
        {
            Name = Check.NotNullOrWhiteSpace(name, nameof(name), maxLength: 256);
            Description = description;
            Notes = notes;

            VerifyInvariants();
        }

        /// <summary>
        /// Update recurrence date and validity range
        /// </summary>
        public void UpdateValidityPeriod(
            int? recurrenceDay,
            int? recurrenceMonth,
            int daysBeforeRecurrence,
            int daysAfterRecurrence)
        {
            RecurrenceDay = recurrenceDay;
            RecurrenceMonth = recurrenceMonth;
            DaysBeforeRecurrence = daysBeforeRecurrence;
            DaysAfterRecurrence = daysAfterRecurrence;

            VerifyInvariants();
        }
    }
}
