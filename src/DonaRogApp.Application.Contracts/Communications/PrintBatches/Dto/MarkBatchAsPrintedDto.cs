using System;
using System.ComponentModel.DataAnnotations;

namespace DonaRogApp.Application.Contracts.Communications.PrintBatches.Dto
{
    /// <summary>
    /// DTO for marking batch as printed
    /// </summary>
    public class MarkBatchAsPrintedDto
    {
        /// <summary>
        /// Batch ID
        /// </summary>
        [Required]
        public Guid BatchId { get; set; }

        /// <summary>
        /// Actual print date (defaults to now)
        /// </summary>
        public DateTime? PrintedAt { get; set; }

        /// <summary>
        /// Optional notes about printing
        /// </summary>
        [StringLength(500)]
        public string? Notes { get; set; }
    }

    /// <summary>
    /// DTO for cancelling batch
    /// </summary>
    public class CancelBatchDto
    {
        /// <summary>
        /// Batch ID
        /// </summary>
        [Required]
        public Guid BatchId { get; set; }

        /// <summary>
        /// Reason for cancellation
        /// </summary>
        [Required]
        [StringLength(500)]
        public string Reason { get; set; } = null!;
    }
}
