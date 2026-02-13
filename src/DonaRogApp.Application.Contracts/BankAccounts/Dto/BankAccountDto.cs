using System;
using Volo.Abp.Application.Dtos;

namespace DonaRogApp.Application.Contracts.BankAccounts.Dto
{
    public class BankAccountDto : FullAuditedEntityDto<Guid>
    {
        public string AccountName { get; set; } = string.Empty;
        public string Iban { get; set; } = string.Empty;
        public string? BankName { get; set; }
        public string? Swift { get; set; }
        public bool IsActive { get; set; }
        public bool IsDefault { get; set; }
        public string? Notes { get; set; }

        // Computed properties
        public string FormattedIban { get; set; } = string.Empty;
        public string MaskedIban { get; set; } = string.Empty;
    }
}
