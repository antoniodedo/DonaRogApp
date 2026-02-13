using DonaRogApp.Enums.Donors;
using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace DonaRogApp.Domain.Donors.Entities
{
    /// <summary>
    /// Storico dei cambi di stato del donatore
    /// </summary>
    public class DonorStatusHistory : CreationAuditedEntity<Guid>, IMultiTenant
    {
        public Guid? TenantId { get; private set; }
        
        public Guid DonorId { get; private set; }
        
        public virtual Donor Donor { get; private set; } = null!;
        
        /// <summary>
        /// Stato precedente
        /// </summary>
        public DonorStatus OldStatus { get; private set; }
        
        /// <summary>
        /// Nuovo stato
        /// </summary>
        public DonorStatus NewStatus { get; private set; }
        
        /// <summary>
        /// Nota/motivazione del cambio stato
        /// </summary>
        public string? Note { get; private set; }
        
        /// <summary>
        /// Data effettiva del cambio (può essere diversa da CreationTime)
        /// </summary>
        public DateTime ChangedAt { get; private set; }

        protected DonorStatusHistory() { }

        public static DonorStatusHistory Create(
            Guid donorId,
            DonorStatus oldStatus,
            DonorStatus newStatus,
            string? note,
            Guid? tenantId)
        {
            return new DonorStatusHistory
            {
                Id = Guid.NewGuid(),
                DonorId = donorId,
                OldStatus = oldStatus,
                NewStatus = newStatus,
                Note = note?.Trim(),
                ChangedAt = DateTime.UtcNow,
                TenantId = tenantId
            };
        }
    }
}
