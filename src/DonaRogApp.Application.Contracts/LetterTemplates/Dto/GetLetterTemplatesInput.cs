using DonaRogApp.Enums.Communications;
using System;
using Volo.Abp.Application.Dtos;

namespace DonaRogApp.LetterTemplates.Dto
{
    /// <summary>
    /// Input DTO for filtering Letter Templates
    /// </summary>
    public class GetLetterTemplatesInput : PagedAndSortedResultRequestDto
    {
        public string? Filter { get; set; }
        public TemplateCategory? Category { get; set; }
        public string? Language { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDefault { get; set; }
        public CommunicationType? CommunicationType { get; set; }
        public bool? IsPlural { get; set; }
    }
}
