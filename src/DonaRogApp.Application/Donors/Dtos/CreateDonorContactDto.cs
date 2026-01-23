using DonaRogApp.Enums.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonaRogApp.Donors.Dtos
{ 
  /// <summary>
  /// DTO per creare contatto donatore
  /// </summary>
    public class CreateDonorContactDto
    {
        public string PhoneNumber { get; set; }
        public ContactType Type { get; set; } = ContactType.Mobile;
    }
}
