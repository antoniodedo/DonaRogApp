using System.Collections.Generic;

namespace DonaRogApp.Application.Contracts.Communications.Dto
{
    /// <summary>
    /// Result of template conversion (DOCX to HTML)
    /// </summary>
    public class TemplateConversionResultDto
    {
        /// <summary>
        /// Converted HTML content
        /// </summary>
        public string HtmlContent { get; set; } = null!;

        /// <summary>
        /// Placeholders found in converted template
        /// </summary>
        public List<string> FoundPlaceholders { get; set; } = new();

        /// <summary>
        /// Unrecognized placeholders (warnings)
        /// </summary>
        public List<string> UnrecognizedPlaceholders { get; set; } = new();

        /// <summary>
        /// Conversion warnings
        /// </summary>
        public List<string> Warnings { get; set; } = new();

        /// <summary>
        /// Was conversion successful?
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Estimated PDF size in bytes
        /// </summary>
        public long EstimatedPdfSizeBytes { get; set; }
    }
}
