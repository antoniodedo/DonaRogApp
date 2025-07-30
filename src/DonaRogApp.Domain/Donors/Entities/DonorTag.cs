using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace DonaRogApp.Donors.Entities
{
    public class DonorTag : AuditedEntity<Guid>, IMultiTenant
    {
        public Guid? TenantId { get; set; }

        public Guid DonorId { get; set; }
        public string Tag { get; set; } = null!;
    }
}
