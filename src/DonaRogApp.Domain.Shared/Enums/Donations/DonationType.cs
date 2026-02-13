using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonaRogApp.Enums.Donations
{
    /// <summary>
    /// Donation channel/payment method
    /// </summary>
    public enum DonationChannel
    {
        /// <summary>
        /// Bank transfer (Bonifico Bancario)
        /// </summary>
        BankTransfer = 1,

        /// <summary>
        /// Postal order (Bollettino Postale)
        /// </summary>
        PostalOrder = 2,

        /// <summary>
        /// Postal order telematic (Bollettino Telematico Postale)
        /// </summary>
        PostalOrderTelematic = 3,

        /// <summary>
        /// Credit card
        /// </summary>
        CreditCard = 4,

        /// <summary>
        /// Direct debit (RID/SDD)
        /// </summary>
        DirectDebit = 5,

        /// <summary>
        /// Cash (Contanti)
        /// </summary>
        Cash = 6,

        /// <summary>
        /// Check (Assegno)
        /// </summary>
        Check = 7,

        /// <summary>
        /// PayPal
        /// </summary>
        PayPal = 8,

        /// <summary>
        /// Stripe
        /// </summary>
        Stripe = 9,

        /// <summary>
        /// Bequest (Lasciti)
        /// </summary>
        Bequest = 10,

        /// <summary>
        /// Unknown (from migrations/legacy data)
        /// </summary>
        Unknown = 98,

        /// <summary>
        /// Other
        /// </summary>
        Other = 99
    }

    /// <summary>
    /// [DEPRECATED] Use DonationChannel instead
    /// </summary>
    [Obsolete("Use DonationChannel instead")]
    public enum DonationType
    {
        BankTransfer = 1,
        PostalOrder = 2,
        CreditCard = 3,
        DirectDebit = 4,
        Cash = 5,
        Check = 6,
        PayPal = 7,
        Stripe = 8
    }
}
