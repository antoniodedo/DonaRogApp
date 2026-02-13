using DonaRogApp.Enums.Projects;
using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace DonaRogApp.Application.Contracts.Projects.Dto
{
    /// <summary>
    /// Full project DTO (for detail view)
    /// </summary>
    public class ProjectDto : FullAuditedEntityDto<Guid>
    {
        /// <summary>
        /// Project code (unique per tenant)
        /// </summary>
        public string Code { get; set; } = null!;

        /// <summary>
        /// Project name
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Project description
        /// </summary>
        public string? Description { get; set; }

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
        /// Project end date (optional)
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Target amount to raise
        /// </summary>
        public decimal? TargetAmount { get; set; }

        /// <summary>
        /// Currency
        /// </summary>
        public string Currency { get; set; } = "EUR";

        /// <summary>
        /// Person responsible for the project
        /// </summary>
        public string? ResponsiblePerson { get; set; }

        /// <summary>
        /// Responsible person email
        /// </summary>
        public string? ResponsibleEmail { get; set; }

        /// <summary>
        /// Responsible person phone
        /// </summary>
        public string? ResponsiblePhone { get; set; }

        /// <summary>
        /// Main image URL
        /// </summary>
        public string? MainImageUrl { get; set; }

        /// <summary>
        /// Thumbnail image URL
        /// </summary>
        public string? ThumbnailUrl { get; set; }

        /// <summary>
        /// Project location
        /// </summary>
        public string? Location { get; set; }

        /// <summary>
        /// Latitude
        /// </summary>
        public decimal? Latitude { get; set; }

        /// <summary>
        /// Longitude
        /// </summary>
        public decimal? Longitude { get; set; }

        // Statistics
        /// <summary>
        /// Total amount raised
        /// </summary>
        public decimal TotalAmountRaised { get; set; }

        /// <summary>
        /// Number of donations
        /// </summary>
        public int TotalDonationsCount { get; set; }

        /// <summary>
        /// Average donation amount
        /// </summary>
        public decimal AverageDonation { get; set; }

        /// <summary>
        /// Last donation date
        /// </summary>
        public DateTime? LastDonationDate { get; set; }

        // Calculated properties
        /// <summary>
        /// Target completion percentage
        /// </summary>
        public decimal TargetCompletionPercentage { get; set; }

        /// <summary>
        /// Remaining amount to reach target
        /// </summary>
        public decimal RemainingAmount { get; set; }

        /// <summary>
        /// Whether target has been reached
        /// </summary>
        public bool HasReachedTarget { get; set; }

        /// <summary>
        /// Whether project is ongoing
        /// </summary>
        public bool IsOngoing { get; set; }

        /// <summary>
        /// Project documents
        /// </summary>
        public List<ProjectDocumentDto> Documents { get; set; } = new();
    }
}
