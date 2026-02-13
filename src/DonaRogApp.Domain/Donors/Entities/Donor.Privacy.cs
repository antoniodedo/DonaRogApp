using DonaRogApp.Domain.Donors.Entities;
using DonaRogApp.Domain.Donors.Events;
using DonaRogApp.Enums.Communications;
using DonaRogApp.Enums.Donors;
using DonaRogApp.ValueObjects;
using System;
using System.Linq;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace DonaRogApp.Domain.Donors.Entities
{
    public partial class Donor : FullAuditedAggregateRoot<Guid>
    {
        public void GrantPrivacyConsent()
        {
            if (!PrivacyConsent)
            {
                PrivacyConsent = true;
                PrivacyConsentDate = DateTime.UtcNow;
                AddLocalEvent(new DonorPrivacyConsentGrantedEvent(this.Id));
            }
        }

        public void RevokePrivacyConsent()
        {
            if (PrivacyConsent)
            {
                PrivacyConsent = false;
                PrivacyConsentRevokedDate = DateTime.UtcNow;
                
                // Revoca anche newsletter e spedizioni cartacee
                if (NewsletterConsent)
                {
                    NewsletterConsent = false;
                    AddLocalEvent(new DonorNewsletterConsentRevokedEvent(this.Id));
                }
                if (MailConsent)
                {
                    MailConsent = false;
                }
                
                AddLocalEvent(new DonorPrivacyConsentRevokedEvent(this.Id));
            }
        }

        public void GrantNewsletterConsent()
        {
            // Non si può abilitare newsletter se privacy è revocata
            if (!PrivacyConsent)
            {
                throw new BusinessException(DonorErrorCodes.CannotGrantConsentWithoutPrivacy)
                    .WithData("consentType", "Newsletter");
            }
            
            if (!NewsletterConsent)
            {
                NewsletterConsent = true;
                NewsletterConsentDate = DateTime.UtcNow;
                AddLocalEvent(new DonorNewsletterConsentGrantedEvent(this.Id));
            }
        }

        public void RevokeNewsletterConsent()
        {
            if (NewsletterConsent)
            {
                NewsletterConsent = false;
                AddLocalEvent(new DonorNewsletterConsentRevokedEvent(this.Id));
            }
        }

        public void GrantPhoneConsent() => PhoneConsent = true;
        public void RevokePhoneConsent() => PhoneConsent = false;

        public void GrantMailConsent()
        {
            // Non si può abilitare mail se privacy è revocata
            if (!PrivacyConsent)
            {
                throw new BusinessException(DonorErrorCodes.CannotGrantConsentWithoutPrivacy)
                    .WithData("consentType", "Spedizioni Cartacee");
            }
            
            if (!MailConsent)
            {
                MailConsent = true;
                MailConsentDate = DateTime.UtcNow;
            }
        }
        
        public void RevokeMailConsent()
        {
            if (MailConsent)
            {
                MailConsent = false;
            }
        }

        public void GrantProfilingConsent() => ProfilingConsent = true;
        public void RevokeProfilingConsent() => ProfilingConsent = false;

        public void GrantThirdPartyConsent() => ThirdPartyConsent = true;
        public void RevokeThirdPartyConsent() => ThirdPartyConsent = false;

        public void Anonymize()
        {
            if (IsAnonymized)
                throw new BusinessException(DonorErrorCodes.DonorAlreadyAnonymized);

            FirstName = "ANONYMIZED";
            LastName = $"USER_{Id:N}".Substring(0, 20);
            MiddleName = null;
            BirthDate = null;
            BirthPlace = null;
            Gender = null;
            CompanyName = null;

            TaxCode = null;
            VatNumber = null;

            Emails.Clear();
            Contacts.Clear();
            Addresses.Clear();

            PrivacyConsent = false;
            NewsletterConsent = false;
            PhoneConsent = false;
            MailConsent = false;
            ProfilingConsent = false;
            ThirdPartyConsent = false;

            IsAnonymized = true;
            AnonymizationDate = DateTime.UtcNow;
            Status = DonorStatus.Inactive;

            AddLocalEvent(new DonorAnonymizedEvent(this.Id));
        }

        public bool CanContact(CommunicationType communicationType)
        {
            if (!PrivacyConsent) return false;
            if (IsAnonymized) return false;
            if (Status == DonorStatus.Inactive) return false;

            return communicationType switch
            {
                CommunicationType.Email => Emails.Any(e => !e.IsInvalid),
                CommunicationType.SMS => Contacts.Any(c => !c.IsDeleted),
                CommunicationType.Letter => Addresses.Any(a => a.EndDate == null),
                _ => false
            };
        }

        public bool CanReceiveMarketing()
        {
            return PrivacyConsent && !IsAnonymized && Status == DonorStatus.Active;
        }

        public bool CanReceiveNewsletter()
        {
            return CanReceiveMarketing() && NewsletterConsent;
        }
    }
}