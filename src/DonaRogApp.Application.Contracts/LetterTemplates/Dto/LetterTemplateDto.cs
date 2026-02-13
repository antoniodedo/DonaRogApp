using DonaRogApp.Enums.Communications;
using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace DonaRogApp.LetterTemplates.Dto
{
    /// <summary>
    /// Complete DTO for Letter Template with all properties
    /// </summary>
    public class LetterTemplateDto : AuditedEntityDto<Guid>
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string Content { get; set; } = null!;
        
        // Categorization
        public TemplateCategory Category { get; set; }
        public string Language { get; set; } = "it";
        public CommunicationType? CommunicationType { get; set; }
        
        // Selection Criteria
        public Guid? ProjectId { get; set; }
        public Guid? RecurrenceId { get; set; }
        public decimal? MinAmount { get; set; }
        public decimal? MaxAmount { get; set; }
        public bool IsForNewDonor { get; set; }
        public bool IsPlural { get; set; }
        
        // Status
        public bool IsActive { get; set; }
        public bool IsDefault { get; set; }
        
        // Multi-recipient
        public string? CcEmails { get; set; }
        public string? BccEmails { get; set; }
        
        // Statistics
        public int UsageCount { get; set; }
        public DateTime? LastUsedDate { get; set; }
        
        // Versioning
        public int Version { get; set; }
        public Guid? PreviousVersionId { get; set; }
        
        // Categorization Advanced
        public string? Tags { get; set; }
        
        // Navigation properties (for display)
        public string? ProjectName { get; set; }
        public string? RecurrenceName { get; set; }
        
        // Attachments
        public List<TemplateAttachmentDto> Attachments { get; set; } = new();
    }
}
