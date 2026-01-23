using DonaRogApp.Enums.Donors;
using DonaRogApp.Enums.Shared;
using System;
using Volo.Abp.EventBus.Distributed;


namespace DonaRogApp.Domain.Donors.Events
{
    // ======================================================================
    // DONOR BASE EVENTS
    // ======================================================================

    /// <summary>
    /// Event lanciato quando le informazioni base del donatore vengono aggiornate
    /// (Nome, Cognome, Titolo, Data di nascita, etc.)
    /// </summary>
    public class DonorInfoUpdatedEvent
    {
        public Guid DonorId { get; set; }

        public DonorInfoUpdatedEvent(Guid donorId)
        {
            DonorId = donorId;
        }
    }

    /// <summary>
    /// Event lanciato quando lo stato del donatore cambia
    /// </summary>
    public class DonorStatusChangedEvent
    {
        public Guid DonorId { get; set; }
        public DonorStatus OldStatus { get; set; }
        public DonorStatus NewStatus { get; set; }
        public string? Reason { get; set; }

        public DonorStatusChangedEvent(
            Guid donorId,
            DonorStatus oldStatus,
            DonorStatus newStatus,
            string? reason = null)
        {
            DonorId = donorId;
            OldStatus = oldStatus;
            NewStatus = newStatus;
            Reason = reason;
        }
    }

    // ======================================================================
    // EMAIL EVENTS
    // ======================================================================

    /// <summary>
    /// Event lanciato quando un email viene aggiunto
    /// </summary>
    public class DonorEmailAddedEvent
    {
        public Guid DonorId { get; set; }
        public string Email { get; set; }
        public EmailType EmailType { get; set; }

        public DonorEmailAddedEvent(Guid donorId, string email, EmailType emailType)
        {
            DonorId = donorId;
            Email = email;
            EmailType = emailType;
        }
    }

    /// <summary>
    /// Event lanciato quando un email viene rimosso
    /// </summary>
    public class DonorEmailRemovedEvent
    {
        public Guid DonorId { get; set; }
        public string Email { get; set; }

        public DonorEmailRemovedEvent(Guid donorId, string email)
        {
            DonorId = donorId;
            Email = email;
        }
    }

    /// <summary>
    /// Event lanciato quando un email viene verificato
    /// </summary>
    public class DonorEmailVerifiedEvent
    {
        public Guid DonorId { get; set; }
        public string Email { get; set; }

        public DonorEmailVerifiedEvent(Guid donorId, string email)
        {
            DonorId = donorId;
            Email = email;
        }
    }

    /// <summary>
    /// Event lanciato quando un email rimbalza
    /// </summary>
    public class DonorEmailBouncedEvent
    {
        public Guid DonorId { get; set; }
        public string Email { get; set; }
        public string BounceType { get; set; }

        public DonorEmailBouncedEvent(Guid donorId, string email, string bounceType)
        {
            DonorId = donorId;
            Email = email;
            BounceType = bounceType;
        }
    }

    // ======================================================================
    // CONTACT EVENTS
    // ======================================================================

    /// <summary>
    /// Event lanciato quando un contatto viene aggiunto
    /// </summary>
    public class DonorContactAddedEvent
    {
        public Guid DonorId { get; set; }
        public string PhoneNumber { get; set; }
        public ContactType ContactType { get; set; }

        public DonorContactAddedEvent(Guid donorId, string phoneNumber, ContactType contactType)
        {
            DonorId = donorId;
            PhoneNumber = phoneNumber;
            ContactType = contactType;
        }
    }

    /// <summary>
    /// Event lanciato quando un contatto viene rimosso
    /// </summary>
    public class DonorContactRemovedEvent
    {
        public Guid DonorId { get; set; }
        public string PhoneNumber { get; set; }

        public DonorContactRemovedEvent(Guid donorId, string phoneNumber)
        {
            DonorId = donorId;
            PhoneNumber = phoneNumber;
        }
    }

    /// <summary>
    /// Event lanciato quando un contatto viene verificato
    /// </summary>
    public class DonorContactVerifiedEvent
    {
        public Guid DonorId { get; set; }
        public string PhoneNumber { get; set; }

        public DonorContactVerifiedEvent(Guid donorId, string phoneNumber)
        {
            DonorId = donorId;
            PhoneNumber = phoneNumber;
        }
    }

    // ======================================================================
    // ADDRESS EVENTS
    // ======================================================================

