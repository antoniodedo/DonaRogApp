// --------------------------------------------------------------
// Domain/Donors/Entities/DonorAddress.cs
// --------------------------------------------------------------
using DonaRogApp.Enums.Shared;
using DonaRogApp.ValueObjects;
using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Timing;

namespace DonaRogApp.Domain.Donors.Entities
{
    /// <summary>
    /// Child Entity: Donor Address
    /// Represents a postal address associated with a donor.
    /// Supports address history with start/end dates.
    /// </summary>
    public class DonorAddress : FullAuditedEntity<Guid>, IMultiTenant
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
        // ADDRESS PROPERTIES
        // --------------------------------------------------------------

        /// <summary>
        /// Street address (including number)
        /// Example: Via Roma, 123
        /// </summary>
        public string Street { get; private set; }

        /// <summary>
        /// City
        /// Example: Milano
        /// </summary>
        public string City { get; private set; }

        /// <summary>
        /// Province/State (optional)
        /// Example: MI (Milan)
        /// </summary>
        public string? Province { get; private set; }

        /// <summary>
        /// Region (optional)
        /// Example: Lombardia
        /// </summary>
        public string? Region { get; private set; }

        /// <summary>
        /// Postal/ZIP code
        /// Example: 20121
        /// </summary>
        public string PostalCode { get; private set; }

        /// <summary>
        /// Country
        /// Example: Italy, France, Germany
        /// </summary>
        public string Country { get; private set; }

        /// <summary>
        /// Address type (Home, Work, Billing, etc.)
        /// </summary>
        public AddressType Type { get; private set; }

        /// <summary>
        /// Is this the default address for the donor?
        /// Only one active address can be default at a time.
        /// </summary>
        public bool IsDefault { get; private set; }

        // --------------------------------------------------------------
        // TEMPORAL TRACKING (Address History)
        // --------------------------------------------------------------

        /// <summary>
        /// Start date of this address
        /// When donor started living/using this address
        /// </summary>
        public DateTime StartDate { get; private set; }

        /// <summary>
        /// End date of this address (optional)
        /// When donor stopped living/using this address
        /// NULL = currently active address
        /// </summary>
        public DateTime? EndDate { get; private set; }

        // --------------------------------------------------------------
        // VERIFICATION
        // --------------------------------------------------------------

        /// <summary>
        /// Is address verified?
        /// </summary>
        public bool IsVerified { get; private set; }

        /// <summary>
        /// Date address was verified
        /// </summary>
        public DateTime? VerifiedDate { get; private set; }

        // --------------------------------------------------------------
        // TRACKING
        // --------------------------------------------------------------

        /// <summary>
        /// Notes about this address
        /// </summary>
        public string? Notes { get; private set; }

        // --------------------------------------------------------------
        // CONSTRUCTOR
        // --------------------------------------------------------------

        /// <summary>
        /// Protected constructor for EF Core
        /// Use factory method Create() to instantiate
        /// </summary>
        protected DonorAddress()
        {
            Street = string.Empty;
            City = string.Empty;
            PostalCode = string.Empty;
            Country = string.Empty;
            StartDate = DateTime.UtcNow;
            IsDefault = false;
            IsVerified = false;
        }

        // --------------------------------------------------------------
        // FACTORY METHOD
        // --------------------------------------------------------------

        /// <summary>
        /// Creates new DonorAddress entity
        /// </summary>
        internal static DonorAddress Create(
            Guid donorId,
            string street,
            string city,
            string postalCode,
            string country,
            AddressType type,
            DateTime startDate,
            Guid? tenantId,
            string? province = null,
            string? region = null,
            string? notes = null)
        {
            return new DonorAddress
            {
                Id = Guid.NewGuid(),
                DonorId = donorId,
                Street = street.Trim(),
                City = city.Trim(),
                Province = province?.Trim(),
                Region = region?.Trim(),
                PostalCode = postalCode.Trim(),
                Country = country.Trim(),
                Type = type,
                StartDate = startDate,
                TenantId = tenantId,
                Notes = notes,
                IsDefault = false,
                IsVerified = false
            };
        }

        // --------------------------------------------------------------
        // BUSINESS METHODS - Default
        // --------------------------------------------------------------

        /// <summary>
        /// Sets this address as default
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
        // BUSINESS METHODS - Temporal
        // --------------------------------------------------------------

        /// <summary>
        /// Ends this address (sets end date)
        /// </summary>
        internal void End(DateTime endDate)
        {
            EndDate = endDate;

            // Cannot be default if ended
            IsDefault = false;
        }

        /// <summary>
        /// Checks if address is active (not ended)
        /// </summary>
        public bool IsActive()
        {
            return !EndDate.HasValue;
        }

        /// <summary>
        /// Checks if address was active at specific date
        /// </summary>
        public bool IsActiveAt(DateTime date)
        {
            return StartDate <= date &&
                   (!EndDate.HasValue || EndDate.Value >= date);
        }

        // --------------------------------------------------------------
        // BUSINESS METHODS - Verification
        // --------------------------------------------------------------

        /// <summary>
        /// Marks address as verified
        /// </summary>
        internal void Verify()
        {
            IsVerified = true;
            VerifiedDate = DateTime.UtcNow;
        }

        /// <summary>
        /// Marks address as unverified
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
        /// Updates address details
        /// </summary>
        internal void Update(
            string street,
            string city,
            string postalCode,
            string country,
            string? province = null,
            string? region = null)
        {
            Street = street.Trim();
            City = city.Trim();
            Province = province?.Trim();
            Region = region?.Trim();
            PostalCode = postalCode.Trim();
            Country = country.Trim();

            // Unverify after address change
            Unverify();
        }

        /// <summary>
        /// Updates notes
        /// </summary>
        internal void UpdateNotes(string? notes)
        {
            Notes = notes;
        }

        // --------------------------------------------------------------
        // QUERY METHODS - Formatting
        // --------------------------------------------------------------

        /// <summary>
        /// Gets full address as formatted string
        /// </summary>
        public string GetFullAddress()
        {
            var parts = new List<string>();

            parts.Add(Street);

            if (!string.IsNullOrWhiteSpace(City))
                parts.Add(City);

            if (!string.IsNullOrWhiteSpace(Province))
                parts.Add(Province);

            if (!string.IsNullOrWhiteSpace(PostalCode))
                parts.Add(PostalCode);

            if (!string.IsNullOrWhiteSpace(Country))
                parts.Add(Country);

            return string.Join(", ", parts);
        }

        /// <summary>
        /// Gets short address (street + city)
        /// </summary>
        public string GetShortAddress()
        {
            return $"{Street}, {City}";
        }

        // --------------------------------------------------------------
        // BUSINESS METHODS - Soft Delete
        // --------------------------------------------------------------

        /// <summary>
        /// Soft deletes address
        /// </summary>
        internal void Delete()
        {
            IsDeleted = true;
            DeletionTime = DateTime.UtcNow;
        }
    }
}