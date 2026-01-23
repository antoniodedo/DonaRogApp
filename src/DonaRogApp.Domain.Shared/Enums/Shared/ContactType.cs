using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonaRogApp.Enums.Shared
{
    /// <summary>
    /// Type of contact (phone number)
    /// </summary>
    public enum ContactType
    {
        /// <summary>
        /// Mobile phone
        /// </summary>
        Mobile = 1,

        /// <summary>
        /// Home landline
        /// </summary>
        HomeLandline = 2,

        /// <summary>
        /// Work landline
        /// </summary>
        WorkLandline = 3,

        /// <summary>
        /// Fax
        /// </summary>
        Fax = 4,

        /// <summary>
        /// WhatsApp
        /// </summary>
        WhatsApp = 5,

        /// <summary>
        /// Other contact type
        /// </summary>
        Other = 6
    }
}
