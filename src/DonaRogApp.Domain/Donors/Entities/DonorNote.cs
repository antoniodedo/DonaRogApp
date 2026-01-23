// --------------------------------------------------------------
// Domain/Donors/Entities/DonorNote.cs
// --------------------------------------------------------------
using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Timing;

namespace DonaRogApp.Domain.Donors.Entities
{
    /// <summary>
    /// Child Entity: Donor Note
    /// Represents a historical note/interaction with a donor.
    /// Used for tracking calls, meetings, events, and other interactions.
    /// </summary>
    public class DonorNote : FullAuditedEntity<Guid>, IMultiTenant
    {
        // --------------------------------------------------------------
        // MULTI-TENANCY
        // --------------------------------------------------------------

        /// <summary>
        /// Tenant ID (inherited from parent Donor)
        /// </summary>
        public Guid? TenantId { get; private set; }

        // --------------------------------------------------------------
        // PARENT RELATIONSHIP
        // --------------------------------------------------------------

        /// <summary>
        /// Parent Donor ID
        /// </summary>
        public Guid DonorId { get; private set; }

        /// <summary>
        /// Parent Donor (navigation property)
        /// </summary>
        public virtual Donor Donor { get; private set; }

        // --------------------------------------------------------------
        // NOTE PROPERTIES
        // --------------------------------------------------------------

        /// <summary>
        /// Title/subject of the note
        /// Example: "Phone call - discussed new campaign"
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// Full content of the note
        /// </summary>
        public string Content { get; private set; }

        /// <summary>
        /// Category/type of note
        /// Example: "Phone Call", "Meeting", "Email", "Event"
        /// </summary>
        public string? Category { get; private set; }

        /// <summary>
        /// Date/time of the interaction
        /// </summary>
        public DateTime InteractionDate { get; private set; }

        // --------------------------------------------------------------
        // TRACKING
        // --------------------------------------------------------------

        /// <summary>
        /// User who created this note
        /// </summary>
        public Guid? CreatedByUserId { get; private set; }

        /// <summary>
        /// Is this note important/flagged?
        /// </summary>
        public bool IsImportant { get; private set; }

        /// <summary>
        /// Is this note private (only visible to creator)?
        /// </summary>
        public bool IsPrivate { get; private set; }

        // --------------------------------------------------------------
        // CONSTRUCTOR
        // --------------------------------------------------------------

        /// <summary>
        /// Protected constructor for EF Core
        /// Use factory method Create() to instantiate
        /// </summary>
        protected DonorNote()
        {
            Title = string.Empty;
            Content = string.Empty;
            InteractionDate = DateTime.UtcNow;
            IsImportant = false;
            IsPrivate = false;
        }

        // --------------------------------------------------------------
        // FACTORY METHOD
        // --------------------------------------------------------------

        /// <summary>
        /// Creates new DonorNote entity
        /// </summary>
        internal static DonorNote Create(
            Guid donorId,
            string title,
            string content,
            Guid? tenantId,
            DateTime? interactionDate = null,
            string? category = null,
            Guid? createdByUserId = null,
            bool isImportant = false,
            bool isPrivate = false)
        {
            return new DonorNote
            {
                Id = Guid.NewGuid(),
                DonorId = donorId,
                Title = title.Trim(),
                Content = content.Trim(),
                Category = category?.Trim(),
                InteractionDate = interactionDate ?? DateTime.UtcNow,
                TenantId = tenantId,
                CreatedByUserId = createdByUserId,
                IsImportant = isImportant,
                IsPrivate = isPrivate
            };
        }

        // --------------------------------------------------------------
        // BUSINESS METHODS - Update
        // --------------------------------------------------------------

        /// <summary>
        /// Updates note content
        /// </summary>
        internal void Update(
            string title,
            string content,
            string? category = null,
            DateTime? interactionDate = null)
        {
            Title = title.Trim();
            Content = content.Trim();
            Category = category?.Trim();

            if (interactionDate.HasValue)
                InteractionDate = interactionDate.Value;
        }

        /// <summary>
        /// Marks note as important
        /// </summary>
        internal void MarkAsImportant()
        {
            IsImportant = true;
        }

        /// <summary>
        /// Removes important flag
        /// </summary>
        internal void RemoveImportantFlag()
        {
            IsImportant = false;
        }

        /// <summary>
        /// Marks note as private
        /// </summary>
        internal void MarkAsPrivate()
        {
            IsPrivate = true;
        }

        /// <summary>
        /// Makes note public
        /// </summary>
        internal void MakePublic()
        {
            IsPrivate = false;
        }

        // --------------------------------------------------------------
        // BUSINESS METHODS - Soft Delete
        // --------------------------------------------------------------

        /// <summary>
        /// Soft deletes note
        /// </summary>
        internal void Delete()
        {
            IsDeleted = true;
            DeletionTime = DateTime.UtcNow;
        }
    }
}