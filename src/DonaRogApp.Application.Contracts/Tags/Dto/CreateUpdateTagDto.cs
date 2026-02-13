using System.ComponentModel.DataAnnotations;

namespace DonaRogApp.Tags.Dto
{
    public class CreateUpdateTagDto
    {
        [Required]
        [MaxLength(64)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(256)]
        public string? Description { get; set; }

        [MaxLength(7)]
        public string? ColorCode { get; set; }

        [MaxLength(32)]
        public string? Category { get; set; }
    }
}
