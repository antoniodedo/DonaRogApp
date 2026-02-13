using System;

namespace DonaRogApp.Domain.Recurrences.Entities
{
    public partial class Recurrence
    {
        /// <summary>
        /// Create a new recurrence
        /// </summary>
        public static Recurrence Create(
            Guid id,
            Guid? tenantId,
            string name,
            string code,
            int? recurrenceDay,
            int? recurrenceMonth,
            int daysBeforeRecurrence,
            int daysAfterRecurrence,
            string? description = null)
        {
            var recurrence = new Recurrence(id, tenantId, name, code, recurrenceDay, recurrenceMonth, daysBeforeRecurrence, daysAfterRecurrence)
            {
                Description = description
            };

            return recurrence;
        }
    }
}
