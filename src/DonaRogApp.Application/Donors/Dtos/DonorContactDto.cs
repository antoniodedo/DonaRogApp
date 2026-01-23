using DonaRogApp.Enums.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonaRogApp.Donors.Dtos
{
    /// <summary>
    /// DTO per leggere contatto donatore
    /// </summary>
    public class DonorContactDto
    {
        public Guid Id { get; set; }
        public string PhoneNumber { get; set; }
        public ContactType Type { get; set; }
        public bool IsDefault { get; set; }
        public bool IsVerified { get; set; }
        public DateTime? VerifiedDate { get; set; }
        public DateTime DateAdded { get; set; }
        public string? Notes { get; set; }
    }
}
