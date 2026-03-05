using System;

namespace DonaRogApp.Application.Contracts.Segmentation.Dto
{
    /// <summary>
    /// DTO for reordering rules
    /// </summary>
    public class RuleOrderDto
    {
        public Guid RuleId { get; set; }
        public int Priority { get; set; }
    }
}
