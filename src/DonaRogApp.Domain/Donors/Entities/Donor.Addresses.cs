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
    /// PARTIAL: Donor.Addresses.cs (ADATTATO)
    /// 
    /// Allineato al design di DonorAddress.cs
    /// Delega ai metodi di DonorAddress per logica encapsulata
    /// Traccia temporale degli indirizzi con StartDate/EndDate
    /// </summary>
    public partial class Donor : FullAuditedAggregateRoot<Guid>
    {
        // ======================================================================
        // ADDRESS MANAGEMENT
        // ======================================================================

        /// <summary>
        /// Aggiunge un nuovo indirizzo al donatore
        /// </summary>
        public void AddAddress(
            string street,
            string city,
            string postalCode,
            string country = "Italy",
            AddressType addressType = AddressType.Home,
            string? province = null,
            string? region = null,
            string? notes = null)
        {
            Check.NotNullOrWhiteSpace(street, nameof(street));
            Check.NotNullOrWhiteSpace(city, nameof(city));
            Check.NotNullOrWhiteSpace(postalCode, nameof(postalCode));
            Check.NotNullOrWhiteSpace(country, nameof(country));

            // Usa factory method di DonorAddress
            var donorAddress = DonorAddress.Create(
                donorId: this.Id,
                street: street,
                city: city,
                postalCode: postalCode,
                country: country,
                type: addressType,
                startDate: DateTime.UtcNow,
                tenantId: this.TenantId,
                province: province,
                region: region,
                notes: notes
            );

            // Se è il primo indirizzo attivo, impostalo come default
            if (!Addresses.Any(a => a.IsActive()))
            {
                donorAddress.SetAsDefault();
            }

            Addresses.Add(donorAddress);
            AddLocalEvent(new DonorAddressAddedEvent(this.Id, city, addressType));
        }

        /// <summary>
        /// Termina un indirizzo (soft delete con EndDate)
        /// </summary>
        public void EndAddress(Guid addressId, DateTime? endDate = null)
        {
            var address = Addresses.FirstOrDefault(a => a.Id == addressId);
            if (address == null)
            {
                throw new BusinessException(DonorErrorCodes.AddressNotFound);
            }

            // Usa metodo di DonorAddress
            address.End(endDate ?? DateTime.UtcNow);

            // Se era default, assegna default a un altro indirizzo attivo
            if (address.IsDefault)
            {
                var newDefault = Addresses.FirstOrDefault(a =>
                    a.IsActive() && a.Id != addressId);

                if (newDefault != null)
                {
                    newDefault.SetAsDefault();
                }
            }

            AddLocalEvent(new DonorAddressEndedEvent(this.Id, addressId));
        }

        /// <summary>
        /// Imposta un indirizzo come indirizzo di default
        /// </summary>
        public void SetDefaultAddress(Guid addressId)
        {
            var address = Addresses.FirstOrDefault(a => a.Id == addressId);
            if (address == null)
            {
                throw new BusinessException(DonorErrorCodes.AddressNotFound);
            }

            if (!address.IsActive())
            {
                throw new BusinessException(DonorErrorCodes.CannotSetInactiveAddressAsDefault);
            }

            // Rimuovi default da tutti gli altri indirizzi attivi
            foreach (var a in Addresses.Where(a => a.IsActive() && a.IsDefault && a.Id != addressId))
            {
                a.RemoveDefault();
            }

            address.SetAsDefault();
        }

        /// <summary>
        /// Ottiene l'indirizzo attuale (attivo e non soft-deleted)
        /// </summary>
        public DonorAddress? GetCurrentAddress()
        {
            return Addresses.FirstOrDefault(a =>
                a.IsActive() && !a.IsDeleted);
        }

        /// <summary>
        /// Ottiene l'indirizzo di default attivo
        /// </summary>
        public DonorAddress? GetDefaultAddress()
        {
            return Addresses.FirstOrDefault(a =>
                a.IsDefault && a.IsActive() && !a.IsDeleted);
        }

        /// <summary>
        /// Ottiene l'indirizzo relativo a una data specifica
        /// Utile per tracking storico
        /// </summary>
        public DonorAddress? GetAddressAtDate(DateTime date)
        {
            return Addresses.FirstOrDefault(a =>
                a.IsActiveAt(date) && !a.IsDeleted);
        }

        /// <summary>
        /// Ottiene tutti gli indirizzi attivi (non ended, non soft-deleted)
        /// </summary>
        public IReadOnlyList<DonorAddress> GetActiveAddresses()
        {
            return Addresses
                .Where(a => a.IsActive() && !a.IsDeleted)
                .ToList()
                .AsReadOnly();
        }

        /// <summary>
        /// Ottiene tutti gli indirizzi (inclusi quelli terminati)
        /// </summary>
        public IReadOnlyList<DonorAddress> GetAllAddresses()
        {
            return Addresses
                .Where(a => !a.IsDeleted)
                .OrderByDescending(a => a.StartDate)
                .ToList()
                .AsReadOnly();
        }

        /// <summary>
        /// Verifica un indirizzo
        /// </summary>
        public void VerifyAddress(Guid addressId)
        {
            var address = Addresses.FirstOrDefault(a => a.Id == addressId);
            if (address == null)
            {
                throw new BusinessException(DonorErrorCodes.AddressNotFound);
            }

            address.Verify();
        }

        /// <summary>
        /// Aggiorna i dettagli di un indirizzo
        /// Auto-unverify dopo modifica
        /// </summary>
        public void UpdateAddress(
            Guid addressId,
            string street,
            string city,
            string postalCode,
            string country = "Italy",
            string? province = null,
            string? region = null)
        {
            Check.NotNullOrWhiteSpace(street, nameof(street));
            Check.NotNullOrWhiteSpace(city, nameof(city));
            Check.NotNullOrWhiteSpace(postalCode, nameof(postalCode));
            Check.NotNullOrWhiteSpace(country, nameof(country));

            var address = Addresses.FirstOrDefault(a => a.Id == addressId);
            if (address == null)
            {
                throw new BusinessException(DonorErrorCodes.AddressNotFound);
            }

            // Usa metodo di DonorAddress - auto-unverify
            address.Update(
                street: street,
                city: city,
                postalCode: postalCode,
                country: country,
                province: province,
                region: region
            );
        }

        /// <summary>
        /// Aggiorna note su un indirizzo
        /// </summary>
        public void UpdateAddressNotes(Guid addressId, string? notes)
        {
            var address = Addresses.FirstOrDefault(a => a.Id == addressId);
            if (address == null)
            {
                throw new BusinessException(DonorErrorCodes.AddressNotFound);
            }

            address.UpdateNotes(notes);
        }

        /// <summary>
        /// Ottiene l'indirizzo formattato completo
        /// </summary>
        public string? GetFormattedAddress()
        {
            return GetCurrentAddress()?.GetFullAddress();
        }

        /// <summary>
        /// Ottiene l'indirizzo in formato breve
        /// </summary>
        public string? GetShortFormattedAddress()
        {
            return GetCurrentAddress()?.GetShortAddress();
        }
    }
}
