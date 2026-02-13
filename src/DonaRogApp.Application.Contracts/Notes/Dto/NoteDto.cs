using System;
using Volo.Abp.Application.Dtos;

namespace DonaRogApp.Notes.Dto
{
    public class NoteDto : EntityDto<Guid>
    {
        public Guid DonorId { get; set; }
        public string Content { get; set; } = string.Empty;
        public string? Category { get; set; }
        public DateTime InteractionDate { get; set; }
        public bool IsImportant { get; set; }
        public bool IsPrivate { get; set; }
        
        // Audit info
        public DateTime CreationTime { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public Guid? CreatorId { get; set; }
        public Guid? LastModifierId { get; set; }
    }
}
