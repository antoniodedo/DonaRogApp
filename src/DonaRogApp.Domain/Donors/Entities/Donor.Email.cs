using DonaRogApp.Domain.Donors.Entities;
using DonaRogApp.Domain.Donors.Events;
using DonaRogApp.Enums.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace DonaRogApp.Domain.Donors.Entities
{
    /// <summary>
    /// PARTIAL: Donor.Email.cs (ADATTATO)
    /// 
    /// Allineato al design di DonorEmail.cs
    /// Delega ai metodi di DonorEmail per logica encapsulata
    /// </summary>
    public partial class Donor : FullAuditedAggregateRoot<Guid>
    {
        /// <summary>
        /// Aggiunge un nuovo indirizzo email al donatore
        /// </summary>
        public void AddEmail(string emailAddress, EmailType type = EmailType.Personal)
        {
            Check.NotNullOrWhiteSpace(emailAddress, nameof(emailAddress));

            if (!IsValidEmail(emailAddress))
            {
                throw new BusinessException(DonorErrorCodes.InvalidEmail)
                    .WithData("email", emailAddress);
            }

            if (Emails.Any(e => e.EmailAddress.Equals(emailAddress, StringComparison.OrdinalIgnoreCase)))
            {
                throw new BusinessException(DonorErrorCodes.DuplicateEmail)
                    .WithData("email", emailAddress);
            }

            var donorEmail = DonorEmail.Create(
                donorId: this.Id,
                emailAddress: emailAddress,
                type: type,
                tenantId: this.TenantId
            );

            if (!Emails.Any())
            {
                donorEmail.SetAsDefault();
            }

            Emails.Add(donorEmail);
            AddLocalEvent(new DonorEmailAddedEvent(this.Id, emailAddress, type));
        }

        /// <summary>
        /// Rimuove un indirizzo email
        /// </summary>
        public void RemoveEmail(string emailAddress)
        {
            Check.NotNullOrWhiteSpace(emailAddress, nameof(emailAddress));

            var email = Emails.FirstOrDefault(e =>
                e.EmailAddress.Equals(emailAddress, StringComparison.OrdinalIgnoreCase));

            if (email == null)
            {
                throw new BusinessException(DonorErrorCodes.EmailNotFound)
                    .WithData("email", emailAddress);
            }

            if (Emails.Count == 1)
            {
                throw new BusinessException(DonorErrorCodes.CannotRemoveOnlyEmail);
            }

            if (email.IsDefault && Emails.Count > 1)
            {
                var newDefault = Emails.First(e => e.Id != email.Id);
                newDefault.SetAsDefault();
            }

            email.Delete();
            AddLocalEvent(new DonorEmailRemovedEvent(this.Id, emailAddress));
        }

        /// <summary>
        /// Imposta un email come indirizzo di default
        /// </summary>
        public void SetDefaultEmail(string emailAddress)
        {
            Check.NotNullOrWhiteSpace(emailAddress, nameof(emailAddress));

            var email = Emails.FirstOrDefault(e =>
                e.EmailAddress.Equals(emailAddress, StringComparison.OrdinalIgnoreCase));

            if (email == null)
            {
                throw new BusinessException(DonorErrorCodes.EmailNotFound)
                    .WithData("email", emailAddress);
            }

            foreach (var e in Emails.Where(e => e.IsDefault && e.Id != email.Id))
            {
                e.RemoveDefault();
            }

            email.SetAsDefault();
        }

        /// <summary>
        /// Verifica un indirizzo email
        /// </summary>
        public void VerifyEmail(string emailAddress)
        {
            Check.NotNullOrWhiteSpace(emailAddress, nameof(emailAddress));

            var email = Emails.FirstOrDefault(e =>
                e.EmailAddress.Equals(emailAddress, StringComparison.OrdinalIgnoreCase));

            if (email == null)
            {
                throw new BusinessException(DonorErrorCodes.EmailNotFound)
                    .WithData("email", emailAddress);
            }

            email.Verify();
            AddLocalEvent(new DonorEmailVerifiedEvent(this.Id, emailAddress));
        }

        /// <summary>
        /// Registra un bounce su indirizzo email
        /// </summary>
        public void RecordEmailBounce(string emailAddress, string? reason = null)
        {
            Check.NotNullOrWhiteSpace(emailAddress, nameof(emailAddress));

            var email = Emails.FirstOrDefault(e =>
                e.EmailAddress.Equals(emailAddress, StringComparison.OrdinalIgnoreCase));

            if (email == null)
            {
                throw new BusinessException(DonorErrorCodes.EmailNotFound)
                    .WithData("email", emailAddress);
            }

            email.RecordBounce(reason);
            AddLocalEvent(new DonorEmailBouncedEvent(this.Id, emailAddress, reason ?? "Unknown"));
        }

        /// <summary>
        /// Ottiene l'indirizzo email di default
        /// </summary>
        public DonorEmail? GetDefaultEmail()
        {
            return Emails.FirstOrDefault(e => e.IsDefault && !e.IsDeleted);
        }

        /// <summary>
        /// Ottiene tutti gli email validi (non invalid, non soft-deleted)
        /// </summary>
        public IReadOnlyList<DonorEmail> GetValidEmails()
        {
            return Emails
                .Where(e => !e.IsInvalid && !e.IsDeleted)
                .ToList()
                .AsReadOnly();
        }

        /// <summary>
        /// Genera token di verifica per un email
        /// </summary>
        public void GenerateEmailVerificationToken(string emailAddress)
        {
            Check.NotNullOrWhiteSpace(emailAddress, nameof(emailAddress));

            var email = Emails.FirstOrDefault(e =>
                e.EmailAddress.Equals(emailAddress, StringComparison.OrdinalIgnoreCase));

            if (email == null)
            {
                throw new BusinessException(DonorErrorCodes.EmailNotFound)
                    .WithData("email", emailAddress);
            }

            email.GenerateVerificationToken();
        }

        /// <summary>
        /// Aggiorna note su un email
        /// </summary>
        public void UpdateEmailNotes(string emailAddress, string? notes)
        {
            Check.NotNullOrWhiteSpace(emailAddress, nameof(emailAddress));

            var email = Emails.FirstOrDefault(e =>
                e.EmailAddress.Equals(emailAddress, StringComparison.OrdinalIgnoreCase));

            if (email == null)
            {
                throw new BusinessException(DonorErrorCodes.EmailNotFound)
                    .WithData("email", emailAddress);
            }

            email.UpdateNotes(notes);
        }

        private static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}