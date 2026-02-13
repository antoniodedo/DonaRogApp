using DonaRogApp.Enums.Donations;
using System;
using System.Collections.Generic;

namespace DonaRogApp.Domain.Donations.Events
{
    // ======================================================================
    // DONATION LIFECYCLE EVENTS
    // ======================================================================

    /// <summary>
    /// Event raised when a new donation is created
    /// </summary>
    public class DonationCreatedEvent
    {
        public Guid DonationId { get; set; }
        public Guid DonorId { get; set; }
        public DonationChannel Channel { get; set; }
        public DonationStatus Status { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime DonationDate { get; set; }
        public Guid? CampaignId { get; set; }

        public DonationCreatedEvent(
            Guid donationId,
            Guid donorId,
            DonationChannel channel,
            DonationStatus status,
            decimal totalAmount,
            DateTime donationDate,
            Guid? campaignId = null)
        {
            DonationId = donationId;
            DonorId = donorId;
            Channel = channel;
            Status = status;
            TotalAmount = totalAmount;
            DonationDate = donationDate;
            CampaignId = campaignId;
        }
    }

    /// <summary>
    /// Event raised when a donation is verified
    /// IMPORTANT: This event triggers donor and project statistics updates
    /// </summary>
    public class DonationVerifiedEvent
    {
        public Guid DonationId { get; set; }
        public Guid DonorId { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime DonationDate { get; set; }
        public Guid? CampaignId { get; set; }
        public Guid VerifiedBy { get; set; }
        public DateTime VerifiedAt { get; set; }
        
        /// <summary>
        /// Project allocations (ProjectId, AllocatedAmount)
        /// </summary>
        public List<(Guid ProjectId, decimal Amount)> ProjectAllocations { get; set; }

        public DonationVerifiedEvent(
            Guid donationId,
            Guid donorId,
            decimal totalAmount,
            DateTime donationDate,
            Guid verifiedBy,
            DateTime verifiedAt,
            Guid? campaignId = null,
            List<(Guid ProjectId, decimal Amount)>? projectAllocations = null)
        {
            DonationId = donationId;
            DonorId = donorId;
            TotalAmount = totalAmount;
            DonationDate = donationDate;
            VerifiedBy = verifiedBy;
            VerifiedAt = verifiedAt;
            CampaignId = campaignId;
            ProjectAllocations = projectAllocations ?? new List<(Guid, decimal)>();
        }
    }

    /// <summary>
    /// Event raised when a donation is rejected
    /// </summary>
    public class DonationRejectedEvent
    {
        public Guid DonationId { get; set; }
        public Guid DonorId { get; set; }
        public RejectionReason Reason { get; set; }
        public string? Notes { get; set; }
        public Guid RejectedBy { get; set; }
        public DateTime RejectedAt { get; set; }

        public DonationRejectedEvent(
            Guid donationId,
            Guid donorId,
            RejectionReason reason,
            Guid rejectedBy,
            DateTime rejectedAt,
            string? notes = null)
        {
            DonationId = donationId;
            DonorId = donorId;
            Reason = reason;
            RejectedBy = rejectedBy;
            RejectedAt = rejectedAt;
            Notes = notes;
        }
    }

    // ======================================================================
    // PROJECT ALLOCATION EVENTS
    // ======================================================================

    /// <summary>
    /// Event raised when a donation is allocated to a project
    /// </summary>
    public class DonationProjectAllocatedEvent
    {
        public Guid DonationId { get; set; }
        public Guid ProjectId { get; set; }
        public decimal AllocatedAmount { get; set; }
        public DonationStatus DonationStatus { get; set; }

        public DonationProjectAllocatedEvent(
            Guid donationId,
            Guid projectId,
            decimal allocatedAmount,
            DonationStatus donationStatus)
        {
            DonationId = donationId;
            ProjectId = projectId;
            AllocatedAmount = allocatedAmount;
            DonationStatus = donationStatus;
        }
    }

    /// <summary>
    /// Event raised when a project allocation is removed from a donation
    /// </summary>
    public class DonationProjectAllocationRemovedEvent
    {
        public Guid DonationId { get; set; }
        public Guid ProjectId { get; set; }
        public decimal RemovedAmount { get; set; }
        public DonationStatus DonationStatus { get; set; }

        public DonationProjectAllocationRemovedEvent(
            Guid donationId,
            Guid projectId,
            decimal removedAmount,
            DonationStatus donationStatus)
        {
            DonationId = donationId;
            ProjectId = projectId;
            RemovedAmount = removedAmount;
            DonationStatus = donationStatus;
        }
    }

    /// <summary>
    /// Event raised when a project allocation amount is updated
    /// </summary>
    public class DonationProjectAllocationUpdatedEvent
    {
        public Guid DonationId { get; set; }
        public Guid ProjectId { get; set; }
        public decimal OldAmount { get; set; }
        public decimal NewAmount { get; set; }
        public DonationStatus DonationStatus { get; set; }

        public DonationProjectAllocationUpdatedEvent(
            Guid donationId,
            Guid projectId,
            decimal oldAmount,
            decimal newAmount,
            DonationStatus donationStatus)
        {
            DonationId = donationId;
            ProjectId = projectId;
            OldAmount = oldAmount;
            NewAmount = newAmount;
            DonationStatus = donationStatus;
        }
    }
}
