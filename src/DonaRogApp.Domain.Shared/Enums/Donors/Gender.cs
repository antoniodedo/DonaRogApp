using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonaRogApp.Enums.Donors
{
    /// <summary>
    /// Gender (for Individual donors only)
    /// </summary>
    public enum Gender
    {
        /// <summary>
        /// Unspecified
        /// </summary>
        Unspecified = 0,

        /// <summary>
        /// Male
        /// </summary>
        Male = 1,

        /// <summary>
        /// Female
        /// </summary>
        Female = 2,

        /// <summary>
        /// Other / Prefer not to say
        /// </summary>
        Other = 3
    }
}
