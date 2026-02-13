using DonaRogApp.Enums.Donors;
using System;
using Volo.Abp.Application.Dtos;

namespace DonaRogApp.Donors.Dto
{
    /// <summary>
    /// Input DTO per ricerca e paginazione donatori
    /// </summary>
    public class GetDonorsInput : PagedAndSortedResultRequestDto
    {
        /// <summary>
        /// Testo di ricerca generico (nome, cognome, ragione sociale, email, telefono, indirizzo, città, CAP, provincia, nazione, codice donatore, codice fiscale, P.IVA)
        /// </summary>
        public string? Filter { get; set; }

        // ======================================================================
        // FILTRI GENERALI
        // ======================================================================

        /// <summary>
        /// Filtro per tipo donatore (Individual/Organization)
        /// </summary>
        public SubjectType? SubjectType { get; set; }

        /// <summary>
        /// Filtro per stato donatore
        /// </summary>
        public DonorStatus? Status { get; set; }

        /// <summary>
        /// Filtro per categoria donatore
        /// </summary>
        public DonorCategory? Category { get; set; }

        /// <summary>
        /// Filtro per titolo
        /// </summary>
        public Guid? TitleId { get; set; }

        // ======================================================================
        // FILTRI SPECIFICI - RICERCA AVANZATA
        // ======================================================================

        /// <summary>
        /// Ricerca specifica per codice donatore
        /// </summary>
        public string? DonorCode { get; set; }

        /// <summary>
        /// Ricerca specifica per email
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Ricerca specifica per numero di telefono
        /// </summary>
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// Ricerca specifica per città/comune
        /// </summary>
        public string? City { get; set; }

        /// <summary>
        /// Ricerca specifica per CAP
        /// </summary>
        public string? PostalCode { get; set; }

        /// <summary>
        /// Ricerca specifica per provincia
        /// </summary>
        public string? Province { get; set; }

        /// <summary>
        /// Ricerca specifica per nazione
        /// </summary>
        public string? Country { get; set; }

        /// <summary>
        /// Costruttore con valori di default
        /// </summary>
        public GetDonorsInput()
        {
            MaxResultCount = 25; // Default 25 risultati per pagina
            Sorting = "CreationTime DESC"; // Default ordinamento per data creazione
        }
    }
}
