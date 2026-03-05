using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace DonaRogApp.Domain.Donors.Entities
{
    /// <summary>
    /// Represents a file attachment associated with a donor.
    /// Supports documents, images, and other file types.
    /// </summary>
    public class DonorAttachment : FullAuditedEntity<Guid>, IMultiTenant
    {
        public Guid? TenantId { get; private set; }

        /// <summary>
        /// Reference to the Donor this attachment belongs to
        /// </summary>
        public Guid DonorId { get; private set; }
        public virtual Donor Donor { get; private set; } = null!;

        /// <summary>
        /// Original file name
        /// </summary>
        public string FileName { get; private set; } = null!;

        /// <summary>
        /// File extension (e.g., .pdf, .jpg, .png)
        /// </summary>
        public string FileExtension { get; private set; } = null!;

        /// <summary>
        /// MIME type (e.g., application/pdf, image/jpeg)
        /// </summary>
        public string MimeType { get; private set; } = null!;

        /// <summary>
        /// File size in bytes
        /// </summary>
        public long FileSizeBytes { get; private set; }

        /// <summary>
        /// Blob name/key used in the blob storage system
        /// </summary>
        public string BlobName { get; private set; } = null!;

        /// <summary>
        /// Type/category of the attachment (e.g., "Letter", "Drawing", "Document", "Photo")
        /// </summary>
        public string? AttachmentType { get; private set; }

        /// <summary>
        /// Optional description or notes about the attachment
        /// </summary>
        public string? Description { get; private set; }

        /// <summary>
        /// Optional display order for sorting
        /// </summary>
        public int DisplayOrder { get; private set; }

        protected DonorAttachment()
        {
            // Required by EF Core
        }

        public DonorAttachment(
            Guid id,
            Guid donorId,
            string fileName,
            string fileExtension,
            string mimeType,
            long fileSizeBytes,
            string blobName,
            Guid? tenantId = null,
            string? attachmentType = null,
            string? description = null,
            int displayOrder = 0)
            : base(id)
        {
            DonorId = donorId;
            FileName = fileName;
            FileExtension = fileExtension;
            MimeType = mimeType;
            FileSizeBytes = fileSizeBytes;
            BlobName = blobName;
            TenantId = tenantId;
            AttachmentType = attachmentType;
            Description = description;
            DisplayOrder = displayOrder;
        }

        public void UpdateDescription(string? description)
        {
            Description = description;
        }

        public void UpdateAttachmentType(string? attachmentType)
        {
            AttachmentType = attachmentType;
        }

        public void UpdateDisplayOrder(int displayOrder)
        {
            DisplayOrder = displayOrder;
        }
    }
}
