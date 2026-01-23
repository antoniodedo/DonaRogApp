using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DonaRogApp.ValueObjects
{
    /// <summary>
    /// Value Object: IBAN (International Bank Account Number)
    /// Supports Italian and international IBAN with validation.
    /// Immutable, self-validating with checksum verification.
    /// </summary>
    public class IBAN : ValueObject
    {
        // --------------------------------------------------------------
        // CONSTANTS
        // --------------------------------------------------------------

        private const int ItalianIBANLength = 27;
        private const int MinIBANLength = 15;
        private const int MaxIBANLength = 34;

        private static readonly Regex IBANPattern = new Regex(
            @"^[A-Z]{2}\d{2}[A-Z0-9]+$",
            RegexOptions.Compiled
        );

        // --------------------------------------------------------------
        // PROPERTIES
        // --------------------------------------------------------------

        /// <summary>
        /// Full IBAN value (normalized, uppercase, no spaces)
        /// Example: IT60X0542811101000000123456
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Country code (2 letters)
        /// Example: IT, FR, DE
        /// </summary>
        public string CountryCode { get; }

        /// <summary>
        /// Check digits (2 digits)
        /// </summary>
        public string CheckDigits { get; }

        /// <summary>
        /// BBAN (Basic Bank Account Number) - country-specific part
        /// Example (IT): X0542811101000000123456
        /// </summary>
        public string BBAN { get; }

        // --------------------------------------------------------------
        // CONSTRUCTOR
        // --------------------------------------------------------------

        private IBAN()
        {
            // EF Core needs parameterless constructor
            Value = string.Empty;
            CountryCode = string.Empty;
            CheckDigits = string.Empty;
            BBAN = string.Empty;
        }

        public IBAN(string iban)
        {
            if (string.IsNullOrWhiteSpace(iban))
                throw new ArgumentException("IBAN cannot be empty", nameof(iban));

            // Normalize: remove spaces, uppercase
            var normalized = iban.Replace(" ", "").Replace("-", "").ToUpperInvariant().Trim();

            // Validate format
            if (!IBANPattern.IsMatch(normalized))
                throw new ArgumentException(
                    $"Invalid IBAN format: {iban}. Expected format: XX00XXXXXXXXXXXX",
                    nameof(iban)
                );

            // Validate length
            if (normalized.Length < MinIBANLength || normalized.Length > MaxIBANLength)
                throw new ArgumentException(
                    $"Invalid IBAN length: {normalized.Length}. Expected {MinIBANLength}-{MaxIBANLength} characters",
                    nameof(iban)
                );

            // Extract parts
            CountryCode = normalized.Substring(0, 2);
            CheckDigits = normalized.Substring(2, 2);
            BBAN = normalized.Substring(4);
            Value = normalized;

            // Validate checksum (mod 97 algorithm)
            if (!ValidateChecksum(normalized))
                throw new ArgumentException(
                    $"Invalid IBAN checksum: {iban}",
                    nameof(iban)
                );
        }

        // --------------------------------------------------------------
        // VALIDATION - MOD 97 Algorithm
        // --------------------------------------------------------------

        private bool ValidateChecksum(string iban)
        {
            // IBAN checksum validation (ISO 13616)
            // 1. Move first 4 chars to end: IT60X... → X...IT60
            var rearranged = iban.Substring(4) + iban.Substring(0, 4);

            // 2. Replace letters with numbers (A=10, B=11, ..., Z=35)
            var numericString = string.Empty;
            foreach (char c in rearranged)
            {
                if (char.IsDigit(c))
                {
                    numericString += c;
                }
                else if (char.IsLetter(c))
                {
                    // A=10, B=11, ..., Z=35
                    numericString += (c - 'A' + 10).ToString();
                }
            }

            // 3. Calculate mod 97
            // For very long numbers, we need to do this in chunks
            var remainder = 0;
            foreach (char digit in numericString)
            {
                remainder = (remainder * 10 + (digit - '0')) % 97;
            }

            // Valid IBAN has remainder = 1
            return remainder == 1;
        }

        // --------------------------------------------------------------
        // QUERY METHODS
        // --------------------------------------------------------------

        public bool IsItalian() => CountryCode == "IT";

        public bool IsFrench() => CountryCode == "FR";

        public bool IsGerman() => CountryCode == "DE";

        public bool IsSpanish() => CountryCode == "ES";

        /// <summary>
        /// Extracts Italian bank code (ABI - 5 digits)
        /// Only for Italian IBANs
        /// </summary>
        public string? GetItalianBankCode()
        {
            if (!IsItalian() || BBAN.Length < 6)
                return null;

            // Italian IBAN structure: IT kk X AAAAA BBBBB CCCCCCCCCCCC
            // Where: X=CIN, AAAAA=ABI (bank), BBBBB=CAB (branch), C=account
            return BBAN.Substring(1, 5); // Skip CIN, take 5-digit ABI
        }

        /// <summary>
        /// Extracts Italian branch code (CAB - 5 digits)
        /// Only for Italian IBANs
        /// </summary>
        public string? GetItalianBranchCode()
        {
            if (!IsItalian() || BBAN.Length < 11)
                return null;

            return BBAN.Substring(6, 5); // Take 5-digit CAB
        }

        /// <summary>
        /// Extracts Italian account number
        /// Only for Italian IBANs
        /// </summary>
        public string? GetItalianAccountNumber()
        {
            if (!IsItalian() || BBAN.Length < 23)
                return null;

            return BBAN.Substring(11); // Account number (12 chars)
        }

        // --------------------------------------------------------------
        // FORMATTING
        // --------------------------------------------------------------

        /// <summary>
        /// Returns unformatted IBAN (no spaces)
        /// Example: IT60X0542811101000000123456
        /// </summary>
        public override string ToString()
        {
            return Value;
        }

        /// <summary>
        /// Returns formatted IBAN (with spaces every 4 characters)
        /// Example: IT60 X054 2811 1010 0000 0123 456
        /// </summary>
        public string ToFormattedString()
        {
            var formatted = string.Empty;
            for (int i = 0; i < Value.Length; i++)
            {
                if (i > 0 && i % 4 == 0)
                    formatted += " ";

                formatted += Value[i];
            }

            return formatted;
        }

        /// <summary>
        /// Returns IBAN for electronic format (unformatted)
        /// Example: IT60X0542811101000000123456
        /// </summary>
        public string ToElectronicFormat()
        {
            return Value;
        }

        /// <summary>
        /// Returns masked IBAN (for display, hides middle part)
        /// Example: IT60 **** **** **** **** **** 456
        /// </summary>
        public string ToMaskedString()
        {
            if (Value.Length < 8)
                return Value;

            var visible = 4; // Show first 4 and last 4
            var start = Value.Substring(0, visible);
            var end = Value.Substring(Value.Length - visible);
            var masked = new string('*', Value.Length - (visible * 2));

            return $"{start} {masked} {end}";
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
