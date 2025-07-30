using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace DonaRogApp.Donors.Entities
{
    public class DonorRelationship : AuditedEntity<Guid>, IMultiTenant
    {
        public Guid? TenantId { get; set; }

        public Guid DonorId { get; set; }
        public Guid RelatedDonorId { get; set; }
        public string RelationshipType { get; set; } = null!;
    }
}
