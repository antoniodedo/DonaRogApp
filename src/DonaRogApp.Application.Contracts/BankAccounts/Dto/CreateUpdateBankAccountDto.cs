using System.ComponentModel.DataAnnotations;

namespace DonaRogApp.Application.Contracts.BankAccounts.Dto
{
    public class CreateUpdateBankAccountDto
    {
        [Required]
        [StringLength(200)]
        public string AccountName { get; set; } = string.Empty;

        [Required]
        [StringLength(34)]
        public string Iban { get; set; } = string.Empty;

        [StringLength(100)]
        public string? BankName { get; set; }

        [StringLength(11)]
        public string? Swift { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }
    }
}
