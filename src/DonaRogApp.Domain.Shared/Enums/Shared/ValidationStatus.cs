using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonaRogApp.Enums.Shared
{
    /// <summary>
    /// Validation status for data (email, address, phone, etc.)
    /// </summary>
    public enum ValidationStatus
    {
        /// <summary>
        /// Not yet validated
        /// </summary>
        NotValidated = 0,

        /// <summary>
        /// Valid (verified)
        /// </summary>
        Valid = 1,

        /// <summary>
        /// Invalid (failed validation)
        /// </summary>
        Invalid = 2,

        /// <summary>
        /// Pending validation
        /// </summary>
        Pending = 3,

        /// <summary>
        /// Validation expired (needs re-validation)
        /// </summary>
        Expired = 4
    }
}
