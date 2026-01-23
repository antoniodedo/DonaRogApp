using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonaRogApp.Enums.Communications
{
    /// <summary>
    /// Preferred frequency for receiving communications
    /// </summary>
    public enum CommunicationFrequency
    {
        /// <summary>
        /// No preference specified
        /// </summary>
        Unspecified = 0,

        /// <summary>
        /// Daily communications
        /// </summary>
        Daily = 1,

        /// <summary>
        /// Weekly communications
        /// </summary>
        Weekly = 2,

        /// <summary>
        /// Bi-weekly (every 2 weeks)
        /// </summary>
        BiWeekly = 3,

        /// <summary>
        /// Monthly communications
        /// </summary>
        Monthly = 4,

        /// <summary>
        /// Quarterly (every 3 months)
        /// </summary>
        Quarterly = 5,

        /// <summary>
        /// Bi-annually (twice a year)
        /// </summary>
        BiAnnually = 6,

        /// <summary>
        /// Annually (once a year)
        /// </summary>
        Annually = 7,

        /// <summary>
        /// Only for important updates
        /// </summary>
        ImportantOnly = 8,

        /// <summary>
        /// Never (opted out)
        /// </summary>
        Never = 99
    }
}