    /// <summary>
    /// Event lanciato quando un indirizzo viene aggiunto
    /// </summary>
    public class DonorAddressAddedEvent
    {
        public Guid DonorId { get; set; }
        public string City { get; set; }
        public AddressType AddressType { get; set; }

        public DonorAddressAddedEvent(Guid donorId, string city, AddressType addressType)
        {
            DonorId = donorId;
            City = city;
            AddressType = addressType;
        }
    }

    /// <summary>
    /// Event lanciato quando un indirizzo viene terminato
    /// </summary>
    public class DonorAddressEndedEvent
    {
        public Guid DonorId { get; set; }
        public Guid AddressId { get; set; }

        public DonorAddressEndedEvent(Guid donorId, Guid addressId)
        {
            DonorId = donorId;
            AddressId = addressId;
        }
    }

    // ======================================================================
    // PRIVACY EVENTS
    // ======================================================================

    /// <summary>
    /// Event lanciato quando il consenso privacy viene concesso
    /// </summary>
    public class DonorPrivacyConsentGrantedEvent
    {
        public Guid DonorId { get; set; }

        public DonorPrivacyConsentGrantedEvent(Guid donorId)
        {
            DonorId = donorId;
        }
    }

    /// <summary>
    /// Event lanciato quando il consenso privacy viene revocato
    /// </summary>
    public class DonorPrivacyConsentRevokedEvent
    {
        public Guid DonorId { get; set; }

        public DonorPrivacyConsentRevokedEvent(Guid donorId)
        {
            DonorId = donorId;
        }
    }

    /// <summary>
    /// Event lanciato quando il consenso newsletter viene concesso
    /// </summary>
    public class DonorNewsletterConsentGrantedEvent
    {
        public Guid DonorId { get; set; }

        public DonorNewsletterConsentGrantedEvent(Guid donorId)
        {
            DonorId = donorId;
        }
    }

    /// <summary>
    /// Event lanciato quando il consenso newsletter viene revocato
    /// </summary>
    public class DonorNewsletterConsentRevokedEvent
    {
        public Guid DonorId { get; set; }

        public DonorNewsletterConsentRevokedEvent(Guid donorId)
        {
            DonorId = donorId;
        }
    }

    /// <summary>
    /// Event lanciato quando il donatore viene anonimizzato (GDPR)
    /// </summary>
    public class DonorAnonymizedEvent
    {
        public Guid DonorId { get; set; }

        public DonorAnonymizedEvent(Guid donorId)
        {
            DonorId = donorId;
        }
    }

    // ======================================================================
    // STATISTICS EVENTS
    // ======================================================================

    /// <summary>
    /// Event lanciato quando le statistiche del donatore vengono aggiornate
    /// </summary>
    public class DonorStatisticsUpdatedEvent
    {
        public Guid DonorId { get; set; }
        public decimal TotalDonated { get; set; }
        public int DonationCount { get; set; }
        public decimal AverageDonationAmount { get; set; }

        public DonorStatisticsUpdatedEvent(
            Guid donorId,
            decimal totalDonated,
            int donationCount,
            decimal averageDonationAmount)
        {
            DonorId = donorId;
            TotalDonated = totalDonated;
            DonationCount = donationCount;
            AverageDonationAmount = averageDonationAmount;
        }
    }

    /// <summary>
    /// Event lanciato quando la categoria del donatore cambia
    /// </summary>
    public class DonorCategoryChangedEvent
    {
        public Guid DonorId { get; set; }
        public DonorCategory OldCategory { get; set; }
        public DonorCategory NewCategory { get; set; }

        public DonorCategoryChangedEvent(
            Guid donorId,
            DonorCategory oldCategory,
            DonorCategory newCategory)
        {
            DonorId = donorId;
            OldCategory = oldCategory;
            NewCategory = newCategory;
        }
    }

    /// <summary>
    /// Event lanciato quando gli RFM scores vengono ricalcolati
    /// </summary>
    public class DonorRfmRecalculatedEvent
    {
        public Guid DonorId { get; set; }
        public int RecencyScore { get; set; }
        public int FrequencyScore { get; set; }
        public int MonetaryScore { get; set; }
        public string? RfmSegment { get; set; }

        public DonorRfmRecalculatedEvent(
            Guid donorId,
            int recencyScore,
            int frequencyScore,
            int monetaryScore,
            string? rfmSegment = null)
        {
            DonorId = donorId;
            RecencyScore = recencyScore;
            FrequencyScore = frequencyScore;
            MonetaryScore = monetaryScore;
            RfmSegment = rfmSegment;
        }
    }

