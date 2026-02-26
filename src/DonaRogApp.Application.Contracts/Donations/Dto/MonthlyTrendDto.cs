namespace DonaRogApp.Application.Contracts.Donations.Dto
{
    /// <summary>
    /// Monthly donation trend data for dashboard charts
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
        /// Number of donations in this month
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Total amount donated in this month
        /// </summary>
        public decimal Amount { get; set; }
    }
}
