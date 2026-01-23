using DonaRogApp.Donors;
using DonaRogApp.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace DonaRogApp.Donors.Dto
{
    public class CreateUpdateDonorTitleDto : EntityDto<Guid>
    {
        [Required]
        [MaxLength(128)]
        public string Title { get; set; } = string.Empty;

        //[Required]
        //public Gender Gender { get; set; } = Gender.Unspecified;

        [Required]
        public bool IsGroup { get; set; }

        [Required]
        public bool IsActive { get; set; }


    }
}
