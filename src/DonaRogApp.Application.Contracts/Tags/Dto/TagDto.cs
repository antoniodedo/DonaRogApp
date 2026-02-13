using System;
using Volo.Abp.Application.Dtos;

namespace DonaRogApp.Tags.Dto
{
    public class TagDto : EntityDto<Guid>
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ColorCode { get; set; }
        public string? Category { get; set; }
        public int UsageCount { get; set; }
        public bool IsActive { get; set; }
        public bool IsSystem { get; set; }
    }
}
