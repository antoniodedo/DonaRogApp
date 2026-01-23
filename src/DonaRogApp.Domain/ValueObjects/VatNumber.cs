using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DonaRogApp.ValueObjects
{
    /// <summary>
    /// Value Object: Italian VAT Number (Partita IVA)
    /// - Format: 11 numeric digits
    /// - Validated with Luhn algorithm
    /// Immutable, self-validating.
    /// </summary>
    public class VatNumber : ValueObject
    {
        // --------------------------------------------------------------
        // CONSTANTS
        // --------------------------------------------------------------

        private const int Length = 11;
        private static readonly Regex Pattern = new Regex(@"^\d{11}$", RegexOptions.Compiled);

        // --------------------------------------------------------------
        // PROPERTIES
        // --------------------------------------------------------------

        /// <summary>
        /// VAT Number value (11 digits)
        /// </summary>
        public string Value { get; }

        // --------------------------------------------------------------
        // CONSTRUCTOR
        // --------------------------------------------------------------

        private VatNumber()
        {
            // EF Core needs parameterless constructor
            Value = string.Empty;
        }

        public VatNumber(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("VAT number cannot be empty", nameof(value));

            // Normalize (remove spaces, keep only digits)
            var normalized = Regex.Replace(value, @"\s+", "");

            // Validate format
            if (!Pattern.IsMatch(normalized))
                throw new ArgumentException(
                    $"Invalid VAT number format: {value}. Expected 11 digits",
                    nameof(value)
                );

            // Validate checksum (Luhn algorithm)
            if (!ValidateLuhnChecksum(normalized))
                throw new ArgumentException(
                    $"Invalid VAT number checksum: {value}",
                    nameof(value)
                );

            Value = normalized;
        }

        // --------------------------------------------------------------
        // VALIDATION - Luhn Algorithm
        // --------------------------------------------------------------

        private bool ValidateLuhnChecksum(string value)
        {
            var sum = 0;

            // Process first 10 digits
            for (int i = 0; i < 10; i++)
            {
                var digit = int.Parse(value[i].ToString());

                // Even positions (0-indexed) are doubled
                if (i % 2 == 0)
                {
                    digit *= 2;

                    // If doubled value > 9, subtract 9
                    if (digit > 9)
                        digit -= 9;
                }

                sum += digit;
            }

            // Calculate expected check digit
            var expectedCheckDigit = (10 - (sum % 10)) % 10;

            // Get actual check digit (last digit)
            var actualCheckDigit = int.Parse(value[10].ToString());

            return expectedCheckDigit == actualCheckDigit;
        }

        // --------------------------------------------------------------
        // QUERY METHODS
        // --------------------------------------------------------------

        /// <summary>
        /// Extracts country code (first 2 digits).
        /// Italian VAT numbers don't have explicit country code,
        /// but some registries use first 2 digits as region identifier.
        /// </summary>
        public string GetRegionCode()
        {
            return Value.Substring(0, 2);
        }

        // --------------------------------------------------------------
        // FORMATTING
        // --------------------------------------------------------------

        public override string ToString() => Value;

        public string ToFormattedString()
        {
            // Format: 12345 67890 1
            return $"{Value.Substring(0, 5)} {Value.Substring(5, 5)} {Value.Substring(10)}";
        }

        public string ToInternationalFormat()
        {
            // IT prefix for Italian VAT
            return $"IT{Value}";
        }

        // --------------------------------------------------------------
        // VALUE OBJECT IMPLEMENTATION
        // --------------------------------------------------------------

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
