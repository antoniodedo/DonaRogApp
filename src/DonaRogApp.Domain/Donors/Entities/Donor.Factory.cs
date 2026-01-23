using DonaRogApp.Enums.Donors;
using DonaRogApp.Enums.Shared;
using DonaRogApp.ValueObjects;
using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace DonaRogApp.Domain.Donors.Entities
{
    /// <summary>
    /// PARTIAL: Donor.Factory.cs
    /// 
    /// RESPONSIBILITY:
    /// - Factory methods per creare Donor (Individual/Organization)
    /// - GenerateDonorCode
    /// - SetTitle (con validazione)
    /// </summary>
    public partial class Donor : FullAuditedAggregateRoot<Guid>
    {
        // ======================================================================
        // FACTORY METHODS
        // ======================================================================

        /// <summary>
        /// Factory method per creare un donatore individuale
        /// </summary>
        public static Donor CreateIndividual(
            Guid? tenantId,
            string firstName,
            string lastName,
            Gender gender,
            DateTime? birthDate,
            TaxCode? taxCode = null,
            Guid? titleId = null,
            string? email = null,
            string? phone = null)
        {
            Check.NotNullOrWhiteSpace(firstName, nameof(firstName));
            Check.NotNullOrWhiteSpace(lastName, nameof(lastName));

            var donor = new Donor
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                SubjectType = SubjectType.Individual,
                FirstName = firstName,
                LastName = lastName,
                Gender = gender,
                BirthDate = birthDate,
                TaxCode = taxCode,
                TitleId = titleId,
                DonorCode = GenerateDonorCode(),
                Status = DonorStatus.Active,
                DonationCount = 0,
                TotalDonated = 0,
                AverageDonationAmount = 0,
                RecencyScore = 0,
                FrequencyScore = 0,
                MonetaryScore = 0,
                PrivacyConsent = false,
                NewsletterConsent = false,
                IsAnonymized = false
            };

            if (!string.IsNullOrWhiteSpace(email))
            {
                donor.AddEmail(email, EmailType.Personal);
            }

            if (!string.IsNullOrWhiteSpace(phone))
            {
                donor.AddContact(new PhoneNumber(phone), ContactType.Mobile);
            }

            return donor;
        }

        /// <summary>
        /// Factory method per creare un donatore organizzazione
        /// </summary>
        public static Donor CreateOrganization(
            Guid? tenantId,
            string companyName,
            OrganizationType organizationType,
            LegalForm legalForm,
            VatNumber vatNumber,
            string? taxCode = null,
            string? email = null,
            string? phone = null)
        {
            Check.NotNullOrWhiteSpace(companyName, nameof(companyName));
            Check.NotNull(vatNumber, nameof(vatNumber));

            var donor = new Donor
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                SubjectType = SubjectType.Organization,
                CompanyName = companyName,
                OrganizationType = organizationType,
                LegalForm = legalForm,
                VatNumber = vatNumber,
                TaxCode = taxCode != null ? new TaxCode(taxCode) : null,
                DonorCode = GenerateDonorCode(),
                Status = DonorStatus.Active,
                DonationCount = 0,
                TotalDonated = 0,
                AverageDonationAmount = 0,
                RecencyScore = 0,
                FrequencyScore = 0,
                MonetaryScore = 0,
                PrivacyConsent = false,
                NewsletterConsent = false,
                IsAnonymized = false
            };

            if (!string.IsNullOrWhiteSpace(email))
            {
                donor.AddEmail(email, EmailType.Personal);
            }

            if (!string.IsNullOrWhiteSpace(phone))
            {
                donor.AddContact(new PhoneNumber(phone), ContactType.Mobile);
            }

            return donor;
        }

        /// <summary>
        /// Genera codice univoco donatore (formato: DONOR-YYYYMMDD-XXXXX)
        /// </summary>
        public static string GenerateDonorCode()
        {
            var timestamp = DateTime.UtcNow.ToString("yyyyMMdd");
            var random = Guid.NewGuid().ToString().Substring(0, 5).ToUpper();
            return $"DONOR-{timestamp}-{random}";
        }

        /// <summary>
        /// Imposta il titolo per un donatore (solo persone fisiche)
        /// </summary>
        public void SetTitle(Guid? titleId)
        {
            // Titolo solo per persone fisiche
            if (titleId.HasValue && SubjectType != SubjectType.Individual)
            {
                throw new BusinessException(DonorErrorCodes.OrganizationCannotHaveTitle)
                    .WithData("subjectType", SubjectType);
            }

            TitleId = titleId;
        }
    }
}
