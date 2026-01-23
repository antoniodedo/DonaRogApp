using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonaRogApp.Enums.Donors
{
    /// <summary>
    /// Donor category based on total donated amount (lifetime value)
    /// Automatically calculated based on TotalDonated
    /// </summary>
    public enum DonorCategory
    {
        /// <summary>
        /// Not yet classified (new donor or prospect)
        /// </summary>
        Unclassified = 0,

        /// <summary>
        /// Standard donor
        /// Total donated: €0 - €999
        /// </summary>
        Standard = 1,

        /// <summary>
        /// Bronze donor
        /// Total donated: €1,000 - €4,999
        /// </summary>
        Bronze = 2,

        /// <summary>
        /// Silver donor
        /// Total donated: €5,000 - €9,999
        /// </summary>
        Silver = 3,

        /// <summary>
        /// Gold donor
        /// Total donated: €10,000 - €49,999
        /// </summary>
        Gold = 4,

        /// <summary>
        /// Major donor
        /// Total donated: €50,000+
        /// </summary>
        Major = 5
    }
}
