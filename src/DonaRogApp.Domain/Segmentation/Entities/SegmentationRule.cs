using DonaRogApp.Domain.Donors.Entities;
using DonaRogApp.Domain.Shared.Entities;
using System;
using System.Collections.Generic;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace DonaRogApp.Domain.Segmentation.Entities
{
    /// <summary>
    /// Segmentation Rule - Define rules for automatic donor segment assignment based on MFR (Monetary, Frequency, Recency)
    /// Regola di segmentazione - Definisce regole per assegnazione automatica dei donatori ai segmenti basata su MFR
    /// </summary>
    public class SegmentationRule : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {
        // ======================================================================
        // MULTI-TENANCY
        // ======================================================================
        /// <summary>
        /// Tenant ID
        /// </summary>
        public Guid? TenantId { get; private set; }

        // ======================================================================
        // IDENTIFICATION
        // ======================================================================
        /// <summary>
        /// Rule name (e.g., "Major Donors", "At Risk Donors")
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Rule description
        /// </summary>
        public string? Description { get; private set; }

        /// <summary>
        /// Is rule active?
        /// </summary>
        public bool IsActive { get; private set; }

        /// <summary>
        /// Priority (lower number = higher priority, evaluated first)
        /// </summary>
        public int Priority { get; private set; }

        // ======================================================================
        // TARGET SEGMENT
        // ======================================================================
        /// <summary>
        /// Target segment ID to assign donors to
        /// </summary>
        public Guid SegmentId { get; private set; }

        /// <summary>
        /// Target segment (navigation property)
        /// </summary>
        public virtual Segment? Segment { get; private set; }

        // ======================================================================
        // RFM SCORE CONDITIONS (1-5 scale)
        // ======================================================================
        /// <summary>
        /// Minimum Recency Score (1-5, nullable = not evaluated)
        /// </summary>
        public int? MinRecencyScore { get; private set; }

        /// <summary>
        /// Maximum Recency Score (1-5, nullable = not evaluated)
        /// </summary>
        public int? MaxRecencyScore { get; private set; }

        /// <summary>
        /// Minimum Frequency Score (1-5, nullable = not evaluated)
        /// </summary>
        public int? MinFrequencyScore { get; private set; }

        /// <summary>
        /// Maximum Frequency Score (1-5, nullable = not evaluated)
        /// </summary>
        public int? MaxFrequencyScore { get; private set; }

        /// <summary>
        /// Minimum Monetary Score (1-5, nullable = not evaluated)
        /// </summary>
        public int? MinMonetaryScore { get; private set; }

        /// <summary>
        /// Maximum Monetary Score (1-5, nullable = not evaluated)
        /// </summary>
        public int? MaxMonetaryScore { get; private set; }

        // ======================================================================
        // RAW VALUE CONDITIONS
        // ======================================================================
        /// <summary>
        /// Minimum total donated amount (€)
        /// </summary>
        public decimal? MinTotalDonated { get; private set; }

        /// <summary>
        /// Maximum total donated amount (€)
        /// </summary>
        public decimal? MaxTotalDonated { get; private set; }

        /// <summary>
        /// Minimum donation count
        /// </summary>
        public int? MinDonationCount { get; private set; }

        /// <summary>
        /// Maximum donation count
        /// </summary>
        public int? MaxDonationCount { get; private set; }

        /// <summary>
        /// Minimum days since last donation
        /// </summary>
        public int? MinDaysSinceLastDonation { get; private set; }

        /// <summary>
        /// Maximum days since last donation
        /// </summary>
        public int? MaxDaysSinceLastDonation { get; private set; }

        // ======================================================================
        // DATE CONDITIONS
        // ======================================================================
        /// <summary>
        /// First donation must be after this date
        /// </summary>
        public DateTime? FirstDonationAfter { get; private set; }

        /// <summary>
        /// First donation must be before this date
        /// </summary>
        public DateTime? FirstDonationBefore { get; private set; }

        /// <summary>
        /// Last donation must be after this date
        /// </summary>
        public DateTime? LastDonationAfter { get; private set; }

        /// <summary>
        /// Last donation must be before this date
        /// </summary>
        public DateTime? LastDonationBefore { get; private set; }

        // ======================================================================
        // CONSTRUCTOR
        // ======================================================================
        /// <summary>
        /// Protected constructor for EF Core
        /// </summary>
        protected SegmentationRule()
        {
            Name = string.Empty;
        }

        // ======================================================================
        // FACTORY METHOD
        // ======================================================================
        /// <summary>
        /// Create a new segmentation rule
        /// </summary>
        public static SegmentationRule Create(
            Guid id,
            Guid? tenantId,
            string name,
            Guid segmentId,
            int priority = 1,
            string? description = null)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));
            if (segmentId == Guid.Empty) throw new ArgumentException("SegmentId cannot be empty", nameof(segmentId));

            return new SegmentationRule
            {
                Id = id,
                TenantId = tenantId,
                Name = name,
                SegmentId = segmentId,
                Priority = priority,
                Description = description,
                IsActive = true
            };
        }

        // ======================================================================
        // UPDATE METHODS
        // ======================================================================
        /// <summary>
        /// Update basic details
        /// </summary>
        public void UpdateDetails(string name, string? description, int priority)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));
            Name = name;
            Description = description;
            Priority = priority;
        }

        /// <summary>
        /// Update target segment
        /// </summary>
        public void UpdateTargetSegment(Guid segmentId)
        {
            if (segmentId == Guid.Empty) throw new ArgumentException("SegmentId cannot be empty", nameof(segmentId));
            SegmentId = segmentId;
        }

        /// <summary>
        /// Update RFM score conditions
        /// </summary>
        public void UpdateRfmConditions(
            int? minRecencyScore,
            int? maxRecencyScore,
            int? minFrequencyScore,
            int? maxFrequencyScore,
            int? minMonetaryScore,
            int? maxMonetaryScore)
        {
            // Validate score ranges (1-5)
            ValidateScore(minRecencyScore, nameof(minRecencyScore));
            ValidateScore(maxRecencyScore, nameof(maxRecencyScore));
            ValidateScore(minFrequencyScore, nameof(minFrequencyScore));
            ValidateScore(maxFrequencyScore, nameof(maxFrequencyScore));
            ValidateScore(minMonetaryScore, nameof(minMonetaryScore));
            ValidateScore(maxMonetaryScore, nameof(maxMonetaryScore));

            MinRecencyScore = minRecencyScore;
            MaxRecencyScore = maxRecencyScore;
            MinFrequencyScore = minFrequencyScore;
            MaxFrequencyScore = maxFrequencyScore;
            MinMonetaryScore = minMonetaryScore;
            MaxMonetaryScore = maxMonetaryScore;
        }

        /// <summary>
        /// Update raw value conditions
        /// </summary>
        public void UpdateRawValueConditions(
            decimal? minTotalDonated,
            decimal? maxTotalDonated,
            int? minDonationCount,
            int? maxDonationCount,
            int? minDaysSinceLastDonation,
            int? maxDaysSinceLastDonation)
        {
            // Validate non-negative values
            if (minTotalDonated.HasValue && minTotalDonated.Value < 0)
                throw new ArgumentException("MinTotalDonated must be >= 0", nameof(minTotalDonated));
            if (maxTotalDonated.HasValue && maxTotalDonated.Value < 0)
                throw new ArgumentException("MaxTotalDonated must be >= 0", nameof(maxTotalDonated));
            if (minDonationCount.HasValue && minDonationCount.Value < 0)
                throw new ArgumentException("MinDonationCount must be >= 0", nameof(minDonationCount));
            if (maxDonationCount.HasValue && maxDonationCount.Value < 0)
                throw new ArgumentException("MaxDonationCount must be >= 0", nameof(maxDonationCount));
            if (minDaysSinceLastDonation.HasValue && minDaysSinceLastDonation.Value < 0)
                throw new ArgumentException("MinDaysSinceLastDonation must be >= 0", nameof(minDaysSinceLastDonation));
            if (maxDaysSinceLastDonation.HasValue && maxDaysSinceLastDonation.Value < 0)
                throw new ArgumentException("MaxDaysSinceLastDonation must be >= 0", nameof(maxDaysSinceLastDonation));

            MinTotalDonated = minTotalDonated;
            MaxTotalDonated = maxTotalDonated;
            MinDonationCount = minDonationCount;
            MaxDonationCount = maxDonationCount;
            MinDaysSinceLastDonation = minDaysSinceLastDonation;
            MaxDaysSinceLastDonation = maxDaysSinceLastDonation;
        }

        /// <summary>
        /// Update date conditions
        /// </summary>
        public void UpdateDateConditions(
            DateTime? firstDonationAfter,
            DateTime? firstDonationBefore,
            DateTime? lastDonationAfter,
            DateTime? lastDonationBefore)
        {
            FirstDonationAfter = firstDonationAfter;
            FirstDonationBefore = firstDonationBefore;
            LastDonationAfter = lastDonationAfter;
            LastDonationBefore = lastDonationBefore;
        }

        /// <summary>
        /// Activate rule
        /// </summary>
        public void Activate()
        {
            IsActive = true;
        }

        /// <summary>
        /// Deactivate rule
        /// </summary>
        public void Deactivate()
        {
            IsActive = false;
        }

        /// <summary>
        /// Update priority
        /// </summary>
        public void UpdatePriority(int priority)
        {
            Priority = priority;
        }

        // ======================================================================
        // EVALUATION METHODS
        // ======================================================================
        /// <summary>
        /// Check if a donor matches this rule's conditions
        /// </summary>
        public bool Matches(Donor donor)
        {
            if (!IsActive) return false;

            // RFM Score conditions
            if (MinRecencyScore.HasValue && donor.RecencyScore < MinRecencyScore.Value) return false;
            if (MaxRecencyScore.HasValue && donor.RecencyScore > MaxRecencyScore.Value) return false;
            if (MinFrequencyScore.HasValue && donor.FrequencyScore < MinFrequencyScore.Value) return false;
            if (MaxFrequencyScore.HasValue && donor.FrequencyScore > MaxFrequencyScore.Value) return false;
            if (MinMonetaryScore.HasValue && donor.MonetaryScore < MinMonetaryScore.Value) return false;
            if (MaxMonetaryScore.HasValue && donor.MonetaryScore > MaxMonetaryScore.Value) return false;

            // Raw value conditions
            if (MinTotalDonated.HasValue && donor.TotalDonated < MinTotalDonated.Value) return false;
            if (MaxTotalDonated.HasValue && donor.TotalDonated > MaxTotalDonated.Value) return false;
            if (MinDonationCount.HasValue && donor.DonationCount < MinDonationCount.Value) return false;
            if (MaxDonationCount.HasValue && donor.DonationCount > MaxDonationCount.Value) return false;

            // Days since last donation
            if (MinDaysSinceLastDonation.HasValue || MaxDaysSinceLastDonation.HasValue)
            {
                if (!donor.LastDonationDate.HasValue)
                {
                    // No last donation date = infinite days since last donation
                    if (MinDaysSinceLastDonation.HasValue)
                        return true; // Any minimum is satisfied
                    return false; // Maximum is never satisfied without a date
                }

                var daysSinceLastDonation = (DateTime.UtcNow - donor.LastDonationDate.Value).Days;
                if (MinDaysSinceLastDonation.HasValue && daysSinceLastDonation < MinDaysSinceLastDonation.Value) return false;
                if (MaxDaysSinceLastDonation.HasValue && daysSinceLastDonation > MaxDaysSinceLastDonation.Value) return false;
            }

            // Date conditions
            if (FirstDonationAfter.HasValue && (!donor.FirstDonationDate.HasValue || donor.FirstDonationDate.Value < FirstDonationAfter.Value)) return false;
            if (FirstDonationBefore.HasValue && (!donor.FirstDonationDate.HasValue || donor.FirstDonationDate.Value > FirstDonationBefore.Value)) return false;
            if (LastDonationAfter.HasValue && (!donor.LastDonationDate.HasValue || donor.LastDonationDate.Value < LastDonationAfter.Value)) return false;
            if (LastDonationBefore.HasValue && (!donor.LastDonationDate.HasValue || donor.LastDonationDate.Value > LastDonationBefore.Value)) return false;

            return true;
        }

        /// <summary>
        /// Get a summary of conditions for display
        /// </summary>
        public string GetConditionsSummary()
        {
            var conditions = new List<string>();

            // RFM conditions
            if (MinRecencyScore.HasValue || MaxRecencyScore.HasValue)
                conditions.Add($"Recency: {FormatRange(MinRecencyScore, MaxRecencyScore)}");
            if (MinFrequencyScore.HasValue || MaxFrequencyScore.HasValue)
                conditions.Add($"Frequency: {FormatRange(MinFrequencyScore, MaxFrequencyScore)}");
            if (MinMonetaryScore.HasValue || MaxMonetaryScore.HasValue)
                conditions.Add($"Monetary: {FormatRange(MinMonetaryScore, MaxMonetaryScore)}");

            // Raw values
            if (MinTotalDonated.HasValue || MaxTotalDonated.HasValue)
                conditions.Add($"Total: {FormatRange(MinTotalDonated, MaxTotalDonated, "€")}");
            if (MinDonationCount.HasValue || MaxDonationCount.HasValue)
                conditions.Add($"Donations: {FormatRange(MinDonationCount, MaxDonationCount)}");
            if (MinDaysSinceLastDonation.HasValue || MaxDaysSinceLastDonation.HasValue)
                conditions.Add($"Days since last: {FormatRange(MinDaysSinceLastDonation, MaxDaysSinceLastDonation)}");

            // Dates
            if (FirstDonationAfter.HasValue || FirstDonationBefore.HasValue)
                conditions.Add("First donation date");
            if (LastDonationAfter.HasValue || LastDonationBefore.HasValue)
                conditions.Add("Last donation date");

            return conditions.Count > 0 ? string.Join(", ", conditions) : "No conditions";
        }

        // ======================================================================
        // HELPER METHODS
        // ======================================================================
        private static void ValidateScore(int? score, string paramName)
        {
            if (score.HasValue && (score.Value < 1 || score.Value > 5))
                throw new ArgumentException($"{paramName} must be between 1 and 5", paramName);
        }

        private static string FormatRange<T>(T? min, T? max, string suffix = "") where T : struct
        {
            if (min.HasValue && max.HasValue)
                return $"{min}{suffix}-{max}{suffix}";
            if (min.HasValue)
                return $">= {min}{suffix}";
            if (max.HasValue)
                return $"<= {max}{suffix}";
            return "any";
        }
    }
}
