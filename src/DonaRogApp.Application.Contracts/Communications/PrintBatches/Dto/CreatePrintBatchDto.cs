using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DonaRogApp.Application.Contracts.Communications.PrintBatches.Dto
{
    /// <summary>
    /// DTO for creating a new print batch
    /// </summary>
    public class CreatePrintBatchDto
    {
        /// <summary>
        /// Optional batch name
        /// </summary>
        [StringLength(200)]
        public string? Name { get; set; }

        /// <summary>
        /// Filters to apply
        /// </summary>
        [Required]
        public PrintBatchFilterDto Filters { get; set; } = null!;

        /// <summary>
        /// Optional notes
        /// </summary>
        [StringLength(1000)]
        public string? Notes { get; set; }

        /// <summary>
        /// Should automatically generate PDF after creation?
        /// </summary>
        public bool AutoGeneratePdf { get; set; } = false;
    }

    /// <summary>
    /// Filters for selecting letters to include in batch
    /// </summary>
    public class PrintBatchFilterDto
    {
        /// <summary>
        /// Minimum donation amount
        /// </summary>
        [Range(0, double.MaxValue)]
        public decimal? MinAmount { get; set; }

        /// <summary>
        /// Maximum donation amount
        /// </summary>
        [Range(0, double.MaxValue)]
        public decimal? MaxAmount { get; set; }

        /// <summary>
        /// Filter by donor region
        /// </summary>
        [StringLength(128)]
        public string? Region { get; set; }

        /// <summary>
        /// Filter by project IDs
        /// </summary>
        public List<Guid>? ProjectIds { get; set; }

        /// <summary>
        /// Filter by campaign IDs
        /// </summary>
        public List<Guid>? CampaignIds { get; set; }

        /// <summary>
        /// Filter by date range (from)
        /// </summary>
        public DateTime? DonationDateFrom { get; set; }

        /// <summary>
        /// Filter by date range (to)
        /// </summary>
        public DateTime? DonationDateTo { get; set; }

        /// <summary>
        /// Filter by donor category
        /// </summary>
        public Enums.Donors.DonorCategory? DonorCategory { get; set; }

        /// <summary>
        /// Include only verified donations?
        /// </summary>
        public bool OnlyVerified { get; set; } = true;

        /// <summary>
        /// Exclude communications already printed?
        /// </summary>
        public bool ExcludePrinted { get; set; } = true;

        /// <summary>
        /// Exclude communications in other batches?
        /// </summary>
        public bool ExcludeInOtherBatches { get; set; } = true;
    }
}
