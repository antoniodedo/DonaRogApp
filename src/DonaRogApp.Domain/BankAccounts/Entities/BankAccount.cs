using DonaRogApp.ValueObjects;
using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace DonaRogApp.Domain.BankAccounts.Entities
{
    /// <summary>
    /// BankAccount Aggregate Root
    /// 
    /// RESPONSIBILITY:
    /// - Store organization's bank account information
    /// - Manage account activation/deactivation
    /// - Track default account for donations
    /// - Associate with received donations
    /// 
    /// Business logic is split across partial classes:
    /// - BankAccount.Factory.cs: Creation factory methods
    /// - BankAccount.Updates.cs: Update methods and status transitions
    /// </summary>
    public partial class BankAccount : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {
        // ======================================================================
        // MULTI-TENANCY
        // ======================================================================
        /// <summary>
        /// Tenant ID
        /// </summary>
        public Guid? TenantId { get; private set; }

        // ======================================================================
        // IDENTIFICATION
        // ======================================================================
        /// <summary>
        /// Account name (user-friendly label)
        /// Example: "Conto Principale UniCredit", "Conto Donazioni PayPal"
        /// </summary>
        public string AccountName { get; private set; }

        /// <summary>
        /// IBAN (International Bank Account Number)
        /// </summary>
        public IBAN Iban { get; private set; }

        // ======================================================================
        // BANK DETAILS
        // ======================================================================
        /// <summary>
        /// Bank name (optional)
        /// Example: "UniCredit", "Intesa Sanpaolo"
        /// </summary>
        public string? BankName { get; private set; }

        /// <summary>
        /// SWIFT/BIC code (optional)
        /// Example: "UNCRITMM"
        /// </summary>
        public string? Swift { get; private set; }

        // ======================================================================
        // STATUS
        // ======================================================================
        /// <summary>
        /// Whether the account is active
        /// Only active accounts can be selected for new donations
        /// </summary>
        public bool IsActive { get; private set; }

        /// <summary>
        /// Whether this is the default account for donations
        /// </summary>
        public bool IsDefault { get; private set; }

        // ======================================================================
        // ADDITIONAL INFO
        // ======================================================================
        /// <summary>
        /// Notes about this account (optional)
        /// </summary>
        public string? Notes { get; private set; }

        // ======================================================================
        // CONSTRUCTOR
        // ======================================================================
        /// <summary>
        /// Private constructor for EF Core
        /// </summary>
        private BankAccount()
        {
            AccountName = string.Empty;
            Iban = new IBAN("IT60X0542811101000000123456"); // Placeholder for EF
        }

        /// <summary>
        /// Constructor for creating new bank account
        /// </summary>
        internal BankAccount(
            Guid id,
            Guid? tenantId,
            string accountName,
            IBAN iban,
            string? bankName = null,
            string? swift = null,
            string? notes = null)
            : base(id)
        {
            TenantId = tenantId;
            AccountName = Check.NotNullOrWhiteSpace(accountName, nameof(accountName), maxLength: 200);
            Iban = Check.NotNull(iban, nameof(iban));
            BankName = bankName;
            Swift = swift;
            Notes = notes;
            
            IsActive = true; // Active by default
            IsDefault = false;
            
            VerifyInvariants();
        }

        // ======================================================================
        // QUERY METHODS
        // ======================================================================
        /// <summary>
        /// Check if account is active
        /// </summary>
        public bool CanReceiveDonations()
        {
            return IsActive;
        }

        /// <summary>
        /// Get formatted IBAN for display
        /// </summary>
        public string GetFormattedIban()
        {
            return Iban.ToFormattedString();
        }

        /// <summary>
        /// Get masked IBAN for display (security)
        /// </summary>
        public string GetMaskedIban()
        {
            return Iban.ToMaskedString();
        }

        // ======================================================================
        // INVARIANTS
        // ======================================================================
        /// <summary>
        /// Verify business invariants
        /// </summary>
        internal void VerifyInvariants()
        {
            Check.NotNullOrWhiteSpace(AccountName, nameof(AccountName));
            Check.NotNull(Iban, nameof(Iban));

            if (!string.IsNullOrEmpty(Swift) && Swift.Length < 8)
            {
                throw new BusinessException("DonaRog:BankAccountInvalidSwift")
                    .WithData("swift", Swift)
                    .WithData("minLength", 8);
            }
        }
    }
}
