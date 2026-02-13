using DonaRogApp.ValueObjects;
using System;
using Volo.Abp;

namespace DonaRogApp.Domain.Campaigns.Entities
{
    public partial class Campaign
    {
        /// <summary>
        /// Generate postal code 674 (format: NNNNYY)
        /// </summary>
        public void GeneratePostalCode(int sequenceNumber)
        {
            if (!IsPostal())
            {
                throw new BusinessException("DonaRog:CampaignNotPostal")
                    .WithData("campaignId", Id)
                    .WithData("channel", Channel);
            }

            YearlySequenceNumber = sequenceNumber;
            PostalCode = new PostalCode674(sequenceNumber, Year);
        }

        /// <summary>
        /// Set Mailchimp campaign ID
        /// </summary>
        public void SetMailchimpCampaignId(string campaignId)
        {
            if (!IsEmail())
            {
                throw new BusinessException("DonaRog:CampaignNotEmail")
                    .WithData("campaignId", Id)
                    .WithData("channel", Channel);
            }

            MailchimpCampaignId = Check.NotNullOrWhiteSpace(campaignId, nameof(campaignId));
        }

        /// <summary>
        /// Set Mailchimp list ID
        /// </summary>
        public void SetMailchimpListId(string listId)
        {
            if (!IsEmail())
            {
                throw new BusinessException("DonaRog:CampaignNotEmail")
                    .WithData("campaignId", Id)
                    .WithData("channel", Channel);
            }

            MailchimpListId = Check.NotNullOrWhiteSpace(listId, nameof(listId));
        }

        /// <summary>
        /// Set SMS provider ID
        /// </summary>
        public void SetSmsProviderId(string providerId)
        {
            if (!IsSMS())
            {
                throw new BusinessException("DonaRog:CampaignNotSMS")
                    .WithData("campaignId", Id)
                    .WithData("channel", Channel);
            }

            SmsProviderId = Check.NotNullOrWhiteSpace(providerId, nameof(providerId));
        }

        /// <summary>
        /// Generate tracking code for a donor
        /// </summary>
        public TrackingCode GenerateTrackingCode(Guid donorId)
        {
            return new TrackingCode(Id, donorId);
        }
    }
}
