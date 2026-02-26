namespace DonaRogApp.Application.Contracts.Donors.Dto
{
    /// <summary>
    /// RFM segmentation statistics for dashboard
    /// </summary>
    public class DonorRfmStatisticsDto
    {
        /// <summary>
        /// Total number of donors
        /// </summary>
        public int TotalDonors { get; set; }

        /// <summary>
        /// Number of active donors (non-lapsed)
        /// </summary>
        public int ActiveDonors { get; set; }

        /// <summary>
        /// Number of lapsed donors
        /// </summary>
        public int LapsedDonors { get; set; }

        /// <summary>
        /// Retention rate (percentage of donors with 2+ donations)
        /// </summary>
        public decimal RetentionRate { get; set; }

        /// <summary>
        /// Attrition rate (percentage of Lost or Lapsed donors)
        /// </summary>
        public decimal AttritionRate { get; set; }

        /// <summary>
        /// Number of Champions (RFM score 13-15)
        /// </summary>
        public int ChampionsCount { get; set; }

        /// <summary>
        /// Number of Loyal donors (RFM score 11-12)
        /// </summary>
        public int LoyalCount { get; set; }

        /// <summary>
        /// Number of Potential donors (RFM score 9-10)
        /// </summary>
        public int PotentialCount { get; set; }

        /// <summary>
        /// Number of AtRisk donors (RFM score 7-8)
        /// </summary>
        public int AtRiskCount { get; set; }

        /// <summary>
        /// Number of Dormant donors (RFM score 5-6)
        /// </summary>
        public int DormantCount { get; set; }

        /// <summary>
        /// Number of Lost donors (RFM score &lt; 5)
        /// </summary>
        public int LostCount { get; set; }
    }
}
