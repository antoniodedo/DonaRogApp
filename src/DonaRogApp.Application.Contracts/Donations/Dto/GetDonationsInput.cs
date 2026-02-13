using DonaRogApp.Enums.Donations;
using System;
using Volo.Abp.Application.Dtos;

namespace DonaRogApp.Application.Contracts.Donations.Dto
{
    public class GetDonationsInput : PagedAndSortedResultRequestDto
    {
        public DonationStatus? Status { get; set; }
        public DonationChannel? Channel { get; set; }
        public Guid? DonorId { get; set; }
        public Guid? CampaignId { get; set; }
        public Guid? ProjectId { get; set; }
        public Guid? BankAccountId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public decimal? MinAmount { get; set; }
        public decimal? MaxAmount { get; set; }
        public string? Search { get; set; }
    }
}
