using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonaRogApp.Enums.Shared
{
    /// <summary>
    /// Type of postal address
    /// </summary>
    public enum AddressType
    {
        /// <summary>
        /// Home address (residential)
        /// </summary>
        Home = 1,

        /// <summary>
        /// Work/office address
        /// </summary>
        Work = 2,

        /// <summary>
        /// Billing address
        /// </summary>
        Billing = 3,

        /// <summary>
        /// Shipping address
        /// </summary>
        Shipping = 4,

        /// <summary>
        /// Temporary address (vacation, temporary relocation)
        /// </summary>
        Temporary = 5,

        /// <summary>
        /// Other address type
        /// </summary>
        Other = 6
    }
}
