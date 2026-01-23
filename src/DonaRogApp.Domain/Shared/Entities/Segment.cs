using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities;

namespace DonaRogApp.Domain.Shared.Entities
{
    /// <summary>
    /// Segment (Segmento marketing)
    /// Shared Entity per categorizzare donatori in gruppi marketing
    /// Esempi: "Major Donors", "Lapsed Donors", "Monthly Contributors"
    /// </summary>
    public class Segment : AggregateRoot<Guid>
    {
        /// <summary>
        /// Codice univoco segmento (es: "MAJ_DONORS", "LAPSED")
        /// </summary>
        public string Code { get; private set; }

        /// <summary>
        /// Nome descrittivo (es: "Donatori Maggiori")
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Descrizione dettagliata del segmento
        /// </summary>
        public string? Description { get; private set; }

        /// <summary>
        /// Colore associato per UI (hex code es: #FF5733)
        /// </summary>
        public string? ColorCode { get; private set; }

        /// <summary>
        /// Icona/emoji per visualizzazione
        /// </summary>
        public string? Icon { get; private set; }

        /// <summary>
        /// Ordine di visualizzazione
        /// </summary>
        public int DisplayOrder { get; private set; }

        /// <summary>
        /// Indica se il segmento è attivo per nuove assegnazioni
        /// </summary>
        public bool IsActive { get; private set; }

        /// <summary>
        /// Segmento di sistema (non cancellabile)
        /// </summary>
        public bool IsSystem { get; private set; }

        // Costruttore privato
        private Segment() { }

        /// <summary>
        /// Factory method
        /// </summary>
        public static Segment Create(
            string code,
            string name,
            string? description = null,
            string? colorCode = null,
            string? icon = null,
            int displayOrder = 0,
            bool isSystem = false)
        {
            Check.NotNullOrWhiteSpace(code, nameof(code));
            Check.NotNullOrWhiteSpace(name, nameof(name));

            return new Segment
            {
                Id = Guid.NewGuid(),
                Code = code.ToUpper(),
                Name = name,
                Description = description,
                ColorCode = colorCode,
                Icon = icon,
                DisplayOrder = displayOrder,
                IsActive = true,
                IsSystem = isSystem
            };
        }

        /// <summary>
        /// Aggiorna i dettagli del segmento
        /// </summary>
        public void Update(string name, string? description, string? colorCode, string? icon, int displayOrder)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));

            Name = name;
            Description = description;
            ColorCode = colorCode;
            Icon = icon;
            DisplayOrder = displayOrder;
        }

        /// <summary>
        /// Disattiva il segmento
        /// </summary>
        public void Deactivate()
        {
            if (IsSystem)
                throw new BusinessException("SegmentErrors:CannotDeactivateSystemSegment");

            IsActive = false;
        }

        /// <summary>
        /// Riattiva il segmento
        /// </summary>
        public void Activate()
        {
            IsActive = true;
        }
    }
}