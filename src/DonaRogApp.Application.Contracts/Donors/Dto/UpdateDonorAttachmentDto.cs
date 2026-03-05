using System.ComponentModel.DataAnnotations;

namespace DonaRogApp.Application.Contracts.Donors.Dto
{
    public class UpdateDonorAttachmentDto
    {
        [MaxLength(50)]
        public string? AttachmentType { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        public int DisplayOrder { get; set; }
    }
}
