using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace DonaRogApp.LetterTemplates
{
    public class LetterTemplate : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {
        public Guid? TenantId { get; set; }
        public string Name { get; set; } = null!;
        public string Content { get; set; } = null!;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
    }
}