    // ======================================================================
    // COMMUNICATION EVENTS
    // ======================================================================

    /// <summary>
    /// Event lanciato quando un'email viene inviata
    /// </summary>
    public class DonorEmailSentEvent
    {
        public Guid DonorId { get; set; }
        public string Subject { get; set; }
        public DateTime SentAt { get; set; }

        public DonorEmailSentEvent(Guid donorId, string subject, DateTime sentAt)
        {
            DonorId = donorId;
            Subject = subject;
            SentAt = sentAt;
        }
    }

    /// <summary>
    /// Event lanciato quando una lettera/bullettino postale viene inviato
    /// </summary>
    public class DonorLetterSentEvent
    {
        public Guid DonorId { get; set; }
        public string Subject { get; set; }
        public DateTime SentAt { get; set; }

        public DonorLetterSentEvent(Guid donorId, string subject, DateTime sentAt)
        {
            DonorId = donorId;
            Subject = subject;
            SentAt = sentAt;
        }
    }

    /// <summary>
    /// Event lanciato quando una comunicazione viene consegnata
    /// </summary>
    public class DonorCommunicationDeliveredEvent
    {
        public Guid DonorId { get; set; }
        public Guid CommunicationId { get; set; }
        public DateTime DeliveredAt { get; set; }

        public DonorCommunicationDeliveredEvent(Guid donorId, Guid communicationId, DateTime deliveredAt)
        {
            DonorId = donorId;
            CommunicationId = communicationId;
            DeliveredAt = deliveredAt;
        }
    }

    /// <summary>
    /// Event lanciato quando un'email viene aperta
    /// </summary>
    public class DonorEmailOpenedEvent
    {
        public Guid DonorId { get; set; }
        public Guid CommunicationId { get; set; }

        public DonorEmailOpenedEvent(Guid donorId, Guid communicationId)
        {
            DonorId = donorId;
            CommunicationId = communicationId;
        }
    }

    /// <summary>
    /// Event lanciato quando un link in un'email viene cliccato
    /// </summary>
    public class DonorEmailClickedEvent
    {
        public Guid DonorId { get; set; }
        public Guid CommunicationId { get; set; }

        public DonorEmailClickedEvent(Guid donorId, Guid communicationId)
        {
            DonorId = donorId;
            CommunicationId = communicationId;
        }
    }

    /// <summary>
    /// Event lanciato quando una comunicazione rimbalza
    /// </summary>
    public class DonorCommunicationBouncedEvent
    {
        public Guid DonorId { get; set; }
        public Guid CommunicationId { get; set; }

        public DonorCommunicationBouncedEvent(Guid donorId, Guid communicationId)
        {
            DonorId = donorId;
            CommunicationId = communicationId;
        }
    }

    // ======================================================================
    // SEGMENT & TAGS EVENTS
    // ======================================================================

    /// <summary>
    /// Event lanciato quando un donatore viene assegnato a un segmento
    /// </summary>
    public class DonorSegmentAssignedEvent
    {
        public Guid DonorId { get; set; }
        public Guid SegmentId { get; set; }

        public DonorSegmentAssignedEvent(Guid donorId, Guid segmentId)
        {
            DonorId = donorId;
            SegmentId = segmentId;
        }
    }

    /// <summary>
    /// Event lanciato quando un donatore viene rimosso da un segmento
    /// </summary>
    public class DonorSegmentRemovedEvent
    {
        public Guid DonorId { get; set; }
        public Guid SegmentId { get; set; }

        public DonorSegmentRemovedEvent(Guid donorId, Guid segmentId)
        {
            DonorId = donorId;
            SegmentId = segmentId;
        }
    }

    /// <summary>
    /// Event lanciato quando un tag viene assegnato a un donatore
    /// </summary>
    public class DonorTagAssignedEvent
    {
        public Guid DonorId { get; set; }
        public Guid TagId { get; set; }

        public DonorTagAssignedEvent(Guid donorId, Guid tagId)
        {
            DonorId = donorId;
            TagId = tagId;
        }
    }

    /// <summary>
    /// Event lanciato quando un tag viene rimosso da un donatore
    /// </summary>
    public class DonorTagRemovedEvent
    {
        public Guid DonorId { get; set; }
        public Guid TagId { get; set; }

        public DonorTagRemovedEvent(Guid donorId, Guid tagId)
        {
            DonorId = donorId;
            TagId = tagId;
        }
    }
}
