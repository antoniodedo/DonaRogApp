using DonaRogApp.Domain.Donors.Entities;
using DonaRogApp.Enums.Campaigns;
using DonaRogApp.ValueObjects;
using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities;

namespace DonaRogApp.Domain.Campaigns.Entities
{
    /// <summary>
    /// Campaign-Donor many-to-many relationship with tracking metadata
    /// Tracks extraction, dispatch, responses, and donations for each donor in campaign
    /// </summary>
    public class CampaignDonor : Entity
    {
        /// <summary>
        /// Campaign ID (composite key)
        /// </summary>
        public Guid CampaignId { get; private set; }

        /// <summary>
        /// Donor ID (composite key)
        /// </summary>
        public Guid DonorId { get; private set; }

        /// <summary>
        /// Tracking code for this donor in this campaign
        /// </summary>
        public TrackingCode? TrackingCode { get; private set; }

        // ======================================================================
        // TRACKING DATES
        // ======================================================================
        
        /// <summary>
        /// When donor was extracted for this campaign
        /// </summary>
        public DateTime ExtractedAt { get; private set; }

        /// <summary>
        /// When communication was dispatched to donor
        /// </summary>
        public DateTime? DispatchedAt { get; private set; }

        /// <summary>
        /// When donor opened email/communication
        /// </summary>
        public DateTime? OpenedAt { get; private set; }

        /// <summary>
        /// When donor clicked link in email
        /// </summary>
        public DateTime? ClickedAt { get; private set; }

        // ======================================================================
        // RESPONSE
        // ======================================================================
        
        /// <summary>
        /// Response type
        /// </summary>
        public ResponseType ResponseType { get; private set; }

        // ======================================================================
        // DONATION TRACKING (simplified - full Donation entity will be in future)
        // ======================================================================
        
        /// <summary>
        /// Donation amount (if donor donated in response to this campaign)
        /// </summary>
        public decimal? DonationAmount { get; private set; }

        /// <summary>
        /// Donation date
        /// </summary>
        public DateTime? DonationDate { get; private set; }

        // ======================================================================
        // METADATA
        // ======================================================================
        
        /// <summary>
        /// Notes
        /// </summary>
        public string? Notes { get; private set; }

        /// <summary>
        /// Soft delete timestamp
        /// </summary>
        public DateTime? RemovedAt { get; private set; }

        /// <summary>
        /// Navigation properties
        /// </summary>
        public virtual Campaign Campaign { get; private set; }
        public virtual Donor Donor { get; private set; }

        // ======================================================================
        // CONSTRUCTOR
        // ======================================================================
        
        /// <summary>
        /// Private constructor for EF Core
        /// </summary>
        private CampaignDonor()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public CampaignDonor(Guid campaignId, Guid donorId, string? notes = null)
        {
            CampaignId = campaignId;
            DonorId = donorId;
            ExtractedAt = DateTime.UtcNow;
            ResponseType = ResponseType.None;
            Notes = notes;
        }

        /// <summary>
        /// Override GetKeys for composite key
        /// </summary>
        public override object[] GetKeys()
        {
            return new object[] { CampaignId, DonorId };
        }

        // ======================================================================
        // TRACKING METHODS
        // ======================================================================

        /// <summary>
        /// Set tracking code
        /// </summary>
        public void SetTrackingCode(TrackingCode trackingCode)
        {
            if (RemovedAt.HasValue)
            {
                throw new BusinessException("DonaRog:CampaignDonorAlreadyRemoved")
                    .WithData("campaignId", CampaignId)
                    .WithData("donorId", DonorId);
            }

            TrackingCode = trackingCode;
        }

        /// <summary>
        /// Mark as dispatched
        /// </summary>
        public void MarkAsDispatched()
        {
            if (RemovedAt.HasValue)
            {
                throw new BusinessException("DonaRog:CampaignDonorAlreadyRemoved")
                    .WithData("campaignId", CampaignId)
                    .WithData("donorId", DonorId);
            }

            DispatchedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Record email opened
        /// </summary>
        public void RecordOpened()
        {
            if (RemovedAt.HasValue)
            {
                throw new BusinessException("DonaRog:CampaignDonorAlreadyRemoved")
                    .WithData("campaignId", CampaignId)
                    .WithData("donorId", DonorId);
            }

            if (!OpenedAt.HasValue)
            {
                OpenedAt = DateTime.UtcNow;
            }

            if (ResponseType == ResponseType.None)
            {
                ResponseType = ResponseType.Opened;
            }
        }

        /// <summary>
        /// Record link clicked
        /// </summary>
        public void RecordClicked()
        {
            if (RemovedAt.HasValue)
            {
                throw new BusinessException("DonaRog:CampaignDonorAlreadyRemoved")
                    .WithData("campaignId", CampaignId)
                    .WithData("donorId", DonorId);
            }

            if (!ClickedAt.HasValue)
            {
                ClickedAt = DateTime.UtcNow;
            }

            // Auto-record opened if not already
            if (!OpenedAt.HasValue)
            {
                OpenedAt = DateTime.UtcNow;
            }

            if (ResponseType == ResponseType.None || ResponseType == ResponseType.Opened)
            {
                ResponseType = ResponseType.Clicked;
            }
        }

        /// <summary>
        /// Record donation
        /// </summary>
        public void RecordDonation(decimal amount, DateTime? donationDate = null)
        {
            if (RemovedAt.HasValue)
            {
                throw new BusinessException("DonaRog:CampaignDonorAlreadyRemoved")
                    .WithData("campaignId", CampaignId)
                    .WithData("donorId", DonorId);
            }

            if (amount <= 0)
            {
                throw new BusinessException("DonaRog:CampaignDonorInvalidDonationAmount")
                    .WithData("amount", amount);
            }

            DonationAmount = amount;
            DonationDate = donationDate ?? DateTime.UtcNow;
            ResponseType = ResponseType.Donated;
        }

        /// <summary>
        /// Record unsubscribe
        /// </summary>
        public void RecordUnsubscribed()
        {
            if (RemovedAt.HasValue)
            {
                throw new BusinessException("DonaRog:CampaignDonorAlreadyRemoved")
                    .WithData("campaignId", CampaignId)
                    .WithData("donorId", DonorId);
            }

            ResponseType = ResponseType.Unsubscribed;
        }

        /// <summary>
        /// Record bounce
        /// </summary>
        public void RecordBounced()
        {
            if (RemovedAt.HasValue)
            {
                throw new BusinessException("DonaRog:CampaignDonorAlreadyRemoved")
                    .WithData("campaignId", CampaignId)
                    .WithData("donorId", DonorId);
            }

            ResponseType = ResponseType.Bounced;
        }

        /// <summary>
        /// Update notes
        /// </summary>
        public void UpdateNotes(string? notes)
        {
            Notes = notes;
        }

        /// <summary>
        /// Soft delete
        /// </summary>
        public void Remove()
        {
            RemovedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Restore
        /// </summary>
        public void Restore()
        {
            RemovedAt = null;
        }
    }
}
