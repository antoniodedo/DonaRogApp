using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonaRogApp.Enums.Donations
{
    /// <summary>
    /// Donation verification status
    /// </summary>
    public enum DonationStatus
    {
        /// <summary>
        /// Pending verification (arrived from external flow)
        /// </summary>
        Pending = 1,

        /// <summary>
        /// Verified and confirmed
        /// </summary>
        Verified = 2,

        /// <summary>
        /// Rejected (duplicate, error, fraudulent, cancelled)
        /// </summary>
        Rejected = 3,

        /// <summary>
        /// Suspended - needs further review/attention later
        /// </summary>
        Suspended = 4
    }
}
