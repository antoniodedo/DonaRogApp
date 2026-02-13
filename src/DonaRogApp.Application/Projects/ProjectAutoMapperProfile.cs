using AutoMapper;
using DonaRogApp.Application.Contracts.Projects.Dto;
using DonaRogApp.Domain.Projects.Entities;

namespace DonaRogApp.Application.Projects
{
    /// <summary>
    /// AutoMapper Profile for Project Aggregate
    /// Configuration for Entity <-> DTO mappings
    /// </summary>
    public class ProjectAutoMapperProfile : Profile
    {
        public ProjectAutoMapperProfile()
        {
            // ======================================================================
            // PROJECT MAPPINGS
            // ======================================================================

            // Entity -> ProjectDto (full)
            CreateMap<Project, ProjectDto>()
                .ForMember(d => d.TargetCompletionPercentage, opt => opt.Ignore()) // Calculated in service
                .ForMember(d => d.RemainingAmount, opt => opt.Ignore()) // Calculated in service
                .ForMember(d => d.HasReachedTarget, opt => opt.Ignore()) // Calculated in service
                .ForMember(d => d.IsOngoing, opt => opt.Ignore()); // Calculated in service

            // Entity -> ProjectListDto (simplified)
            CreateMap<Project, ProjectListDto>()
                .ForMember(d => d.TargetCompletionPercentage, opt => opt.Ignore()) // Calculated in service
                .ForMember(d => d.IsOngoing, opt => opt.Ignore()); // Calculated in service

            // CreateProjectDto -> Entity (not used with factory method, but kept for reference)
            CreateMap<CreateProjectDto, Project>()
                .ForMember(d => d.Id, opt => opt.Ignore())
                .ForMember(d => d.TenantId, opt => opt.Ignore())
                .ForMember(d => d.Status, opt => opt.Ignore())
                .ForMember(d => d.TotalAmountRaised, opt => opt.Ignore())
                .ForMember(d => d.TotalDonationsCount, opt => opt.Ignore())
                .ForMember(d => d.AverageDonation, opt => opt.Ignore())
                .ForMember(d => d.LastDonationDate, opt => opt.Ignore())
                .ForMember(d => d.Documents, opt => opt.Ignore());

            // UpdateProjectDto -> Entity (partial update)
            CreateMap<UpdateProjectDto, Project>()
                .ForMember(d => d.Id, opt => opt.Ignore())
                .ForMember(d => d.TenantId, opt => opt.Ignore())
                .ForMember(d => d.TotalAmountRaised, opt => opt.Ignore())
                .ForMember(d => d.TotalDonationsCount, opt => opt.Ignore())
                .ForMember(d => d.AverageDonation, opt => opt.Ignore())
                .ForMember(d => d.LastDonationDate, opt => opt.Ignore())
                .ForMember(d => d.Documents, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // ======================================================================
            // PROJECT DOCUMENT MAPPINGS
            // ======================================================================

            // ProjectDocument -> ProjectDocumentDto
            CreateMap<ProjectDocument, ProjectDocumentDto>();

            // CreateProjectDocumentDto -> ProjectDocument (not used with entity method, but kept for reference)
            CreateMap<CreateProjectDocumentDto, ProjectDocument>()
                .ForMember(d => d.Id, opt => opt.Ignore())
                .ForMember(d => d.ProjectId, opt => opt.Ignore())
                .ForMember(d => d.DisplayOrder, opt => opt.Ignore())
                .ForMember(d => d.Project, opt => opt.Ignore());

            // UpdateProjectDocumentDto -> ProjectDocument
            CreateMap<UpdateProjectDocumentDto, ProjectDocument>()
                .ForMember(d => d.Id, opt => opt.Ignore())
                .ForMember(d => d.ProjectId, opt => opt.Ignore())
                .ForMember(d => d.FileUrl, opt => opt.Ignore())
                .ForMember(d => d.FileType, opt => opt.Ignore())
                .ForMember(d => d.FileSize, opt => opt.Ignore())
                .ForMember(d => d.Project, opt => opt.Ignore());
        }
    }
}
