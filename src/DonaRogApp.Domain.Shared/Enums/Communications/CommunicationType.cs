using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonaRogApp.Enums.Communications
{
    /// <summary>
    /// Type of communication channel
    /// </summary>
    public enum CommunicationType
    {
        /// <summary>
        /// Email communication
        /// </summary>
        Email = 1,

        /// <summary>
        /// Postal letter (physical mail)
        /// </summary>
        Letter = 2,

        /// <summary>
        /// SMS text message
        /// </summary>
        SMS = 3,

        /// <summary>
        /// WhatsApp message
        /// </summary>
        WhatsApp = 4,

        /// <summary>
        /// Phone call
        /// </summary>
        PhoneCall = 5,

        /// <summary>
        /// Push notification (mobile app)
        /// </summary>
        PushNotification = 6,

        /// <summary>
        /// In-app message
        /// </summary>
        InApp = 7,

        /// <summary>
        /// Social media message
        /// </summary>
        SocialMedia = 8
    }
}
