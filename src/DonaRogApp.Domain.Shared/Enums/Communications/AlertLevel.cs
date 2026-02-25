using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonaRogApp.Enums.Communications
{
    /// <summary>
    /// Alert severity level for duplicate letter warnings
    /// </summary>
    public enum AlertLevel
    {
        /// <summary>
        /// No alert - no recent letters found
        /// </summary>
        None = 0,

        /// <summary>
        /// Info - letter sent more than 15 days ago (informational only)
        /// </summary>
        Info = 1,

        /// <summary>
        /// Warning - letter sent 7-15 days ago (suggest waiting)
        /// </summary>
        Warning = 2,

        /// <summary>
        /// Error - letter sent less than 7 days ago (strong warning, may want to skip)
        /// </summary>
        Error = 3
    }
}
