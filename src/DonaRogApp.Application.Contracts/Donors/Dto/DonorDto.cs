using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace DonaRogApp.Donors.Dto
{
    public class DonorDto : EntityDto<Guid>
    {
        public string? FirstName { get; set; }
        public string LastName { get; set; } = null!;
        public string? RawAddress { get; set; }
        public string? RawCap { get; set; }
        public string? RawComune { get; set; }
        //public string? SecondLastName { get; set; }
        //public DonorStatus Status { get; set; }
    }
}
