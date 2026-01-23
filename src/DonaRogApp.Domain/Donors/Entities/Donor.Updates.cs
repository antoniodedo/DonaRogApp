using DonaRogApp.Enums.Donors;
using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace DonaRogApp.Domain.Donors.Entities
{
    /// <summary>
    /// PARTIAL: Donor.Updates.cs
    /// 
    /// RESPONSIBILITY:
    /// - Update individual information
    /// - Update organization information
    /// - Update preferences (language, channel)
    /// - Update notes
    /// - Update origin
    /// </summary>
    public partial class Donor : FullAuditedAggregateRoot<Guid>
    {
        // ======================================================================
        // UPDATE INDIVIDUAL INFO
        // ======================================================================

        /// <summary>
        /// Aggiorna informazioni persona fisica
        /// </summary>
        public void UpdateIndividualInfo(
            string firstName,
            string lastName,
            Guid? titleId = null,
            Gender? gender = null,
            DateTime? birthDate = null,
            string? birthPlace = null)
        {
            Check.NotNullOrWhiteSpace(firstName, nameof(firstName));
            Check.NotNullOrWhiteSpace(lastName, nameof(lastName));

            if (SubjectType != SubjectType.Individual)
            {
                throw new BusinessException("DonorErrorCodes.OrganizationCannotHaveTitle")
                    .WithData("reason", "Cannot update individual info on organization donor");
            }

            FirstName = firstName;
            LastName = lastName;

            if (titleId.HasValue)
            {
                TitleId = titleId;
            }

            if (gender.HasValue)
            {
                Gender = gender;
            }

            if (birthDate.HasValue)
            {
                BirthDate = birthDate;
            }

            if (!string.IsNullOrWhiteSpace(birthPlace))
            {
                BirthPlace = birthPlace;
            }
        }

        // ======================================================================
        // UPDATE ORGANIZATION INFO
        // ======================================================================

        /// <summary>
        /// Aggiorna informazioni organizzazione
        /// </summary>
        public void UpdateOrganizationInfo(
            string companyName,
            OrganizationType? organizationType = null,
            LegalForm? legalForm = null,
            string? businessSector = null)
        {
            Check.NotNullOrWhiteSpace(companyName, nameof(companyName));

            if (SubjectType != SubjectType.Organization)
            {
                throw new BusinessException("DonorErrorCodes.IndividualWithoutName")
                    .WithData("reason", "Cannot update organization info on individual donor");
            }

            CompanyName = companyName;

            if (organizationType.HasValue)
            {
                OrganizationType = organizationType;
            }

            if (legalForm.HasValue)
            {
                LegalForm = legalForm;
            }

            if (!string.IsNullOrWhiteSpace(businessSector))
            {
                BusinessSector = businessSector;
            }
        }

        // ======================================================================
        // UPDATE PREFERENCES
        // ======================================================================

        /// <summary>
        /// Imposta lingua preferita
        /// </summary>
        public void SetPreferredLanguage(string language)
        {
            Check.NotNullOrWhiteSpace(language, nameof(language));
            PreferredLanguage = language;
        }

        /// <summary>
        /// Imposta canale di comunicazione preferito
        /// </summary>
        public void SetPreferredChannel(string channel)
        {
            Check.NotNullOrWhiteSpace(channel, nameof(channel));
            PreferredChannel = channel;
        }

        // ======================================================================
        // UPDATE OTHER INFO
        // ======================================================================

        /// <summary>
        /// Aggiorna note
        /// </summary>
        public void UpdateNotes(string? notes)
        {
            Notes = notes;
        }

        /// <summary>
        /// Imposta origine donatore
        /// </summary>
        public void SetOrigin(DonorOrigin origin)
        {
            Origin = origin;
        }

        /// <summary>
        /// Aggiorna categoria donatore (manualmente)
        /// Di solito viene calcolata automaticamente da Statistics
        /// </summary>
        public void SetCategory(DonorCategory category)
        {
            Category = category;
        }

        /// <summary>
        /// Aggiorna stato donatore
        /// </summary>
        public void SetStatus(DonorStatus status)
        {
            Status = status;
        }
    }
}
