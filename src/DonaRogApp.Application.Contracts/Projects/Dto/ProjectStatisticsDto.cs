using System;
using System.Collections.Generic;

namespace DonaRogApp.Application.Contracts.Projects.Dto
{
    /// <summary>
    /// Project statistics and KPIs
    /// </summary>
    public class ProjectStatisticsDto
    {
        /// <summary>
        /// Project ID
        /// </summary>
        public Guid ProjectId { get; set; }

        /// <summary>
        /// Project code
        /// </summary>
        public string Code { get; set; } = null!;

        /// <summary>
        /// Project name
        /// </summary>
        public string Name { get; set; } = null!;

        // Overall statistics
        /// <summary>
        /// Total amount raised (all time)
        /// </summary>
        public decimal TotalAmountRaised { get; set; }

        /// <summary>
        /// Total number of donations
        /// </summary>
        public int TotalDonationsCount { get; set; }

        /// <summary>
        /// Average donation amount
        /// </summary>
        public decimal AverageDonation { get; set; }

        /// <summary>
        /// Largest single donation
        /// </summary>
        public decimal LargestDonation { get; set; }

        /// <summary>
        /// Smallest donation
        /// </summary>
        public decimal SmallestDonation { get; set; }

        /// <summary>
        /// Last donation date
        /// </summary>
        public DateTime? LastDonationDate { get; set; }

        /// <summary>
        /// First donation date
        /// </summary>
        public DateTime? FirstDonationDate { get; set; }

        // Budget-related
        /// <summary>
        /// Target amount
        /// </summary>
        public decimal? TargetAmount { get; set; }

        /// <summary>
        /// Remaining amount to reach target
        /// </summary>
        public decimal RemainingAmount { get; set; }

        /// <summary>
        /// Target completion percentage
        /// </summary>
        public decimal TargetCompletionPercentage { get; set; }

        /// <summary>
        /// Whether target has been reached
        /// </summary>
        public bool HasReachedTarget { get; set; }

        // Year-by-year breakdown
        /// <summary>
        /// Amount raised per year
        /// </summary>
        public List<YearlyStatisticsDto> YearlyStatistics { get; set; } = new();

        // Top donors
        /// <summary>
        /// Top 10 donors for this project
        /// </summary>
        public List<ProjectTopDonorDto> TopDonors { get; set; } = new();

        // Trends
        /// <summary>
        /// Monthly trend for last 12 months
        /// </summary>
        public List<MonthlyTrendDto> MonthlyTrend { get; set; } = new();
    }

    /// <summary>
    /// Yearly statistics breakdown
    /// </summary>
    public class YearlyStatisticsDto
    {
        /// <summary>
        /// Year
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// Amount raised in this year
        /// </summary>
        public decimal AmountRaised { get; set; }

        /// <summary>
        /// Number of donations in this year
        /// </summary>
        public int DonationsCount { get; set; }

        /// <summary>
        /// Average donation in this year
        /// </summary>
        public decimal AverageDonation { get; set; }
    }

    /// <summary>
    /// Top donor for a project
    /// </summary>
    public class ProjectTopDonorDto
    {
        /// <summary>
        /// Donor ID
        /// </summary>
        public Guid DonorId { get; set; }

        /// <summary>
        /// Donor full name
        /// </summary>
        public string DonorName { get; set; } = null!;

        /// <summary>
        /// Total amount donated to this project
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// Number of donations
        /// </summary>
        public int DonationsCount { get; set; }

        /// <summary>
        /// Last donation date
        /// </summary>
        public DateTime LastDonationDate { get; set; }
    }

    /// <summary>
    /// Monthly trend data
    /// </summary>
    public class MonthlyTrendDto
    {
        /// <summary>
        /// Year
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// Month (1-12)
        /// </summary>
        public int Month { get; set; }

        /// <summary>
        /// Amount raised in this month
        /// </summary>
        public decimal AmountRaised { get; set; }

        /// <summary>
        /// Number of donations in this month
        /// </summary>
        public int DonationsCount { get; set; }
    }
}
