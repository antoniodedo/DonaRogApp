using DonaRogApp.Enums.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonaRogApp.Donors.Dtos
{
    public class CreateDonorAddressDto
    {
        public string Street { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string? Country { get; set; } = "Italy";
        public AddressType Type { get; set; } = AddressType.Home;
        public string? Province { get; set; }
        public string? Region { get; set; }
        public string? Notes { get; set; }
    }
}
