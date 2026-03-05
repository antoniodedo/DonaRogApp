using System;

namespace DonaRogApp.Application.Contracts.Segmentation.Dto
{
    /// <summary>
    /// Result of batch segmentation processing
    /// </summary>
    public class SegmentationBatchResultDto
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int DonorsProcessed { get; set; }
        public int AssignmentsCreated { get; set; }
        public int AssignmentsRemoved { get; set; }
        public int Errors { get; set; }
        public double DurationSeconds { get; set; }
        public bool Success { get; set; }
        public string? Message { get; set; }
    }
}
