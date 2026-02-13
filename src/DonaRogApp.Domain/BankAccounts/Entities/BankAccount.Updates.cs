using DonaRogApp.ValueObjects;
using System;
using Volo.Abp;

namespace DonaRogApp.Domain.BankAccounts.Entities
{
    public partial class BankAccount
    {
        /// <summary>
        /// Update bank account information
        /// </summary>
        public void Update(
            string accountName,
            string iban,
            string? bankName = null,
            string? swift = null,
            string? notes = null)
        {
            Check.NotNullOrWhiteSpace(accountName, nameof(accountName));
            Check.NotNullOrWhiteSpace(iban, nameof(iban));

            AccountName = Check.NotNullOrWhiteSpace(accountName, nameof(accountName), maxLength: 200);
            Iban = new IBAN(iban);
            BankName = bankName;
            Swift = swift;
            Notes = notes;

            VerifyInvariants();
        }

        /// <summary>
        /// Activate the bank account
        /// </summary>
        public void Activate()
        {
            IsActive = true;
        }

        /// <summary>
        /// Deactivate the bank account
        /// Cannot deactivate if it's the default account
        /// </summary>
        public void Deactivate()
        {
            if (IsDefault)
            {
                throw new BusinessException("DonaRog:CannotDeactivateDefaultBankAccount")
                    .WithData("accountId", Id)
                    .WithData("accountName", AccountName);
            }

            IsActive = false;
        }

        /// <summary>
        /// Set as default bank account
        /// Automatically activates the account if inactive
        /// </summary>
        public void SetAsDefault()
        {
            IsDefault = true;
            
            // Auto-activate if setting as default
            if (!IsActive)
            {
                IsActive = true;
            }
        }

        /// <summary>
        /// Remove default status
        /// </summary>
        public void RemoveDefaultStatus()
        {
            IsDefault = false;
        }

        /// <summary>
        /// Update notes
        /// </summary>
        public void UpdateNotes(string? notes)
        {
            Notes = notes;
        }
    }
}
