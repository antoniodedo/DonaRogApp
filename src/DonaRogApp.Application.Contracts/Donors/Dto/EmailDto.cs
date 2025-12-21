using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace DonaRogApp.Donors.Dto
{
    public  class EmailDto : AuditedEntityDto<Guid>
    {
        public Guid DonorId { get; set; }
        public string Address { get; set; } = null!;
        public bool IsPrimary { get; set; } = false;    
    }
}
