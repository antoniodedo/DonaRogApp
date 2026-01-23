using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Timing;

namespace DonaRogApp.ValueObjects
{
    /// <summary>
    /// Value Object: Date Range (Period)
    /// Represents a time period with start and optional end date.
    /// Immutable, self-validating.
    /// </summary>
    public class DateRange : ValueObject
    {
        // --------------------------------------------------------------
        // PROPERTIES
        // --------------------------------------------------------------

        /// <summary>
        /// Start date of the period (inclusive)
        /// </summary>
        public DateTime StartDate { get; }

        /// <summary>
        /// End date of the period (inclusive)
        /// NULL = open-ended period (still active)
        /// </summary>
        public DateTime? EndDate { get; }

        // --------------------------------------------------------------
        // CONSTRUCTOR
        // --------------------------------------------------------------

        private DateRange()
        {
            // EF Core needs parameterless constructor
        }

        public DateRange(DateTime startDate, DateTime? endDate = null)
        {
            // Validation
            if (endDate.HasValue && startDate > endDate.Value)
                throw new ArgumentException(
                    $"Start date ({startDate:yyyy-MM-dd}) cannot be after end date ({endDate.Value:yyyy-MM-dd})",
                    nameof(startDate)
                );

            StartDate = startDate.Date; // Remove time component
            EndDate = endDate?.Date;
        }

        // --------------------------------------------------------------
        // STATIC FACTORIES
        // --------------------------------------------------------------

        /// <summary>
        /// Creates a date range starting from today with no end date.
        /// </summary>
        public static DateRange StartingToday() => new DateRange(DateTime.UtcNow.Date);

        /// <summary>
        /// Creates a date range for a specific month.
        /// </summary>
        public static DateRange ForMonth(int year, int month)
        {
            var start = new DateTime(year, month, 1);
            var end = start.AddMonths(1).AddDays(-1);
            return new DateRange(start, end);
        }

        /// <summary>
        /// Creates a date range for a specific year.
        /// </summary>
        public static DateRange ForYear(int year)
        {
            var start = new DateTime(year, 1, 1);
            var end = new DateTime(year, 12, 31);
            return new DateRange(start, end);
        }

        /// <summary>
        /// Creates a date range for current month.
        /// </summary>
        public static DateRange CurrentMonth()
        {
            var today = DateTime.UtcNow;
            return ForMonth(today.Year, today.Month);
        }

        /// <summary>
        /// Creates a date range for current year.
        /// </summary>
        public static DateRange CurrentYear()
        {
            return ForYear(DateTime.UtcNow.Year);
        }

        // --------------------------------------------------------------
        // QUERY METHODS
        // --------------------------------------------------------------

        /// <summary>
        /// Checks if the period is currently active (no end date or end date in future).
        /// </summary>
        public bool IsActive()
        {
            return !EndDate.HasValue || EndDate.Value >= DateTime.UtcNow.Date;
        }

        /// <summary>
        /// Checks if the period has ended.
        /// </summary>
        public bool HasEnded()
        {
            return EndDate.HasValue && EndDate.Value < DateTime.UtcNow.Date;
        }

        /// <summary>
        /// Checks if a date is within this period (inclusive).
        /// </summary>
        public bool Contains(DateTime date)
        {
            var dateOnly = date.Date;

            if (dateOnly < StartDate)
                return false;

            if (EndDate.HasValue && dateOnly > EndDate.Value)
                return false;

            return true;
        }

        /// <summary>
        /// Checks if another date range overlaps with this one.
        /// </summary>
        public bool Overlaps(DateRange other)
        {
            Check.NotNull(other, nameof(other));

            // Case 1: This range has no end date
            if (!EndDate.HasValue)
                return other.EndDate.HasValue
                    ? other.EndDate.Value >= StartDate
                    : other.StartDate >= StartDate;

            // Case 2: Other range has no end date
            if (!other.EndDate.HasValue)
                return other.StartDate <= EndDate.Value;

            // Case 3: Both have end dates
            return StartDate <= other.EndDate.Value &&
                   other.StartDate <= EndDate.Value;
        }

        /// <summary>
        /// Calculates duration in days.
        /// Returns NULL if period is open-ended.
        /// </summary>
        public int? GetDurationInDays()
        {
            if (!EndDate.HasValue)
                return null;

            return (EndDate.Value - StartDate).Days + 1; // +1 to include both start and end
        }

        /// <summary>
        /// Calculates duration until today (for active periods).
        /// </summary>
        public int GetDaysToToday()
        {
            var referenceDate = EndDate.HasValue && EndDate.Value < DateTime.UtcNow.Date
                ? EndDate.Value
                : DateTime.UtcNow.Date;

            return (referenceDate - StartDate).Days + 1;
        }

        // --------------------------------------------------------------
        // FORMATTING
        // --------------------------------------------------------------

        public override string ToString()
        {
            if (!EndDate.HasValue)
                return $"From {StartDate:yyyy-MM-dd} (ongoing)";

            return $"{StartDate:yyyy-MM-dd} to {EndDate.Value:yyyy-MM-dd}";
        }

        public string ToShortString()
        {
            if (!EndDate.HasValue)
                return $"{StartDate:dd/MM/yyyy} - ongoing";

            return $"{StartDate:dd/MM/yyyy} - {EndDate.Value:dd/MM/yyyy}";
        }

        // --------------------------------------------------------------
        // VALUE OBJECT IMPLEMENTATION
        // --------------------------------------------------------------

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return StartDate;
            yield return EndDate;
        }
    }
}
