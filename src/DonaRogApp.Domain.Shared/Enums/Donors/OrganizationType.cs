using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonaRogApp.Enums.Donors
{
    /// <summary>
    /// Type of organization (for Organization donors only)
    /// </summary>
    public enum OrganizationType
    {
        /// <summary>
        /// Non-profit association (Associazione)
        /// </summary>
        Association = 1,

        /// <summary>
        /// Foundation (Fondazione)
        /// </summary>
        Foundation = 2,

        /// <summary>
        /// NGO (Non-Governmental Organization)
        /// </summary>
        NGO = 3,

        /// <summary>
        /// Charity organization (Ente Benefico)
        /// </summary>
        Charity = 4,

        /// <summary>
        /// Religious organization (Ente Religioso)
        /// </summary>
        Religious = 5,

        /// <summary>
        /// Educational institution (Istituzione Educativa)
        /// </summary>
        Educational = 6,

        /// <summary>
        /// Healthcare organization (Ente Sanitario)
        /// </summary>
        Healthcare = 7,

        /// <summary>
        /// Government entity (Ente Pubblico)
        /// </summary>
        Government = 8,

        /// <summary>
        /// Private company (Azienda Privata)
        /// </summary>
        PrivateCompany = 9,

        /// <summary>
        /// Cooperative (Cooperativa)
        /// </summary>
        Cooperative = 10,

        /// <summary>
        /// Social enterprise (Impresa Sociale)
        /// </summary>
        SocialEnterprise = 11,

        /// <summary>
        /// Other type
        /// </summary>
        Other = 99
    }
}
