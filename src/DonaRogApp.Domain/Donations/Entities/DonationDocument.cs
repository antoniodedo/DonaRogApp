using DonaRogApp.Enums.Donations;
using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace DonaRogApp.Domain.Donations.Entities
{
    /// <summary>
    /// Represents a document (image, PDF, etc) or text description attached to a donation
    /// Can be from external flow (scanned remittance, PayPal receipt, bank transfer description) or manually uploaded
    /// </summary>
    public class DonationDocument : CreationAuditedEntity<Guid>
    {
        public Guid DonationId { get; private set; }
        
        /// <summary>
        /// Original filename (null for text-only documents)
        /// </summary>
        public string? FileName { get; private set; }
        
        /// <summary>
        /// File extension (jpg, pdf, png, etc) - null for text-only documents
        /// </summary>
        public string? FileExtension { get; private set; }
        
        /// <summary>
        /// MIME type (image/jpeg, application/pdf, etc) - null for text-only documents
        /// </summary>
        public string? MimeType { get; private set; }
        
        /// <summary>
        /// File size in bytes (0 for text-only documents)
        /// </summary>
        public long FileSizeBytes { get; private set; }
        
        /// <summary>
        /// Storage path or key (depending on storage provider)
        /// e.g., "donations/2024/01/guid.pdf" or S3 key
        /// null for text-only documents
        /// </summary>
        public string? StoragePath { get; private set; }
        
        /// <summary>
        /// Text content for documents without physical file (e.g., bank transfer description from external import)
        /// </summary>
        public string? TextContent { get; private set; }
        
        /// <summary>
        /// Document type/category
        /// </summary>
        public DonationDocumentType DocumentType { get; private set; }
        
        /// <summary>
        /// True if document came from external flow (PayPal, scan, etc)
        /// False if manually uploaded by operator
        /// </summary>
        public bool IsFromExternalFlow { get; private set; }
        
        /// <summary>
        /// Optional notes about the document
        /// </summary>
        public string? Notes { get; set; }
        
        /// <summary>
        /// Returns true if this is a text-only document (no physical file)
        /// </summary>
        public bool IsTextDocument => string.IsNullOrEmpty(StoragePath) && !string.IsNullOrEmpty(TextContent);

        private DonationDocument()
        {
            // EF Core
        }

        /// <summary>
        /// Constructor for documents with physical file
        /// </summary>
        public DonationDocument(
            Guid id,
            Guid donationId,
            string fileName,
            string fileExtension,
            string mimeType,
            long fileSizeBytes,
            string storagePath,
            DonationDocumentType documentType,
            bool isFromExternalFlow = false)
        {
            Id = id;
            DonationId = donationId;
            FileName = fileName;
            FileExtension = fileExtension;
            MimeType = mimeType;
            FileSizeBytes = fileSizeBytes;
            StoragePath = storagePath;
            DocumentType = documentType;
            IsFromExternalFlow = isFromExternalFlow;
        }

        /// <summary>
        /// Constructor for text-only documents (e.g., bank transfer description from external import)
        /// </summary>
        public static DonationDocument CreateTextDocument(
            Guid id,
            Guid donationId,
            string textContent,
            DonationDocumentType documentType,
            bool isFromExternalFlow = true)
        {
            return new DonationDocument
            {
                Id = id,
                DonationId = donationId,
                TextContent = textContent,
                DocumentType = documentType,
                IsFromExternalFlow = isFromExternalFlow,
                FileSizeBytes = 0
            };
        }

        public void UpdateNotes(string? notes)
        {
            Notes = notes;
        }
        
        public void UpdateTextContent(string? textContent)
        {
            TextContent = textContent;
        }
    }
}
