using AutoMapper;
using DonaRogApp.Application.Contracts.Projects;
using DonaRogApp.Application.Contracts.Projects.Dto;
using DonaRogApp.Domain.Projects.Entities;
using DonaRogApp.Enums.Projects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace DonaRogApp.Application.Projects
{
    /// <summary>
    /// Application Service for managing Projects
    /// </summary>
    public class ProjectAppService : CrudAppService<
        Project,
        ProjectDto,
        Guid,
        GetProjectsInput,
        CreateProjectDto,
        UpdateProjectDto>,
        IProjectAppService
    {
        private readonly IRepository<Project, Guid> _projectRepository;
        private readonly IRepository<ProjectDocument, Guid> _documentRepository;

        public ProjectAppService(
            IRepository<Project, Guid> projectRepository,
            IRepository<ProjectDocument, Guid> documentRepository)
            : base(projectRepository)
        {
            _projectRepository = projectRepository;
            _documentRepository = documentRepository;
        }

        // ======================================================================
        // OVERRIDE CRUD METHODS
        // ======================================================================

        protected override async Task<IQueryable<Project>> CreateFilteredQueryAsync(GetProjectsInput input)
        {
            var query = await base.CreateFilteredQueryAsync(input);

            // Apply filters
            query = ApplyFilters(query, input);

            return query;
        }

        private IQueryable<Project> ApplyFilters(IQueryable<Project> query, GetProjectsInput input)
        {
            // General search filter
            if (!string.IsNullOrWhiteSpace(input.Filter))
            {
                query = query.Where(p =>
                    p.Code.Contains(input.Filter) ||
                    p.Name.Contains(input.Filter) ||
                    (p.Description != null && p.Description.Contains(input.Filter)));
            }

            // Status filter
            if (input.Status.HasValue)
            {
                query = query.Where(p => p.Status == input.Status.Value);
            }

            // Category filter
            if (input.Category.HasValue)
            {
                query = query.Where(p => p.Category == input.Category.Value);
            }

            // Start date range filters
            if (input.StartDateFrom.HasValue)
            {
                query = query.Where(p => p.StartDate >= input.StartDateFrom.Value);
            }

            if (input.StartDateTo.HasValue)
            {
                query = query.Where(p => p.StartDate <= input.StartDateTo.Value);
            }

            // End date range filters
            if (input.EndDateFrom.HasValue)
            {
                query = query.Where(p => p.EndDate >= input.EndDateFrom.Value);
            }

            if (input.EndDateTo.HasValue)
            {
                query = query.Where(p => p.EndDate <= input.EndDateTo.Value);
            }

            // Has budget filter
            if (input.HasBudget.HasValue)
            {
                if (input.HasBudget.Value)
                {
                    query = query.Where(p => p.TargetAmount != null && p.TargetAmount > 0);
                }
                else
                {
                    query = query.Where(p => p.TargetAmount == null || p.TargetAmount <= 0);
                }
            }

            // Only ongoing filter
            if (input.OnlyOngoing.HasValue && input.OnlyOngoing.Value)
            {
                query = query.Where(p => p.EndDate == null || p.EndDate > DateTime.UtcNow);
            }

            // Responsible person filter
            if (!string.IsNullOrWhiteSpace(input.ResponsiblePerson))
            {
                query = query.Where(p => p.ResponsiblePerson != null && p.ResponsiblePerson.Contains(input.ResponsiblePerson));
            }

            // Location filter
            if (!string.IsNullOrWhiteSpace(input.Location))
            {
                query = query.Where(p => p.Location != null && p.Location.Contains(input.Location));
            }

            return query;
        }

        public override async Task<ProjectDto> CreateAsync(CreateProjectDto input)
        {
            // Create project using factory method
            var project = Project.Create(
                id: GuidGenerator.Create(),
                tenantId: CurrentTenant.Id,
                code: input.Code,
                name: input.Name,
                category: input.Category,
                startDate: input.StartDate,
                description: input.Description);

            // Set dates
            if (input.EndDate.HasValue)
            {
                project.UpdateDates(input.StartDate, input.EndDate.Value);
            }

            // Set budget
            if (input.TargetAmount.HasValue)
            {
                project.SetBudget(input.TargetAmount.Value, input.Currency);
            }

            // Set responsible
            if (!string.IsNullOrWhiteSpace(input.ResponsiblePerson) ||
                !string.IsNullOrWhiteSpace(input.ResponsibleEmail) ||
                !string.IsNullOrWhiteSpace(input.ResponsiblePhone))
            {
                project.SetResponsible(input.ResponsiblePerson, input.ResponsibleEmail, input.ResponsiblePhone);
            }

            // Set images
            if (!string.IsNullOrWhiteSpace(input.MainImageUrl))
            {
                project.SetMainImage(input.MainImageUrl, input.ThumbnailUrl);
            }

            // Set location
            if (!string.IsNullOrWhiteSpace(input.Location))
            {
                project.SetLocation(input.Location, input.Latitude, input.Longitude);
            }

            await _projectRepository.InsertAsync(project);
            
            if (CurrentUnitOfWork != null)
            {
                await CurrentUnitOfWork.SaveChangesAsync();
            }

            return await MapToGetOutputDtoAsync(project);
        }

        public override async Task<ProjectDto> UpdateAsync(Guid id, UpdateProjectDto input)
        {
            var project = await _projectRepository.GetAsync(id);

            // Update basic info
            project.UpdateInfo(input.Name, input.Category, input.Description);

            // Update code if changed
            if (project.Code != input.Code)
            {
                project.UpdateCode(input.Code);
            }

            // Update status
            if (project.Status != input.Status)
            {
                project.ChangeStatus(input.Status);
            }

            // Update dates
            project.UpdateDates(input.StartDate, input.EndDate);

            // Update budget
            project.SetBudget(input.TargetAmount, input.Currency);

            // Update responsible
            project.SetResponsible(input.ResponsiblePerson, input.ResponsibleEmail, input.ResponsiblePhone);

            // Update images
            project.SetMainImage(input.MainImageUrl, input.ThumbnailUrl);

            // Update location
            project.SetLocation(input.Location, input.Latitude, input.Longitude);

            await _projectRepository.UpdateAsync(project);

            if (CurrentUnitOfWork != null)
            {
                await CurrentUnitOfWork.SaveChangesAsync();
            }

            return await MapToGetOutputDtoAsync(project);
        }

        protected override async Task<ProjectDto> MapToGetOutputDtoAsync(Project entity)
        {
            var dto = await base.MapToGetOutputDtoAsync(entity);

            // Calculate additional properties
            dto.TargetCompletionPercentage = entity.GetTargetCompletionPercentage();
            dto.RemainingAmount = entity.GetRemainingAmount();
            dto.HasReachedTarget = entity.HasReachedTarget();
            dto.IsOngoing = entity.IsOngoing();

            return dto;
        }

        // ======================================================================
        // LIGHTWEIGHT LIST
        // ======================================================================

        public async Task<PagedResultDto<ProjectListDto>> GetProjectListAsync(GetProjectsInput input)
        {
            var query = await CreateFilteredQueryAsync(input);

            var totalCount = await AsyncExecuter.CountAsync(query);

            // Apply sorting
            if (!string.IsNullOrWhiteSpace(input.Sorting))
            {
                query = query.OrderBy(input.Sorting);
            }

            // Apply paging
            query = query.Skip(input.SkipCount).Take(input.MaxResultCount);

            var entities = await AsyncExecuter.ToListAsync(query);

            var dtos = entities.Select(entity =>
            {
                var dto = ObjectMapper.Map<Project, ProjectListDto>(entity);
                dto.TargetCompletionPercentage = entity.GetTargetCompletionPercentage();
                dto.IsOngoing = entity.IsOngoing();
                return dto;
            }).ToList();

            return new PagedResultDto<ProjectListDto>(totalCount, dtos);
        }

        // ======================================================================
        // STATUS MANAGEMENT
        // ======================================================================

        public async Task ChangeStatusAsync(Guid id, ProjectStatus status)
        {
            var project = await _projectRepository.GetAsync(id);
            project.ChangeStatus(status);
            await _projectRepository.UpdateAsync(project);
        }

        public async Task ActivateAsync(Guid id)
        {
            await ChangeStatusAsync(id, ProjectStatus.Active);
        }

        public async Task DeactivateAsync(Guid id)
        {
            await ChangeStatusAsync(id, ProjectStatus.Inactive);
        }

        public async Task ArchiveAsync(Guid id)
        {
            await ChangeStatusAsync(id, ProjectStatus.Archived);
        }

        // ======================================================================
        // DOCUMENT MANAGEMENT
        // ======================================================================

        public async Task<ListResultDto<ProjectDocumentDto>> GetDocumentsAsync(Guid projectId)
        {
            var project = await _projectRepository.GetAsync(projectId, includeDetails: true);
            var documents = project.Documents.OrderBy(d => d.DisplayOrder).ToList();
            var dtos = ObjectMapper.Map<List<ProjectDocument>, List<ProjectDocumentDto>>(documents);
            return new ListResultDto<ProjectDocumentDto>(dtos);
        }

        public async Task<ProjectDocumentDto> AddDocumentAsync(Guid projectId, CreateProjectDocumentDto input)
        {
            var project = await _projectRepository.GetAsync(projectId, includeDetails: true);

            var document = project.AddDocument(
                GuidGenerator.Create(),
                input.FileName,
                input.FileUrl,
                input.FileType,
                input.FileSize,
                input.Description);

            await _projectRepository.UpdateAsync(project);

            if (CurrentUnitOfWork != null)
            {
                await CurrentUnitOfWork.SaveChangesAsync();
            }

            return ObjectMapper.Map<ProjectDocument, ProjectDocumentDto>(document);
        }

        public async Task<ProjectDocumentDto> UpdateDocumentAsync(Guid projectId, Guid documentId, UpdateProjectDocumentDto input)
        {
            var project = await _projectRepository.GetAsync(projectId, includeDetails: true);
            var document = project.GetDocument(documentId);

            if (document == null)
            {
                throw new Volo.Abp.BusinessException("DonaRog:DocumentNotFound")
                    .WithData("documentId", documentId);
            }

            document.UpdateInfo(input.FileName, input.Description, input.DisplayOrder);

            await _projectRepository.UpdateAsync(project);

            if (CurrentUnitOfWork != null)
            {
                await CurrentUnitOfWork.SaveChangesAsync();
            }

            return ObjectMapper.Map<ProjectDocument, ProjectDocumentDto>(document);
        }

        public async Task RemoveDocumentAsync(Guid projectId, Guid documentId)
        {
            var project = await _projectRepository.GetAsync(projectId, includeDetails: true);
            project.RemoveDocument(documentId);
            await _projectRepository.UpdateAsync(project);
        }

        // ======================================================================
        // STATISTICS
        // ======================================================================

        public async Task<ProjectStatisticsDto> GetStatisticsAsync(Guid projectId)
        {
            var project = await _projectRepository.GetAsync(projectId);

            // For now, return statistics based on current project data
            // When donations are implemented, these will be calculated from actual donation data
            var statistics = new ProjectStatisticsDto
            {
                ProjectId = project.Id,
                Code = project.Code,
                Name = project.Name,
                TotalAmountRaised = project.TotalAmountRaised,
                TotalDonationsCount = project.TotalDonationsCount,
                AverageDonation = project.AverageDonation,
                LargestDonation = 0, // TODO: Calculate from donations
                SmallestDonation = 0, // TODO: Calculate from donations
                LastDonationDate = project.LastDonationDate,
                FirstDonationDate = null, // TODO: Calculate from donations
                TargetAmount = project.TargetAmount,
                RemainingAmount = project.GetRemainingAmount(),
                TargetCompletionPercentage = project.GetTargetCompletionPercentage(),
                HasReachedTarget = project.HasReachedTarget(),
                YearlyStatistics = new List<YearlyStatisticsDto>(), // TODO: Calculate from donations
                TopDonors = new List<ProjectTopDonorDto>(), // TODO: Calculate from donations
                MonthlyTrend = new List<MonthlyTrendDto>() // TODO: Calculate from donations
            };

            return statistics;
        }

        public async Task<ProjectAggregateStatisticsDto> GetAggregateStatisticsAsync(GetProjectsInput? input = null)
        {
            input ??= new GetProjectsInput();

            var query = await CreateFilteredQueryAsync(input);
            var allProjects = await AsyncExecuter.ToListAsync(query);

            var statistics = new ProjectAggregateStatisticsDto
            {
                TotalProjects = allProjects.Count,
                ActiveProjects = allProjects.Count(p => p.Status == ProjectStatus.Active),
                InactiveProjects = allProjects.Count(p => p.Status == ProjectStatus.Inactive),
                ArchivedProjects = allProjects.Count(p => p.Status == ProjectStatus.Archived),
                TotalAmountRaised = allProjects.Sum(p => p.TotalAmountRaised),
                TotalTargetAmount = allProjects.Where(p => p.TargetAmount.HasValue).Sum(p => p.TargetAmount!.Value),
                TotalDonationsCount = allProjects.Sum(p => p.TotalDonationsCount),
                AverageDonation = allProjects.Count > 0 && allProjects.Sum(p => p.TotalDonationsCount) > 0
                    ? allProjects.Sum(p => p.TotalAmountRaised) / allProjects.Sum(p => p.TotalDonationsCount)
                    : 0,
                ProjectsNearTarget = allProjects.Count(p => p.GetTargetCompletionPercentage() >= 90),
                ProjectsUnderFunded = allProjects.Count(p => p.GetTargetCompletionPercentage() < 50)
            };

            // Top projects by amount
            statistics.TopProjectsByAmount = allProjects
                .OrderByDescending(p => p.TotalAmountRaised)
                .Take(5)
                .Select(p => new TopProjectDto
                {
                    Id = p.Id,
                    Code = p.Code,
                    Name = p.Name,
                    TotalAmountRaised = p.TotalAmountRaised,
                    TotalDonationsCount = p.TotalDonationsCount,
                    TargetCompletionPercentage = p.GetTargetCompletionPercentage()
                })
                .ToList();

            // Top projects by donations count
            statistics.TopProjectsByDonations = allProjects
                .OrderByDescending(p => p.TotalDonationsCount)
                .Take(5)
                .Select(p => new TopProjectDto
                {
                    Id = p.Id,
                    Code = p.Code,
                    Name = p.Name,
                    TotalAmountRaised = p.TotalAmountRaised,
                    TotalDonationsCount = p.TotalDonationsCount,
                    TargetCompletionPercentage = p.GetTargetCompletionPercentage()
                })
                .ToList();

            // Category distribution
            statistics.CategoryDistribution = allProjects
                .GroupBy(p => p.Category)
                .Select(g => new CategoryDistributionDto
                {
                    Category = g.Key,
                    ProjectCount = g.Count(),
                    TotalAmountRaised = g.Sum(p => p.TotalAmountRaised),
                    TotalTargetAmount = g.Where(p => p.TargetAmount.HasValue).Sum(p => p.TargetAmount!.Value)
                })
                .ToList();

            return statistics;
        }
    }
}
