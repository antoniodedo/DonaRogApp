using DonaRogApp.Application.Contracts.Projects.Dto;
using DonaRogApp.Enums.Projects;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace DonaRogApp.Application.Contracts.Projects
{
    /// <summary>
    /// Project App Service Interface - manages charity projects
    /// </summary>
    public interface IProjectAppService :
        ICrudAppService<
            ProjectDto,
            Guid,
            GetProjectsInput,
            CreateProjectDto,
            UpdateProjectDto>
    {
        /// <summary>
        /// Get list of projects (lightweight with ProjectListDto)
        /// </summary>
        Task<PagedResultDto<ProjectListDto>> GetProjectListAsync(GetProjectsInput input);

        // ======================================================================
        // STATUS MANAGEMENT
        // ======================================================================

        /// <summary>
        /// Change project status
        /// </summary>
        Task ChangeStatusAsync(Guid id, ProjectStatus status);

        /// <summary>
        /// Activate project
        /// </summary>
        Task ActivateAsync(Guid id);

        /// <summary>
        /// Deactivate project (pause)
        /// </summary>
        Task DeactivateAsync(Guid id);

        /// <summary>
        /// Archive project
        /// </summary>
        Task ArchiveAsync(Guid id);

        // ======================================================================
        // DOCUMENT MANAGEMENT
        // ======================================================================

        /// <summary>
        /// Get documents for a project
        /// </summary>
        Task<ListResultDto<ProjectDocumentDto>> GetDocumentsAsync(Guid projectId);

        /// <summary>
        /// Add document to project
        /// </summary>
        Task<ProjectDocumentDto> AddDocumentAsync(Guid projectId, CreateProjectDocumentDto input);

        /// <summary>
        /// Update document info
        /// </summary>
        Task<ProjectDocumentDto> UpdateDocumentAsync(Guid projectId, Guid documentId, UpdateProjectDocumentDto input);

        /// <summary>
        /// Remove document from project
        /// </summary>
        Task RemoveDocumentAsync(Guid projectId, Guid documentId);

        // ======================================================================
        // STATISTICS
        // ======================================================================

        /// <summary>
        /// Get project statistics and KPIs
        /// </summary>
        Task<ProjectStatisticsDto> GetStatisticsAsync(Guid projectId);

        /// <summary>
        /// Get aggregate statistics for all projects (or filtered)
        /// </summary>
        Task<ProjectAggregateStatisticsDto> GetAggregateStatisticsAsync(GetProjectsInput? input = null);
    }

    /// <summary>
    /// Aggregate statistics for multiple projects
    /// </summary>
    public class ProjectAggregateStatisticsDto
    {
        /// <summary>
        /// Total number of projects
        /// </summary>
        public int TotalProjects { get; set; }

        /// <summary>
        /// Number of active projects
        /// </summary>
        public int ActiveProjects { get; set; }

        /// <summary>
        /// Number of inactive projects
        /// </summary>
        public int InactiveProjects { get; set; }

        /// <summary>
        /// Number of archived projects
        /// </summary>
        public int ArchivedProjects { get; set; }

        /// <summary>
        /// Total amount raised across all projects
        /// </summary>
        public decimal TotalAmountRaised { get; set; }

        /// <summary>
        /// Total target amount across all projects
        /// </summary>
        public decimal TotalTargetAmount { get; set; }

        /// <summary>
        /// Total number of donations
        /// </summary>
        public int TotalDonationsCount { get; set; }

        /// <summary>
        /// Average donation amount
        /// </summary>
        public decimal AverageDonation { get; set; }

        /// <summary>
        /// Top 5 projects by amount raised
        /// </summary>
        public System.Collections.Generic.List<TopProjectDto> TopProjectsByAmount { get; set; } = new();

        /// <summary>
        /// Top 5 projects by number of donations
        /// </summary>
        public System.Collections.Generic.List<TopProjectDto> TopProjectsByDonations { get; set; } = new();

        /// <summary>
        /// Distribution by category
        /// </summary>
        public System.Collections.Generic.List<CategoryDistributionDto> CategoryDistribution { get; set; } = new();

        /// <summary>
        /// Projects near target (>90%)
        /// </summary>
        public int ProjectsNearTarget { get; set; }

        /// <summary>
        /// Projects under-funded (<50%)
        /// </summary>
        public int ProjectsUnderFunded { get; set; }
    }

    /// <summary>
    /// Top project summary
    /// </summary>
    public class TopProjectDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public decimal TotalAmountRaised { get; set; }
        public int TotalDonationsCount { get; set; }
        public decimal TargetCompletionPercentage { get; set; }
    }

    /// <summary>
    /// Category distribution
    /// </summary>
    public class CategoryDistributionDto
    {
        public ProjectCategory Category { get; set; }
        public int ProjectCount { get; set; }
        public decimal TotalAmountRaised { get; set; }
        public decimal TotalTargetAmount { get; set; }
    }
}
