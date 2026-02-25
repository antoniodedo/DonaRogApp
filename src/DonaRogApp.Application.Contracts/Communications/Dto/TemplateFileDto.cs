using System;

namespace DonaRogApp.Application.Contracts.Communications.Dto
{
    /// <summary>
    /// DTO for template file information
    /// </summary>
    public class TemplateFileDto
    {
        /// <summary>
        /// Template ID
        /// </summary>
        public Guid TemplateId { get; set; }

        /// <summary>
        /// Original filename
        /// </summary>
        public string? FileName { get; set; }

        /// <summary>
        /// File extension
        /// </summary>
        public string? FileExtension { get; set; }

        /// <summary>
        /// File size in bytes
        /// </summary>
        public long? FileSizeBytes { get; set; }

        /// <summary>
        /// Upload date
        /// </summary>
        public DateTime? UploadedAt { get; set; }

        /// <summary>
        /// Storage path (for internal use)
        /// </summary>
        public string? StoragePath { get; set; }

        /// <summary>
        /// Is file available for download?
        /// </summary>
        public bool IsAvailable => !string.IsNullOrEmpty(StoragePath) && FileSizeBytes > 0;
    }
}
