using DonaRogApp.Enums.Projects;
using System;
using Volo.Abp.Application.Dtos;

namespace DonaRogApp.Application.Contracts.Projects.Dto
{
    /// <summary>
    /// Lightweight project DTO (for list view)
    /// </summary>
    public class ProjectListDto : EntityDto<Guid>
    {
        /// <summary>
        /// Project code
        /// </summary>
        public string Code { get; set; } = null!;

        /// <summary>
        /// Project name
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Project category
        /// </summary>
        public ProjectCategory Category { get; set; }

        /// <summary>
        /// Project status
        /// </summary>
        public ProjectStatus Status { get; set; }

        /// <summary>
        /// Project start date
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Project end date
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Target amount
        /// </summary>
        public decimal? TargetAmount { get; set; }

        /// <summary>
        /// Currency
        /// </summary>
        public string Currency { get; set; } = "EUR";

        /// <summary>
        /// Total amount raised
        /// </summary>
        public decimal TotalAmountRaised { get; set; }

        /// <summary>
        /// Number of donations
        /// </summary>
        public int TotalDonationsCount { get; set; }

        /// <summary>
        /// Target completion percentage
        /// </summary>
        public decimal TargetCompletionPercentage { get; set; }

        /// <summary>
        /// Thumbnail image URL
        /// </summary>
        public string? ThumbnailUrl { get; set; }

        /// <summary>
        /// Responsible person
        /// </summary>
        public string? ResponsiblePerson { get; set; }

        /// <summary>
        /// Project location
        /// </summary>
        public string? Location { get; set; }

        /// <summary>
        /// Whether project is ongoing
        /// </summary>
        public bool IsOngoing { get; set; }

        /// <summary>
        /// Creation time
        /// </summary>
        public DateTime CreationTime { get; set; }
    }
}
