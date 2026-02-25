using DonaRogApp.Enums.Donations;
using System;

namespace DonaRogApp.Application.Contracts.Donations.Dto
{
    public class DonationDocumentDto
    {
        public Guid Id { get; set; }
        public Guid DonationId { get; set; }
        public string? FileName { get; set; }
        public string? FileExtension { get; set; }
        public string? MimeType { get; set; }
        public long FileSizeBytes { get; set; }
        public string? StoragePath { get; set; }
        public string? TextContent { get; set; }
        public DonationDocumentType DocumentType { get; set; }
        public bool IsFromExternalFlow { get; set; }
        public string? Notes { get; set; }
        public DateTime CreationTime { get; set; }
        
        /// <summary>
        /// True if this is a text-only document (no physical file)
        /// </summary>
        public bool IsTextDocument { get; set; }
        
        /// <summary>
        /// Presigned URL for download (if cloud storage with presigned URLs is enabled)
        /// </summary>
        public string? DownloadUrl { get; set; }
    }
    
    public class UploadDonationDocumentDto
    {
        public DonationDocumentType DocumentType { get; set; }
        public string? Notes { get; set; }
    }
    
    /// <summary>
    /// DTO for creating text-only documents (e.g., from external import flows)
    /// </summary>
    public class CreateTextDocumentDto
    {
        public string TextContent { get; set; } = string.Empty;
        public DonationDocumentType DocumentType { get; set; }
        public bool IsFromExternalFlow { get; set; } = true;
        public string? Notes { get; set; }
    }
}
