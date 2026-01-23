using DonaRogApp.Domain.Donors.Entities;
using DonaRogApp.Domain.Donors.Events;
using DonaRogApp.Enums.Shared;
using DonaRogApp.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace DonaRogApp.Domain.Donors.Entities
{
    /// <summary>
    /// PARTIAL: Donor.Contacts.cs (ADATTATO)
    /// 
    /// Allineato al design di DonorContact.cs
    /// Delega ai metodi di DonorContact per logica encapsulata
    /// </summary>
    public partial class Donor : FullAuditedAggregateRoot<Guid>
    {
        // ======================================================================
        // CONTACT MANAGEMENT (Phone, SMS, etc.)
        // ======================================================================

        /// <summary>
        /// Aggiunge un nuovo contatto telefonico al donatore
        /// </summary>
        public void AddContact(PhoneNumber phoneNumber, ContactType type = ContactType.Mobile)
        {
            Check.NotNull(phoneNumber, nameof(phoneNumber));

            if (Contacts.Any(c => c.PhoneNumber.Equals(phoneNumber)))
            {
                throw new BusinessException(DonorErrorCodes.DuplicateContact)
                    .WithData("phone", phoneNumber.InternationalNumber);
            }

            // Usa factory method di DonorContact
            var donorContact = DonorContact.Create(
                donorId: this.Id,
                phoneNumber: phoneNumber,
                type: type,
                tenantId: this.TenantId
            );

            // Se è il primo contatto, impostalo come default
            if (!Contacts.Any())
            {
                donorContact.SetAsDefault();
            }

            Contacts.Add(donorContact);
            AddLocalEvent(new DonorContactAddedEvent(this.Id, phoneNumber.InternationalNumber, type));
        }

        /// <summary>
        /// Rimuove un contatto telefonico
        /// </summary>
        public void RemoveContact(string phoneNumber)
        {
            Check.NotNullOrWhiteSpace(phoneNumber, nameof(phoneNumber));

            var contact = Contacts.FirstOrDefault(c =>
                c.PhoneNumber.InternationalNumber.Equals(phoneNumber));

            if (contact == null)
            {
                throw new BusinessException(DonorErrorCodes.ContactNotFound)
                    .WithData("phone", phoneNumber);
            }

            if (Contacts.Count == 1)
            {
                throw new BusinessException(DonorErrorCodes.CannotRemoveOnlyContact);
            }

            // Se è default, assegna default a un altro
            if (contact.IsDefault && Contacts.Count > 1)
            {
                var newDefault = Contacts.First(c => c.Id != contact.Id);
                newDefault.SetAsDefault();
            }

            // Soft delete tramite metodo di DonorContact
            contact.Delete();
            AddLocalEvent(new DonorContactRemovedEvent(this.Id, phoneNumber));
        }

        /// <summary>
        /// Imposta un contatto come numero di default
        /// </summary>
        public void SetDefaultContact(string phoneNumber)
        {
            Check.NotNullOrWhiteSpace(phoneNumber, nameof(phoneNumber));

            var contact = Contacts.FirstOrDefault(c =>
                c.PhoneNumber.InternationalNumber.Equals(phoneNumber));

            if (contact == null)
            {
                throw new BusinessException(DonorErrorCodes.ContactNotFound)
                    .WithData("phone", phoneNumber);
            }

            // Rimuovi default da tutti gli altri
            foreach (var c in Contacts.Where(c => c.IsDefault && c.Id != contact.Id))
            {
                c.RemoveDefault();
            }

            contact.SetAsDefault();
        }

        /// <summary>
        /// Verifica un contatto telefonico
        /// </summary>
        public void VerifyContact(string phoneNumber)
        {
            Check.NotNullOrWhiteSpace(phoneNumber, nameof(phoneNumber));

            var contact = Contacts.FirstOrDefault(c =>
                c.PhoneNumber.InternationalNumber.Equals(phoneNumber));

            if (contact == null)
            {
                throw new BusinessException(DonorErrorCodes.ContactNotFound)
                    .WithData("phone", phoneNumber);
            }

            // Usa metodo di DonorContact
            contact.Verify();
            AddLocalEvent(new DonorContactVerifiedEvent(this.Id, phoneNumber));
        }

        /// <summary>
        /// Ottiene il contatto di default
        /// </summary>
        public DonorContact? GetDefaultContact()
        {
            return Contacts.FirstOrDefault(c => c.IsDefault && !c.IsDeleted);
        }

        /// <summary>
        /// Ottiene tutti i contatti validi (non soft-deleted)
        /// </summary>
        public IReadOnlyList<DonorContact> GetValidContacts()
        {
            return Contacts
                .Where(c => !c.IsDeleted)
                .ToList()
                .AsReadOnly();
        }

        /// <summary>
        /// Aggiorna il numero di telefono di un contatto
        /// </summary>
        public void UpdateContactPhoneNumber(string oldPhoneNumber, PhoneNumber newPhoneNumber)
        {
            Check.NotNullOrWhiteSpace(oldPhoneNumber, nameof(oldPhoneNumber));
            Check.NotNull(newPhoneNumber, nameof(newPhoneNumber));

            var contact = Contacts.FirstOrDefault(c =>
                c.PhoneNumber.InternationalNumber.Equals(oldPhoneNumber));

            if (contact == null)
            {
                throw new BusinessException(DonorErrorCodes.ContactNotFound)
                    .WithData("phone", oldPhoneNumber);
            }

            if (Contacts.Any(c =>
                c.Id != contact.Id && c.PhoneNumber.Equals(newPhoneNumber)))
            {
                throw new BusinessException(DonorErrorCodes.DuplicateContact)
                    .WithData("phone", newPhoneNumber.InternationalNumber);
            }

            // Usa metodo di DonorContact - auto-unverify dopo cambio
            contact.UpdatePhoneNumber(newPhoneNumber);
        }

        /// <summary>
        /// Aggiorna il tipo di contatto
        /// </summary>
        public void UpdateContactType(string phoneNumber, ContactType newType)
        {
            Check.NotNullOrWhiteSpace(phoneNumber, nameof(phoneNumber));

            var contact = Contacts.FirstOrDefault(c =>
                c.PhoneNumber.InternationalNumber.Equals(phoneNumber));

            if (contact == null)
            {
                throw new BusinessException(DonorErrorCodes.ContactNotFound)
                    .WithData("phone", phoneNumber);
            }

            contact.UpdateType(newType);
        }

        /// <summary>
        /// Aggiorna note su un contatto
        /// </summary>
        public void UpdateContactNotes(string phoneNumber, string? notes)
        {
            Check.NotNullOrWhiteSpace(phoneNumber, nameof(phoneNumber));

            var contact = Contacts.FirstOrDefault(c =>
                c.PhoneNumber.InternationalNumber.Equals(phoneNumber));

            if (contact == null)
            {
                throw new BusinessException(DonorErrorCodes.ContactNotFound)
                    .WithData("phone", phoneNumber);
            }

            contact.UpdateNotes(notes);
        }
    }
}
