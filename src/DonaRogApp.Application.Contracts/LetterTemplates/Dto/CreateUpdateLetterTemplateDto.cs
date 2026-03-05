using DonaRogApp.Enums.Communications;
using System;
using System.ComponentModel.DataAnnotations;

namespace DonaRogApp.LetterTemplates.Dto
{
    /// <summary>
    /// DTO for creating or updating a Letter Template
    /// </summary>
    public class CreateUpdateLetterTemplateDto
    {
        [Required]
        [MaxLength(256)]
        public string Name { get; set; } = null!;
        
        [MaxLength(1000)]
        public string? Description { get; set; }
        
        [Required]
        public string Content { get; set; } = null!;
        
        // Categorization
        [Required]
        public TemplateCategory Category { get; set; }
        
        [Required]
        [MaxLength(5)]
        public string Language { get; set; } = "it";
        
        public CommunicationType? CommunicationType { get; set; }
        
        // Template Characteristics
        public bool IsPlural { get; set; }
        
        // Status
        public bool IsActive { get; set; }
        public bool IsDefault { get; set; }
        
        // Multi-recipient
        [MaxLength(500)]
        public string? CcEmails { get; set; }
        
        [MaxLength(500)]
        public string? BccEmails { get; set; }
        
        // Categorization Advanced
        [MaxLength(500)]
        public string? Tags { get; set; }
    }
}
