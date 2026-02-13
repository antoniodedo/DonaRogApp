using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DonaRogApp.ValueObjects
{
    /// <summary>
    /// Value Object: Postal Code for form 674 (Italian postal slip)
    /// Format: NNNNYY (5-digit sequence + 2-digit year)
    /// Example: 0012324 (123rd slip of 2024)
    /// Immutable, self-validating
    /// </summary>
    public class PostalCode674 : ValueObject
    {
        // --------------------------------------------------------------
        // CONSTANTS
        // --------------------------------------------------------------

        private static readonly Regex CodePattern = new Regex(
            @"^(\d{5})(\d{2})$",
            RegexOptions.Compiled
        );

        // --------------------------------------------------------------
        // PROPERTIES
        // --------------------------------------------------------------

        /// <summary>
        /// Sequence number component (5 digits, zero-padded)
        /// </summary>
        public int SequenceNumber { get; }

        /// <summary>
        /// Year component (2 digits - last 2 digits of year)
        /// </summary>
        public int YearSuffix { get; }

        /// <summary>
        /// Full year (reconstructed from YearSuffix)
        /// </summary>
        public int FullYear => 2000 + YearSuffix;

        /// <summary>
        /// Full formatted code (NNNNYY)
        /// </summary>
        public string Value => GetFormattedCode();

        // --------------------------------------------------------------
        // CONSTRUCTOR
        // --------------------------------------------------------------

        private PostalCode674()
        {
            // EF Core needs parameterless constructor
        }

        public PostalCode674(int sequenceNumber, int year)
        {
            if (year < 2000 || year > 2099)
            {
                throw new ArgumentException(
                    $"Year must be between 2000 and 2099, got: {year}",
                    nameof(year));
            }

            if (sequenceNumber < 1 || sequenceNumber > 99999)
            {
                throw new ArgumentException(
                    $"Sequence number must be between 1 and 99999, got: {sequenceNumber}",
                    nameof(sequenceNumber));
            }

            SequenceNumber = sequenceNumber;
            YearSuffix = year % 100; // Get last 2 digits
        }

        /// <summary>
        /// Create from formatted string (NNNNYY)
        /// </summary>
        public static PostalCode674 FromString(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Postal code cannot be empty", nameof(value));
            }

            var match = CodePattern.Match(value.Trim());
            if (!match.Success)
            {
                throw new ArgumentException(
                    $"Invalid postal code format: {value}. Expected format: NNNNYY (7 digits)",
                    nameof(value));
            }

            var sequenceNumber = int.Parse(match.Groups[1].Value);
            var yearSuffix = int.Parse(match.Groups[2].Value);
            var fullYear = 2000 + yearSuffix;

            return new PostalCode674(sequenceNumber, fullYear);
        }

        // --------------------------------------------------------------
        // QUERY METHODS
        // --------------------------------------------------------------

        /// <summary>
        /// Check if code is for current year
        /// </summary>
        public bool IsCurrentYear()
        {
            return FullYear == DateTime.UtcNow.Year;
        }

        /// <summary>
        /// Check if code is for specified year
        /// </summary>
        public bool IsForYear(int year)
        {
            return FullYear == year;
        }

        /// <summary>
        /// Get formatted code (NNNNYY)
        /// </summary>
        public string GetFormattedCode()
        {
            return $"{SequenceNumber:D5}{YearSuffix:D2}";
        }

        // --------------------------------------------------------------
        // FORMATTING
        // --------------------------------------------------------------

        public override string ToString() => GetFormattedCode();

        // --------------------------------------------------------------
        // VALUE OBJECT IMPLEMENTATION
        // --------------------------------------------------------------

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return SequenceNumber;
            yield return YearSuffix;
        }
    }
}
