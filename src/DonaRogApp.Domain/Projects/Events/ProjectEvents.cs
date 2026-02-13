using DonaRogApp.Enums.Projects;
using System;

namespace DonaRogApp.Domain.Projects.Events
{
    /// <summary>
    /// Base class for project domain events
    /// </summary>
    public abstract class ProjectEventBase
    {
        public Guid ProjectId { get; }
        public Guid? TenantId { get; }
        public DateTime OccurredOn { get; }

        protected ProjectEventBase(Guid projectId, Guid? tenantId)
        {
            ProjectId = projectId;
            TenantId = tenantId;
            OccurredOn = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Event raised when a new project is created
    /// </summary>
    public class ProjectCreatedEvent : ProjectEventBase
    {
        public string Code { get; }
        public string Name { get; }
        public ProjectCategory Category { get; }

        public ProjectCreatedEvent(
            Guid projectId,
            Guid? tenantId,
            string code,
            string name,
            ProjectCategory category)
            : base(projectId, tenantId)
        {
            Code = code;
            Name = name;
            Category = category;
        }
    }

    /// <summary>
    /// Event raised when project status changes
    /// </summary>
    public class ProjectStatusChangedEvent : ProjectEventBase
    {
        public ProjectStatus OldStatus { get; }
        public ProjectStatus NewStatus { get; }

        public ProjectStatusChangedEvent(
            Guid projectId,
            Guid? tenantId,
            ProjectStatus oldStatus,
            ProjectStatus newStatus)
            : base(projectId, tenantId)
        {
            OldStatus = oldStatus;
            NewStatus = newStatus;
        }
    }

    /// <summary>
    /// Event raised when project information is updated
    /// </summary>
    public class ProjectInfoUpdatedEvent : ProjectEventBase
    {
        public string Name { get; }
        public ProjectCategory Category { get; }

        public ProjectInfoUpdatedEvent(
            Guid projectId,
            Guid? tenantId,
            string name,
            ProjectCategory category)
            : base(projectId, tenantId)
        {
            Name = name;
            Category = category;
        }
    }

    /// <summary>
    /// Event raised when a document is added to a project
    /// </summary>
    public class ProjectDocumentAddedEvent : ProjectEventBase
    {
        public Guid DocumentId { get; }
        public string FileName { get; }

        public ProjectDocumentAddedEvent(
            Guid projectId,
            Guid? tenantId,
            Guid documentId,
            string fileName)
            : base(projectId, tenantId)
        {
            DocumentId = documentId;
            FileName = fileName;
        }
    }

    /// <summary>
    /// Event raised when a document is removed from a project
    /// </summary>
    public class ProjectDocumentRemovedEvent : ProjectEventBase
    {
        public Guid DocumentId { get; }

        public ProjectDocumentRemovedEvent(
            Guid projectId,
            Guid? tenantId,
            Guid documentId)
            : base(projectId, tenantId)
        {
            DocumentId = documentId;
        }
    }

    /// <summary>
    /// Event raised when project statistics are updated
    /// </summary>
    public class ProjectStatisticsUpdatedEvent : ProjectEventBase
    {
        public decimal TotalAmountRaised { get; }
        public int TotalDonationsCount { get; }
        public decimal AverageDonation { get; }

        public ProjectStatisticsUpdatedEvent(
            Guid projectId,
            Guid? tenantId,
            decimal totalAmountRaised,
            int totalDonationsCount,
            decimal averageDonation)
            : base(projectId, tenantId)
        {
            TotalAmountRaised = totalAmountRaised;
            TotalDonationsCount = totalDonationsCount;
            AverageDonation = averageDonation;
        }
    }

    /// <summary>
    /// Event raised when project target is reached
    /// </summary>
    public class ProjectTargetReachedEvent : ProjectEventBase
    {
        public decimal TargetAmount { get; }
        public decimal TotalAmountRaised { get; }

        public ProjectTargetReachedEvent(
            Guid projectId,
            Guid? tenantId,
            decimal targetAmount,
            decimal totalAmountRaised)
            : base(projectId, tenantId)
        {
            TargetAmount = targetAmount;
            TotalAmountRaised = totalAmountRaised;
        }
    }
}
