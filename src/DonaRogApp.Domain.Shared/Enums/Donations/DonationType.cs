using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonaRogApp.Enums.Donations
{
    /// <summary>
    /// Type of donation payment method
    /// </summary>
    public enum DonationType
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
        /// Credit card
        /// </summary>
        CreditCard = 3,

        /// <summary>
        /// Direct debit (RID/SDD)
        /// </summary>
        DirectDebit = 4,

        /// <summary>
        /// Cash (Contanti)
        /// </summary>
        Cash = 5,

        /// <summary>
        /// Check (Assegno)
        /// </summary>
        Check = 6,

        /// <summary>
        /// PayPal
        /// </summary>
        PayPal = 7,

        /// <summary>
        /// Stripe
        /// </summary>
        Stripe = 8
    }
}
