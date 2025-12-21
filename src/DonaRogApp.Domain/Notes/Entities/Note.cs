using DonaRogApp.Donors.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace DonaRogApp.Notes.Entities
{
    public class Note : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {
        public Guid? TenantId { get; set; }
        public string Content { get; set; } = null!;
        public Guid? DonorId { get; set; }
        public bool IsImportant { get; set; } = false;
    }
}
