using System;
using Volo.Abp.Application.Dtos;

namespace DonaRogApp.Application.Contracts.Projects.Dto
{
    /// <summary>
    /// Project document DTO
    /// </summary>
    public class ProjectDocumentDto : FullAuditedEntityDto<Guid>
    {
        /// <summary>
        /// Parent project ID
        /// </summary>
        public Guid ProjectId { get; set; }

        /// <summary>
        /// Document file name
        /// </summary>
        public string FileName { get; set; } = null!;

        /// <summary>
        /// Document file URL
        /// </summary>
        public string FileUrl { get; set; } = null!;

        /// <summary>
        /// File MIME type
        /// </summary>
        public string? FileType { get; set; }

        /// <summary>
        /// File size in bytes
        /// </summary>
        public long FileSize { get; set; }

        /// <summary>
        /// Document description
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Display order
        /// </summary>
        public int DisplayOrder { get; set; }
    }

    /// <summary>
    /// DTO for creating/uploading a project document
    /// </summary>
    public class CreateProjectDocumentDto
    {
        /// <summary>
        /// Document file name
        /// </summary>
        public string FileName { get; set; } = null!;

        /// <summary>
        /// Document file URL (after upload to blob storage)
        /// </summary>
        public string FileUrl { get; set; } = null!;

        /// <summary>
        /// File MIME type
        /// </summary>
        public string? FileType { get; set; }

        /// <summary>
        /// File size in bytes
        /// </summary>
        public long FileSize { get; set; }

        /// <summary>
        /// Document description
        /// </summary>
        public string? Description { get; set; }
    }

    /// <summary>
    /// DTO for updating a project document
    /// </summary>
    public class UpdateProjectDocumentDto
    {
        /// <summary>
        /// Document file name
        /// </summary>
        public string FileName { get; set; } = null!;

        /// <summary>
        /// Document description
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Display order
        /// </summary>
        public int DisplayOrder { get; set; }
    }
}
