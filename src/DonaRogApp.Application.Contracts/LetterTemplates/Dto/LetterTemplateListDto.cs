using DonaRogApp.Enums.Communications;
using System;
using Volo.Abp.Application.Dtos;

namespace DonaRogApp.LetterTemplates.Dto
{
    /// <summary>
    /// Lightweight DTO for Letter Template lists (without Content for performance)
    /// </summary>
    public class LetterTemplateListDto : EntityDto<Guid>
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        
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
        
        // Statistics
        public int UsageCount { get; set; }
        public DateTime? LastUsedDate { get; set; }
        
        // Versioning
        public int Version { get; set; }
        
        // Navigation properties (for display)
        public string? ProjectName { get; set; }
        public string? RecurrenceName { get; set; }
        
        // Tags
        public string? Tags { get; set; }
        
        // Audit
        public DateTime CreationTime { get; set; }
        public DateTime? LastModificationTime { get; set; }
    }
}
