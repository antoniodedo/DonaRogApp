using DonaRogApp.Enums.Communications;
using System;
using Volo.Abp.Application.Dtos;

namespace DonaRogApp.Application.Contracts.Communications.Dto
{
    /// <summary>
    /// Input for getting communication history
    /// </summary>
    public class GetCommunicationHistoryInput : PagedAndSortedResultRequestDto
    {
        /// <summary>
        /// Donor ID
        /// </summary>
        public Guid? DonorId { get; set; }

        /// <summary>
        /// Communication type
        /// </summary>
        public CommunicationType? Type { get; set; }

        /// <summary>
        /// Category
        /// </summary>
        public TemplateCategory? Category { get; set; }

        /// <summary>
        /// Date from
        /// </summary>
        public DateTime? DateFrom { get; set; }

        /// <summary>
        /// Date to
        /// </summary>
        public DateTime? DateTo { get; set; }

        /// <summary>
        /// Status
        /// </summary>
        public CommunicationStatus? Status { get; set; }

        /// <summary>
        /// Only printed?
        /// </summary>
        public bool? OnlyPrinted { get; set; }

        public GetCommunicationHistoryInput()
        {
            Sorting = "SentDate DESC";
            MaxResultCount = 50;
        }
    }

    /// <summary>
    /// Communication history DTO
    /// </summary>
    public class CommunicationHistoryDto : EntityDto<Guid>
    {
        public Guid DonorId { get; set; }
        public string DonorName { get; set; } = null!;
        public CommunicationType Type { get; set; }
        public TemplateCategory? Category { get; set; }
        public string Subject { get; set; } = null!;
        public DateTime SentDate { get; set; }
        public CommunicationStatus Status { get; set; }
        public bool IsPrinted { get; set; }
        public DateTime? PrintedAt { get; set; }
        public Guid? PrintBatchId { get; set; }
        public string? PrintBatchNumber { get; set; }
        public Guid? DonationId { get; set; }
        public string? DonationReference { get; set; }
        public DateTime CreationTime { get; set; }
    }
}
