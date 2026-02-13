using DonaRogApp.Enums.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Timing;

namespace DonaRogApp.Domain.Donors.Entities
{
    /// <summary>
    /// Child Entity: Donor Email Address
    /// Represents an email address associated with a donor.
    /// Supports verification, bounce tracking, and default email management.
    /// </summary>
    public class DonorEmail : FullAuditedEntity<Guid>, IMultiTenant
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
        // EMAIL PROPERTIES
        // --------------------------------------------------------------

        /// <summary>
        /// Email address (normalized, lowercase)
        /// Example: john.doe@example.com
        /// </summary>
        public string EmailAddress { get; private set; }

        /// <summary>
        /// Email type (Personal, Work, Other)
        /// </summary>
        public EmailType Type { get; private set; }

        /// <summary>
        /// Is this the default email for the donor?
        /// Only one email can be default at a time.
        /// </summary>
        public bool IsDefault { get; private set; }

        // --------------------------------------------------------------
        // VERIFICATION
        // --------------------------------------------------------------

        /// <summary>
        /// Is email verified?
        /// </summary>
        public bool IsVerified { get; private set; }

        /// <summary>
        /// Date email was verified
        /// </summary>
        public DateTime? VerifiedDate { get; private set; }

        /// <summary>
        /// Verification token (for email confirmation)
        /// </summary>
        public string? VerificationToken { get; private set; }

        // --------------------------------------------------------------
        // BOUNCE TRACKING
        // --------------------------------------------------------------

        /// <summary>
        /// Number of bounces (failed deliveries)
        /// </summary>
        public int BounceCount { get; private set; }

        /// <summary>
        /// Date of last bounce
        /// </summary>
        public DateTime? LastBounceDate { get; private set; }

        /// <summary>
        /// Reason for last bounce
        /// </summary>
        public string? LastBounceReason { get; private set; }

        /// <summary>
        /// Is email marked as invalid (too many bounces)?
        /// </summary>
        public bool IsInvalid { get; private set; }

        // --------------------------------------------------------------
        // TRACKING
        // --------------------------------------------------------------

        /// <summary>
        /// Date email was added to donor
        /// </summary>
        public DateTime DateAdded { get; private set; }

        /// <summary>
        /// Notes about this email
        /// </summary>
        public string? Notes { get; private set; }

        // --------------------------------------------------------------
        // CONSTRUCTOR
        // --------------------------------------------------------------

        /// <summary>
        /// Protected constructor for EF Core
        /// Use factory method Create() to instantiate
        /// </summary>
        protected DonorEmail()
        {
            EmailAddress = string.Empty;
            DateAdded = DateTime.UtcNow;
            BounceCount = 0;
            IsDefault = false;
            IsVerified = false;
            IsInvalid = false;
        }

        // --------------------------------------------------------------
        // FACTORY METHOD
        // --------------------------------------------------------------

        /// <summary>
        /// Creates new DonorEmail entity
        /// </summary>
        internal static DonorEmail Create(
            Guid donorId,
            string emailAddress,
            EmailType type,
            Guid? tenantId,
            string? notes = null)
        {
            return new DonorEmail
            {
                Id = Guid.NewGuid(),
                DonorId = donorId,
                EmailAddress = emailAddress.ToLowerInvariant().Trim(),
                Type = type,
                TenantId = tenantId,
                DateAdded = DateTime.UtcNow,
                Notes = notes,
                IsDefault = false,
                IsVerified = false,
                BounceCount = 0,
                IsInvalid = false
            };
        }

        // --------------------------------------------------------------
        // BUSINESS METHODS - Default
        // --------------------------------------------------------------

        /// <summary>
        /// Sets this email as default
        /// </summary>
        internal void SetAsDefault()
        {
            IsDefault = true;
        }

        /// <summary>
        /// Removes default status
        /// </summary>
        internal void RemoveDefault()
        {
            IsDefault = false;
        }

        // --------------------------------------------------------------
        // BUSINESS METHODS - Verification
        // --------------------------------------------------------------

        /// <summary>
        /// Marks email as verified
        /// </summary>
        internal void Verify()
        {
            IsVerified = true;
            VerifiedDate = DateTime.UtcNow;
            VerificationToken = null;

            // Reset invalid status if verified
            IsInvalid = false;
        }

        /// <summary>
        /// Generates verification token
        /// </summary>
        public void GenerateVerificationToken()
        {
            VerificationToken = Guid.NewGuid().ToString("N");
        }

        /// <summary>
        /// Marks email as unverified
        /// </summary>
        internal void Unverify()
        {
            IsVerified = false;
            VerifiedDate = null;
        }

        // --------------------------------------------------------------
        // BUSINESS METHODS - Bounce Tracking
        // --------------------------------------------------------------

        /// <summary>
        /// Records email bounce
        /// </summary>
        internal void RecordBounce(string? reason = null)
        {
            BounceCount++;
            LastBounceDate = DateTime.UtcNow;
            LastBounceReason = reason;

            // Mark as invalid if too many bounces (3+)
            if (BounceCount >= 3)
            {
                IsInvalid = true;
                IsVerified = false;
            }
        }

        /// <summary>
        /// Resets bounce count (after successful delivery)
        /// </summary>
        internal void ResetBounceCount()
        {
            BounceCount = 0;
            LastBounceDate = null;
            LastBounceReason = null;
            IsInvalid = false;
        }

        /// <summary>
        /// Marks email as invalid
        /// </summary>
        internal void MarkAsInvalid(string? reason = null)
        {
            IsInvalid = true;
            IsVerified = false;
            LastBounceReason = reason;
        }

        // --------------------------------------------------------------
        // BUSINESS METHODS - Notes
        // --------------------------------------------------------------

        /// <summary>
        /// Updates notes
        /// </summary>
        internal void UpdateNotes(string? notes)
        {
            Notes = notes;
        }

        // --------------------------------------------------------------
        // BUSINESS METHODS - Soft Delete / Reactivate
        // --------------------------------------------------------------

        /// <summary>
        /// Soft deletes email
        /// </summary>
        internal void Delete()
        {
            IsDeleted = true;
            DeletionTime = DateTime.UtcNow;
        }

        /// <summary>
        /// Reactivates a soft-deleted email
        /// </summary>
        internal void Reactivate(EmailType type)
        {
            IsDeleted = false;
            DeletionTime = null;
            DeleterId = null;
            Type = type;
            DateAdded = DateTime.UtcNow;
            
            // Reset verification and bounce status
            IsVerified = false;
            VerifiedDate = null;
            VerificationToken = null;
            BounceCount = 0;
            LastBounceDate = null;
            LastBounceReason = null;
            IsInvalid = false;
        }
    }
}
