using System;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace DonaRogApp.Domain.Communications.Entities
{
    /// <summary>
    /// Tracks template usage per donor for LRU rotation
    /// </summary>
    public class DonorTemplateUsage : Entity, IMultiTenant
    {
        /// <summary>
        /// Tenant ID
        /// </summary>
        public Guid? TenantId { get; set; }

        /// <summary>
        /// Donor ID
        /// </summary>
        public Guid DonorId { get; set; }

        /// <summary>
        /// Template ID
        /// </summary>
        public Guid TemplateId { get; set; }

        /// <summary>
        /// Last time this template was used for this donor
        /// </summary>
        public DateTime LastUsedDate { get; set; }

        /// <summary>
        /// Total number of times this template was used for this donor
        /// </summary>
        public int UsageCount { get; set; }

        // ======================================================================
        // NAVIGATION PROPERTIES
        // ======================================================================
        public virtual Domain.Donors.Entities.Donor Donor { get; set; } = null!;
        public virtual DonaRogApp.LetterTemplates.LetterTemplate Template { get; set; } = null!;

        // ======================================================================
        // CONSTRUCTOR
        // ======================================================================
        private DonorTemplateUsage()
        {
        }

        public DonorTemplateUsage(Guid? tenantId, Guid donorId, Guid templateId, DateTime lastUsedDate)
        {
            TenantId = tenantId;
            DonorId = donorId;
            TemplateId = templateId;
            LastUsedDate = lastUsedDate;
            UsageCount = 1;
        }

        /// <summary>
        /// Record a new usage of this template
        /// </summary>
        public void RecordUsage(DateTime usedDate)
        {
            LastUsedDate = usedDate;
            UsageCount++;
        }

        public override object[] GetKeys()
        {
            return new object[] { DonorId, TemplateId };
        }
    }
}
