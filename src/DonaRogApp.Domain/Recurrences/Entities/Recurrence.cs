using System;
using System.Collections.Generic;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace DonaRogApp.Domain.Recurrences.Entities
{
    /// <summary>
    /// Recurrence Aggregate Root - Represents annual recurring periods (Christmas, Easter, etc.)
    /// 
    /// RESPONSIBILITY:
    /// - Store recurrence/period properties (dates, validity)
    /// - Manage lifecycle (planned → active → completed)
    /// - Serve as reference for campaigns and thank you letters
    /// 
    /// Business logic is split across partial classes:
    /// - Recurrence.Factory.cs: Creation
    /// - Recurrence.Updates.cs: Update methods and workflow transitions
    /// </summary>
    public partial class Recurrence : FullAuditedAggregateRoot<Guid>, IMultiTenant
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
        /// Recurrence name (e.g., "Natale", "Pasqua", "San Antonio")
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Unique recurrence code per tenant (optional, e.g., "NAT", "PAS", "SANT")
        /// </summary>
        public string? Code { get; private set; }

        /// <summary>
        /// Description
        /// </summary>
        public string? Description { get; private set; }

        // ======================================================================
        // RECURRENCE DATE AND VALIDITY RANGE
        // ======================================================================
        /// <summary>
        /// Day of the recurrence (1-31, optional, e.g., 25 for Christmas)
        /// </summary>
        public int? RecurrenceDay { get; private set; }

        /// <summary>
        /// Month of the recurrence (1-12, optional, e.g., 12 for Christmas)
        /// </summary>
        public int? RecurrenceMonth { get; private set; }

        /// <summary>
        /// Number of days before the recurrence date when campaigns are valid
        /// </summary>
        public int DaysBeforeRecurrence { get; private set; }

        /// <summary>
        /// Number of days after the recurrence date when campaigns are valid
        /// </summary>
        public int DaysAfterRecurrence { get; private set; }

        // ======================================================================
        // NOTES
        // ======================================================================
        /// <summary>
        /// Internal notes
        /// </summary>
        public string? Notes { get; private set; }

        // ======================================================================
        // STATUS AND ACTIVATION
        // ======================================================================
        /// <summary>
        /// Whether the recurrence is currently active
        /// </summary>
        public bool IsActive { get; private set; }

        /// <summary>
        /// Date when recurrence was deactivated
        /// </summary>
        public DateTime? DeactivatedDate { get; private set; }

        /// <summary>
        /// Reason for deactivation
        /// </summary>
        public string? DeactivationReason { get; private set; }

        // ======================================================================
        // CONSTRUCTOR
        // ======================================================================
        /// <summary>
        /// Private constructor for EF Core
        /// </summary>
        private Recurrence()
        {
        }

        /// <summary>
        /// Constructor for creating new recurrence
        /// </summary>
        private Recurrence(
            Guid id,
            Guid? tenantId,
            string name,
            string code,
            int? recurrenceDay,
            int? recurrenceMonth,
            int daysBeforeRecurrence,
            int daysAfterRecurrence)
            : base(id)
        {
            TenantId = tenantId;
            Name = Check.NotNullOrWhiteSpace(name, nameof(name), maxLength: 256);
            Code = string.IsNullOrWhiteSpace(code) ? null : Check.Length(code, nameof(code), maxLength: 64);
            RecurrenceDay = recurrenceDay;
            RecurrenceMonth = recurrenceMonth;
            DaysBeforeRecurrence = daysBeforeRecurrence;
            DaysAfterRecurrence = daysAfterRecurrence;
            IsActive = true; // New recurrences are active by default
            
            VerifyInvariants();
        }

        // ======================================================================
        // QUERY METHODS
        // ======================================================================
        /// <summary>
        /// Get the recurrence date for a specific year
        /// </summary>
        public DateTime? GetRecurrenceDateForYear(int year)
        {
            if (!RecurrenceDay.HasValue || !RecurrenceMonth.HasValue) return null;
            
            try
            {
                return new DateTime(year, RecurrenceMonth.Value, RecurrenceDay.Value);
            }
            catch
            {
                return null; // Invalid date (e.g., Feb 30)
            }
        }

        /// <summary>
        /// Get the start date of the validity period for a specific year
        /// </summary>
        public DateTime? GetValidityStartDateForYear(int year)
        {
            var recurrenceDate = GetRecurrenceDateForYear(year);
            if (recurrenceDate == null) return null;
            return recurrenceDate.Value.AddDays(-DaysBeforeRecurrence);
        }

        /// <summary>
        /// Get the end date of the validity period for a specific year
        /// </summary>
        public DateTime? GetValidityEndDateForYear(int year)
        {
            var recurrenceDate = GetRecurrenceDateForYear(year);
            if (recurrenceDate == null) return null;
            return recurrenceDate.Value.AddDays(DaysAfterRecurrence);
        }

        /// <summary>
        /// Check if a date falls within the recurrence validity period for its year
        /// </summary>
        public bool IsDateInValidityPeriod(DateTime date)
        {
            if (!IsActive || !RecurrenceDay.HasValue || !RecurrenceMonth.HasValue) return false;
            
            var startDate = GetValidityStartDateForYear(date.Year);
            var endDate = GetValidityEndDateForYear(date.Year);
            
            if (startDate == null || endDate == null) return false;
            
            return date >= startDate && date <= endDate;
        }

        /// <summary>
        /// Check if recurrence is currently in validity period
        /// </summary>
        public bool IsCurrentlyInValidityPeriod()
        {
            return IsDateInValidityPeriod(DateTime.UtcNow);
        }

        /// <summary>
        /// Get total validity period duration in days
        /// </summary>
        public int GetValidityDurationInDays()
        {
            return DaysBeforeRecurrence + DaysAfterRecurrence + 1;
        }

        /// <summary>
        /// Get full display name
        /// </summary>
        public string GetFullDisplayName()
        {
            return RecurrenceDay.HasValue && RecurrenceMonth.HasValue
                ? $"{Name} ({RecurrenceDay:00}/{RecurrenceMonth:00})" 
                : Name;
        }

        /// <summary>
        /// Deactivate the recurrence
        /// </summary>
        public void Deactivate(string reason)
        {
            if (!IsActive)
            {
                throw new BusinessException("DonaRog:RecurrenceAlreadyDeactivated");
            }

            IsActive = false;
            DeactivatedDate = DateTime.UtcNow;
            DeactivationReason = reason;
        }

        /// <summary>
        /// Reactivate the recurrence
        /// </summary>
        public void Reactivate()
        {
            if (IsActive)
            {
                throw new BusinessException("DonaRog:RecurrenceAlreadyActive");
            }

            IsActive = true;
            DeactivatedDate = null;
            DeactivationReason = null;
        }

        // ======================================================================
        // INVARIANTS
        // ======================================================================
        /// <summary>
        /// Verify business invariants
        /// </summary>
        internal void VerifyInvariants()
        {
            Check.NotNullOrWhiteSpace(Name, nameof(Name));
            // Code is now optional - no check needed

            if (DaysBeforeRecurrence < 0)
            {
                throw new BusinessException("DonaRog:RecurrenceInvalidDaysBeforeRecurrence")
                    .WithData("daysBeforeRecurrence", DaysBeforeRecurrence);
            }

            if (DaysAfterRecurrence < 0)
            {
                throw new BusinessException("DonaRog:RecurrenceInvalidDaysAfterRecurrence")
                    .WithData("daysAfterRecurrence", DaysAfterRecurrence);
            }
        }
    }
}
