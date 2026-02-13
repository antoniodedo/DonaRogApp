using DonaRogApp.Enums.Projects;
using System;
using System.ComponentModel.DataAnnotations;

namespace DonaRogApp.Application.Contracts.Projects.Dto
{
    /// <summary>
    /// DTO for creating a new project
    /// </summary>
    public class CreateProjectDto
    {
        /// <summary>
        /// Project code (unique per tenant)
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Code { get; set; } = null!;

        /// <summary>
        /// Project name
        /// </summary>
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = null!;

        /// <summary>
        /// Project description
        /// </summary>
        [StringLength(2000)]
        public string? Description { get; set; }

        /// <summary>
        /// Project category
        /// </summary>
        [Required]
        public ProjectCategory Category { get; set; }

        /// <summary>
        /// Project start date
        /// </summary>
        [Required]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Project end date (optional)
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Target amount to raise
        /// </summary>
        [Range(0, double.MaxValue)]
        public decimal? TargetAmount { get; set; }

        /// <summary>
        /// Currency (default EUR)
        /// </summary>
        [StringLength(3)]
        public string? Currency { get; set; }

        /// <summary>
        /// Person responsible for the project
        /// </summary>
        [StringLength(100)]
        public string? ResponsiblePerson { get; set; }

        /// <summary>
        /// Responsible person email
        /// </summary>
        [EmailAddress]
        [StringLength(256)]
        public string? ResponsibleEmail { get; set; }

        /// <summary>
        /// Responsible person phone
        /// </summary>
        [StringLength(50)]
        public string? ResponsiblePhone { get; set; }

        /// <summary>
        /// Main image URL
        /// </summary>
        [StringLength(1000)]
        public string? MainImageUrl { get; set; }

        /// <summary>
        /// Thumbnail image URL
        /// </summary>
        [StringLength(1000)]
        public string? ThumbnailUrl { get; set; }

        /// <summary>
        /// Project location
        /// </summary>
        [StringLength(200)]
        public string? Location { get; set; }

        /// <summary>
        /// Latitude
        /// </summary>
        [Range(-90, 90)]
        public decimal? Latitude { get; set; }

        /// <summary>
        /// Longitude
        /// </summary>
        [Range(-180, 180)]
        public decimal? Longitude { get; set; }
    }
}
