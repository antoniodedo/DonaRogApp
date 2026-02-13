using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace DonaRogApp.LetterTemplates
{
    /// <summary>
    /// Template Attachment Entity
    /// 
    /// RESPONSIBILITY:
    /// - Store attachment metadata for letter templates
    /// - Track file information (name, path, size)
    /// </summary>
    public class TemplateAttachment : CreationAuditedEntity<Guid>
    {
        /// <summary>
        /// Parent template ID
        /// </summary>
        public Guid TemplateId { get; set; }

        /// <summary>
        /// File name
        /// </summary>
        public string FileName { get; set; } = null!;

        /// <summary>
        /// File path (could be physical path or blob storage path)
        /// </summary>
        public string FilePath { get; set; } = null!;

        /// <summary>
        /// File size in bytes
        /// </summary>
        public long FileSize { get; set; }

        /// <summary>
        /// Optional description
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Parent template (navigation property)
        /// </summary>
        public virtual LetterTemplate Template { get; set; } = null!;

        /// <summary>
        /// Protected constructor for EF Core
        /// </summary>
        protected TemplateAttachment()
        {
        }

        /// <summary>
        /// Factory method to create a new attachment
        /// </summary>
        internal static TemplateAttachment Create(
            Guid id,
            Guid templateId,
            string fileName,
            string filePath,
            long fileSize,
            string? description = null)
        {
            return new TemplateAttachment
            {
                Id = id,
                TemplateId = templateId,
                FileName = fileName,
                FilePath = filePath,
                FileSize = fileSize,
                Description = description
            };
        }
    }
}
