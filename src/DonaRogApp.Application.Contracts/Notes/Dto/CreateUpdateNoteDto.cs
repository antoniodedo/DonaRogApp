using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace DonaRogApp.Notes.Dto
{
    public class CreateUpdateNoteDto : EntityDto<Guid>
    {
        public string Content { get; set; } = null!;
        public Guid? DonorId { get; set; }
        public bool IsImportant { get; set; } = false;
    }
}
