using DonaRogApp.Domain.Donors.Entities;
using DonaRogApp.Domain.Donors.Events;
using DonaRogApp.Enums.Communications;
using System;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace DonaRogApp.Domain.Donors.Entities
{
    /// <summary>
    /// PARTIAL: Donor.Communication.cs (ADATTATO)
    /// 
    /// Allineato al design di Communication.cs
    /// Delega ai metodi di Communication per logica encapsulata
    /// Traccia completa di email, lettere, SMS, ecc.
    /// </summary>
    public partial class Donor : FullAuditedAggregateRoot<Guid>
    {
        // ======================================================================
        // COMMUNICATION TRACKING
        // ======================================================================

        /// <summary>
        /// Registra un'email inviata al donatore
        /// </summary>
        public void RecordEmailSent(
            string subject,
            string recipient,
            TemplateCategory? category = null,
            Guid? templateId = null,
            Guid? campaignId = null,
            string? body = null,
            Guid? sentByUserId = null,
            string? notes = null)
        {
            Check.NotNullOrWhiteSpace(subject, nameof(subject));
            Check.NotNullOrWhiteSpace(recipient, nameof(recipient));

            if (!CanContact(CommunicationType.Email))
            {
                throw new BusinessException(DonorErrorCodes.CannotContactDonor)
                    .WithData("reason", "Privacy consent missing or invalid emails");
            }

            var communication = Communication.Create(
                donorId: this.Id,
                type: CommunicationType.Email,
                subject: subject,
                recipient: recipient,
                tenantId: this.TenantId,
                category: category,
                templateId: templateId,
                campaignId: campaignId,
                body: body,
                sentByUserId: sentByUserId,
                notes: notes
            );

            Communications.Add(communication);
            EmailsSentCount++;
            LastEmailSentDate = DateTime.UtcNow;

            AddLocalEvent(new DonorEmailSentEvent(this.Id, subject, DateTime.UtcNow));
        }

        /// <summary>
        /// Registra una lettera/documento inviato al donatore
        /// </summary>
        public void RecordLetterSent(
            string subject,
            string recipient,
            TemplateCategory? category = null,
            Guid? templateId = null,
            Guid? donationId = null,
            Guid? campaignId = null,
            string? body = null,
            Guid? sentByUserId = null,
            string? notes = null)
        {
            Check.NotNullOrWhiteSpace(subject, nameof(subject));
            Check.NotNullOrWhiteSpace(recipient, nameof(recipient));

            if (!CanContact(CommunicationType.Letter))
            {
                throw new BusinessException(DonorErrorCodes.CannotContactDonor)
                    .WithData("reason", "Privacy consent missing or no valid address");
            }

            var communication = Communication.Create(
                donorId: this.Id,
                type: CommunicationType.Letter,
                subject: subject,
                recipient: recipient,
                tenantId: this.TenantId,
                category: category,
                templateId: templateId,
                donationId: donationId,
                campaignId: campaignId,
                body: body,
                sentByUserId: sentByUserId,
                notes: notes
            );

            Communications.Add(communication);
            LettersSentCount++;
            LastThankYouLetterDate = DateTime.UtcNow;

            AddLocalEvent(new DonorLetterSentEvent(this.Id, subject, DateTime.UtcNow));
        }

        /// <summary>
        /// Registra SMS inviato al donatore
        /// </summary>
        public void RecordSmsSent(
            string subject,
            string recipient,
            Guid? campaignId = null,
            string? body = null,
            Guid? sentByUserId = null,
            string? notes = null)
        {
            Check.NotNullOrWhiteSpace(subject, nameof(subject));
            Check.NotNullOrWhiteSpace(recipient, nameof(recipient));

            if (!CanContact(CommunicationType.SMS))
            {
                throw new BusinessException(DonorErrorCodes.CannotContactDonor)
                    .WithData("reason", "Privacy consent missing or no valid phone");
            }

            var communication = Communication.Create(
                donorId: this.Id,
                type: CommunicationType.SMS,
                subject: subject,
                recipient: recipient,
                tenantId: this.TenantId,
                campaignId: campaignId,
                body: body,
                sentByUserId: sentByUserId,
                notes: notes
            );

            Communications.Add(communication);
            AddLocalEvent(new DonorCommunicationDeliveredEvent(this.Id, communication.Id, DateTime.UtcNow));
        }

        /// <summary>
        /// Registra consegna di una comunicazione
        /// </summary>
        public void MarkCommunicationAsDelivered(Guid communicationId, string? externalId = null)
        {
            var communication = Communications.FirstOrDefault(c => c.Id == communicationId);
            if (communication == null)
            {
                throw new BusinessException(DonorErrorCodes.CommunicationNotFound);
            }

            communication.MarkAsDelivered(externalId);
            AddLocalEvent(new DonorCommunicationDeliveredEvent(this.Id, communicationId, DateTime.UtcNow));
        }

        /// <summary>
        /// Registra fallimento di una comunicazione
        /// </summary>
        public void MarkCommunicationAsFailed(Guid communicationId, string? reason = null)
        {
            var communication = Communications.FirstOrDefault(c => c.Id == communicationId);
            if (communication == null)
            {
                throw new BusinessException(DonorErrorCodes.CommunicationNotFound);
            }

            communication.MarkAsFailed(reason);
            AddLocalEvent(new DonorCommunicationBouncedEvent(this.Id, communicationId));
        }

        /// <summary>
        /// Registra apertura di un'email
        /// </summary>
        public void RecordEmailOpened(Guid communicationId)
        {
            var communication = Communications.FirstOrDefault(c => c.Id == communicationId);
            if (communication == null || communication.Type != CommunicationType.Email)
            {
                throw new BusinessException(DonorErrorCodes.CommunicationNotFound);
            }

            communication.RecordOpen();
            AddLocalEvent(new DonorEmailOpenedEvent(this.Id, communicationId));
        }

        /// <summary>
        /// Registra click su link in email
        /// </summary>
        public void RecordEmailClicked(Guid communicationId)
        {
            var communication = Communications.FirstOrDefault(c => c.Id == communicationId);
            if (communication == null || communication.Type != CommunicationType.Email)
            {
                throw new BusinessException(DonorErrorCodes.CommunicationNotFound);
            }

            communication.RecordClick();
            AddLocalEvent(new DonorEmailClickedEvent(this.Id, communicationId));
        }

        /// <summary>
        /// Ottiene l'ultimo invio per tipo di comunicazione
        /// </summary>
        public Communication? GetLastCommunication(CommunicationType communicationType)
        {
            return Communications
                .Where(c => c.Type == communicationType && !c.IsDeleted)
                .OrderByDescending(c => c.SentDate)
                .FirstOrDefault();
        }

        /// <summary>
        /// Verifica se può inviare comunicazione (rate limiting)
        /// </summary>
        public bool CanSendCommunication(CommunicationType communicationType, int minDaysBetween = 7)
        {
            if (!CanContact(communicationType))
                return false;

            var lastComm = GetLastCommunication(communicationType);
            if (lastComm != null)
            {
                var daysSinceLastComm = (DateTime.UtcNow - lastComm.SentDate).TotalDays;
                if (daysSinceLastComm < minDaysBetween)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Ottiene comunicazioni per tipo
        /// </summary>
        public IReadOnlyList<Communication> GetCommunicationsByType(CommunicationType communicationType)
        {
            return Communications
                .Where(c => c.Type == communicationType && !c.IsDeleted)
                .OrderByDescending(c => c.SentDate)
                .ToList()
                .AsReadOnly();
        }

        /// <summary>
        /// Ottiene comunicazioni per categoria
        /// </summary>
        public IReadOnlyList<Communication> GetCommunicationsByCategory(TemplateCategory category)
        {
            return Communications
                .Where(c => c.Category == category && !c.IsDeleted)
                .OrderByDescending(c => c.SentDate)
                .ToList()
                .AsReadOnly();
        }

        /// <summary>
        /// Ottiene comunicazioni per campagna
        /// </summary>
        public IReadOnlyList<Communication> GetCommunicationsByCampaign(Guid campaignId)
        {
            return Communications
                .Where(c => c.CampaignId == campaignId && !c.IsDeleted)
                .OrderByDescending(c => c.SentDate)
                .ToList()
                .AsReadOnly();
        }

        /// <summary>
        /// Conta email aperte
        /// </summary>
        public int GetEmailOpenCount()
        {
            return Communications.Count(c =>
                c.Type == CommunicationType.Email &&
                c.IsOpened &&
                !c.IsDeleted);
        }

        /// <summary>
        /// Conta email con click
        /// </summary>
        public int GetEmailClickCount()
        {
            return Communications.Count(c =>
                c.Type == CommunicationType.Email &&
                c.IsClicked &&
                !c.IsDeleted);
        }

        /// <summary>
        /// Conta totale email inviate
        /// </summary>
        public int GetEmailSentCount()
        {
            return Communications.Count(c =>
                c.Type == CommunicationType.Email &&
                !c.IsDeleted);
        }

        /// <summary>
        /// Calcola email open rate (%)
        /// </summary>
        public decimal GetEmailOpenRate()
        {
            var emailCount = GetEmailSentCount();
            if (emailCount == 0) return 0;

            var openCount = GetEmailOpenCount();
            return Math.Round((decimal)openCount / emailCount * 100, 2);
        }

        /// <summary>
        /// Calcola email click rate (%)
        /// </summary>
        public decimal GetEmailClickRate()
        {
            var emailCount = GetEmailSentCount();
            if (emailCount == 0) return 0;

            var clickCount = GetEmailClickCount();
            return Math.Round((decimal)clickCount / emailCount * 100, 2);
        }

        /// <summary>
        /// Aggiorna note su comunicazione
        /// </summary>
        public void UpdateCommunicationNotes(Guid communicationId, string? notes)
        {
            var communication = Communications.FirstOrDefault(c => c.Id == communicationId);
            if (communication == null)
            {
                throw new BusinessException(DonorErrorCodes.CommunicationNotFound);
            }

            communication.UpdateNotes(notes);
        }

        /// <summary>
        /// Ottiene tutte le comunicazioni non soft-deleted
        /// </summary>
        public IReadOnlyList<Communication> GetAllCommunications()
        {
            return Communications
                .Where(c => !c.IsDeleted)
                .OrderByDescending(c => c.SentDate)
                .ToList()
                .AsReadOnly();
        }
    }
}
