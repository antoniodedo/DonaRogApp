using DonaRogApp.Enums.Donors;
using System;
using System.Collections.Generic;

namespace DonaRogApp.Application.Contracts.Campaigns.Dto
{
    /// <summary>
    /// Complex input for donor extraction with query builder
    /// </summary>
    public class ExtractDonorsInput
    {
        // ======================================================================
        // DONATION FILTERS
        // ======================================================================

        /// <summary>
        /// Donation date range filter
        /// </summary>
        public DateRangeFilter? DonationDateRange { get; set; }

        /// <summary>
        /// Total donation amount range filter (min/max sum in date range)
        /// </summary>
        public DecimalRangeFilter? DonationAmountRange { get; set; }

        // ======================================================================
        // TAG FILTERS
        // ======================================================================

        /// <summary>
        /// Tags to include
        /// </summary>
        public List<Guid> IncludedTagIds { get; set; } = new();

        /// <summary>
        /// Tags to exclude
        /// </summary>
        public List<Guid> ExcludedTagIds { get; set; } = new();

        /// <summary>
        /// Tag filter mode (All/Any)
        /// </summary>
        public TagFilterMode TagFilterMode { get; set; } = TagFilterMode.Any;

        // ======================================================================
        // SEGMENT FILTERS
        // ======================================================================

        /// <summary>
        /// Segments to include
        /// </summary>
        public List<Guid> IncludedSegmentIds { get; set; } = new();

        /// <summary>
        /// Segments to exclude
        /// </summary>
        public List<Guid> ExcludedSegmentIds { get; set; } = new();

        // ======================================================================
        // NAME FILTERS
        // ======================================================================

        /// <summary>
        /// Specific donor names to include (partial match)
        /// </summary>
        public List<string> IncludedDonorNames { get; set; } = new();

        /// <summary>
        /// Specific donor names to exclude (partial match)
        /// </summary>
        public List<string> ExcludedDonorNames { get; set; } = new();

        // ======================================================================
        // CAMPAIGN PARTICIPATION FILTERS
        // ======================================================================

        /// <summary>
        /// Campaigns to include (donors who participated)
        /// </summary>
        public List<Guid> IncludedCampaignIds { get; set; } = new();

        /// <summary>
        /// Campaigns to exclude (donors who participated)
        /// </summary>
        public List<Guid> ExcludedCampaignIds { get; set; } = new();

        // ======================================================================
        // GEOGRAPHIC FILTERS
        // ======================================================================

        /// <summary>
        /// Regions to include
        /// </summary>
        public List<string> IncludedRegions { get; set; } = new();

        /// <summary>
        /// Regions to exclude
        /// </summary>
        public List<string> ExcludedRegions { get; set; } = new();

        /// <summary>
        /// Provinces to include
        /// </summary>
        public List<string> IncludedProvinces { get; set; } = new();

        /// <summary>
        /// Provinces to exclude
        /// </summary>
        public List<string> ExcludedProvinces { get; set; } = new();

        // ======================================================================
        // DONOR STATUS FILTERS
        // ======================================================================

        /// <summary>
        /// Donor status filter
        /// </summary>
        public DonorStatus? DonorStatus { get; set; }

        /// <summary>
        /// Donor category filter
        /// </summary>
        public DonorCategory? DonorCategory { get; set; }

        // ======================================================================
        // CONSENT FILTERS
        // ======================================================================

        /// <summary>
        /// Require newsletter consent
        /// </summary>
        public bool? RequireNewsletterConsent { get; set; }

        /// <summary>
        /// Require mail consent
        /// </summary>
        public bool? RequireMailConsent { get; set; }

        // ======================================================================
        // LOGICAL OPERATOR
        // ======================================================================

        /// <summary>
        /// Logical operator for combining filters (AND/OR)
        /// </summary>
        public LogicalOperator LogicalOperator { get; set; } = LogicalOperator.And;

        // ======================================================================
        // RESULTS CONTROL
        // ======================================================================

        /// <summary>
        /// Maximum number of donors to extract
        /// </summary>
        public int? MaxResults { get; set; }

        /// <summary>
        /// Random sampling (for testing)
        /// </summary>
        public bool RandomSample { get; set; }
    }

    /// <summary>
    /// Date range filter
    /// </summary>
    public class DateRangeFilter
    {
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
    }

    /// <summary>
    /// Decimal range filter
    /// </summary>
    public class DecimalRangeFilter
    {
        public decimal? Min { get; set; }
        public decimal? Max { get; set; }
    }

    /// <summary>
    /// Tag filter mode
    /// </summary>
    public enum TagFilterMode
    {
        /// <summary>
        /// Donor must have ANY of the tags
        /// </summary>
        Any,

        /// <summary>
        /// Donor must have ALL of the tags
        /// </summary>
        All
    }

    /// <summary>
    /// Logical operator for combining filters
    /// </summary>
    public enum LogicalOperator
    {
        /// <summary>
        /// All filters must match (AND)
        /// </summary>
        And,

        /// <summary>
        /// Any filter must match (OR)
        /// </summary>
        Or
    }
}
