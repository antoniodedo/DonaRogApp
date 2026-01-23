using DonaRogApp.Enums.Donors;
using DonaRogApp.ValueObjects;
using System;

namespace DonaRogApp.Donors.Dtos
{
    /// <summary>
    /// DTO per creare un nuovo donatore
    /// </summary>
    public class CreateDonorDto
    {
        /// <summary>
        /// Tipo di donatore (Individual o Organization)
        /// </summary>
        public SubjectType SubjectType { get; set; }

        #region Individual Properties

        /// <summary>
        /// Nome (solo per persone fisiche)
        /// </summary>
        public string? FirstName { get; set; }

        /// <summary>
        /// Cognome (solo per persone fisiche)
        /// </summary>
        public string? LastName { get; set; }

        /// <summary>
        /// Secondo nome (opzionale)
        /// </summary>
        public string? MiddleName { get; set; }

        /// <summary>
        /// ID del titolo (Sig., Dott., Prof., etc.)
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

        /// <summary>
        /// Codice fiscale italiano
        /// </summary>
        public string? TaxCode { get; set; }

        #endregion

        #region Organization Properties

        /// <summary>
        /// Nome azienda (solo per organizzazioni)
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

        /// <summary>
        /// Numero di partita IVA
        /// </summary>
        public string? VatNumber { get; set; }

        #endregion

        #region Contact Information

        /// <summary>
        /// Email principale
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Numero di telefono principale
        /// </summary>
        public string? PhoneNumber { get; set; }

        #endregion

        #region Additional Info

        /// <summary>
        /// Origine del donatore (Web, Event, Referral, etc.)
        /// </summary>
        public DonorOrigin? Origin { get; set; }

        /// <summary>
        /// Lingua preferita (default: IT)
        /// </summary>
        public string PreferredLanguage { get; set; } = "IT";

        /// <summary>
        /// Note iniziali
        /// </summary>
        public string? Notes { get; set; }

        #endregion
    }
}