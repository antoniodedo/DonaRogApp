using DonaRogApp.Enums.Projects;
using System;
using Volo.Abp.Application.Dtos;

namespace DonaRogApp.Application.Contracts.Projects.Dto
{
    /// <summary>
    /// Input DTO for filtering projects list
    /// </summary>
    public class GetProjectsInput : PagedAndSortedResultRequestDto
    {
        /// <summary>
        /// Generic filter (searches in code, name, description)
        /// </summary>
        public string? Filter { get; set; }

        /// <summary>
        /// Filter by status
        /// </summary>
        public ProjectStatus? Status { get; set; }

        /// <summary>
        /// Filter by category
        /// </summary>
        public ProjectCategory? Category { get; set; }

        /// <summary>
        /// Filter by start date from
        /// </summary>
        public DateTime? StartDateFrom { get; set; }

        /// <summary>
        /// Filter by start date to
        /// </summary>
        public DateTime? StartDateTo { get; set; }

        /// <summary>
        /// Filter by end date from
        /// </summary>
        public DateTime? EndDateFrom { get; set; }

        /// <summary>
        /// Filter by end date to
        /// </summary>
        public DateTime? EndDateTo { get; set; }

        /// <summary>
        /// Filter only projects with budget
        /// </summary>
        public bool? HasBudget { get; set; }

        /// <summary>
        /// Filter only ongoing projects (not ended)
        /// </summary>
        public bool? OnlyOngoing { get; set; }

        /// <summary>
        /// Filter by responsible person
        /// </summary>
        public string? ResponsiblePerson { get; set; }

        /// <summary>
        /// Filter by location
        /// </summary>
        public string? Location { get; set; }

        public GetProjectsInput()
        {
            MaxResultCount = 25;
            Sorting = "CreationTime DESC";
        }
    }
}
