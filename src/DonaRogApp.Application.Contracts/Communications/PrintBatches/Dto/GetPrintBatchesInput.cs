using DonaRogApp.Enums.Communications;
using System;
using Volo.Abp.Application.Dtos;

namespace DonaRogApp.Application.Contracts.Communications.PrintBatches.Dto
{
    /// <summary>
    /// Input for querying print batches
    /// </summary>
    public class GetPrintBatchesInput : PagedAndSortedResultRequestDto
    {
        /// <summary>
        /// Filter by batch number or name
        /// </summary>
        public string? Filter { get; set; }

        /// <summary>
        /// Filter by status
        /// </summary>
        public PrintBatchStatus? Status { get; set; }

        /// <summary>
        /// Filter by generation date (from)
        /// </summary>
        public DateTime? GeneratedFrom { get; set; }

        /// <summary>
        /// Filter by generation date (to)
        /// </summary>
        public DateTime? GeneratedTo { get; set; }

        /// <summary>
        /// Filter by user who generated
        /// </summary>
        public Guid? GeneratedBy { get; set; }

        /// <summary>
        /// Filter by printed status
        /// </summary>
        public bool? IsPrinted { get; set; }

        /// <summary>
        /// Include cancelled batches?
        /// </summary>
        public bool IncludeCancelled { get; set; } = false;

        public GetPrintBatchesInput()
        {
            Sorting = "CreationTime DESC";
            MaxResultCount = 20;
        }
    }
}
