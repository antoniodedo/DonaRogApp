using System;
using Volo.Abp.Application.Dtos;

namespace DonaRogApp.Application.Contracts.BankAccounts.Dto
{
    /// <summary>
    /// Lightweight DTO for bank account lists
    /// </summary>
    public class BankAccountListDto : EntityDto<Guid>
    {
        public string AccountName { get; set; } = string.Empty;
        public string MaskedIban { get; set; } = string.Empty;
        public string? BankName { get; set; }
        public bool IsActive { get; set; }
        public bool IsDefault { get; set; }
    }
}
