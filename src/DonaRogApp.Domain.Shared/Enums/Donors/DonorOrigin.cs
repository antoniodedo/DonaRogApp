using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonaRogApp.Enums.Donors
{
    /// <summary>
    /// Source/origin of donor acquisition
    /// </summary>
    public enum DonorOrigin
    {
        /// <summary>
        /// Unknown origin
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Website form submission
        /// </summary>
        Website = 1,

        /// <summary>
        /// Social media campaign
        /// </summary>
        SocialMedia = 2,

        /// <summary>
        /// In-person event
        /// </summary>
        Event = 3,

        /// <summary>
        /// Phone campaign
        /// </summary>
        PhoneCampaign = 4,

        /// <summary>
        /// Direct mail campaign
        /// </summary>
        DirectMail = 5,

        /// <summary>
        /// Email campaign
        /// </summary>
        EmailCampaign = 6,

        /// <summary>
        /// Referral from existing donor
        /// </summary>
        Referral = 7,

        /// <summary>
        /// Partnership with another organization
        /// </summary>
        Partnership = 8,

        /// <summary>
        /// Media coverage (TV, newspaper, radio)
        /// </summary>
        Media = 9,

        /// <summary>
        /// Walk-in to office
        /// </summary>
        WalkIn = 10,

        /// <summary>
        /// Corporate sponsorship
        /// </summary>
        Corporate = 11,

        /// <summary>
        /// Legacy/Bequest conversion
        /// </summary>
        Bequest = 12,

        /// <summary>
        /// Other source
        /// </summary>
        Other = 99
    }
}
