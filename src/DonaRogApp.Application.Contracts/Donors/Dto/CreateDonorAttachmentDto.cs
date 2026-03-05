using System;
using System.ComponentModel.DataAnnotations;

namespace DonaRogApp.Application.Contracts.Donors.Dto
{
    public class CreateDonorAttachmentDto
    {
        [Required]
        public Guid DonorId { get; set; }

        [Required]
        [MaxLength(255)]
        public string FileName { get; set; } = null!;

        [MaxLength(50)]
        public string? AttachmentType { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        public int DisplayOrder { get; set; }
    }
}
