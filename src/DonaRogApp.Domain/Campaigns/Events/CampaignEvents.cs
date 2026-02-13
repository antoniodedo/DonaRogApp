using DonaRogApp.Enums.Campaigns;
using System;

namespace DonaRogApp.Domain.Campaigns.Events
{
    // ======================================================================
    // CAMPAIGN BASE EVENTS
    // ======================================================================

    /// <summary>
    /// Event raised when a campaign is created
    /// </summary>
    public class CampaignCreatedEvent
    {
        public Guid CampaignId { get; set; }
        public string Name { get; set; }
        public int Year { get; set; }
        public string Code { get; set; }
        public CampaignType CampaignType { get; set; }
        public CampaignChannel Channel { get; set; }

        public CampaignCreatedEvent(
            Guid campaignId,
            string name,
            int year,
            string code,
            CampaignType campaignType,
            CampaignChannel channel)
        {
            CampaignId = campaignId;
            Name = name;
            Year = year;
            Code = code;
            CampaignType = campaignType;
            Channel = channel;
        }
    }

    /// <summary>
    /// Event raised when campaign status changes
    /// </summary>
    public class CampaignStatusChangedEvent
    {
        public Guid CampaignId { get; set; }
        public CampaignStatus OldStatus { get; set; }
        public CampaignStatus NewStatus { get; set; }

        public CampaignStatusChangedEvent(
            Guid campaignId,
            CampaignStatus oldStatus,
            CampaignStatus newStatus)
        {
            CampaignId = campaignId;
            OldStatus = oldStatus;
            NewStatus = newStatus;
        }
    }

    // ======================================================================
    // DONOR EXTRACTION EVENTS
    // ======================================================================

    /// <summary>
    /// Event raised when donors are extracted for a campaign
    /// </summary>
    public class CampaignDonorsExtractedEvent
    {
        public Guid CampaignId { get; set; }
        public int DonorCount { get; set; }
        public DateTime ExtractionDate { get; set; }

        public CampaignDonorsExtractedEvent(Guid campaignId, int donorCount, DateTime extractionDate)
        {
            CampaignId = campaignId;
            DonorCount = donorCount;
            ExtractionDate = extractionDate;
        }
    }

    /// <summary>
    /// Event raised when campaign is dispatched
    /// </summary>
    public class CampaignDispatchedEvent
    {
        public Guid CampaignId { get; set; }
        public int DispatchedCount { get; set; }
        public DateTime DispatchDate { get; set; }
        public CampaignChannel Channel { get; set; }

        public CampaignDispatchedEvent(
            Guid campaignId,
            int dispatchedCount,
            DateTime dispatchDate,
            CampaignChannel channel)
        {
            CampaignId = campaignId;
            DispatchedCount = dispatchedCount;
            DispatchDate = dispatchDate;
            Channel = channel;
        }
    }

    /// <summary>
    /// Event raised when campaign is completed
    /// </summary>
    public class CampaignCompletedEvent
    {
        public Guid CampaignId { get; set; }
        public int ExtractedDonorCount { get; set; }
        public int ResponseCount { get; set; }
        public decimal ResponseRate { get; set; }
        public decimal TotalRaised { get; set; }
        public decimal ROI { get; set; }

        public CampaignCompletedEvent(
            Guid campaignId,
            int extractedDonorCount,
            int responseCount,
            decimal responseRate,
            decimal totalRaised,
            decimal roi)
        {
            CampaignId = campaignId;
            ExtractedDonorCount = extractedDonorCount;
            ResponseCount = responseCount;
            ResponseRate = responseRate;
            TotalRaised = totalRaised;
            ROI = roi;
        }
    }

    // ======================================================================
    // RESPONSE TRACKING EVENTS
    // ======================================================================

    /// <summary>
    /// Event raised when a donor response is recorded
    /// </summary>
    public class CampaignResponseReceivedEvent
    {
        public Guid CampaignId { get; set; }
        public Guid DonorId { get; set; }
        public ResponseType ResponseType { get; set; }
        public DateTime ResponseDate { get; set; }

        public CampaignResponseReceivedEvent(
            Guid campaignId,
            Guid donorId,
            ResponseType responseType,
            DateTime responseDate)
        {
            CampaignId = campaignId;
            DonorId = donorId;
            ResponseType = responseType;
            ResponseDate = responseDate;
        }
    }

    /// <summary>
    /// Event raised when a donation is recorded for a campaign
    /// </summary>
    public class CampaignDonationReceivedEvent
    {
        public Guid CampaignId { get; set; }
        public Guid DonorId { get; set; }
        public decimal Amount { get; set; }
        public DateTime DonationDate { get; set; }

        public CampaignDonationReceivedEvent(
            Guid campaignId,
            Guid donorId,
            decimal amount,
            DateTime donationDate)
        {
            CampaignId = campaignId;
            DonorId = donorId;
            Amount = amount;
            DonationDate = donationDate;
        }
    }

    // ======================================================================
    // TRACKING CODE EVENTS
    // ======================================================================

    /// <summary>
    /// Event raised when postal codes are generated
    /// </summary>
    public class CampaignPostalCodesGeneratedEvent
    {
        public Guid CampaignId { get; set; }
        public string PostalCode { get; set; }
        public DateTime GeneratedDate { get; set; }

        public CampaignPostalCodesGeneratedEvent(Guid campaignId, string postalCode, DateTime generatedDate)
        {
            CampaignId = campaignId;
            PostalCode = postalCode;
            GeneratedDate = generatedDate;
        }
    }

    /// <summary>
    /// Event raised when email tracking codes are generated
    /// </summary>
    public class CampaignEmailTrackingCodesGeneratedEvent
    {
        public Guid CampaignId { get; set; }
        public int DonorCount { get; set; }
        public DateTime GeneratedDate { get; set; }

        public CampaignEmailTrackingCodesGeneratedEvent(Guid campaignId, int donorCount, DateTime generatedDate)
        {
            CampaignId = campaignId;
            DonorCount = donorCount;
            GeneratedDate = generatedDate;
        }
    }

    // ======================================================================
    // STATISTICS EVENTS
    // ======================================================================

    /// <summary>
    /// Event raised when campaign statistics are updated
    /// </summary>
    public class CampaignStatisticsUpdatedEvent
    {
        public Guid CampaignId { get; set; }
        public int ExtractedDonorCount { get; set; }
        public int DispatchedCount { get; set; }
        public int ResponseCount { get; set; }
        public decimal ResponseRate { get; set; }
        public int DonationCount { get; set; }
        public decimal TotalRaised { get; set; }
        public decimal ConversionRate { get; set; }
        public decimal ROI { get; set; }

        public CampaignStatisticsUpdatedEvent(
            Guid campaignId,
            int extractedDonorCount,
            int dispatchedCount,
            int responseCount,
            decimal responseRate,
            int donationCount,
            decimal totalRaised,
            decimal conversionRate,
            decimal roi)
        {
            CampaignId = campaignId;
            ExtractedDonorCount = extractedDonorCount;
            DispatchedCount = dispatchedCount;
            ResponseCount = responseCount;
            ResponseRate = responseRate;
            DonationCount = donationCount;
            TotalRaised = totalRaised;
            ConversionRate = conversionRate;
            ROI = roi;
        }
    }
}
