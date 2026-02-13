using DonaRogApp.Enums.Campaigns;
using System;

namespace DonaRogApp.Domain.Campaigns.Entities
{
    public partial class Campaign
    {
        /// <summary>
        /// Create a new campaign
        /// </summary>
        public static Campaign Create(
            Guid id,
            Guid? tenantId,
            string name,
            int year,
            string code,
            CampaignType campaignType,
            CampaignChannel channel,
            string? description = null,
            Guid? recurrenceId = null)
        {
            var campaign = new Campaign(id, tenantId, name, year, code, campaignType, channel, recurrenceId)
            {
                Description = description
            };

            return campaign;
        }

        /// <summary>
        /// Create a prospect campaign
        /// </summary>
        public static Campaign CreateProspect(
            Guid id,
            Guid? tenantId,
            string name,
            int year,
            string code,
            CampaignChannel channel,
            string? description = null)
        {
            return Create(id, tenantId, name, year, code, CampaignType.Prospect, channel, description);
        }

        /// <summary>
        /// Create an archive campaign
        /// </summary>
        public static Campaign CreateArchive(
            Guid id,
            Guid? tenantId,
            string name,
            int year,
            string code,
            CampaignChannel channel,
            string? description = null)
        {
            return Create(id, tenantId, name, year, code, CampaignType.Archive, channel, description);
        }

        /// <summary>
        /// Create an event-based campaign
        /// </summary>
        public static Campaign CreateForEvent(
            Guid id,
            Guid? tenantId,
            string name,
            int year,
            string code,
            CampaignType campaignType,
            CampaignChannel channel,
            Guid recurrenceId,
            string? description = null)
        {
            return Create(id, tenantId, name, year, code, campaignType, channel, description, recurrenceId);
        }
    }
}
