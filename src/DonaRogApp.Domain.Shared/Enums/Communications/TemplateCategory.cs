using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonaRogApp.Enums.Communications
{
    /// <summary>
    /// Category of communication template
    /// </summary>
    public enum TemplateCategory
    {
        /// <summary>
        /// Thank you letter after donation
        /// </summary>
        ThankYou = 1,

        /// <summary>
        /// Reminder (payment, event, etc.)
        /// </summary>
        Reminder = 2,

        /// <summary>
        /// Newsletter
        /// </summary>
        Newsletter = 3,

        /// <summary>
        /// Holiday greetings (Christmas, Easter, etc.)
        /// </summary>
        HolidayGreeting = 4,

        /// <summary>
        /// Project update communication
        /// </summary>
        ProjectUpdate = 5,

        /// <summary>
        /// Payment request/solicitation
        /// </summary>
        Solicitation = 6,

        /// <summary>
        /// Confirmation (donation received, registration, etc.)
        /// </summary>
        Confirmation = 7,

        /// <summary>
        /// Request for information
        /// </summary>
        InformationRequest = 8,

        /// <summary>
        /// Birthday greeting
        /// </summary>
        Birthday = 9,

        /// <summary>
        /// Anniversary (of first donation, membership, etc.)
        /// </summary>
        Anniversary = 10,

        /// <summary>
        /// Survey/Feedback request
        /// </summary>
        Survey = 11,

        /// <summary>
        /// Event invitation
        /// </summary>
        EventInvitation = 12,

        /// <summary>
        /// Fiscal receipt (tax deduction)
        /// </summary>
        FiscalReceipt = 13,

        /// <summary>
        /// Other category
        /// </summary>
        Other = 99
    }
}
