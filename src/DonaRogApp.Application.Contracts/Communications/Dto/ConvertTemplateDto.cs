using System;
using System.ComponentModel.DataAnnotations;

namespace DonaRogApp.Application.Contracts.Communications.Dto
{
    /// <summary>
    /// DTO for converting DOCX template to HTML
    /// </summary>
    public class ConvertTemplateDto
    {
        /// <summary>
        /// Template ID
        /// </summary>
        [Required]
        public Guid TemplateId { get; set; }

        /// <summary>
        /// Should convert Word merge fields to standard placeholders?
        /// </summary>
        public bool ConvertMergeFields { get; set; } = true;

        /// <summary>
        /// Should save converted HTML to template?
        /// </summary>
        public bool SaveToTemplate { get; set; } = true;
    }
}
