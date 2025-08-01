using DonaRogApp.Donors;
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
    public class CreateUpdateDonorDto : EntityDto<Guid>
    {
     
        [MaxLength(100)]
        public string? FirstName { get; set; }

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; }

        public string? RawAddress { get; set; }
        public string? RawCap { get; set; }
        public string? RawComune { get; set; }

        //[MaxLength(100)]
        //public string SecondLastName { get; set; }

        //[MaxLength(50)]
        //public string Title { get; set; } 

        // Contatti
        //public List<EmailDto> Emails { get; set; } = new List<EmailDto>();

        //public List<PhoneNumberDto> PhoneNumbers { get; set; } = new List<PhoneNumberDto>();

        //public List<TagDto> TagIds { get; set; } = new List<Guid>();

        //public List<NoteDto> Notes { get; set; } = new List<NoteDto>();
    }
}
