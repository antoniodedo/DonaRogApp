using DonaRogApp.Enums.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonaRogApp.Donors.Dtos
{
    public class CreateDonorEmailDto
    {
        public string EmailAddress { get; set; }
        public EmailType Type { get; set; } = EmailType.Personal;
    }
}
