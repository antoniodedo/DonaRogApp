// --------------------------------------------------------------
// Domain/Donors/Entities/Communication.cs
// --------------------------------------------------------------

using DonaRogApp.Enums.Communications;
using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Timing;

namespace DonaRogApp.Domain.Donors.Entities
{
    /// <summary>
    /// Child Entity: Communication
    /// Represents a communication sent to a donor.
    /// Tracks emails, letters, SMS, etc. sent to the donor.
    /// </summary>
    public class Communication : FullAuditedEntity<Guid>, IMultiTenant
    {
        // --------------------------------------------------------------
        // MULTI-TENANCY
        // --------------------------------------------------------------

        /// <summary>
        /// Tenant ID (inherited from parent Donor)
        /// </summary>
        public Guid? TenantId { get; private set; }

        // --------------------------------------------------------------
        // PARENT RELATIONSHIP
        // --------------------------------------------------------------

        /// <summary>
        /// Parent Donor ID
        /// </summary>
        public Guid DonorId { get; private set; }

        /// <summary>
        /// Parent Donor (navigation property)
        /// </summary>
        public virtual Donor Donor { get; private set; }

        // --------------------------------------------------------------
        // COMMUNICATION PROPERTIES
        // --------------------------------------------------------------

        /// <summary>
        /// Type of communication (Email, Letter, SMS, etc.)
        /// </summary>
        public CommunicationType Type { get; private set; }

        /// <summary>
        /// Subject/title of the communication
        /// </summary>
        public string Subject { get; private set; }

        /// <summary>
        /// Category of communication (ThankYou, Newsletter, etc.)
        /// </summary>
        public TemplateCategory? Category { get; private set; }

        /// <summary>
        /// Template ID used (if any)
        /// </summary>
        public Guid? TemplateId { get; private set; }

        /// <summary>
        /// Donation ID related to this communication (if any)
        /// Example: Thank you letter for specific donation
        /// </summary>
        public Guid? DonationId { get; private set; }

        /// <summary>
        /// Campaign ID related to this communication (if any)
        /// </summary>
        public Guid? CampaignId { get; private set; }

        // --------------------------------------------------------------
        // DELIVERY TRACKING
        // --------------------------------------------------------------

        /// <summary>
        /// Date communication was sent
        /// </summary>
        public DateTime SentDate { get; private set; }

        /// <summary>
        /// Was communication successfully delivered?
        /// </summary>
        public bool IsDelivered { get; private set; }

        /// <summary>
        /// Date communication was delivered
        /// </summary>
        public DateTime? DeliveredDate { get; private set; }

        /// <summary>
        /// Was communication opened/read? (for emails)
        /// </summary>
        public bool IsOpened { get; private set; }

        /// <summary>
        /// Date communication was opened
        /// </summary>
        public DateTime? OpenedDate { get; private set; }

        /// <summary>
        /// Number of times opened
        /// </summary>
        public int OpenCount { get; private set; }

        /// <summary>
        /// Was communication clicked? (for emails with links)
        /// </summary>
        public bool IsClicked { get; private set; }

        /// <summary>
        /// Date of first click
        /// </summary>
        public DateTime? ClickedDate { get; private set; }

        /// <summary>
        /// Number of clicks
        /// </summary>
        public int ClickCount { get; private set; }

        // --------------------------------------------------------------
        // FAILURE TRACKING
        // --------------------------------------------------------------

        /// <summary>
        /// Did communication fail to deliver?
        /// </summary>
        public bool IsFailed { get; private set; }

        /// <summary>
        /// Failure reason
        /// </summary>
        public string? FailureReason { get; private set; }

        /// <summary>
        /// Date of failure
        /// </summary>
        public DateTime? FailureDate { get; private set; }

        // --------------------------------------------------------------
        // CONTENT
        // --------------------------------------------------------------

        /// <summary>
        /// Recipient email/phone/address (depends on type)
        /// </summary>
        public string Recipient { get; private set; }

        /// <summary>
        /// Body/content of the communication (optional storage)
        /// May be null if content is stored elsewhere
        /// </summary>
        public string? Body { get; private set; }

