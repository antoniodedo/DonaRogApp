using DonaRogApp.ValueObjects;
using System;
using Volo.Abp;

namespace DonaRogApp.Domain.BankAccounts.Entities
{
    public partial class BankAccount
    {
        /// <summary>
        /// Factory method to create a new bank account
        /// </summary>
        public static BankAccount Create(
            Guid id,
            Guid? tenantId,
            string accountName,
            string iban,
            string? bankName = null,
            string? swift = null,
            string? notes = null)
        {
            Check.NotNull(id, nameof(id));
            Check.NotNullOrWhiteSpace(accountName, nameof(accountName));
            Check.NotNullOrWhiteSpace(iban, nameof(iban));

            var ibanVO = new IBAN(iban);

            return new BankAccount(
                id,
                tenantId,
                accountName,
                ibanVO,
                bankName,
                swift,
                notes
            );
        }
    }
}
