using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace DonaRogApp.Domain.Shared.Entities
{
    /// <summary>
    /// Interest (Area di interesse tematica)
    /// Shared Entity per tracciare interessi dei donatori
    /// Esempi: "Educazione", "Sanità", "Ambiente", "Cultura"
    /// </summary>
    public class Interest : AggregateRoot<Guid>, IMultiTenant
    {
        /// <summary>
        /// Tenant ID - ogni tenant ha le sue aree di interesse
        /// </summary>
        public Guid? TenantId { get; private set; }
        /// <summary>
        /// Codice univoco (es: "EDU", "HEALTH", "ENV")
        /// </summary>
        public string Code { get; private set; }

        /// <summary>
        /// Nome descrittivo
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Descrizione dettagliata dell'area di interesse
        /// </summary>
        public string? Description { get; private set; }

        /// <summary>
        /// Categoria superiore per organizzazione gerarchica (es: "Sociale", "Culturale")
        /// </summary>
        public string? Category { get; private set; }

        /// <summary>
        /// Icona/emoji per visualizzazione
        /// </summary>
        public string? Icon { get; private set; }

        /// <summary>
        /// Colore associato (#FF5733)
        /// </summary>
        public string? ColorCode { get; private set; }

        /// <summary>
        /// Ordine di visualizzazione
        /// </summary>
        public int DisplayOrder { get; private set; }

        /// <summary>
        /// Numero di donatori interessati
        /// </summary>
        public int DonorCount { get; private set; }

        /// <summary>
        /// Indica se attivo
        /// </summary>
        public bool IsActive { get; private set; }

        // Costruttore privato
        private Interest() { }

        /// <summary>
        /// Factory method
        /// </summary>
        public static Interest Create(
            string code,
            string name,
            string? description = null,
            string? category = null,
            string? icon = null,
            string? colorCode = null,
            int displayOrder = 0)
        {
            Check.NotNullOrWhiteSpace(code, nameof(code));
            Check.NotNullOrWhiteSpace(name, nameof(name));

            return new Interest
            {
                Id = Guid.NewGuid(),
                Code = code.ToUpper(),
                Name = name,
                Description = description,
                Category = category,
                Icon = icon,
                ColorCode = colorCode,
                DisplayOrder = displayOrder,
                DonorCount = 0,
                IsActive = true
            };
        }

        /// <summary>
        /// Incrementa contatore donatori
        /// </summary>
        public void IncrementDonorCount()
        {
            DonorCount++;
        }

        /// <summary>
        /// Decrementa contatore donatori
        /// </summary>
        public void DecrementDonorCount()
        {
            if (DonorCount > 0)
                DonorCount--;
        }

        /// <summary>
        /// Aggiorna i dettagli
        /// </summary>
        public void Update(
            string name,
            string? description,
            string? category,
            string? icon,
            string? colorCode,
            int displayOrder)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));

            Name = name;
            Description = description;
            Category = category;
            Icon = icon;
            ColorCode = colorCode;
            DisplayOrder = displayOrder;
        }

        /// <summary>
        /// Disattiva l'interesse
        /// </summary>
        public void Deactivate()
        {
            IsActive = false;
        }

        /// <summary>
        /// Riattiva l'interesse
        /// </summary>
        public void Activate()
        {
            IsActive = true;
        }
    }
}