        /// <summary>
        /// External ID (from email service, postal service, etc.)
        /// </summary>
        public string? ExternalId { get; private set; }

        // --------------------------------------------------------------
        // TRACKING
        // --------------------------------------------------------------

        /// <summary>
        /// User who sent this communication
        /// </summary>
        public Guid? SentByUserId { get; private set; }

        /// <summary>
        /// Notes about this communication
        /// </summary>
        public string? Notes { get; private set; }

        // --------------------------------------------------------------
        // CONSTRUCTOR
        // --------------------------------------------------------------

        /// <summary>
        /// Protected constructor for EF Core
        /// Use factory method Create() to instantiate
        /// </summary>
        protected Communication()
        {
            Subject = string.Empty;
            Recipient = string.Empty;
            SentDate = DateTime.UtcNow;
            IsDelivered = false;
            IsOpened = false;
            IsClicked = false;
            IsFailed = false;
            OpenCount = 0;
            ClickCount = 0;
        }

        // --------------------------------------------------------------
        // FACTORY METHOD
        // --------------------------------------------------------------

        /// <summary>
        /// Creates new Communication entity
        /// </summary>
        internal static Communication Create(
            Guid donorId,
            CommunicationType type,
            string subject,
            string recipient,
            Guid? tenantId,
            TemplateCategory? category = null,
            Guid? templateId = null,
            Guid? donationId = null,
            Guid? campaignId = null,
            string? body = null,
            Guid? sentByUserId = null,
            string? notes = null)
        {
            return new Communication
            {
                Id = Guid.NewGuid(),
                DonorId = donorId,
                Type = type,
                Subject = subject.Trim(),
                Recipient = recipient.Trim(),
                Category = category,
                TemplateId = templateId,
                DonationId = donationId,
                CampaignId = campaignId,
                Body = body,
                SentDate = DateTime.UtcNow,
                TenantId = tenantId,
                SentByUserId = sentByUserId,
                Notes = notes,
                IsDelivered = false,
                IsOpened = false,
                IsClicked = false,
                IsFailed = false,
                OpenCount = 0,
                ClickCount = 0
            };
        }

        // --------------------------------------------------------------
        // BUSINESS METHODS - Delivery Tracking
        // --------------------------------------------------------------

        /// <summary>
        /// Marks communication as delivered
        /// </summary>
        internal void MarkAsDelivered(string? externalId = null)
        {
            IsDelivered = true;
            DeliveredDate = DateTime.UtcNow;
            ExternalId = externalId;
            IsFailed = false;
        }

        /// <summary>
        /// Marks communication as failed
        /// </summary>
        internal void MarkAsFailed(string? reason = null)
        {
            IsFailed = true;
            FailureReason = reason;
            FailureDate = DateTime.UtcNow;
            IsDelivered = false;
        }

        /// <summary>
        /// Records email open
        /// </summary>
        internal void RecordOpen()
        {
            if (!IsOpened)
            {
                IsOpened = true;
                OpenedDate = DateTime.UtcNow;
            }

            OpenCount++;
        }

        /// <summary>
        /// Records link click
        /// </summary>
        internal void RecordClick()
        {
            if (!IsClicked)
            {
                IsClicked = true;
                ClickedDate = DateTime.UtcNow;
            }

            ClickCount++;
        }

        // --------------------------------------------------------------
        // BUSINESS METHODS - Update
        // --------------------------------------------------------------

        /// <summary>
        /// Updates external ID
        /// </summary>
        internal void UpdateExternalId(string externalId)
        {
            ExternalId = externalId;
        }

        /// <summary>
        /// Updates notes
        /// </summary>
        internal void UpdateNotes(string? notes)
        {
            Notes = notes;
        }

        // --------------------------------------------------------------
        // BUSINESS METHODS - Soft Delete
        // --------------------------------------------------------------

        /// <summary>
        /// Soft deletes communication
        /// </summary>
        internal void Delete()
        {
            IsDeleted = true;
            DeletionTime = DateTime.UtcNow;
        }
    }
}