using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonaRogApp.Enums.Communications
{
    /// <summary>
    /// Communication workflow status
    /// </summary>
    public enum CommunicationStatus
    {
        /// <summary>
        /// Draft - communication created but not sent
        /// </summary>
        Draft = 1,

        /// <summary>
        /// Pending Print - letter queued for batch printing
        /// </summary>
        PendingPrint = 2,

        /// <summary>
        /// In Batch - letter included in a print batch
        /// </summary>
        InBatch = 3,

        /// <summary>
        /// Printed - letter physically printed
        /// </summary>
        Printed = 4,

        /// <summary>
        /// Sent - email/SMS sent or letter mailed
        /// </summary>
        Sent = 5,

        /// <summary>
        /// Delivered - confirmed delivery (for emails with tracking)
        /// </summary>
        Delivered = 6,

        /// <summary>
        /// Failed - delivery failed
        /// </summary>
        Failed = 7,

        /// <summary>
        /// Cancelled - communication cancelled before sending
        /// </summary>
        Cancelled = 99
    }
}
