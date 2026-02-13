using DonaRogApp.Enums.Donors;
using System;

namespace DonaRogApp.Donors.Dtos
{
    /// <summary>
    /// DTO per leggere informazioni donatore
    /// </summary>
    public class DonorDto
    {
        public Guid Id { get; set; }

        #region Identification

        /// <summary>
        /// Codice univoco donatore
        /// </summary>
        public string DonorCode { get; set; }

        /// <summary>
        /// Tipo di donatore
        /// </summary>
        public SubjectType SubjectType { get; set; }

        #endregion

        #region Individual Info

        /// <summary>
        /// Nome
        /// </summary>
        public string? FirstName { get; set; }

        /// <summary>
        /// Secondo nome
        /// </summary>
        public string? MiddleName { get; set; }

        /// <summary>
        /// Cognome
        /// </summary>
        public string? LastName { get; set; }

        /// <summary>
        /// Nome completo (formattato)
        /// </summary>
        public string FullName { get; set; }

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
        /// Età (calcolata)
        /// </summary>
        public int? Age { get; set; }

        /// <summary>
        /// ID del titolo
        /// </summary>
        public Guid? TitleId { get; set; }

        /// <summary>
        /// Nome del titolo
        /// </summary>
        public string? TitleName { get; set; }

        #endregion

        #region Organization Info

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

        #region Tax Codes

        /// <summary>
        /// Codice fiscale
        /// </summary>
        public string? TaxCode { get; set; }

        /// <summary>
        /// Partita IVA
        /// </summary>
        public string? VatNumber { get; set; }

        #endregion

        #region Classification

        /// <summary>
        /// Stato donatore
        /// </summary>
        public DonorStatus Status { get; set; }

        /// <summary>
        /// Categoria donatore
        /// </summary>
        public DonorCategory Category { get; set; }

        /// <summary>
        /// Origine donatore
        /// </summary>
        public DonorOrigin? Origin { get; set; }

        #endregion

        #region Statistics

        /// <summary>
        /// Totale donato (lifetime value)
        /// </summary>
        public decimal TotalDonated { get; set; }

        /// <summary>
        /// Numero di donazioni
        /// </summary>
        public int DonationCount { get; set; }

        /// <summary>
        /// Donazione media
        /// </summary>
        public decimal AverageDonationAmount { get; set; }

        /// <summary>
        /// Prima donazione (importo)
        /// </summary>
        public decimal? FirstDonationAmount { get; set; }

        /// <summary>
        /// Ultima donazione (importo)
        /// </summary>
        public decimal? LastDonationAmount { get; set; }

        /// <summary>
        /// Data prima donazione
        /// </summary>
        public DateTime? FirstDonationDate { get; set; }

        /// <summary>
        /// Data ultima donazione
        /// </summary>
        public DateTime? LastDonationDate { get; set; }

        #endregion

        #region RFM

        /// <summary>
        /// Recency score (1-5)
        /// </summary>
        public int RecencyScore { get; set; }

        /// <summary>
        /// Frequency score (1-5)
        /// </summary>
        public int FrequencyScore { get; set; }

        /// <summary>
        /// Monetary score (1-5)
        /// </summary>
        public int MonetaryScore { get; set; }

        /// <summary>
        /// RFM segment
        /// </summary>
        public string? RfmSegment { get; set; }

        #endregion

        #region Preferences

        /// <summary>
        /// Lingua preferita
        /// </summary>
        public string PreferredLanguage { get; set; }

        /// <summary>
        /// Canale di comunicazione preferito
        /// </summary>
        public string? PreferredChannel { get; set; }

        #endregion

        #region Communication Tracking

        /// <summary>
        /// Numero lettere inviate
        /// </summary>
        public int LettersSentCount { get; set; }

        /// <summary>
        /// Numero email inviate
        /// </summary>
        public int EmailsSentCount { get; set; }

        /// <summary>
        /// Data ultima lettera
        /// </summary>
        public DateTime? LastThankYouLetterDate { get; set; }

        /// <summary>
        /// Data ultima email
        /// </summary>
        public DateTime? LastEmailSentDate { get; set; }

        #endregion

        #region Privacy

        /// <summary>
        /// Consenso privacy
        /// </summary>
        public bool PrivacyConsent { get; set; }

        /// <summary>
        /// Data consenso privacy
        /// </summary>
        public DateTime? PrivacyConsentDate { get; set; }

        /// <summary>
        /// Data revoca consenso privacy
        /// </summary>
        public DateTime? PrivacyConsentRevokedDate { get; set; }

        /// <summary>
        /// Consenso newsletter
        /// </summary>
        public bool NewsletterConsent { get; set; }

        /// <summary>
        /// Data consenso newsletter
        /// </summary>
        public DateTime? NewsletterConsentDate { get; set; }

        /// <summary>
        /// Consenso telefono
        /// </summary>
        public bool PhoneConsent { get; set; }

        /// <summary>
        /// Consenso posta (spedizioni cartacee)
        /// </summary>
        public bool MailConsent { get; set; }

        /// <summary>
        /// Data consenso spedizioni cartacee
        /// </summary>
        public DateTime? MailConsentDate { get; set; }

        /// <summary>
        /// Donatore anonimizzato
        /// </summary>
        public bool IsAnonymized { get; set; }

        #endregion

        #region Primary Contact Info

        /// <summary>
        /// Email principale (default)
        /// </summary>
        public string? PrimaryEmail { get; set; }

        /// <summary>
        /// Indirizzo principale formattato (default)
        /// </summary>
        public string? PrimaryAddress { get; set; }

        /// <summary>
        /// Città dell'indirizzo principale
        /// </summary>
        public string? PrimaryCity { get; set; }

        #endregion

        #region Audit

        /// <summary>
        /// Data creazione
        /// </summary>
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// Data ultima modifica
        /// </summary>
        public DateTime? LastModificationTime { get; set; }

        /// <summary>
        /// Note
        /// </summary>
        public string? Notes { get; set; }

        #endregion
    }
}