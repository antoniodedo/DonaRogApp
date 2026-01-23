using DonaRogApp.Enums.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonaRogApp.Donors.Dtos
{
    public class DonorEmailDto
    {
        public Guid Id { get; set; }
        public string EmailAddress { get; set; }
        public EmailType Type { get; set; }
        public bool IsDefault { get; set; }
        public bool IsVerified { get; set; }
        public DateTime? VerifiedDate { get; set; }
        public int BounceCount { get; set; }
        public DateTime? LastBounceDate { get; set; }
        public string? LastBounceReason { get; set; }
        public bool IsInvalid { get; set; }
        public DateTime DateAdded { get; set; }
        public string? Notes { get; set; }
    }
}
