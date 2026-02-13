using Volo.Abp.Application.Dtos;

namespace DonaRogApp.Application.Contracts.BankAccounts.Dto
{
    public class GetBankAccountsInput : PagedAndSortedResultRequestDto
    {
        public bool? IsActive { get; set; }
        public bool? IsDefault { get; set; }
        public string? Search { get; set; }
    }
}
