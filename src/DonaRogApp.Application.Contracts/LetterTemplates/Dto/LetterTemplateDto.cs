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
        
        // Template Characteristics
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
        
        // Associated Thank You Rules with priority in pool
        public List<RuleTemplateAssociationDto> AssociatedRules { get; set; } = new();
        
        // Attachments
        public List<TemplateAttachmentDto> Attachments { get; set; } = new();
    }
    
    /// <summary>
    /// DTO for displaying rule-template associations
    /// </summary>
    public class RuleTemplateAssociationDto
    {
        public Guid RuleId { get; set; }
        public string RuleName { get; set; } = null!;
        public Guid TemplateId { get; set; }
        public int TemplatePriorityInPool { get; set; }
        public bool IsTemplateActiveInPool { get; set; }
        public int RulePriority { get; set; }
        public bool IsRuleActive { get; set; }
        public bool IsRuleTemporary { get; set; }
        public DateTime? RuleValidFrom { get; set; }
        public DateTime? RuleValidUntil { get; set; }
    }
}
