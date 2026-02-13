using System;

namespace DonaRogApp.Donors.Dtos
{
    public class DonorTagDto
    {
        public Guid TagId { get; set; }
        public string TagName { get; set; } = string.Empty;
        public string TagCode { get; set; } = string.Empty;
        public string? ColorCode { get; set; }
        public string? Category { get; set; }
        public DateTime TaggedAt { get; set; }
        public bool IsAutomatic { get; set; }
        public string? TaggingReason { get; set; }
        public int Priority { get; set; }
    }
}
