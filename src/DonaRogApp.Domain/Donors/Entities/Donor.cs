using DonaRogApp.Domain.Donors.Entities;
using DonaRogApp.Domain.Shared.Entities;
using DonaRogApp.Enums;
using DonaRogApp.Enums.Communications;
using DonaRogApp.Enums.Donors;
using DonaRogApp.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace DonaRogApp.Domain.Donors.Entities
{
    /// <summary>
    /// Donor Aggregate Root
    /// 
    /// RESPONSIBILITY:
    /// - Store all donor properties
    /// - Provide query methods (GetFullName, IsLapsed, etc.)
    /// - Define business invariants
    /// 
    /// Business logic is split across partial classes:
    /// - Donor.Factory.cs: Creation
    /// - Donor.Email.cs: Email management
    /// - Donor.Contacts.cs: Phone/contact management
    /// - Donor.Addresses.cs: Address management
    /// - Donor.Addresses.cs: Address management
    /// - Donor.Statistics.cs: RFM, Categories, Lapsed detection
    /// - Donor.Privacy.cs: GDPR, Consents, Anonymization
    /// - Donor.Communication.cs: Communication tracking
    /// </summary>
    public partial class Donor : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {
        // ======================================================================
        // MULTI-TENANCY
        // ======================================================================
        /// <summary>
        /// Tenant ID
        /// </summary>
        public Guid? TenantId { get; private set; }

        // ======================================================================
        // IDENTIFICATION
        // ======================================================================
        /// <summary>
        /// Unique donor code, automatically generated
        /// Format: DONOR-YYYYMMDD-XXXXX
        /// </summary>
        public string DonorCode { get; private set; }

        /// <summary>
        /// Subject type: Individual or Organization
        /// Determines which fields are active
        /// </summary>
        public SubjectType SubjectType { get; private set; }

        // ======================================================================
        // INDIVIDUAL (Persona Fisica)
        // ======================================================================
        /// <summary>
        /// Title/courtesy (Sig., Dott., Prof., etc.)
        /// Only for individuals
        /// </summary>
        public Guid? TitleId { get; private set; }
        public virtual Title? Title { get; private set; }

        /// <summary>
        /// First name (required for individuals)
        /// </summary>
        public string? FirstName { get; private set; }

        /// <summary>
        /// Middle name
        /// </summary>
        public string? MiddleName { get; private set; }

        /// <summary>
        /// Last name (required for individuals)
        /// </summary>
        public string? LastName { get; private set; }

        /// <summary>
        /// Gender (M/F/Other)
        /// </summary>
        public Gender? Gender { get; private set; }

        /// <summary>
        /// Birth date
        /// </summary>
        public DateTime? BirthDate { get; private set; }

        /// <summary>
        /// Birth place (city)
        /// </summary>
        public string? BirthPlace { get; private set; }

        // ======================================================================
        // ORGANIZATION (Persona Giuridica)
        // ======================================================================
        /// <summary>
        /// Company/organization name (required for organizations)
        /// </summary>
        public string? CompanyName { get; private set; }

        /// <summary>
        /// Organization type (Association, Foundation, Company, etc.)
        /// </summary>
        public OrganizationType? OrganizationType { get; private set; }

        /// <summary>
        /// Legal form (LLC, Corporation, NGO, etc.)
        /// </summary>
        public LegalForm? LegalForm { get; private set; }

        /// <summary>
        /// Business sector/industry
        /// </summary>
        public string? BusinessSector { get; private set; }

        // ======================================================================
        // TAX CODES (Value Objects)
        // ======================================================================
        /// <summary>
        /// Italian Tax Code (16 chars individual, 11 organization)
        /// Value Object with automatic validation
        /// </summary>
        public TaxCode? TaxCode { get; private set; }

        /// <summary>
        /// VAT Number (11 digits)
        /// Value Object with checksum validation
        /// </summary>
        public VatNumber? VatNumber { get; private set; }

        // ======================================================================
        // CLASSIFICATION
        // ======================================================================
        /// <summary>
        /// Donor status (New, Active, Lapsed, Inactive)
        /// </summary>
        public DonorStatus Status { get; private set; }

        /// <summary>
        /// Donor category based on total donated
        /// (Standard, Bronze, Silver, Gold, Major)
        /// Calculated automatically
        /// </summary>
        public DonorCategory Category { get; private set; }

        /// <summary>
        /// Donor origin/source (Web, Event, Referral, etc.)
        /// </summary>
        public DonorOrigin? Origin { get; private set; }

        // ======================================================================
        // DONATION STATISTICS
        // ======================================================================
        /// <summary>
        /// Total amount donated (lifetime value)
        /// </summary>
        public decimal TotalDonated { get; private set; }

        /// <summary>
        /// Total number of donations
        /// </summary>
        public int DonationCount { get; private set; }

        /// <summary>
        /// Average donation amount (TotalDonated / DonationCount)
        /// Calculated automatically
        /// </summary>
        public decimal AverageDonationAmount { get; private set; }

        /// <summary>
        /// Amount of first donation
        /// </summary>
        public decimal? FirstDonationAmount { get; private set; }

        /// <summary>
        /// Amount of last donation
        /// </summary>
        public decimal? LastDonationAmount { get; private set; }

        /// <summary>
        /// Date of first donation
        /// </summary>
        public DateTime? FirstDonationDate { get; private set; }

        /// <summary>
        /// Date of last donation (for lapsed calculation)
        /// </summary>
        public DateTime? LastDonationDate { get; private set; }

        /// <summary>
        /// Date of first conversion (prospect to donor)
        /// </summary>
        public DateTime? FirstConversionDate { get; private set; }

        // ======================================================================
        // RFM SCORES (Recency, Frequency, Monetary)
        // ======================================================================
        /// <summary>
        /// Recency Score (1-5): how recent is last donation
        /// 5 = very recent, 1 = very old
        /// </summary>
        public int RecencyScore { get; private set; }

        /// <summary>
        /// Frequency Score (1-5): how frequently they donate
        /// 5 = very frequent, 1 = rarely
        /// </summary>
        public int FrequencyScore { get; private set; }

        /// <summary>
        /// Monetary Score (1-5): how high is donated amount
        /// 5 = very high, 1 = very low
        /// </summary>
        public int MonetaryScore { get; private set; }

        /// <summary>
        /// RFM Segment (e.g., "Champions", "Loyal", "At Risk")
        /// Based on RFM score combination
        /// </summary>
        public string? RfmSegment { get; private set; }

        // ======================================================================
        // COMMUNICATION PREFERENCES
        // ======================================================================
        /// <summary>
        /// Preferred language (ISO 639-1, default: IT)
        /// </summary>
        public string PreferredLanguage { get; private set; }

        /// <summary>
        /// Preferred communication frequency
        /// (Weekly, Monthly, Quarterly, etc.)
        /// </summary>
        public CommunicationFrequency? PreferredFrequency { get; private set; }

        /// <summary>
        /// Preferred communication channel
        /// (Email, Mail, SMS, WhatsApp)
        /// </summary>
        public string? PreferredChannel { get; private set; }

        // ======================================================================
        // COMMUNICATION TRACKING
        // ======================================================================
        /// <summary>
        /// Number of thank you letters sent
        /// </summary>
        public int LettersSentCount { get; private set; }

        /// <summary>
        /// Number of emails sent
        /// </summary>
        public int EmailsSentCount { get; private set; }

        /// <summary>
        /// Date of last thank you letter sent
        /// Used to avoid excessive mailings
        /// </summary>
        public DateTime? LastThankYouLetterDate { get; private set; }

        /// <summary>
        /// Date of last email sent
        /// </summary>
        public DateTime? LastEmailSentDate { get; private set; }

        // ======================================================================
        // PRIVACY & GDPR CONSENTS
        // ======================================================================
        /// <summary>
        /// Privacy consent (required for data processing)
        /// </summary>
        public bool PrivacyConsent { get; private set; }

        /// <summary>
        /// Privacy consent date
        /// </summary>
        public DateTime? PrivacyConsentDate { get; private set; }

        /// <summary>
        /// Newsletter/marketing communications consent
        /// </summary>
        public bool NewsletterConsent { get; private set; }

        /// <summary>
        /// Newsletter consent date
        /// </summary>
        public DateTime? NewsletterConsentDate { get; private set; }

        /// <summary>
        /// Phone contact consent
        /// </summary>
        public bool PhoneConsent { get; private set; }

        /// <summary>
        /// Postal mail consent
        /// </summary>
        public bool MailConsent { get; private set; }

        /// <summary>
        /// Postal mail consent date
        /// </summary>
        public DateTime? MailConsentDate { get; private set; }

        /// <summary>
        /// Date when privacy consent was revoked
        /// </summary>
        public DateTime? PrivacyConsentRevokedDate { get; private set; }

        /// <summary>
        /// Profiling consent (behavioral analysis)
        /// </summary>
        public bool ProfilingConsent { get; private set; }

        /// <summary>
        /// Third-party data sharing consent
        /// </summary>
        public bool ThirdPartyConsent { get; private set; }

        /// <summary>
        /// Donor anonymized (GDPR right to be forgotten)
        /// </summary>
        public bool IsAnonymized { get; private set; }

        /// <summary>
        /// Anonymization date
        /// </summary>
        public DateTime? AnonymizationDate { get; private set; }

        // ======================================================================
        // COMMUNICATION PREFERENCES
        // ======================================================================
        /// <summary>
        /// Preferred channel for thank you communications
        /// Used by automatic rules when creating thank you letters/emails
        /// </summary>
        public PreferredThankYouChannel? PreferredThankYouChannel { get; private set; }

        // ======================================================================
        // NOTES
        // ======================================================================
        /// <summary>
        /// General notes about the donor
        /// </summary>
        public string? Notes { get; private set; }

        // ======================================================================
        // NAVIGATION PROPERTIES - Child Collections
        // ======================================================================
        /// <summary>
        /// Donor email addresses (at least 1 required for active donors)
        /// </summary>
        public virtual ICollection<DonorEmail> Emails { get; private set; }

        /// <summary>
        /// Donor phone numbers
        /// </summary>
        public virtual ICollection<DonorContact> Contacts { get; private set; }

        /// <summary>
        /// Donor postal addresses
        /// </summary>
        public virtual ICollection<DonorAddress> Addresses { get; private set; }

        /// <summary>
        /// Historical notes about the donor
        /// Tracks interactions, calls, events
        /// </summary>
        public virtual ICollection<DonorNote> HistoricalNotes { get; private set; }

        /// <summary>
        /// Communication history sent to donor
        /// </summary>
        public virtual ICollection<Communication> Communications { get; private set; }

        /// <summary>
        /// Segment memberships (for targeted campaigns)
        /// </summary>
        public virtual ICollection<DonorSegment> Segments { get; private set; }

        /// <summary>
        /// Free tags for categorization
        /// </summary>
        public virtual ICollection<DonorTag> Tags { get; private set; }

        /// <summary>
        /// Donor interests in specific topics/themes
        /// </summary>
        public virtual ICollection<DonorInterest> Interests { get; private set; }

        /// <summary>
        /// Status history tracking
        /// </summary>
        public virtual ICollection<DonorStatusHistory> StatusHistory { get; private set; }

        // ======================================================================
        // NAVIGATION PROPERTIES - Campaigns
        // ======================================================================
        /// <summary>
        /// Campaigns the donor has been part of
        /// </summary>
        public virtual ICollection<DonaRogApp.Domain.Campaigns.Entities.CampaignDonor> CampaignParticipations { get; private set; }

        // ======================================================================
        // CONSTRUCTOR
        // ======================================================================
        /// <summary>
        /// Protected constructor for EF Core
        /// Use Factory Methods (in Donor.Factory.cs) to create instances
        /// </summary>
        protected Donor()
        {
            // Initialize collections
            Emails = new List<DonorEmail>();
            Contacts = new List<DonorContact>();
            Addresses = new List<DonorAddress>();
            HistoricalNotes = new List<DonorNote>();
            Communications = new List<Communication>();
            Segments = new List<DonorSegment>();
            Tags = new List<DonorTag>();
            Interests = new List<DonorInterest>();
            StatusHistory = new List<DonorStatusHistory>();
            CampaignParticipations = new List<DonaRogApp.Domain.Campaigns.Entities.CampaignDonor>();

            // Default values
            PreferredLanguage = "IT";
            Status = DonorStatus.New;
            Category = DonorCategory.Unclassified;

            // Initial statistics
            TotalDonated = 0;
            DonationCount = 0;
            AverageDonationAmount = 0;

            // Initial RFM
            RecencyScore = 0;
            FrequencyScore = 0;
            MonetaryScore = 0;

            // Initial communication counts
            LettersSentCount = 0;
            EmailsSentCount = 0;

            // Initial privacy (all false for GDPR compliance)
            PrivacyConsent = false;
            NewsletterConsent = false;
            PhoneConsent = false;
            MailConsent = false;
            ProfilingConsent = false;
            ThirdPartyConsent = false;
            IsAnonymized = false;
        }

        // ======================================================================
        // QUERY METHODS - Display Names
        // ======================================================================

        /// <summary>
        /// Gets donor's full name
        /// For Individual: "Title FirstName LastName"
        /// For Organization: "CompanyName"
        /// </summary>
        public string GetFullName()
        {
            if (SubjectType == SubjectType.Organization)
                return CompanyName ?? "";

            var parts = new List<string>();

            if (Title != null && !string.IsNullOrWhiteSpace(Title.Abbreviation))
                parts.Add(Title.Abbreviation);

            if (!string.IsNullOrWhiteSpace(FirstName))
                parts.Add(FirstName);

            if (!string.IsNullOrWhiteSpace(LastName))
                parts.Add(LastName);

            return string.Join(" ", parts);
        }

        /// <summary>
        /// Gets formal name for letter salutations
        /// E.g., "Dear Mr. Smith" or "Dear ABC Foundation"
        /// </summary>
        public string GetFormalName()
        {
            if (SubjectType == SubjectType.Organization)
                return $"Dear {CompanyName}";

            var title = Title?.Abbreviation ?? "";
            return $"Dear {title} {LastName}".Trim();
        }

        // ======================================================================
        // QUERY METHODS - Status & Classification
        // ======================================================================

        /// <summary>
        /// Checks if donor is lapsed (>18 months without donations)
        /// </summary>
        public bool IsLapsed()
        {
            if (!LastDonationDate.HasValue)
                return false;

            var daysSinceLastDonation = (DateTime.UtcNow - LastDonationDate.Value).Days;
            return daysSinceLastDonation > 540; // 18 months
        }

        /// <summary>
        /// Checks if donor is active (has donated at least once)
        /// </summary>
        public bool IsActive()
        {
            return DonationCount > 0;
        }

        /// <summary>
        /// Checks if donor is prospect (never donated)
        /// </summary>
        public bool IsProspect()
        {
            return DonationCount == 0;
        }

        /// <summary>
        /// Checks if donor is recurring (2+ donations)
        /// </summary>
        public bool IsRecurring()
        {
            return DonationCount >= 2;
        }

        /// <summary>
        /// Calculates donor's age (Individual only)
        /// </summary>
        public int? GetAge()
        {
            if (SubjectType != SubjectType.Individual || !BirthDate.HasValue)
                return null;

            var today = DateTime.UtcNow;
            var age = today.Year - BirthDate.Value.Year;

            if (BirthDate.Value.Date > today.AddYears(-age))
                age--;

            return age;
        }

        // ======================================================================
        // BUSINESS INVARIANTS - Automatic Checks
        // ======================================================================

        /// <summary>
        /// Verifies aggregate invariants
        /// Called automatically before saving
        /// </summary>
        internal void VerifyInvariants()
        {
            // Invariant 1: Individual must have FirstName and LastName
            if (SubjectType == SubjectType.Individual)
            {
                if (string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(LastName))
                    throw new BusinessException(
                        DonorErrorCodes.IndividualWithoutName,
                        "Individual must have FirstName and LastName"
                    );
            }

            // Invariant 2: Organization must have CompanyName
            if (SubjectType == SubjectType.Organization)
            {
                if (string.IsNullOrWhiteSpace(CompanyName))
                    throw new BusinessException(
                        DonorErrorCodes.OrganizationWithoutName,
                        "Organization must have CompanyName"
                    );
            }

            // Invariant 3: Title only for individuals
            if (TitleId.HasValue && SubjectType != SubjectType.Individual)
            {
                throw new BusinessException(
                    DonorErrorCodes.OrganizationCannotHaveTitle,
                    "Only individuals can have a title"
                );
            }

            // Invariant 4: At least 1 email for active donors
            if (Status == DonorStatus.Active && !Emails.Any())
            {
                throw new BusinessException(
                    DonorErrorCodes.ActiveDonorWithoutEmail,
                    "Active donor must have at least 1 email"
                );
            }

            // Invariant 5: Only 1 default email
            if (Emails.Count(e => e.IsDefault) > 1)
            {
                throw new BusinessException(
                    DonorErrorCodes.MultipleDefaultEmails,
                    "Only 1 email can be default"
                );
            }

            // Invariant 6: Only 1 default active address
            var defaultActiveAddresses = Addresses
                .Where(a => a.IsDefault && !a.EndDate.HasValue)
                .Count();

            if (defaultActiveAddresses > 1)
            {
                throw new BusinessException(
                    DonorErrorCodes.MultipleDefaultAddresses,
                    "Only 1 active address can be default"
                );
            }

            // Invariant 7: Privacy consent required for active donors
            if (Status != DonorStatus.New && !PrivacyConsent)
            {
                throw new BusinessException(
                    DonorErrorCodes.PrivacyConsentRequired,
                    "Privacy consent required for active donors"
                );
            }
        }
    }

    // ======================================================================
    // ERROR CODES
    // ======================================================================

    public static class DonorErrorCodes
    {
        // Subject type (Donor:001-004)
        public const string InvalidSubjectType = "Donor:001";
        public const string OrganizationCannotHaveTitle = "Donor:002";
        public const string IndividualWithoutName = "Donor:003";
        public const string OrganizationWithoutName = "Donor:004";

        // Email (Donor:010-019)
        public const string InvalidEmail = "Donor:010";
        public const string DuplicateEmail = "Donor:011";
        public const string EmailNotFound = "Donor:012";
        public const string MultipleDefaultEmails = "Donor:013";
        public const string ActiveDonorWithoutEmail = "Donor:014";
        public const string CannotRemoveOnlyEmail = "Donor:015";

        // Contacts (Donor:020-029)
        public const string InvalidContact = "Donor:020";
        public const string DuplicateContact = "Donor:021";
        public const string ContactNotFound = "Donor:022";
        public const string CannotRemoveOnlyContact = "Donor:023";

        // Addresses (Donor:030-039)
        public const string InvalidAddress = "Donor:030";
        public const string DuplicateAddress = "Donor:031";
        public const string AddressNotFound = "Donor:032";
        public const string MultipleDefaultAddresses = "Donor:033";
        public const string CannotSetInactiveAddressAsDefault = "Donor:034";

        // Privacy (Donor:040-049)
        public const string PrivacyConsentRequired = "Donor:040";
        public const string DonorAlreadyAnonymized = "Donor:041";
        public const string CannotContactDonor = "Donor:042";
        public const string CannotGrantConsentWithoutPrivacy = "Donor:043";

        // Statistics (Donor:050-059)
        public const string InconsistentStatistics = "Donor:050";

        // Communication (Donor:060-069)
        public const string CommunicationNotFound = "Donor:060";
    }
}
