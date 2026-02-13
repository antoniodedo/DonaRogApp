using System;
using Volo.Abp.Application.Dtos;

namespace DonaRogApp.LetterTemplates.Dto
{
    /// <summary>
    /// DTO for Template Attachment
    /// </summary>
    public class TemplateAttachmentDto : EntityDto<Guid>
    {
        public Guid TemplateId { get; set; }
        public string FileName { get; set; } = null!;
        public string FilePath { get; set; } = null!;
        public long FileSize { get; set; }
        public string? Description { get; set; }
        public DateTime CreationTime { get; set; }
    }
}
