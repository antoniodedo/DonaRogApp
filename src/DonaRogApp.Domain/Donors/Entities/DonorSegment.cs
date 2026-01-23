// --------------------------------------------------------------
// Domain/Donors/Entities/DonorSegment.cs
// --------------------------------------------------------------

using DonaRogApp.Domain.Shared.Entities;
using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Timing;

namespace DonaRogApp.Domain.Donors.Entities
{
    // ======================================================================
    // DONOTSEGMENT.CS - Many-to-Many: Donor ↔ Segment
    // ======================================================================
    /// <summary>
    /// Many-to-Many Mapping: Donor ←→ Segment
    /// Represents donor membership in a marketing segment.
    /// Un donatore può appartenere a più segmenti contemporaneamente.
    /// </summary>
    public class DonorSegment : Entity, IMultiTenant
    {
        // --------------------------------------------------------------
        // MULTI-TENANCY
        // --------------------------------------------------------------
        /// <summary>
        /// Tenant ID
        /// </summary>
        public Guid? TenantId { get; private set; }

        // --------------------------------------------------------------
        // RELATIONSHIP PROPERTIES
        // --------------------------------------------------------------
        /// <summary>
        /// Donor ID (Foreign Key)
        /// </summary>
        public Guid DonorId { get; private set; }

        /// <summary>
        /// Donor (navigation property)
        /// </summary>
        public virtual Donor? Donor { get; private set; }

        /// <summary>
        /// Segment ID (Foreign Key)
        /// </summary>
        public Guid SegmentId { get; private set; }

        /// <summary>
        /// Segment (navigation property)
        /// Segment entity is in Domain/Shared/Entities/
        /// </summary>
        public virtual Segment? Segment { get; private set; }

        // --------------------------------------------------------------
        // TRACKING
        // --------------------------------------------------------------
        /// <summary>
        /// Data di assegnazione al segmento
        /// </summary>
        public DateTime AssignedAt { get; private set; }

        /// <summary>
        /// Data di rimozione dal segmento (NULL = ancora nel segmento)
        /// </summary>
        public DateTime? RemovedAt { get; private set; }

        /// <summary>
        /// Note sulla assegnazione (es: "Assegnato automaticamente da RFM")
        /// </summary>
        public string? AssignmentNotes { get; private set; }

        /// <summary>
        /// Indicatore se l'assegnazione è automatica (da regole) o manuale (da utente)
        /// </summary>
        public bool IsAutomatic { get; private set; }

        /// <summary>
        /// Motivo dell'assegnazione automatica (es: "RfmScore", "TotalDonated")
        /// </summary>
        public string? AutomaticReason { get; private set; }

        /// <summary>
        /// User who added donor to segment
        /// </summary>
        public Guid? AddedByUserId { get; private set; }

        // --------------------------------------------------------------
        // CONSTRUCTOR
        // --------------------------------------------------------------
        /// <summary>
        /// Protected constructor for EF Core
        /// Use factory method Create() to instantiate
        /// </summary>
        protected DonorSegment()
        {
        }

        // --------------------------------------------------------------
        // FACTORY METHODS
        // --------------------------------------------------------------
        /// <summary>
        /// Creates new DonorSegment mapping (manuale)
        /// </summary>
        internal static DonorSegment CreateManual(
            Guid donorId,
            Guid segmentId,
            Guid? tenantId,
            Guid? addedByUserId = null,
            string? notes = null)
        {
            if (donorId == Guid.Empty) throw new ArgumentException("Value cannot be empty", nameof(donorId));
            if (segmentId == Guid.Empty) throw new ArgumentException("Value cannot be empty", nameof(segmentId));

            return new DonorSegment
            {
                DonorId = donorId,
                SegmentId = segmentId,
                TenantId = tenantId,
                AssignedAt = DateTime.UtcNow,
                RemovedAt = null,
                AssignmentNotes = notes,
                IsAutomatic = false,
                AutomaticReason = null,
                AddedByUserId = addedByUserId
            };
        }

        /// <summary>
        /// Creates new DonorSegment mapping (automatico)
        /// </summary>
        internal static DonorSegment CreateAutomatic(
            Guid donorId,
            Guid segmentId,
            Guid? tenantId,
            string automaticReason,
            string? notes = null)
        {
            if (donorId == Guid.Empty) throw new ArgumentException("Value cannot be empty", nameof(donorId));
            if (segmentId == Guid.Empty) throw new ArgumentException("Value cannot be empty", nameof(segmentId));
            Check.NotNullOrWhiteSpace(automaticReason, nameof(automaticReason));

            return new DonorSegment
            {
                DonorId = donorId,
                SegmentId = segmentId,
                TenantId = tenantId,
                AssignedAt = DateTime.UtcNow,
                RemovedAt = null,
                AssignmentNotes = notes,
                IsAutomatic = true,
                AutomaticReason = automaticReason,
                AddedByUserId = null
            };
        }

        // --------------------------------------------------------------
        // METHODS
        // --------------------------------------------------------------
        /// <summary>
        /// Rimuove il donatore dal segmento (soft delete)
        /// </summary>
        public void Remove()
        {
            if (RemovedAt.HasValue)
                return;

            RemovedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Controlla se il donatore è attualmente nel segmento
        /// </summary>
        public bool IsActive => !RemovedAt.HasValue;

        /// <summary>
        /// Calcola i giorni di permanenza nel segmento
        /// </summary>
        public int GetDaysInSegment()
        {
            var endDate = RemovedAt ?? DateTime.UtcNow;
            return (int)(endDate - AssignedAt).TotalDays;
        }

        // --------------------------------------------------------------
        // ENTITY KEY
        // --------------------------------------------------------------
        public override object[] GetKeys()
        {
            return new object[] { DonorId, SegmentId };
        }
    }
}