using System;
using Volo.Abp.Application.Dtos;

namespace DonaRogApp.Donors.Dtos
{
    public class DonorStatusHistoryDto : EntityDto<Guid>
    {
        public Guid DonorId { get; set; }
        public int OldStatus { get; set; }
        public int NewStatus { get; set; }
        public string? Note { get; set; }
        public DateTime ChangedAt { get; set; }
        public DateTime CreationTime { get; set; }
        public Guid? CreatorId { get; set; }
    }
}
