using DonaRogApp.Enums.Projects;
using System;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace DonaRogApp.Domain.Projects.Entities
{
    /// <summary>
    /// Project Aggregate Root
    /// 
    /// RESPONSIBILITY:
    /// - Store charity project information
    /// - Manage project lifecycle (Active/Inactive/Archived)
    /// - Track project documents and attachments
    /// - Calculate statistics (donations, amounts raised)
    /// - Associate with donations and thank-you letters
    /// 
    /// Business logic is split across partial classes:
    /// - Project.Factory.cs: Creation factory methods
    /// - Project.Updates.cs: Update methods and status transitions
    /// </summary>
    public partial class Project : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {
        // ======================================================================
        // MULTI-TENANCY
        // ======================================================================
        /// <summary>
        /// Tenant ID
        /// </summary>
        public Guid? TenantId { get; private set; }

        // ======================================================================
        // IDENTIFICATION
        // ======================================================================
        /// <summary>
        /// Project code (unique per tenant)
        /// </summary>
        public string Code { get; private set; }

        /// <summary>
        /// Project name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Project description (long text)
        /// </summary>
        public string? Description { get; private set; }

        // ======================================================================
        // CLASSIFICATION
        // ======================================================================
        /// <summary>
        /// Project category
        /// </summary>
        public ProjectCategory Category { get; private set; }

        /// <summary>
        /// Project status (Active/Inactive/Archived)
        /// </summary>
        public ProjectStatus Status { get; private set; }

        // ======================================================================
        // DATES
        // ======================================================================
        /// <summary>
        /// Project start date
        /// </summary>
        public DateTime StartDate { get; private set; }

        /// <summary>
        /// Project end date (optional, null = ongoing)
        /// </summary>
        public DateTime? EndDate { get; private set; }

        // ======================================================================
        // BUDGET AND TARGETS
        // ======================================================================
        /// <summary>
        /// Target amount to raise
        /// </summary>
        public decimal? TargetAmount { get; private set; }

        /// <summary>
        /// Currency (default EUR)
        /// </summary>
        public string Currency { get; private set; }

        // ======================================================================
        // RESPONSIBILITY
        // ======================================================================
        /// <summary>
        /// Person responsible for the project
        /// </summary>
        public string? ResponsiblePerson { get; private set; }

        /// <summary>
        /// Responsible person email
        /// </summary>
        public string? ResponsibleEmail { get; private set; }

        /// <summary>
        /// Responsible person phone
        /// </summary>
        public string? ResponsiblePhone { get; private set; }

        // ======================================================================
        // MEDIA
        // ======================================================================
        /// <summary>
        /// Main image URL
        /// </summary>
        public string? MainImageUrl { get; private set; }

        /// <summary>
        /// Thumbnail image URL (for lists)
        /// </summary>
        public string? ThumbnailUrl { get; private set; }

        // ======================================================================
        // LOCATION (Optional)
        // ======================================================================
        /// <summary>
        /// Project location/country
        /// </summary>
        public string? Location { get; private set; }

        /// <summary>
        /// Geographic coordinates (latitude)
        /// </summary>
        public decimal? Latitude { get; private set; }

        /// <summary>
        /// Geographic coordinates (longitude)
        /// </summary>
        public decimal? Longitude { get; private set; }

        // ======================================================================
        // STATISTICS (Calculated - will be populated when donations are implemented)
        // ======================================================================
        /// <summary>
        /// Total amount raised for this project (calculated from donations)
        /// </summary>
        public decimal TotalAmountRaised { get; private set; }

        /// <summary>
        /// Number of donations received for this project
        /// </summary>
        public int TotalDonationsCount { get; private set; }

        /// <summary>
        /// Average donation amount
        /// </summary>
        public decimal AverageDonation { get; private set; }

        /// <summary>
        /// Last donation date
        /// </summary>
        public DateTime? LastDonationDate { get; private set; }

        // ======================================================================
        // RELATIONSHIPS
        // ======================================================================
        /// <summary>
        /// Project documents (attachments)
        /// </summary>
        public virtual ICollection<ProjectDocument> Documents { get; private set; }

        // ======================================================================
        // CONSTRUCTOR
        // ======================================================================
        /// <summary>
        /// Private constructor for EF Core
        /// </summary>
        private Project()
        {
            Code = string.Empty;
            Name = string.Empty;
            Currency = "EUR";
            Documents = new List<ProjectDocument>();
        }

        /// <summary>
        /// Constructor for creating new project
        /// </summary>
        internal Project(
            Guid id,
            Guid? tenantId,
            string code,
            string name,
            ProjectCategory category,
            DateTime startDate,
            string? description = null)
            : base(id)
        {
            TenantId = tenantId;
            Code = Check.NotNullOrWhiteSpace(code, nameof(code), maxLength: 50);
            Name = Check.NotNullOrWhiteSpace(name, nameof(name), maxLength: 200);
            Category = category;
            StartDate = startDate;
            Description = description;
            
            Status = ProjectStatus.Active;
            Currency = "EUR";
            
            Documents = new List<ProjectDocument>();
            
            VerifyInvariants();
        }

        // ======================================================================
        // QUERY METHODS
        // ======================================================================
        /// <summary>
        /// Check if project is active
        /// </summary>
        public bool IsActive()
        {
            return Status == ProjectStatus.Active;
        }

        /// <summary>
        /// Check if project is archived
        /// </summary>
        public bool IsArchived()
        {
            return Status == ProjectStatus.Archived;
        }

        /// <summary>
        /// Check if project is ongoing (not ended)
        /// </summary>
        public bool IsOngoing()
        {
            return !EndDate.HasValue || EndDate.Value > DateTime.UtcNow;
        }

        /// <summary>
        /// Check if project has ended
        /// </summary>
        public bool HasEnded()
        {
            return EndDate.HasValue && EndDate.Value <= DateTime.UtcNow;
        }

        /// <summary>
        /// Calculate target completion percentage
        /// </summary>
        public decimal GetTargetCompletionPercentage()
        {
            if (!TargetAmount.HasValue || TargetAmount.Value <= 0)
                return 0;

            return (TotalAmountRaised / TargetAmount.Value) * 100;
        }

        /// <summary>
        /// Get remaining amount to reach target
        /// </summary>
        public decimal GetRemainingAmount()
        {
            if (!TargetAmount.HasValue)
                return 0;

            var remaining = TargetAmount.Value - TotalAmountRaised;
            return remaining > 0 ? remaining : 0;
        }

        /// <summary>
        /// Check if target has been reached
        /// </summary>
        public bool HasReachedTarget()
        {
            if (!TargetAmount.HasValue)
                return false;

            return TotalAmountRaised >= TargetAmount.Value;
        }

        // ======================================================================
        // DOCUMENT MANAGEMENT
        // ======================================================================
        /// <summary>
        /// Add a document to the project
        /// </summary>
        public ProjectDocument AddDocument(
            Guid documentId,
            string fileName,
            string fileUrl,
            string? fileType,
            long fileSize,
            string? description = null)
        {
            var document = new ProjectDocument(
                documentId,
                Id,
                fileName,
                fileUrl,
                fileType,
                fileSize,
                description,
                Documents.Count);

            Documents.Add(document);
            return document;
        }

        /// <summary>
        /// Remove a document from the project
        /// </summary>
        public void RemoveDocument(Guid documentId)
        {
            var document = Documents.FirstOrDefault(d => d.Id == documentId);
            if (document != null)
            {
                Documents.Remove(document);
            }
        }

        /// <summary>
        /// Get document by ID
        /// </summary>
        public ProjectDocument? GetDocument(Guid documentId)
        {
            return Documents.FirstOrDefault(d => d.Id == documentId);
        }

        // ======================================================================
        // STATISTICS UPDATE (will be called when donations are implemented)
        // ======================================================================
        /// <summary>
        /// Update statistics from donations
        /// Method to be called by domain events or application service
        /// </summary>
        public void UpdateStatistics(decimal totalAmount, int donationsCount, decimal averageDonation, DateTime? lastDonationDate)
        {
            TotalAmountRaised = totalAmount;
            TotalDonationsCount = donationsCount;
            AverageDonation = averageDonation;
            LastDonationDate = lastDonationDate;
        }

        // ======================================================================
        // INVARIANTS
        // ======================================================================
        /// <summary>
        /// Verify business invariants
        /// </summary>
        internal void VerifyInvariants()
        {
            Check.NotNullOrWhiteSpace(Code, nameof(Code));
            Check.NotNullOrWhiteSpace(Name, nameof(Name));

            if (EndDate.HasValue && EndDate.Value < StartDate)
            {
                throw new BusinessException("DonaRog:ProjectEndDateBeforeStartDate")
                    .WithData("startDate", StartDate)
                    .WithData("endDate", EndDate.Value);
            }

            if (TargetAmount.HasValue && TargetAmount.Value < 0)
            {
                throw new BusinessException("DonaRog:ProjectNegativeTargetAmount")
                    .WithData("targetAmount", TargetAmount.Value);
            }

            if (!string.IsNullOrEmpty(ResponsibleEmail) && !ResponsibleEmail.Contains("@"))
            {
                throw new BusinessException("DonaRog:ProjectInvalidEmail")
                    .WithData("email", ResponsibleEmail);
            }
        }
    }
}
