using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace DonaRogApp.Domain.Shared.Entities
{
    /// <summary>
    /// Tag (Etichetta libera)
    /// Shared Entity per taggare donatori con etichette personalizzate
    /// Esempi: "VIP", "ProblemaDonazione", "InternoProgetto"
    /// </summary>
    public class Tag : AggregateRoot<Guid>, IMultiTenant
    {
        /// <summary>
        /// Tenant ID - ogni tenant ha i suoi tag
        /// </summary>
        public Guid? TenantId { get; private set; }
        /// <summary>
        /// Codice univoco del tag (slugified)
        /// </summary>
        public string Code { get; private set; }

        /// <summary>
        /// Nome descrittivo del tag
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Descrizione estesa
        /// </summary>
        public string? Description { get; private set; }

        /// <summary>
        /// Colore hex per visualizzazione (#FF5733)
        /// </summary>
        public string? ColorCode { get; private set; }

        /// <summary>
        /// Categoria del tag per organizzazione (es: "Status", "Priority", "Note")
        /// </summary>
        public string? Category { get; private set; }

        /// <summary>
        /// Numero di donatori che hanno questo tag
        /// </summary>
        public int UsageCount { get; private set; }

        /// <summary>
        /// Indica se il tag è attivo
        /// </summary>
        public bool IsActive { get; private set; }

        /// <summary>
        /// Tag creato dal sistema (non cancellabile)
        /// </summary>
        public bool IsSystem { get; private set; }

        // Costruttore privato
        private Tag() { }

        /// <summary>
        /// Factory method
        /// </summary>
        public static Tag Create(
            string name,
            string? description = null,
            string? colorCode = null,
            string? category = null,
            bool isSystem = false)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));

            var code = SlugifyName(name);

            return new Tag
            {
                Id = Guid.NewGuid(),
                Code = code,
                Name = name,
                Description = description,
                ColorCode = colorCode,
                Category = category,
                UsageCount = 0,
                IsActive = true,
                IsSystem = isSystem
            };
        }

        /// <summary>
        /// Incrementa contatore di utilizzo
        /// </summary>
        public void IncrementUsageCount()
        {
            UsageCount++;
        }

        /// <summary>
        /// Decrementa contatore di utilizzo
        /// </summary>
        public void DecrementUsageCount()
        {
            if (UsageCount > 0)
                UsageCount--;
        }

        /// <summary>
        /// Aggiorna i dettagli del tag
        /// </summary>
        public void Update(string name, string? description, string? colorCode, string? category)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));

            Name = name;
            Code = SlugifyName(name);
            Description = description;
            ColorCode = colorCode;
            Category = category;
        }

        /// <summary>
        /// Disattiva il tag
        /// </summary>
        public void Deactivate()
        {
            if (IsSystem)
                throw new BusinessException("TagErrors:CannotDeactivateSystemTag");

            IsActive = false;
        }

        /// <summary>
        /// Riattiva il tag
        /// </summary>
        public void Activate()
        {
            IsActive = true;
        }

        /// <summary>
        /// Converte nome in slug (es: "My Tag" → "my-tag")
        /// </summary>
        private static string SlugifyName(string name)
        {
            return name
                .ToLower()
                .Replace(" ", "-")
                .Replace("--", "-")
                .Trim('-');
        }
    }
}