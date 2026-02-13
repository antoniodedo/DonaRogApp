using DonaRogApp.Enums.Campaigns;
using System;
using Volo.Abp.Application.Dtos;

namespace DonaRogApp.Application.Contracts.Campaigns.Dto
{
    /// <summary>
    /// Input DTO for querying campaigns
    /// </summary>
    public class GetCampaignsInput : PagedAndSortedResultRequestDto
    {
        /// <summary>
        /// General search filter (name, code, description)
        /// </summary>
        public string? Filter { get; set; }

        /// <summary>
        /// Filter by year
        /// </summary>
        public int? Year { get; set; }

        /// <summary>
        /// Filter by status
        /// </summary>
        public CampaignStatus? Status { get; set; }

        /// <summary>
        /// Filter by campaign type
        /// </summary>
        public CampaignType? CampaignType { get; set; }

        /// <summary>
        /// Filter by channel
        /// </summary>
        public CampaignChannel? Channel { get; set; }

        /// <summary>
        /// Filter by associated event
        /// </summary>
        public Guid? RecurrenceId { get; set; }

        /// <summary>
        /// Filter campaigns from this date
        /// </summary>
        public DateTime? DateFrom { get; set; }

        /// <summary>
        /// Filter campaigns to this date
        /// </summary>
        public DateTime? DateTo { get; set; }
    }
}
