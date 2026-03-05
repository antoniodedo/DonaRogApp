using System;
using Volo.Abp.Application.Dtos;

namespace DonaRogApp.Application.Contracts.Donors.Dto
{
    public class DonorAttachmentDto : EntityDto<Guid>
    {
        public Guid DonorId { get; set; }
        public string FileName { get; set; } = null!;
        public string FileExtension { get; set; } = null!;
        public string MimeType { get; set; } = null!;
        public long FileSizeBytes { get; set; }
        public string? AttachmentType { get; set; }
        public string? Description { get; set; }
        public int DisplayOrder { get; set; }
        public DateTime CreationTime { get; set; }
        public string? CreatorUserName { get; set; }
    }
}
