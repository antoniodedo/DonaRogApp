using DonaRogApp.Enums.Donors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DonaRogApp.ValueObjects
{
    /// <summary>
    /// Value Object: Italian Tax Code (Codice Fiscale)
    /// - Individual: 16 alphanumeric characters (RSSMRA80A01H501Z)
    /// - Organization: 11 numeric characters (12345678901)
    /// Immutable, self-validating with checksum verification.
    /// </summary>
    public class TaxCode : ValueObject
    {
        // --------------------------------------------------------------
        // CONSTANTS
        // --------------------------------------------------------------

        private const int IndividualLength = 16;
        private const int OrganizationLength = 11;

        // Regex pattern for Individual Tax Code (16 chars)
        private static readonly Regex IndividualPattern = new Regex(
            @"^[A-Z]{6}\d{2}[A-Z]\d{2}[A-Z]\d{3}[A-Z]$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase
        );

        // Regex pattern for Organization Tax Code (11 digits)
        private static readonly Regex OrganizationPattern = new Regex(
            @"^\d{11}$",
            RegexOptions.Compiled
        );

        // Checksum conversion tables for Individual
        private static readonly Dictionary<char, int> OddCharValues = new Dictionary<char, int>
        {
            {'0', 1}, {'1', 0}, {'2', 5}, {'3', 7}, {'4', 9}, {'5', 13}, {'6', 15}, {'7', 17}, {'8', 19}, {'9', 21},
            {'A', 1}, {'B', 0}, {'C', 5}, {'D', 7}, {'E', 9}, {'F', 13}, {'G', 15}, {'H', 17}, {'I', 19}, {'J', 21},
            {'K', 2}, {'L', 4}, {'M', 18}, {'N', 20}, {'O', 11}, {'P', 3}, {'Q', 6}, {'R', 8}, {'S', 12}, {'T', 14},
            {'U', 16}, {'V', 10}, {'W', 22}, {'X', 25}, {'Y', 24}, {'Z', 23}
        };

        private static readonly Dictionary<char, int> EvenCharValues = new Dictionary<char, int>
        {
            {'0', 0}, {'1', 1}, {'2', 2}, {'3', 3}, {'4', 4}, {'5', 5}, {'6', 6}, {'7', 7}, {'8', 8}, {'9', 9},
            {'A', 0}, {'B', 1}, {'C', 2}, {'D', 3}, {'E', 4}, {'F', 5}, {'G', 6}, {'H', 7}, {'I', 8}, {'J', 9},
            {'K', 10}, {'L', 11}, {'M', 12}, {'N', 13}, {'O', 14}, {'P', 15}, {'Q', 16}, {'R', 17}, {'S', 18}, {'T', 19},
            {'U', 20}, {'V', 21}, {'W', 22}, {'X', 23}, {'Y', 24}, {'Z', 25}
        };

        private static readonly char[] ChecksumChars =
            "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

        // --------------------------------------------------------------
        // PROPERTIES
        // --------------------------------------------------------------

        /// <summary>
        /// Tax Code value (normalized to uppercase)
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Type of tax code (Individual or Organization)
        /// </summary>
        public TaxCodeType Type { get; }

        // --------------------------------------------------------------
        // CONSTRUCTOR
        // --------------------------------------------------------------

        private TaxCode()
        {
            // EF Core needs parameterless constructor
            Value = string.Empty;
        }

        public TaxCode(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Tax code cannot be empty", nameof(value));

            // Normalize
            var normalized = value.ToUpperInvariant().Trim();

            // Determine type and validate
            if (normalized.Length == IndividualLength)
            {
                ValidateIndividual(normalized);
                Type = TaxCodeType.Individual;
            }
            else if (normalized.Length == OrganizationLength)
            {
                ValidateOrganization(normalized);
                Type = TaxCodeType.Organization;
            }
            else
            {
                throw new ArgumentException(
                    $"Invalid tax code length: {normalized.Length}. Expected {IndividualLength} or {OrganizationLength}",
                    nameof(value)
                );
            }

            Value = normalized;
        }

        // --------------------------------------------------------------
        // VALIDATION - Individual (16 chars)
        // --------------------------------------------------------------

        private void ValidateIndividual(string value)
        {
            // Check pattern
            if (!IndividualPattern.IsMatch(value))
                throw new ArgumentException(
                    $"Invalid individual tax code format: {value}",
                    nameof(value)
                );

            // Verify checksum
            if (!VerifyIndividualChecksum(value))
                throw new ArgumentException(
                    $"Invalid individual tax code checksum: {value}",
                    nameof(value)
                );
        }

        private bool VerifyIndividualChecksum(string value)
        {
            var sum = 0;

            // Sum first 15 characters
            for (int i = 0; i < 15; i++)
            {
                var c = value[i];

                // Odd positions (1-based) use OddCharValues
                if (i % 2 == 0)
                {
                    sum += OddCharValues[c];
                }
                // Even positions use EvenCharValues
                else
                {
                    sum += EvenCharValues[c];
                }
            }

            // Calculate expected checksum character
            var expectedChecksum = ChecksumChars[sum % 26];
            var actualChecksum = value[15];

            return expectedChecksum == actualChecksum;
        }

        // --------------------------------------------------------------
        // VALIDATION - Organization (11 digits)
        // --------------------------------------------------------------

        private void ValidateOrganization(string value)
        {
            // Check pattern
            if (!OrganizationPattern.IsMatch(value))
                throw new ArgumentException(
                    $"Invalid organization tax code format: {value}",
                    nameof(value)
                );

            // Verify Luhn checksum (same as VAT)
            if (!VerifyLuhnChecksum(value))
                throw new ArgumentException(
                    $"Invalid organization tax code checksum: {value}",
                    nameof(value)
                );
        }

        private bool VerifyLuhnChecksum(string value)
        {
            var sum = 0;

            for (int i = 0; i < 10; i++)
            {
                var digit = int.Parse(value[i].ToString());

                // Even positions (0-based) are doubled
                if (i % 2 == 0)
                {
                    digit *= 2;
                    if (digit > 9)
                        digit -= 9;
                }

                sum += digit;
            }

            var checkDigit = (10 - (sum % 10)) % 10;
            var actualCheckDigit = int.Parse(value[10].ToString());

            return checkDigit == actualCheckDigit;
        }

        // --------------------------------------------------------------
        // QUERY METHODS
        // --------------------------------------------------------------

        public bool IsIndividual() => Type == TaxCodeType.Individual;

        public bool IsOrganization() => Type == TaxCodeType.Organization;

        /// <summary>
        /// Extracts birth date from individual tax code.
        /// Returns null for organization tax codes.
        /// </summary>
        public DateTime? ExtractBirthDate()
        {
            if (Type != TaxCodeType.Individual)
                return null;

            try
            {
                // Characters 7-8: Year (2 digits)
                var yearChars = Value.Substring(6, 2);
                var year = int.Parse(yearChars);

                // Assume 20th century if >= 50, otherwise 21st
                year += year >= 50 ? 1900 : 2000;

                // Character 9: Month (A=Jan, B=Feb, ..., L=Dec for males)
                // For females, add 40 to day, so month is same
                var monthChar = Value[8];
                var month = monthChar - 'A' + 1;

                // Characters 10-11: Day (1-31 for males, 41-71 for females)
                var dayChars = Value.Substring(9, 2);
                var day = int.Parse(dayChars);

                // If day > 40, it's female (subtract 40)
                if (day > 40)
                    day -= 40;

                return new DateTime(year, month, day);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Extracts gender from individual tax code.
        /// Returns null for organization tax codes.
        /// </summary>
        public Gender? ExtractGender()
        {
            if (Type != TaxCodeType.Individual)
                return null;

            try
            {
                // Characters 10-11: Day
                var dayChars = Value.Substring(9, 2);
                var day = int.Parse(dayChars);

                // Day > 40 indicates female
                return day > 40 ? Gender.Female : Gender.Male;
            }
            catch
            {
                return null;
            }
        }

        // --------------------------------------------------------------
        // FORMATTING
        // --------------------------------------------------------------

        public override string ToString() => Value;

        public string ToFormattedString()
        {
            if (Type == TaxCodeType.Individual)
            {
                // Format: RSSMRA 80A01 H501Z
                return $"{Value.Substring(0, 6)} {Value.Substring(6, 5)} {Value.Substring(11)}";
            }

            return Value;
        }

        // --------------------------------------------------------------
        // VALUE OBJECT IMPLEMENTATION
        // --------------------------------------------------------------

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Value;
        }
    }

    // --------------------------------------------------------------════════
    // ENUM
    // --------------------------------------------------------------════════

    public enum TaxCodeType
    {
        Individual,     // 16 characters
        Organization    // 11 digits
    }
}
