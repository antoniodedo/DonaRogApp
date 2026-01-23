using DonaRogApp.Enums.Donors;
using System;
using System.Collections.Generic;
using Volo.Abp;
using Volo.Abp.Domain.Entities;

namespace DonaRogApp.Domain.Shared.Entities
{
    /// <summary>
    /// Title (Titolo di cortesia)
    /// Shared Entity usata in multiple Bounded Contexts
    /// Esempi: Sig., Sig.ra, Dott., Ing., Prof., Avv., etc.
    /// </summary>
    public class Title : AggregateRoot<Guid>
    {
        /// <summary>
        /// Codice univoco del titolo (es: "MR", "DR", "PROF")
        /// </summary>
        public string Code { get; private set; }

        /// <summary>
        /// Nome del titolo in italiano (es: "Signore", "Dottore", "Professore")
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Abbreviazione per stampe (es: "Sig.", "Dott.", "Prof.")
        /// </summary>
        public string Abbreviation { get; private set; }

        /// <summary>
        /// Genere associato (es: Masculine, Feminine, Neutral)
        /// </summary>
        public Gender? AssociatedGender { get; private set; }

        /// <summary>
        /// Ordine di visualizzazione nelle liste (sorting)
        /// </summary>
        public int DisplayOrder { get; private set; }

        /// <summary>
        /// Indica se il titolo è attivo
        /// </summary>
        public bool IsActive { get; private set; }

        /// <summary>
        /// Note interne
        /// </summary>
        public string? Notes { get; private set; }

        // Costruttore privato per EF Core
        private Title() { }

        /// <summary>
        /// Factory method per creare un nuovo titolo
        /// </summary>
        public static Title Create(
            string code,
            string name,
            string abbreviation,
            Gender? associatedGender = null,
            int displayOrder = 0)
        {
            Check.NotNullOrWhiteSpace(code, nameof(code));
            Check.NotNullOrWhiteSpace(name, nameof(name));
            Check.NotNullOrWhiteSpace(abbreviation, nameof(abbreviation));

            return new Title
            {
                Id = Guid.NewGuid(),
                Code = code.ToUpper(),
                Name = name,
                Abbreviation = abbreviation,
                AssociatedGender = associatedGender,
                DisplayOrder = displayOrder,
                IsActive = true
            };
        }

        /// <summary>
        /// Disattiva il titolo (soft delete)
        /// </summary>
        public void Deactivate()
        {
            IsActive = false;
        }

        /// <summary>
        /// Riattiva il titolo
        /// </summary>
        public void Activate()
        {
            IsActive = true;
        }

        /// <summary>
        /// Aggiorna note interne
        /// </summary>
        public void UpdateNotes(string? notes)
        {
            Notes = notes;
        }
    }
}