using DonaRogApp.Enums.Communications;
using System;
using System.ComponentModel.DataAnnotations;

namespace DonaRogApp.Application.Contracts.Communications.Dto
{
    /// <summary>
    /// Input for checking duplicate letters
    /// </summary>
    public class CheckDuplicateLettersDto
    {
        /// <summary>
        /// Donor ID to check
        /// </summary>
        [Required]
        public Guid DonorId { get; set; }

        /// <summary>
        /// Category to check (null = all thank you letters)
        /// </summary>
        public TemplateCategory? Category { get; set; } = TemplateCategory.ThankYou;

        /// <summary>
        /// Check letters from last N days
        /// </summary>
        [Range(1, 365)]
        public int CheckLastDays { get; set; } = 30;

        /// <summary>
        /// Threshold for error alert (days)
        /// </summary>
        [Range(1, 365)]
        public int ErrorThresholdDays { get; set; } = 7;

        /// <summary>
        /// Threshold for warning alert (days)
        /// </summary>
        [Range(1, 365)]
        public int WarningThresholdDays { get; set; } = 15;
    }
}
