using System;
using System.ComponentModel.DataAnnotations;

namespace DonaRogApp.Application.Contracts.Communications.Dto
{
    /// <summary>
    /// DTO for previewing template with sample data
    /// </summary>
    public class PreviewTemplateDto
    {
        /// <summary>
        /// Template ID
        /// </summary>
        [Required]
        public Guid TemplateId { get; set; }

        /// <summary>
        /// Sample donor ID (optional, will use default sample if not provided)
        /// </summary>
        public Guid? SampleDonorId { get; set; }

        /// <summary>
        /// Sample donation ID (optional, will use default sample if not provided)
        /// </summary>
        public Guid? SampleDonationId { get; set; }

        /// <summary>
        /// Output format
        /// </summary>
        public PreviewOutputFormat OutputFormat { get; set; } = PreviewOutputFormat.Html;
    }

    /// <summary>
    /// Preview output format
    /// </summary>
    public enum PreviewOutputFormat
    {
        /// <summary>
        /// HTML preview
        /// </summary>
        Html = 1,

        /// <summary>
        /// PDF preview (base64 encoded)
        /// </summary>
        Pdf = 2
    }
}
