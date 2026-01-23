using DonaRogApp.Enums.Donors;
using System;

namespace DonaRogApp.Donors.Dtos
{
    /// <summary>
    /// DTO per aggiornare un donatore
    /// </summary>
    public class UpdateDonorDto
    {
        #region Individual Properties

        /// <summary>
        /// Nome
        /// </summary>
        public string? FirstName { get; set; }

        /// <summary>
        /// Cognome
        /// </summary>
        public string? LastName { get; set; }

        /// <summary>
        /// Secondo nome
        /// </summary>
        public string? MiddleName { get; set; }

        /// <summary>
        /// ID del titolo
        /// </summary>
        public Guid? TitleId { get; set; }

        /// <summary>
        /// Genere
        /// </summary>
        public Gender? Gender { get; set; }

        /// <summary>
        /// Data di nascita
        /// </summary>
        public DateTime? BirthDate { get; set; }

        /// <summary>
        /// Luogo di nascita
        /// </summary>
        public string? BirthPlace { get; set; }

        #endregion

        #region Organization Properties

        /// <summary>
        /// Nome azienda
        /// </summary>
        public string? CompanyName { get; set; }

        /// <summary>
        /// Tipo di organizzazione
        /// </summary>
        public OrganizationType? OrganizationType { get; set; }

        /// <summary>
        /// Forma giuridica
        /// </summary>
        public LegalForm? LegalForm { get; set; }

        /// <summary>
        /// Settore di business
        /// </summary>
        public string? BusinessSector { get; set; }

        #endregion

        #region Preferences

        /// <summary>
        /// Lingua preferita
        /// </summary>
        public string? PreferredLanguage { get; set; }

        /// <summary>
        /// Canale di comunicazione preferito
        /// </summary>
        public string? PreferredChannel { get; set; }

        #endregion

        #region Additional Info

        /// <summary>
        /// Note
        /// </summary>
        public string? Notes { get; set; }

        #endregion
    }
}
