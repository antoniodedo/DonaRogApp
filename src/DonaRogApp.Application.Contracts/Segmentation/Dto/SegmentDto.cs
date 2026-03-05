using System;

namespace DonaRogApp.Application.Contracts.Segmentation.Dto
{
    /// <summary>
    /// Simple DTO for Segment
    /// </summary>
    public class SegmentDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ColorCode { get; set; }
        public string? Icon { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
    }
}
