using DonaRogApp.Enums.Communications;
using System;
using System.Collections.Generic;

namespace DonaRogApp.Application.Contracts.Communications.Dto
{
    /// <summary>
    /// DTO for recent communication (for duplicate checks)
    /// </summary>
    public class RecentCommunicationDto
    {
        public Guid Id { get; set; }
        public CommunicationType Type { get; set; }
        public TemplateCategory? Category { get; set; }
        public string Subject { get; set; } = null!;
        public DateTime SentDate { get; set; }
        public bool IsPrinted { get; set; }
        public bool IsInBatch { get; set; }
        public string? PrintBatchNumber { get; set; }
        public Guid? DonationId { get; set; }
        public string? DonationReference { get; set; }
        public decimal? DonationAmount { get; set; }
        public int DaysAgo { get; set; }
        public AlertLevel AlertLevel { get; set; }
    }

    /// <summary>
    /// Result of duplicate check
    /// </summary>
    public class DuplicateCheckResultDto
    {
        /// <summary>
        /// Highest alert level found
        /// </summary>
        public AlertLevel AlertLevel { get; set; }

        /// <summary>
        /// Recent communications found
        /// </summary>
        public List<RecentCommunicationDto> RecentCommunications { get; set; } = new();

        /// <summary>
        /// Has critical alert (less than 7 days)?
        /// </summary>
        public bool HasCriticalAlert => AlertLevel == AlertLevel.Error;

        /// <summary>
        /// Summary message
        /// </summary>
        public string? Message { get; set; }
    }
}
