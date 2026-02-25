using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonaRogApp.Enums.Communications
{
    /// <summary>
    /// Donor's preferred channel for thank you communications
    /// </summary>
    public enum PreferredThankYouChannel
    {
        /// <summary>
        /// Email communication (immediate, digital)
        /// </summary>
        Email = 1,

        /// <summary>
        /// Postal letter (physical mail, batch printed)
        /// </summary>
        Letter = 2,

        /// <summary>
        /// Both email and letter (email immediate + letter in batch)
        /// </summary>
        Both = 3,

        /// <summary>
        /// No thank you communications (donor preference)
        /// </summary>
        None = 99
    }
}
