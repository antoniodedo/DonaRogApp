using System;
using Volo.Abp.Domain.Entities;

namespace DonaRogApp.Domain.Communications.Entities
{
    /// <summary>
    /// Many-to-Many relationship between ThankYouRule and LetterTemplate
    /// Allows a rule to have multiple templates in a pool for LRU rotation
    /// </summary>
    public class RuleTemplateAssociation : Entity
    {
        /// <summary>
        /// Thank You Rule ID
        /// </summary>
        public Guid RuleId { get; set; }

        /// <summary>
        /// Letter Template ID
        /// </summary>
        public Guid TemplateId { get; set; }

        /// <summary>
        /// Priority/Order within the pool (1 = first choice, 2 = second, etc.)
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// Is this template active in the pool?
        /// </summary>
        public bool IsActive { get; set; }

        // ======================================================================
        // NAVIGATION PROPERTIES
        // ======================================================================
        public virtual ThankYouRule Rule { get; set; } = null!;
        public virtual DonaRogApp.LetterTemplates.LetterTemplate Template { get; set; } = null!;

        // ======================================================================
        // CONSTRUCTOR
        // ======================================================================
        private RuleTemplateAssociation()
        {
        }

        public RuleTemplateAssociation(Guid ruleId, Guid templateId, int priority, bool isActive = true)
        {
            RuleId = ruleId;
            TemplateId = templateId;
            Priority = priority;
            IsActive = isActive;
        }

        public override object[] GetKeys()
        {
            return new object[] { RuleId, TemplateId };
        }
    }
}
