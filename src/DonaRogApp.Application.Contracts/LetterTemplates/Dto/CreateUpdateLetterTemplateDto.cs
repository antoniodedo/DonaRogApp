using DonaRogApp.Donors;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace DonaRogApp.LetterTemplates.Dto
{
    public class CreateUpdateLetterTemplateDto : EntityDto<Guid>
    {
        [MaxLength(128)]
        public string? Name { get; set; }
        public string Content { get; set; }
        public string? Description { get; set; }

        public bool IsActive { get; set; }
    }
}
