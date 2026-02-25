using DonaRogApp.Domain.Projects.Entities;
using DonaRogApp.Domain.Recurrences.Entities;
using DonaRogApp.Enums.Communications;
using System;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace DonaRogApp.LetterTemplates
{
    /// <summary>
    /// Letter Template Aggregate Root
    /// 
    /// RESPONSIBILITY:
    /// - Store template content and metadata
    /// - Manage selection criteria for automatic template matching
    /// - Track usage statistics
    /// - Support versioning
    /// - Associate with projects and recurrences
    /// </summary>
    public class LetterTemplate : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {
        // ======================================================================
        // MULTI-TENANCY
        // ======================================================================
        public Guid? TenantId { get; set; }

        // ======================================================================
        // IDENTIFICATION
        // ======================================================================
        /// <summary>
        /// Template name
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Template description
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// HTML content with placeholders (e.g., {{DonorName}}, {{DonationAmount}})
        /// For Html templates: edited inline in WYSIWYG editor
        /// For Docx templates: result of DOCX→HTML conversion
        /// </summary>
        public string Content { get; set; } = null!;

        // ======================================================================
        // TEMPLATE FORMAT & FILE UPLOAD
        // ======================================================================
        /// <summary>
        /// Template format type (Html editor or Docx upload)
        /// </summary>
        public TemplateType TemplateType { get; set; }

        /// <summary>
        /// Path to uploaded template file (for Docx type)
        /// Null for Html type (content is in Content field)
        /// </summary>
        public string? TemplateFilePath { get; set; }

        /// <summary>
        /// Original filename of uploaded template
        /// </summary>
        public string? TemplateFileName { get; set; }

        /// <summary>
        /// File size in bytes
        /// </summary>
        public long? TemplateFileSizeBytes { get; set; }

        /// <summary>
        /// When template file was uploaded
        /// </summary>
        public DateTime? TemplateFileUploadedAt { get; set; }

        // ======================================================================
        // CATEGORIZATION
        // ======================================================================
        /// <summary>
        /// Template category (ThankYou, Newsletter, etc.)
        /// </summary>
        public TemplateCategory Category { get; set; }

        /// <summary>
        /// Language code (it, en, etc.)
        /// </summary>
        public string Language { get; set; } = "it";

        /// <summary>
        /// Communication type (Email, Letter, null = both)
        /// </summary>
        public CommunicationType? CommunicationType { get; set; }

        /// <summary>
        /// Email subject line (for email templates only)
        /// </summary>
        public string? EmailSubject { get; set; }

        // ======================================================================
        // SELECTION CRITERIA (for automatic template matching)
        // ======================================================================
        /// <summary>
        /// Project ID (optional - template specific to a project)
        /// </summary>
        public Guid? ProjectId { get; set; }

        /// <summary>
        /// Recurrence ID (optional - template for seasonal communications)
        /// </summary>
        public Guid? RecurrenceId { get; set; }

        /// <summary>
        /// Minimum donation amount for this template
        /// </summary>
        public decimal? MinAmount { get; set; }

        /// <summary>
        /// Maximum donation amount for this template
        /// </summary>
        public decimal? MaxAmount { get; set; }

        /// <summary>
        /// Is this template for new donors only?
        /// </summary>
        public bool IsForNewDonor { get; set; }

        /// <summary>
        /// Is this template for plural recipients (families, couples, organizations)?
        /// </summary>
        public bool IsPlural { get; set; }

        // ======================================================================
        // STATUS AND ACTIVATION
        // ======================================================================
        /// <summary>
        /// Is this template active and available for use?
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Is this the default/fallback template for its category and language?
        /// </summary>
        public bool IsDefault { get; set; }

        // ======================================================================
        // MULTI-RECIPIENT (for emails)
        // ======================================================================
        /// <summary>
        /// CC email addresses (semicolon separated)
        /// </summary>
        public string? CcEmails { get; set; }

        /// <summary>
        /// BCC email addresses (semicolon separated)
        /// </summary>
        public string? BccEmails { get; set; }

        // ======================================================================
        // STATISTICS
        // ======================================================================
        /// <summary>
        /// Number of times this template has been used
        /// </summary>
        public int UsageCount { get; private set; }

        /// <summary>
        /// Last date this template was used
        /// </summary>
        public DateTime? LastUsedDate { get; private set; }

        // ======================================================================
        // VERSIONING
        // ======================================================================
        /// <summary>
        /// Version number (incremented on updates)
        /// </summary>
        public int Version { get; private set; } = 1;

        /// <summary>
        /// Previous version ID (for version history tracking)
        /// </summary>
        public Guid? PreviousVersionId { get; set; }

        // ======================================================================
        // CATEGORIZATION (Advanced)
        // ======================================================================
        /// <summary>
        /// Custom tags (comma separated) for advanced categorization
        /// </summary>
        public string? Tags { get; set; }

        // ======================================================================
        // NAVIGATION PROPERTIES
        // ======================================================================
        /// <summary>
        /// Associated project (if ProjectId is set)
        /// </summary>
        public virtual Project? Project { get; set; }

        /// <summary>
        /// Associated recurrence (if RecurrenceId is set)
        /// </summary>
        public virtual Recurrence? Recurrence { get; set; }

        /// <summary>
        /// Template attachments
        /// </summary>
        public virtual ICollection<TemplateAttachment> Attachments { get; set; } = new List<TemplateAttachment>();

        // ======================================================================
        // BUSINESS METHODS - Usage Tracking
        // ======================================================================
        /// <summary>
        /// Increment usage counter when template is used
        /// </summary>
        public void IncrementUsage()
        {
            UsageCount++;
            LastUsedDate = DateTime.UtcNow;
        }

        /// <summary>
        /// Create a new version of this template
        /// </summary>
        public LetterTemplate CreateNewVersion()
        {
            var newVersion = new LetterTemplate
            {
                Id = Guid.NewGuid(),
                TenantId = TenantId,
                Name = Name,
                Description = Description,
                Content = Content,
                Category = Category,
                Language = Language,
                CommunicationType = CommunicationType,
                ProjectId = ProjectId,
                RecurrenceId = RecurrenceId,
                MinAmount = MinAmount,
                MaxAmount = MaxAmount,
                IsForNewDonor = IsForNewDonor,
                IsPlural = IsPlural,
                IsActive = IsActive,
                IsDefault = IsDefault,
                CcEmails = CcEmails,
                BccEmails = BccEmails,
                Tags = Tags,
                Version = Version + 1,
                PreviousVersionId = Id
            };

            return newVersion;
        }

        // ======================================================================
        // BUSINESS METHODS - Criteria Matching
        // ======================================================================
        /// <summary>
        /// Check if this template matches the given criteria
        /// </summary>
        public bool MatchesCriteria(
            decimal donationAmount,
            bool isNewDonor,
            bool isPlural,
            Guid? projectId = null,
            Guid? recurrenceId = null)
        {
            // Check active status
            if (!IsActive) return false;

            // Check amount range
            if (MinAmount.HasValue && donationAmount < MinAmount.Value) return false;
            if (MaxAmount.HasValue && donationAmount > MaxAmount.Value) return false;

            // Check new donor flag
            if (IsForNewDonor != isNewDonor) return false;

            // Check plural flag
            if (IsPlural != isPlural) return false;

            // Check project (if template is project-specific)
            if (ProjectId.HasValue && ProjectId.Value != projectId) return false;

            // Check recurrence (if template is recurrence-specific)
            if (RecurrenceId.HasValue && RecurrenceId.Value != recurrenceId) return false;

            return true;
        }

        /// <summary>
        /// Calculate match score for ranking (higher = better match)
        /// </summary>
        public int GetMatchScore(
            decimal donationAmount,
            bool isNewDonor,
            bool isPlural,
            Guid? projectId = null,
            Guid? recurrenceId = null)
        {
            if (!MatchesCriteria(donationAmount, isNewDonor, isPlural, projectId, recurrenceId))
                return 0;

            int score = 0;

            // More specific templates get higher scores
            if (ProjectId.HasValue && ProjectId.Value == projectId) score += 10;
            if (RecurrenceId.HasValue && RecurrenceId.Value == recurrenceId) score += 10;
            if (MinAmount.HasValue || MaxAmount.HasValue) score += 5;
            if (IsForNewDonor) score += 3;
            if (IsPlural) score += 3;

            return score;
        }

        // ======================================================================
        // BUSINESS METHODS - Template File Upload
        // ======================================================================
        /// <summary>
        /// Set template file information after upload (for Docx type)
        /// </summary>
        public void SetTemplateFile(
            string filePath,
            string fileName,
            long fileSizeBytes,
            string htmlContent)
        {
            if (TemplateType != TemplateType.Docx)
            {
                throw new BusinessException("DonaRog:CanOnlySetFileForDocxTemplates")
                    .WithData("templateId", Id)
                    .WithData("templateType", TemplateType);
            }

            TemplateFilePath = Check.NotNullOrWhiteSpace(filePath, nameof(filePath));
            TemplateFileName = Check.NotNullOrWhiteSpace(fileName, nameof(fileName));
            TemplateFileSizeBytes = fileSizeBytes;
            TemplateFileUploadedAt = DateTime.UtcNow;
            
            // Store converted HTML content
            Content = Check.NotNullOrWhiteSpace(htmlContent, nameof(htmlContent));
        }

        /// <summary>
        /// Update HTML content (for Html type or after Docx conversion)
        /// </summary>
        public void UpdateContent(string content)
        {
            Content = Check.NotNullOrWhiteSpace(content, nameof(content));
        }

        /// <summary>
        /// Update template type
        /// </summary>
        public void UpdateTemplateType(TemplateType templateType)
        {
            TemplateType = templateType;
            
            // Clear file info if switching to Html
            if (templateType == TemplateType.Html)
            {
                TemplateFilePath = null;
                TemplateFileName = null;
                TemplateFileSizeBytes = null;
                TemplateFileUploadedAt = null;
            }
        }

        /// <summary>
        /// Update email subject (for email templates)
        /// </summary>
        public void UpdateEmailSubject(string? subject)
        {
            EmailSubject = subject;
        }

        // ======================================================================
        // BUSINESS METHODS - Attachments
        // ======================================================================
        /// <summary>
        /// Add an attachment to this template
        /// </summary>
        public TemplateAttachment AddAttachment(
            Guid attachmentId,
            string fileName,
            string filePath,
            long fileSize,
            string? description = null)
        {
            var attachment = TemplateAttachment.Create(
                attachmentId,
                Id,
                fileName,
                filePath,
                fileSize,
                description
            );

            Attachments.Add(attachment);
            return attachment;
        }

        /// <summary>
        /// Remove an attachment
        /// </summary>
        public void RemoveAttachment(Guid attachmentId)
        {
            var attachment = Attachments.FirstOrDefault(a => a.Id == attachmentId);
            if (attachment != null)
            {
                Attachments.Remove(attachment);
            }
        }

        // ======================================================================
        // INVARIANTS
        // ======================================================================
        /// <summary>
        /// Verify business invariants
        /// </summary>
        public void VerifyInvariants()
        {
            Check.NotNullOrWhiteSpace(Name, nameof(Name));
            Check.NotNullOrWhiteSpace(Content, nameof(Content));
            Check.NotNullOrWhiteSpace(Language, nameof(Language));

            if (MinAmount.HasValue && MinAmount.Value < 0)
            {
                throw new BusinessException("DonaRog:TemplateNegativeMinAmount")
                    .WithData("minAmount", MinAmount.Value);
            }

            if (MaxAmount.HasValue && MaxAmount.Value < 0)
            {
                throw new BusinessException("DonaRog:TemplateNegativeMaxAmount")
                    .WithData("maxAmount", MaxAmount.Value);
            }

            if (MinAmount.HasValue && MaxAmount.HasValue && MinAmount.Value > MaxAmount.Value)
            {
                throw new BusinessException("DonaRog:TemplateMinAmountGreaterThanMax")
                    .WithData("minAmount", MinAmount.Value)
                    .WithData("maxAmount", MaxAmount.Value);
            }
        }
    }
}
