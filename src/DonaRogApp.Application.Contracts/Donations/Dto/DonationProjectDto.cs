using System;

namespace DonaRogApp.Application.Contracts.Donations.Dto
{
    public class DonationProjectDto
    {
        public Guid ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public decimal AllocatedAmount { get; set; }
    }
}
