using DonaRogApp.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace DonaRogApp.Donors.Entities
{
    public class DonorTitle : AuditedEntity<Guid>, IMultiTenant
    {
        public Guid? TenantId { get; set; }

        public string Title { get; set; }

        public Gender Gender { get; set; }

        public bool IsGroup { get; set; }

        public bool IsActive { get; set; }
    }
}
