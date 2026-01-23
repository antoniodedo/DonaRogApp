using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonaRogApp.Enums.Donors
{
    /// <summary>
    /// Legal form of organization (Italian types)
    /// </summary>
    public enum LegalForm
    {
        /// <summary>
        /// Sole proprietorship (Ditta Individuale)
        /// </summary>
        SoleProprietorship = 1,

        /// <summary>
        /// Simple partnership (Società Semplice - S.S.)
        /// </summary>
        SimplePartnership = 2,

        /// <summary>
        /// General partnership (Società in Nome Collettivo - S.N.C.)
        /// </summary>
        GeneralPartnership = 3,

        /// <summary>
        /// Limited partnership (Società in Accomandita Semplice - S.A.S.)
        /// </summary>
        LimitedPartnership = 4,

        /// <summary>
        /// Limited liability company (Società a Responsabilità Limitata - S.R.L.)
        /// </summary>
        LLC = 5,

        /// <summary>
        /// Simplified LLC (S.R.L. Semplificata - S.R.L.S.)
        /// </summary>
        SimplifiedLLC = 6,

        /// <summary>
        /// Joint-stock company (Società per Azioni - S.P.A.)
        /// </summary>
        Corporation = 7,

        /// <summary>
        /// Partnership limited by shares (Società in Accomandita per Azioni - S.A.P.A.)
        /// </summary>
        PartnershipLimitedByShares = 8,

        /// <summary>
        /// Cooperative (Società Cooperativa)
        /// </summary>
        Cooperative = 9,

        /// <summary>
        /// Social cooperative (Cooperativa Sociale)
        /// </summary>
        SocialCooperative = 10,

        /// <summary>
        /// Association (Associazione)
        /// </summary>
        Association = 11,

        /// <summary>
        /// Recognized association (Associazione Riconosciuta)
        /// </summary>
        RecognizedAssociation = 12,

        /// <summary>
        /// Foundation (Fondazione)
        /// </summary>
        Foundation = 13,

        /// <summary>
        /// Committee (Comitato)
        /// </summary>
        Committee = 14,

        /// <summary>
        /// ONLUS (Organizzazione Non Lucrativa di Utilità Sociale)
        /// </summary>
        ONLUS = 15,

        /// <summary>
        /// ETS (Ente del Terzo Settore) - Third Sector Entity
        /// </summary>
        ETS = 16,

        /// <summary>
        /// ODV (Organizzazione di Volontariato) - Volunteer Organization
        /// </summary>
        ODV = 17,

        /// <summary>
        /// APS (Associazione di Promozione Sociale) - Social Promotion Association
        /// </summary>
        APS = 18,

        /// <summary>
        /// Other legal form
        /// </summary>
        Other = 99
    }
}
