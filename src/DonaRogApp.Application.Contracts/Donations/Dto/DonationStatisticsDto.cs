using System;

namespace DonaRogApp.Application.Contracts.Donations.Dto
{
    public class DonationStatisticsDto
    {
        public int TotalCount { get; set; }
        public int PendingCount { get; set; }
        public int VerifiedCount { get; set; }
        public int RejectedCount { get; set; }
        
        public decimal TotalAmount { get; set; }
        public decimal TotalVerifiedAmount { get; set; }
        public decimal AverageAmount { get; set; }
        
        public DateTime? FirstDonationDate { get; set; }
        public DateTime? LastDonationDate { get; set; }
    }
}
