using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DonaRogApp.ValueObjects
{
    /// <summary>
    /// Value Object: Phone Number
    /// Normalized international phone number with E.164 format support.
    /// Immutable, self-validating.
    /// </summary>
    public class PhoneNumber : ValueObject
    {
        // --------------------------------------------------------------
        // CONSTANTS
        // --------------------------------------------------------------

        private const string DefaultCountryCode = "39"; // Italy

        // Regex patterns
        private static readonly Regex DigitsOnly = new Regex(@"\D", RegexOptions.Compiled);
        private static readonly Regex InternationalPattern = new Regex(
            @"^\+?(\d{1,3})(\d{6,14})$",
            RegexOptions.Compiled
        );

        // --------------------------------------------------------------
        // PROPERTIES
        // --------------------------------------------------------------

        /// <summary>
        /// Country code (1-3 digits)
        /// Examples: 39 (Italy), 1 (USA), 44 (UK)
        /// </summary>
        public string CountryCode { get; private set; }

        /// <summary>
        /// National number (6-14 digits)
        /// Without country code, spaces, or formatting
        /// </summary>
        public string NationalNumber { get; private set; }

        /// <summary>
        /// Full international number (E.164 format)
        /// Format: +{CountryCode}{NationalNumber}
        /// Example: +393501234567
        /// </summary>
        public string InternationalNumber => $"+{CountryCode}{NationalNumber}";

        // --------------------------------------------------------------
        // CONSTRUCTOR
        // --------------------------------------------------------------

        private PhoneNumber()
        {
            // EF Core needs parameterless constructor
            CountryCode = DefaultCountryCode;
            NationalNumber = string.Empty;
        }

        public PhoneNumber(string phoneNumber, string? countryCode = null)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new ArgumentException("Phone number cannot be empty", nameof(phoneNumber));

            // Remove all non-digit characters
            var digitsOnly = DigitsOnly.Replace(phoneNumber, "");

            // Handle international format (+39...)
            if (phoneNumber.TrimStart().StartsWith("+"))
            {
                ParseInternationalFormat(digitsOnly);
            }
            // Handle explicit country code parameter
            else if (!string.IsNullOrWhiteSpace(countryCode))
            {
                CountryCode = NormalizeCountryCode(countryCode);
                NationalNumber = NormalizeNationalNumber(digitsOnly);
            }
            // Handle Italian national format (starts with 0)
            else if (digitsOnly.StartsWith("0"))
            {
                CountryCode = DefaultCountryCode;
                NationalNumber = NormalizeNationalNumber(digitsOnly);
            }
            // Assume Italian mobile (starts with 3)
            else if (digitsOnly.StartsWith("3"))
            {
                CountryCode = DefaultCountryCode;
                NationalNumber = NormalizeNationalNumber(digitsOnly);
            }
            // Default to Italian
            else
            {
                CountryCode = DefaultCountryCode;
                NationalNumber = NormalizeNationalNumber(digitsOnly);
            }

            // Validate
            Validate();
        }

        // --------------------------------------------------------------
        // PARSING
        // --------------------------------------------------------------

        private void ParseInternationalFormat(string digitsOnly)
        {
            // Try to match international pattern
            var match = InternationalPattern.Match(digitsOnly);

            if (!match.Success)
                throw new ArgumentException(
                    $"Invalid international phone number format: {digitsOnly}",
                    nameof(digitsOnly)
                );

            CountryCode = match.Groups[1].Value;
            NationalNumber = match.Groups[2].Value;
        }

        private string NormalizeCountryCode(string countryCode)
        {
            var normalized = DigitsOnly.Replace(countryCode, "");

            if (normalized.Length < 1 || normalized.Length > 3)
                throw new ArgumentException(
                    $"Invalid country code: {countryCode}. Expected 1-3 digits",
                    nameof(countryCode)
                );

            return normalized;
        }

        private string NormalizeNationalNumber(string number)
        {
            if (number.Length < 6 || number.Length > 14)
                throw new ArgumentException(
                    $"Invalid phone number length: {number}. Expected 6-14 digits",
                    nameof(number)
                );

            return number;
        }

        // --------------------------------------------------------------
        // VALIDATION
        // --------------------------------------------------------------

        private void Validate()
        {
            // Country code validation
            if (string.IsNullOrWhiteSpace(CountryCode) ||
                CountryCode.Length < 1 ||
                CountryCode.Length > 3)
            {
                throw new ArgumentException(
                    $"Invalid country code: {CountryCode}",
                    nameof(CountryCode)
                );
            }

            // National number validation
            if (string.IsNullOrWhiteSpace(NationalNumber) ||
                NationalNumber.Length < 6 ||
                NationalNumber.Length > 14)
            {
                throw new ArgumentException(
                    $"Invalid national number: {NationalNumber}",
                    nameof(NationalNumber)
                );
            }

            // Total length check (E.164 max is 15 digits including country code)
            if (InternationalNumber.Length > 16) // +15 digits
            {
                throw new ArgumentException(
                    $"Phone number too long: {InternationalNumber}",
                    nameof(InternationalNumber)
                );
            }
        }

        // --------------------------------------------------------------
        // QUERY METHODS
        // --------------------------------------------------------------

        public bool IsItalian() => CountryCode == "39";

        public bool IsUSA() => CountryCode == "1";

        public bool IsUK() => CountryCode == "44";

        /// <summary>
        /// Checks if this is an Italian mobile number.
        /// Italian mobiles start with 3 (e.g., 333, 347, 388)
        /// </summary>
        public bool IsItalianMobile()
        {
            return IsItalian() && NationalNumber.StartsWith("3");
        }

        /// <summary>
        /// Checks if this is an Italian landline.
        /// Italian landlines start with 0 (e.g., 02, 06, 091)
        /// </summary>
        public bool IsItalianLandline()
        {
            return IsItalian() && NationalNumber.StartsWith("0");
        }

        // --------------------------------------------------------------
        // FORMATTING
        // --------------------------------------------------------------

        /// <summary>
        /// Returns E.164 format: +393501234567
        /// </summary>
        public override string ToString()
        {
            return InternationalNumber;
        }

        /// <summary>
        /// Returns formatted national number.
        /// Italian: 350 123 4567
        /// USA: (555) 123-4567
        /// </summary>
        public string ToFormattedString()
        {
            if (IsItalian())
            {
                return FormatItalianNumber();
            }
            else if (IsUSA())
            {
                return FormatUSANumber();
            }

            // Default: insert space every 3 digits
            return FormatGeneric();
        }

        private string FormatItalianNumber()
        {
            // Mobile: 333 123 4567
            if (IsItalianMobile() && NationalNumber.Length == 10)
            {
                return $"{NationalNumber.Substring(0, 3)} {NationalNumber.Substring(3, 3)} {NationalNumber.Substring(6)}";
            }

            // Landline: 02 1234 5678 (Milan)
            if (IsItalianLandline())
            {
                // Major cities: 2-digit area code
                if (NationalNumber.Length == 10 &&
                    (NationalNumber.StartsWith("02") || NationalNumber.StartsWith("06")))
                {
                    return $"{NationalNumber.Substring(0, 2)} {NationalNumber.Substring(2, 4)} {NationalNumber.Substring(6)}";
                }

                // Other cities: 3-digit area code
                if (NationalNumber.Length == 10)
                {
                    return $"{NationalNumber.Substring(0, 3)} {NationalNumber.Substring(3, 3)} {NationalNumber.Substring(6)}";
                }
            }

            return FormatGeneric();
        }

        private string FormatUSANumber()
        {
            // USA: (555) 123-4567
            if (NationalNumber.Length == 10)
            {
                return $"({NationalNumber.Substring(0, 3)}) {NationalNumber.Substring(3, 3)}-{NationalNumber.Substring(6)}";
            }

            return FormatGeneric();
        }

        private string FormatGeneric()
        {
            // Insert space every 3 digits
            var formatted = string.Empty;
            for (int i = 0; i < NationalNumber.Length; i++)
            {
                if (i > 0 && i % 3 == 0)
                    formatted += " ";

                formatted += NationalNumber[i];
            }

            return formatted;
        }

        /// <summary>
        /// Returns formatted international number.
        /// +39 350 123 4567
        /// </summary>
        public string ToFormattedInternationalString()
        {
            return $"+{CountryCode} {ToFormattedString()}";
        }

        /// <summary>
        /// Returns national format (without country code).
        /// Italian: 350 123 4567
        /// </summary>
        public string ToNationalFormat()
        {
            return ToFormattedString();
        }

        /// <summary>
        /// Returns clickable tel: link.
        /// tel:+393501234567
        /// </summary>
        public string ToTelLink()
        {
            return $"tel:{InternationalNumber}";
        }

        // --------------------------------------------------------------
        // VALUE OBJECT IMPLEMENTATION
        // --------------------------------------------------------------

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return CountryCode;
            yield return NationalNumber;
        }
    }
}
