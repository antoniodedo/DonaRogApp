using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace DonaRogApp.Donors.Entities
{
    public class Note : AuditedEntity<Guid>, IMultiTenant
    {
        public Guid? TenantId { get; set; }

        public Guid DonorId { get; set; }

        public string Content { get; set; } = null!;

        public bool IsImportant { get; set; } = false;
    }
}
