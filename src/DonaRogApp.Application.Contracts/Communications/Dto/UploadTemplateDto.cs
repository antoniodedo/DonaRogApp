using System;
using System.ComponentModel.DataAnnotations;

namespace DonaRogApp.Application.Contracts.Communications.Dto
{
    /// <summary>
    /// DTO for uploading DOCX template file
    /// </summary>
    public class UploadTemplateDto
    {
        /// <summary>
        /// Template ID to update
        /// </summary>
        [Required]
        public Guid TemplateId { get; set; }

        /// <summary>
        /// Original filename
        /// </summary>
        [Required]
        [StringLength(255)]
        public string FileName { get; set; } = null!;

        /// <summary>
        /// File extension (e.g., ".docx")
        /// </summary>
        [Required]
        [StringLength(20)]
        public string FileExtension { get; set; } = null!;

        /// <summary>
        /// File content as base64 string
        /// </summary>
        [Required]
        public string FileContentBase64 { get; set; } = null!;

        /// <summary>
        /// File size in bytes
        /// </summary>
        [Range(1, 10485760)] // Max 10 MB
        public long FileSizeBytes { get; set; }
    }
}
