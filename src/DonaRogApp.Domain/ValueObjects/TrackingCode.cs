using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace DonaRogApp.ValueObjects
{
    /// <summary>
    /// Value Object: Tracking Code for campaigns
    /// Used to track email opens, clicks, and conversions
    /// Format: GUID-based with optional hash for verification
    /// Immutable, self-validating
    /// </summary>
    public class TrackingCode : ValueObject
    {
        // --------------------------------------------------------------
        // PROPERTIES
        // --------------------------------------------------------------

        /// <summary>
        /// Unique tracking identifier (GUID)
        /// </summary>
        public Guid Code { get; }

        /// <summary>
        /// Campaign ID (for context)
        /// </summary>
        public Guid CampaignId { get; }

        /// <summary>
        /// Donor ID (for context)
        /// </summary>
        public Guid DonorId { get; }

        /// <summary>
        /// Verification hash (SHA256 of Code + Campaign + Donor + Secret)
        /// Prevents tampering
        /// </summary>
        public string Hash { get; }

        /// <summary>
        /// Full tracking code value (includes code and hash)
        /// </summary>
        public string Value => GetFullCode();

        // --------------------------------------------------------------
        // CONSTRUCTOR
        // --------------------------------------------------------------

        private TrackingCode()
        {
            // EF Core needs parameterless constructor
        }

        public TrackingCode(Guid campaignId, Guid donorId, string? secretKey = null)
        {
            if (campaignId == Guid.Empty)
            {
                throw new ArgumentException("Campaign ID cannot be empty", nameof(campaignId));
            }

            if (donorId == Guid.Empty)
            {
                throw new ArgumentException("Donor ID cannot be empty", nameof(donorId));
            }

            Code = Guid.NewGuid();
            CampaignId = campaignId;
            DonorId = donorId;
            Hash = GenerateHash(Code, campaignId, donorId, secretKey ?? "DonaRogDefaultSecret");
        }

        /// <summary>
        /// Create from existing code (for verification)
        /// </summary>
        public TrackingCode(Guid code, Guid campaignId, Guid donorId, string hash)
        {
            Code = code;
            CampaignId = campaignId;
            DonorId = donorId;
            Hash = hash;
        }

        // --------------------------------------------------------------
        // GENERATION
        // --------------------------------------------------------------

        /// <summary>
        /// Generate verification hash
        /// </summary>
        private static string GenerateHash(Guid code, Guid campaignId, Guid donorId, string secretKey)
        {
            var input = $"{code}|{campaignId}|{donorId}|{secretKey}";
            
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(input);
                var hashBytes = sha256.ComputeHash(bytes);
                
                // Return first 8 characters of hex hash (sufficient for tracking)
                return BitConverter.ToString(hashBytes)
                    .Replace("-", "")
                    .Substring(0, 8)
                    .ToLowerInvariant();
            }
        }

        // --------------------------------------------------------------
        // VALIDATION
        // --------------------------------------------------------------

        /// <summary>
        /// Verify that the hash is valid
        /// </summary>
        public bool Verify(string secretKey = "DonaRogDefaultSecret")
        {
            var expectedHash = GenerateHash(Code, CampaignId, DonorId, secretKey);
            return Hash.Equals(expectedHash, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Parse tracking code from string
        /// Format: {code}-{hash}
        /// </summary>
        public static (Guid code, string hash) Parse(string trackingCode)
        {
            if (string.IsNullOrWhiteSpace(trackingCode))
            {
                throw new ArgumentException("Tracking code cannot be empty", nameof(trackingCode));
            }

            var parts = trackingCode.Split('-');
            if (parts.Length != 2)
            {
                throw new ArgumentException(
                    $"Invalid tracking code format: {trackingCode}. Expected format: CODE-HASH",
                    nameof(trackingCode));
            }

            if (!Guid.TryParse(parts[0], out var code))
            {
                throw new ArgumentException(
                    $"Invalid tracking code GUID: {parts[0]}",
                    nameof(trackingCode));
            }

            return (code, parts[1]);
        }

        // --------------------------------------------------------------
        // QUERY METHODS
        // --------------------------------------------------------------

        /// <summary>
        /// Get full tracking code (CODE-HASH)
        /// </summary>
        public string GetFullCode()
        {
            return $"{Code:N}-{Hash}";
        }

        /// <summary>
        /// Get short code (CODE only, for simple tracking)
        /// </summary>
        public string GetShortCode()
        {
            return Code.ToString("N");
        }

        /// <summary>
        /// Get URL-safe code
        /// </summary>
        public string GetUrlSafeCode()
        {
            return Uri.EscapeDataString(GetFullCode());
        }

        // --------------------------------------------------------------
        // FORMATTING
        // --------------------------------------------------------------

        public override string ToString() => GetFullCode();

        // --------------------------------------------------------------
        // VALUE OBJECT IMPLEMENTATION
        // --------------------------------------------------------------

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Code;
            yield return CampaignId;
            yield return DonorId;
            yield return Hash;
        }
    }
}
