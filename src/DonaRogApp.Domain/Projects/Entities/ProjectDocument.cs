using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace DonaRogApp.Domain.Projects.Entities
{
    /// <summary>
    /// Document attached to a project (PDF, DOCX, images, etc.)
    /// </summary>
    public class ProjectDocument : FullAuditedEntity<Guid>
    {
        /// <summary>
        /// Parent project ID
        /// </summary>
        public Guid ProjectId { get; private set; }

        /// <summary>
        /// Navigation property to parent project
        /// </summary>
        public virtual Project Project { get; private set; } = null!;

        /// <summary>
        /// Document file name
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// Document file URL (blob storage or file path)
        /// </summary>
        public string FileUrl { get; private set; }

        /// <summary>
        /// File MIME type
        /// </summary>
        public string? FileType { get; private set; }

        /// <summary>
        /// File size in bytes
        /// </summary>
        public long FileSize { get; private set; }

        /// <summary>
        /// Document description/notes
        /// </summary>
        public string? Description { get; private set; }

        /// <summary>
        /// Display order
        /// </summary>
        public int DisplayOrder { get; private set; }

        /// <summary>
        /// Private constructor for EF Core
        /// </summary>
        private ProjectDocument()
        {
            FileName = string.Empty;
            FileUrl = string.Empty;
        }

        /// <summary>
        /// Constructor for creating new document
        /// </summary>
        internal ProjectDocument(
            Guid id,
            Guid projectId,
            string fileName,
            string fileUrl,
            string? fileType,
            long fileSize,
            string? description = null,
            int displayOrder = 0)
            : base(id)
        {
            ProjectId = projectId;
            FileName = Check.NotNullOrWhiteSpace(fileName, nameof(fileName), maxLength: 255);
            FileUrl = Check.NotNullOrWhiteSpace(fileUrl, nameof(fileUrl), maxLength: 1000);
            FileType = fileType;
            FileSize = fileSize;
            Description = description;
            DisplayOrder = displayOrder;

            VerifyInvariants();
        }

        /// <summary>
        /// Update document info
        /// </summary>
        public void UpdateInfo(string fileName, string? description = null, int? displayOrder = null)
        {
            FileName = Check.NotNullOrWhiteSpace(fileName, nameof(fileName), maxLength: 255);
            Description = description;
            
            if (displayOrder.HasValue)
            {
                DisplayOrder = displayOrder.Value;
            }

            VerifyInvariants();
        }

        /// <summary>
        /// Verify business invariants
        /// </summary>
        private void VerifyInvariants()
        {
            Check.NotNullOrWhiteSpace(FileName, nameof(FileName));
            Check.NotNullOrWhiteSpace(FileUrl, nameof(FileUrl));

            if (FileSize < 0)
            {
                throw new BusinessException("DonaRog:InvalidFileSize")
                    .WithData("fileSize", FileSize);
            }

            if (FileSize > 10 * 1024 * 1024) // 10MB max
            {
                throw new BusinessException("DonaRog:FileSizeTooLarge")
                    .WithData("fileSize", FileSize)
                    .WithData("maxSize", 10 * 1024 * 1024);
            }
        }
    }
}
