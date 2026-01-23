using DonaRogApp.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace DonaRogApp.Donors.Dto
{
    public class DonorTitleDto : EntityDto<Guid>
    {
        public string Title { get; set; }

        //public Gender Gender { get; set; }

        public bool IsGroup { get; set; }

        public bool IsActive { get; set; }
    }
}
