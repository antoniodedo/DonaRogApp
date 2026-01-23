using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonaRogApp.Enums.Donors
{
    /// <summary>
    /// Donor lifecycle status
    /// </summary>
    public enum DonorStatus
    {
        /// <summary>
        /// New donor (prospect, never donated)
        /// </summary>
        New = 1,

        /// <summary>
        /// Active donor (donated recently)
        /// </summary>
        Active = 2,

        /// <summary>
        /// Lapsed donor (not donated for >18 months)
        /// </summary>
        Lapsed = 3,

        /// <summary>
        /// Inactive (explicitly marked as inactive)
        /// </summary>
        Inactive = 4,

        /// <summary>
        /// Suspended
        /// </summary>
        Suspended = 5,

        /// <summary>
        /// Disabled
        /// </summary>
        Disabled = 6,

        /// <summary>
        /// Deceased (Individual only)
        /// </summary>
        Deceased = 7,

        /// <summary>
        /// Do Not Contact (requested to stop communications)
        /// </summary>
        DoNotContact = 8,

        /// <summary>
        /// Duplicate (merged into another donor record)
        /// </summary>
        Duplicate = 9
    }
}
