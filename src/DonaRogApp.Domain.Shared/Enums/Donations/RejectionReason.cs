using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonaRogApp.Enums.Donations
{
    /// <summary>
    /// Reason for rejecting a donation
    /// </summary>
    public enum RejectionReason
    {
        /// <summary>
        /// Duplicate donation
        /// </summary>
        Duplicate = 1,

        /// <summary>
        /// Invalid or incorrect data
        /// </summary>
        InvalidData = 2,

        /// <summary>
        /// Fraudulent or suspicious donation
        /// </summary>
        Fraudulent = 3,

        /// <summary>
        /// Cancelled or refunded by donor
        /// </summary>
        Cancelled = 4,

        /// <summary>
        /// Other reason
        /// </summary>
        Other = 99
    }
}
