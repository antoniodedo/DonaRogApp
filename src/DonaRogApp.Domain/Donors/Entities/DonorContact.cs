// --------------------------------------------------------------
// Domain/Donors/Entities/DonorContact.cs
// --------------------------------------------------------------
using DonaRogApp.Enums.Shared;
using DonaRogApp.ValueObjects;
using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace DonaRogApp.Domain.Donors.Entities
{
    /// <summary>
    /// Child Entity: Donor Contact (Phone Number)
    /// Represents a phone number associated with a donor.
    /// Supports verification and default contact management.
    /// </summary>
    public class DonorContact : FullAuditedEntity<Guid>, IMultiTenant
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
        // CONTACT PROPERTIES
        // --------------------------------------------------------------

        /// <summary>
        /// Phone number (stored as Value Object)
        /// </summary>
        public PhoneNumber PhoneNumber { get; private set; }

        /// <summary>
        /// Contact type (Mobile, Landline, Fax, etc.)
        /// </summary>
        public ContactType Type { get; private set; }

        /// <summary>
        /// Is this the default contact for the donor?
        /// Only one contact can be default at a time.
        /// </summary>
        public bool IsDefault { get; private set; }

        // --------------------------------------------------------------
        // VERIFICATION
        // --------------------------------------------------------------

        /// <summary>
        /// Is contact verified?
        /// </summary>
        public bool IsVerified { get; private set; }

        /// <summary>
        /// Date contact was verified
        /// </summary>
        public DateTime? VerifiedDate { get; private set; }

        // --------------------------------------------------------------
        // TRACKING
        // --------------------------------------------------------------

        /// <summary>
        /// Date contact was added to donor
        /// </summary>
        public DateTime DateAdded { get; private set; }

        /// <summary>
        /// Notes about this contact
        /// </summary>
        public string? Notes { get; private set; }

        // --------------------------------------------------------------
        // CONSTRUCTOR
        // --------------------------------------------------------------

        /// <summary>
        /// Protected constructor for EF Core
        /// Use factory method Create() to instantiate
        /// </summary>
        protected DonorContact()
        {
            PhoneNumber = null!;
            DateAdded = DateTime.UtcNow;
            IsDefault = false;
            IsVerified = false;
        }

        // --------------------------------------------------------------
        // FACTORY METHOD
        // --------------------------------------------------------------

        /// <summary>
        /// Creates new DonorContact entity
        /// </summary>
        internal static DonorContact Create(
            Guid donorId,
            PhoneNumber phoneNumber,
            ContactType type,
            Guid? tenantId,
            string? notes = null)
        {
            return new DonorContact
            {
                Id = Guid.NewGuid(),
                DonorId = donorId,
                PhoneNumber = phoneNumber,
                Type = type,
                TenantId = tenantId,
                DateAdded = DateTime.UtcNow,
                Notes = notes,
                IsDefault = false,
                IsVerified = false
            };
        }

        // --------------------------------------------------------------
        // BUSINESS METHODS - Default
        // --------------------------------------------------------------

        /// <summary>
        /// Sets this contact as default
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
        /// Marks contact as verified
        /// </summary>
        internal void Verify()
        {
            IsVerified = true;
            VerifiedDate = DateTime.UtcNow;
        }

        /// <summary>
        /// Marks contact as unverified
        /// </summary>
        internal void Unverify()
        {
            IsVerified = false;
            VerifiedDate = null;
        }

        // --------------------------------------------------------------
        // BUSINESS METHODS - Update
        // --------------------------------------------------------------

        /// <summary>
        /// Updates phone number
        /// </summary>
        internal void UpdatePhoneNumber(PhoneNumber newPhoneNumber)
        {
            PhoneNumber = newPhoneNumber;

            // Unverify after phone change
            Unverify();
        }

        /// <summary>
        /// Updates contact type
        /// </summary>
        internal void UpdateType(ContactType newType)
        {
            Type = newType;
        }

        /// <summary>
        /// Updates notes
        /// </summary>
        internal void UpdateNotes(string? notes)
        {
            Notes = notes;
        }

        // --------------------------------------------------------------
        // BUSINESS METHODS - Soft Delete
        // --------------------------------------------------------------

        /// <summary>
        /// Soft deletes contact
        /// </summary>
        internal void Delete()
        {
            IsDeleted = true;
            DeletionTime = DateTime.UtcNow;
        }
    }
}
