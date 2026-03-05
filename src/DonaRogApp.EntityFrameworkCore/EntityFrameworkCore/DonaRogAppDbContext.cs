using DonaRogApp.Domain.BankAccounts.Entities;
using DonaRogApp.Domain.Campaigns.Entities;
using DonaRogApp.Domain.Communications.Entities;
using DonaRogApp.Domain.Donations.Entities;
using DonaRogApp.Domain.Donors.Entities;
using DonaRogApp.Domain.Recurrences.Entities;
using DonaRogApp.Domain.Segmentation.Entities;
using DonaRogApp.Domain.Shared.Entities;
using DonaRogApp.LetterTemplates;
using DonaRogApp.Notes.Entities;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.BlobStoring.Database.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.OpenIddict.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.TenantManagement;
using Volo.Abp.TenantManagement.EntityFrameworkCore;

namespace DonaRogApp.EntityFrameworkCore;

[ReplaceDbContext(typeof(IIdentityDbContext))]
[ReplaceDbContext(typeof(ITenantManagementDbContext))]
[ConnectionStringName("Default")]
public class DonaRogAppDbContext :
    AbpDbContext<DonaRogAppDbContext>,
    ITenantManagementDbContext,
    IIdentityDbContext
{
    #region Donor Aggregate Root & Child Entities

    public DbSet<Donor> Donors { get; set; }
    public DbSet<DonorEmail> DonorEmails { get; set; }
    public DbSet<DonorContact> DonorContacts { get; set; }
    public DbSet<DonorAddress> DonorAddresses { get; set; }
    public DbSet<DonorNote> DonorNotes { get; set; }
    public DbSet<DonorStatusHistory> DonorStatusHistories { get; set; }
    public DbSet<DonorAttachment> DonorAttachments { get; set; }
    public DbSet<Communication> Communications { get; set; }

    #endregion

    #region Donor Many-to-Many Junctions

    public DbSet<DonorSegment> DonorSegments { get; set; }
    public DbSet<DonorTag> DonorTags { get; set; }
    public DbSet<DonorInterest> DonorInterests { get; set; }

    #endregion

    #region Recurrence Aggregate Root (Annual Recurring Periods)

    public DbSet<Recurrence> Recurrences { get; set; }

    #endregion

    #region Campaign Aggregate Root & Junction

    public DbSet<Campaign> Campaigns { get; set; }
    public DbSet<CampaignDonor> CampaignDonors { get; set; }

    #endregion

    #region Project Aggregate Root & Child Entities

    public DbSet<Domain.Projects.Entities.Project> Projects { get; set; }
    public DbSet<Domain.Projects.Entities.ProjectDocument> ProjectDocuments { get; set; }

    #endregion

    #region Letter Template Aggregate Root & Child Entities

    public DbSet<LetterTemplate> LetterTemplates { get; set; }
    public DbSet<TemplateAttachment> TemplateAttachments { get; set; }

    #endregion

    #region Communications Aggregate Roots

    public DbSet<PrintBatch> PrintBatches { get; set; }
    public DbSet<ThankYouRule> ThankYouRules { get; set; }
    public DbSet<RuleTemplateAssociation> RuleTemplateAssociations { get; set; }
    public DbSet<DonorTemplateUsage> DonorTemplateUsages { get; set; }

    #endregion

    #region Segmentation Aggregate Roots

    public DbSet<SegmentationRule> SegmentationRules { get; set; }

    #endregion

    #region Shared Entities (Lookup Tables)

    public DbSet<Title> Titles { get; set; }
    public DbSet<Segment> Segments { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<Interest> Interests { get; set; }

    #endregion

    #region Bank Account Aggregate Root

    public DbSet<Domain.BankAccounts.Entities.BankAccount> BankAccounts { get; set; }

    #endregion

    #region Donation Aggregate Root & Junction

    public DbSet<Domain.Donations.Entities.Donation> Donations { get; set; }
    public DbSet<Domain.Donations.Entities.DonationProject> DonationProjects { get; set; }
    public DbSet<Domain.Donations.Entities.DonationDocument> DonationDocuments { get; set; }

    #endregion

    #region Entities from the modules

    // Identity
    public DbSet<IdentityUser> Users { get; set; }
    public DbSet<IdentityRole> Roles { get; set; }
    public DbSet<IdentityClaimType> ClaimTypes { get; set; }
    public DbSet<OrganizationUnit> OrganizationUnits { get; set; }
    public DbSet<IdentitySecurityLog> SecurityLogs { get; set; }
    public DbSet<IdentityLinkUser> LinkUsers { get; set; }
    public DbSet<IdentityUserDelegation> UserDelegations { get; set; }
    public DbSet<IdentitySession> Sessions { get; set; }

    // Tenant Management
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<TenantConnectionString> TenantConnectionStrings { get; set; }

    #endregion

    public DonaRogAppDbContext(DbContextOptions<DonaRogAppDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        /* Include modules to your migration db context */

        builder.ConfigurePermissionManagement();
        builder.ConfigureSettingManagement();
        builder.ConfigureBackgroundJobs();
        builder.ConfigureAuditLogging();
        builder.ConfigureFeatureManagement();
        builder.ConfigureIdentity();
        builder.ConfigureOpenIddict();
        builder.ConfigureTenantManagement();
        builder.ConfigureBlobStoring();

        /* Configure Donor Aggregate */

        // DONOR AGGREGATE ROOT
        builder.Entity<Donor>(b =>
        {
            b.ToTable(DonaRogAppConsts.DbTablePrefix + "Donors", DonaRogAppConsts.DbSchema);
            b.ConfigureByConvention();

            b.HasKey(x => x.Id);
            b.Property(x => x.DonorCode).IsRequired().HasMaxLength(50);
            b.Property(x => x.FirstName).HasMaxLength(128);
            b.Property(x => x.MiddleName).HasMaxLength(128);
            b.Property(x => x.LastName).HasMaxLength(128);
            b.Property(x => x.CompanyName).HasMaxLength(255);
            b.Property(x => x.BusinessSector).HasMaxLength(128);
            b.Property(x => x.BirthPlace).HasMaxLength(128);
            b.Property(x => x.PreferredLanguage).HasMaxLength(5).HasDefaultValue("IT");
            b.Property(x => x.PreferredChannel).HasMaxLength(50);
            b.Property(x => x.Notes).HasMaxLength(1000);
            b.Property(x => x.RfmSegment).HasMaxLength(50);

            // Foreign key to Title
            b.HasOne<Title>()
                .WithMany()
                .HasForeignKey(x => x.TitleId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            // Child collections
            b.HasMany(x => x.Emails)
                .WithOne(x => x.Donor)
                .HasForeignKey(x => x.DonorId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasMany(x => x.Contacts)
                .WithOne(x => x.Donor)
                .HasForeignKey(x => x.DonorId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasMany(x => x.Addresses)
                .WithOne(x => x.Donor)
                .HasForeignKey(x => x.DonorId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasMany(x => x.HistoricalNotes)
                .WithOne(x => x.Donor)
                .HasForeignKey(x => x.DonorId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasMany(x => x.Communications)
                .WithOne(x => x.Donor)
                .HasForeignKey(x => x.DonorId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasMany(x => x.Segments)
                .WithOne(x => x.Donor)
                .HasForeignKey(x => x.DonorId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasMany(x => x.Tags)
                .WithOne(x => x.Donor)
                .HasForeignKey(x => x.DonorId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasMany(x => x.Interests)
                .WithOne(x => x.Donor)
                .HasForeignKey(x => x.DonorId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasMany(x => x.Attachments)
                .WithOne(x => x.Donor)
                .HasForeignKey(x => x.DonorId)
                .OnDelete(DeleteBehavior.Cascade);
            
            b.OwnsOne(x => x.TaxCode, tb =>
            {
                tb.Property(p => p.Value).HasColumnName("TaxCode").HasMaxLength(16);
            });

            b.OwnsOne(x => x.VatNumber, vb =>
            {
                vb.Property(p => p.Value).HasColumnName("VatNumber").HasMaxLength(11);
            });

            // Indexes
            b.HasIndex(x => x.DonorCode).IsUnique();
            b.HasIndex(x => x.Status);
            b.HasIndex(x => x.Category);
            b.HasIndex(x => x.TenantId);
        });

        // DONOR EMAIL
        builder.Entity<DonorEmail>(b =>
        {
            b.ToTable(DonaRogAppConsts.DbTablePrefix + "DonorEmails", DonaRogAppConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.EmailAddress).IsRequired().HasMaxLength(256);
            b.Property(x => x.LastBounceReason).HasMaxLength(256);
            b.Property(x => x.VerificationToken).HasMaxLength(500);
            b.Property(x => x.Notes).HasMaxLength(500);

            b.HasIndex(x => new { x.DonorId, x.EmailAddress }).IsUnique();
            b.HasIndex(x => x.IsVerified);
            b.HasIndex(x => x.IsInvalid);
        });

        // DONOR CONTACT
        builder.Entity<DonorContact>(b =>
        {
            b.ToTable(DonaRogAppConsts.DbTablePrefix + "DonorContacts", DonaRogAppConsts.DbSchema);
            b.ConfigureByConvention();

            b.OwnsOne(x => x.PhoneNumber, pb =>
            {
                pb.Property(p => p.CountryCode).HasColumnName("PhoneCountryCode").HasMaxLength(3);
                pb.Property(p => p.NationalNumber).HasColumnName("PhoneNationalNumber").HasMaxLength(14);
            });

            b.Property(x => x.Notes).HasMaxLength(500);

            b.HasIndex(x => x.IsVerified);
        });

        // DONOR ADDRESS
        builder.Entity<DonorAddress>(b =>
        {
            b.ToTable(DonaRogAppConsts.DbTablePrefix + "DonorAddresses", DonaRogAppConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.Street).IsRequired().HasMaxLength(256);
            b.Property(x => x.City).IsRequired().HasMaxLength(128);
            b.Property(x => x.Province).HasMaxLength(50);
            b.Property(x => x.Region).HasMaxLength(128);
            b.Property(x => x.PostalCode).IsRequired().HasMaxLength(20);
            b.Property(x => x.Country).IsRequired().HasMaxLength(100);
            b.Property(x => x.Notes).HasMaxLength(500);

            b.HasIndex(x => new { x.DonorId, x.StartDate });
            b.HasIndex(x => x.IsVerified);
        });

        // DONOR NOTE
        builder.Entity<DonorNote>(b =>
        {
            b.ToTable(DonaRogAppConsts.DbTablePrefix + "DonorNotes", DonaRogAppConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.Content).IsRequired().HasMaxLength(1000);
            b.Property(x => x.Category).HasMaxLength(50);
        });

        // DONOR STATUS HISTORY
        builder.Entity<DonorStatusHistory>(b =>
        {
            b.ToTable(DonaRogAppConsts.DbTablePrefix + "DonorStatusHistories", DonaRogAppConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.Note).HasMaxLength(500);
            
            b.HasOne(x => x.Donor)
                .WithMany()
                .HasForeignKey(x => x.DonorId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasIndex(x => new { x.DonorId, x.ChangedAt });
        });

        // DONOR ATTACHMENT
        builder.Entity<DonorAttachment>(b =>
        {
            b.ToTable(DonaRogAppConsts.DbTablePrefix + "DonorAttachments", DonaRogAppConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.FileName).IsRequired().HasMaxLength(255);
            b.Property(x => x.FileExtension).IsRequired().HasMaxLength(20);
            b.Property(x => x.MimeType).IsRequired().HasMaxLength(100);
            b.Property(x => x.BlobName).IsRequired().HasMaxLength(500);
            b.Property(x => x.AttachmentType).HasMaxLength(50);
            b.Property(x => x.Description).HasMaxLength(1000);

            b.HasIndex(x => x.DonorId);
            b.HasIndex(x => new { x.DonorId, x.DisplayOrder });
            b.HasIndex(x => x.AttachmentType);
            b.HasIndex(x => x.CreationTime);
        });

        // COMMUNICATION
        builder.Entity<Communication>(b =>
        {
            b.ToTable(DonaRogAppConsts.DbTablePrefix + "Communications", DonaRogAppConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.Subject).IsRequired().HasMaxLength(500);
            b.Property(x => x.Recipient).IsRequired().HasMaxLength(256);
            b.Property(x => x.Body).HasColumnType("TEXT");
            b.Property(x => x.FailureReason).HasMaxLength(500);
            b.Property(x => x.ExternalId).HasMaxLength(256);
            b.Property(x => x.Notes).HasMaxLength(500);

            // Relationship with PrintBatch
            b.HasOne(x => x.PrintBatch)
                .WithMany(x => x.Communications)
                .HasForeignKey(x => x.PrintBatchId)
                .OnDelete(DeleteBehavior.SetNull);

            b.HasIndex(x => new { x.DonorId, x.SentDate });
            b.HasIndex(x => x.Type);
            b.HasIndex(x => x.Status);
            b.HasIndex(x => x.IsDelivered);
            b.HasIndex(x => x.IsFailed);
            b.HasIndex(x => x.IsPrinted);
            b.HasIndex(x => x.PrintBatchId);
        });

        /* Configure M:M Junctions */

        // DONOR SEGMENT (M:M)
        builder.Entity<DonorSegment>(b =>
        {
            b.ToTable(DonaRogAppConsts.DbTablePrefix + "DonorSegments", DonaRogAppConsts.DbSchema);
            b.ConfigureByConvention();

            b.HasKey(x => new { x.DonorId, x.SegmentId });

            b.Property(x => x.AssignmentNotes).HasMaxLength(500);
            b.Property(x => x.AutomaticReason).HasMaxLength(256);

            b.HasOne<Donor>()
                .WithMany(x => x.Segments)
                .HasForeignKey(x => x.DonorId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasOne<Segment>()
                .WithMany()
                .HasForeignKey(x => x.SegmentId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasIndex(x => new { x.DonorId, x.RemovedAt });
        });

        // DONOR TAG (M:M)
        builder.Entity<DonorTag>(b =>
        {
            b.ToTable(DonaRogAppConsts.DbTablePrefix + "DonorTags", DonaRogAppConsts.DbSchema);
            b.ConfigureByConvention();

            b.HasKey(x => new { x.DonorId, x.TagId });

            b.Property(x => x.TaggingReason).HasMaxLength(256);

            b.HasOne<Donor>()
                .WithMany(x => x.Tags)
                .HasForeignKey(x => x.DonorId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasOne<Tag>()
                .WithMany()
                .HasForeignKey(x => x.TagId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasIndex(x => new { x.DonorId, x.RemovedAt });
        });

        // DONOR INTEREST (M:M)
        builder.Entity<DonorInterest>(b =>
        {
            b.ToTable(DonaRogAppConsts.DbTablePrefix + "DonorInterests", DonaRogAppConsts.DbSchema);
            b.ConfigureByConvention();

            b.HasKey(x => new { x.DonorId, x.InterestId });

            b.Property(x => x.DiscoveryMethod).HasMaxLength(256);

            b.HasOne<Donor>()
                .WithMany(x => x.Interests)
                .HasForeignKey(x => x.DonorId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasOne<Interest>()
                .WithMany()
                .HasForeignKey(x => x.InterestId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasIndex(x => new { x.DonorId, x.RemovedAt });
        });

        /* Configure Shared Entities (Lookup Tables) */

        // TITLE
        builder.Entity<Title>(b =>
        {
            b.ToTable(DonaRogAppConsts.DbTablePrefix + "Titles", DonaRogAppConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.Code).IsRequired().HasMaxLength(50);
            b.Property(x => x.Name).IsRequired().HasMaxLength(128);
            b.Property(x => x.Abbreviation).IsRequired().HasMaxLength(10);

            // Indice univoco su Code + TenantId per supportare multi-tenancy
            b.HasIndex(x => new { x.TenantId, x.Code }).IsUnique();
            b.HasIndex(x => x.IsActive);
        });

        // SEGMENT
        builder.Entity<Segment>(b =>
        {
            b.ToTable(DonaRogAppConsts.DbTablePrefix + "Segments", DonaRogAppConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.Code).IsRequired().HasMaxLength(50);
            b.Property(x => x.Name).IsRequired().HasMaxLength(128);
            b.Property(x => x.Description).HasMaxLength(500);
            b.Property(x => x.ColorCode).HasMaxLength(7);
            b.Property(x => x.Icon).HasMaxLength(50);

            // Indice univoco su Code + TenantId per supportare multi-tenancy
            b.HasIndex(x => new { x.TenantId, x.Code }).IsUnique();
            b.HasIndex(x => x.IsActive);
            b.HasIndex(x => x.IsSystem);
        });

        // TAG
        builder.Entity<Tag>(b =>
        {
            b.ToTable(DonaRogAppConsts.DbTablePrefix + "Tags", DonaRogAppConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.Code).IsRequired().HasMaxLength(128);
            b.Property(x => x.Name).IsRequired().HasMaxLength(128);
            b.Property(x => x.Category).HasMaxLength(50);

            // Indice univoco su Code + TenantId per supportare multi-tenancy
            b.HasIndex(x => new { x.TenantId, x.Code }).IsUnique();
            b.HasIndex(x => x.IsActive);
            b.HasIndex(x => x.IsSystem);
        });

        // INTEREST
        builder.Entity<Interest>(b =>
        {
            b.ToTable(DonaRogAppConsts.DbTablePrefix + "Interests", DonaRogAppConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.Code).IsRequired().HasMaxLength(50);
            b.Property(x => x.Name).IsRequired().HasMaxLength(128);
            b.Property(x => x.Category).HasMaxLength(50);
            b.Property(x => x.Icon).HasMaxLength(50);
            b.Property(x => x.ColorCode).HasMaxLength(7);

            // Indice univoco su Code + TenantId per supportare multi-tenancy
            b.HasIndex(x => new { x.TenantId, x.Code }).IsUnique();
            b.HasIndex(x => x.IsActive);
        });

        /* Configure Recurrence Aggregate */

        // RECURRENCE
        builder.Entity<Recurrence>(b =>
        {
            b.ToTable(DonaRogAppConsts.DbTablePrefix + "Recurrences", DonaRogAppConsts.DbSchema);
            b.ConfigureByConvention();

            // Properties
            b.Property(x => x.Name).IsRequired().HasMaxLength(256);
            b.Property(x => x.Code).HasMaxLength(64); // Non più obbligatorio
            b.Property(x => x.Description).HasMaxLength(2000);
            b.Property(x => x.Notes).HasMaxLength(2000);
            b.Property(x => x.DeactivationReason).HasMaxLength(512);

            // Indexes
            b.HasIndex(x => new { x.TenantId, x.Code }).IsUnique().HasFilter("\"Code\" IS NOT NULL");
            b.HasIndex(x => x.IsActive);
            b.HasIndex(x => new { x.RecurrenceMonth, x.RecurrenceDay });
        });

        /* Configure Campaign Aggregate */

        // CAMPAIGN
        builder.Entity<Campaign>(b =>
        {
            b.ToTable(DonaRogAppConsts.DbTablePrefix + "Campaigns", DonaRogAppConsts.DbSchema);
            b.ConfigureByConvention();

            // Properties
            b.Property(x => x.Name).IsRequired().HasMaxLength(256);
            b.Property(x => x.Code).IsRequired().HasMaxLength(64);
            b.Property(x => x.Description).HasMaxLength(2000);
            b.Property(x => x.MailchimpCampaignId).HasMaxLength(128);
            b.Property(x => x.MailchimpListId).HasMaxLength(128);
            b.Property(x => x.SmsProviderId).HasMaxLength(128);

            // Indexes
            b.HasIndex(x => new { x.TenantId, x.Year, x.Code }).IsUnique();
            b.HasIndex(x => x.Status);
            b.HasIndex(x => x.Channel);
            b.HasIndex(x => x.Year);
            b.HasIndex(x => x.RecurrenceId);
            b.HasIndex(x => x.CampaignType);
            b.HasIndex(x => x.DispatchDate);
            b.HasIndex(x => new { x.Year, x.Status });

            // Owned types (PostalCode674: format NNNNYY)
            b.OwnsOne(x => x.PostalCode, postal =>
            {
                postal.Property(p => p.SequenceNumber).HasColumnName("PostalCodeSequence");
                postal.Property(p => p.YearSuffix).HasColumnName("PostalCodeYearSuffix");
            });

            // Relationships
            b.HasOne(x => x.Recurrence)
                .WithMany()
                .HasForeignKey(x => x.RecurrenceId)
                .OnDelete(DeleteBehavior.SetNull);

            b.HasMany(x => x.CampaignDonors)
                .WithOne(x => x.Campaign)
                .HasForeignKey(x => x.CampaignId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // CAMPAIGN DONOR (M:M with tracking)
        builder.Entity<CampaignDonor>(b =>
        {
            b.ToTable(DonaRogAppConsts.DbTablePrefix + "CampaignDonors", DonaRogAppConsts.DbSchema);

            b.HasKey(x => new { x.CampaignId, x.DonorId });

            b.Property(x => x.Notes).HasMaxLength(1000);

            // Owned type for tracking code
            b.OwnsOne(x => x.TrackingCode, tracking =>
            {
                tracking.Property(t => t.Code).HasColumnName("TrackingCode");
                tracking.Property(t => t.CampaignId).HasColumnName("TrackingCampaignId");
                tracking.Property(t => t.DonorId).HasColumnName("TrackingDonorId");
                tracking.Property(t => t.Hash).HasColumnName("TrackingHash").HasMaxLength(64);
            });

            // Relationship to Donor - NO ACTION to avoid cascade delete cycles
            b.HasOne(x => x.Donor)
                .WithMany(d => d.CampaignParticipations)
                .HasForeignKey(x => x.DonorId)
                .OnDelete(DeleteBehavior.NoAction);

            b.HasIndex(x => x.ExtractedAt);
            b.HasIndex(x => x.DispatchedAt);
            b.HasIndex(x => x.OpenedAt);
            b.HasIndex(x => x.ClickedAt);
            b.HasIndex(x => x.ResponseType);
            b.HasIndex(x => x.DonationDate);
            b.HasIndex(x => new { x.CampaignId, x.ResponseType });
        });

        /* Configure Project Aggregate */

        // PROJECT AGGREGATE ROOT
        builder.Entity<Domain.Projects.Entities.Project>(b =>
        {
            b.ToTable(DonaRogAppConsts.DbTablePrefix + "Projects", DonaRogAppConsts.DbSchema);
            b.ConfigureByConvention();

            // Properties
            b.Property(x => x.Code).IsRequired().HasMaxLength(50);
            b.Property(x => x.Name).IsRequired().HasMaxLength(200);
            b.Property(x => x.Description).HasMaxLength(2000);
            b.Property(x => x.Currency).IsRequired().HasMaxLength(3).HasDefaultValue("EUR");
            b.Property(x => x.ResponsiblePerson).HasMaxLength(100);
            b.Property(x => x.ResponsibleEmail).HasMaxLength(256);
            b.Property(x => x.ResponsiblePhone).HasMaxLength(50);
            b.Property(x => x.MainImageUrl).HasMaxLength(1000);
            b.Property(x => x.ThumbnailUrl).HasMaxLength(1000);
            b.Property(x => x.Location).HasMaxLength(200);
            b.Property(x => x.Latitude).HasPrecision(10, 7);
            b.Property(x => x.Longitude).HasPrecision(10, 7);
            b.Property(x => x.TargetAmount).HasPrecision(18, 2);
            b.Property(x => x.TotalAmountRaised).HasPrecision(18, 2);
            b.Property(x => x.AverageDonation).HasPrecision(18, 2);

            // Relationships
            b.HasMany(x => x.Documents)
                .WithOne(x => x.Project)
                .HasForeignKey(x => x.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            b.HasIndex(x => new { x.TenantId, x.Code })
                .IsUnique()
                .HasFilter("\"TenantId\" IS NOT NULL");
            
            b.HasIndex(x => x.Status);
            b.HasIndex(x => x.Category);
            b.HasIndex(x => x.StartDate);
            b.HasIndex(x => x.EndDate);
            b.HasIndex(x => new { x.Status, x.Category });
        });

        // PROJECT DOCUMENT (Child Entity)
        builder.Entity<Domain.Projects.Entities.ProjectDocument>(b =>
        {
            b.ToTable(DonaRogAppConsts.DbTablePrefix + "ProjectDocuments", DonaRogAppConsts.DbSchema);
            b.ConfigureByConvention();

            // Properties
            b.Property(x => x.FileName).IsRequired().HasMaxLength(255);
            b.Property(x => x.FileUrl).IsRequired().HasMaxLength(1000);
            b.Property(x => x.FileType).HasMaxLength(50);
            b.Property(x => x.Description).HasMaxLength(500);

            // Indexes
            b.HasIndex(x => x.ProjectId);
            b.HasIndex(x => new { x.ProjectId, x.DisplayOrder });
        });

        /* Configure Letter Template Aggregate */

        // LETTER TEMPLATE AGGREGATE ROOT
        builder.Entity<LetterTemplate>(b =>
        {
            b.ToTable(DonaRogAppConsts.DbTablePrefix + "LetterTemplates", DonaRogAppConsts.DbSchema);
            b.ConfigureByConvention();

            // Properties
            b.Property(x => x.Name).IsRequired().HasMaxLength(256);
            b.Property(x => x.Description).HasMaxLength(1000);
            b.Property(x => x.Content).IsRequired().HasColumnType("TEXT");
            b.Property(x => x.Language).IsRequired().HasMaxLength(5).HasDefaultValue("it");
            b.Property(x => x.CcEmails).HasMaxLength(500);
            b.Property(x => x.BccEmails).HasMaxLength(500);
            b.Property(x => x.Tags).HasMaxLength(500);

            // Relationships
            b.HasMany(x => x.Attachments)
                .WithOne(x => x.Template)
                .HasForeignKey(x => x.TemplateId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            b.HasIndex(x => x.Category);
            b.HasIndex(x => x.Language);
            b.HasIndex(x => x.IsActive);
            b.HasIndex(x => x.IsDefault);
            b.HasIndex(x => new { x.TenantId, x.Category, x.Language });
            b.HasIndex(x => new { x.Category, x.Language, x.IsActive });
            b.HasIndex(x => x.LastUsedDate);
        });

        // TEMPLATE ATTACHMENT (Child Entity)
        builder.Entity<TemplateAttachment>(b =>
        {
            b.ToTable(DonaRogAppConsts.DbTablePrefix + "TemplateAttachments", DonaRogAppConsts.DbSchema);
            b.ConfigureByConvention();

            // Properties
            b.Property(x => x.FileName).IsRequired().HasMaxLength(255);
            b.Property(x => x.FilePath).IsRequired().HasMaxLength(1000);
            b.Property(x => x.Description).HasMaxLength(500);

            // Indexes
            b.HasIndex(x => x.TemplateId);
        });

        /* Configure BankAccount Aggregate */

        // BANK ACCOUNT AGGREGATE ROOT
        builder.Entity<BankAccount>(b =>
        {
            b.ToTable(DonaRogAppConsts.DbTablePrefix + "BankAccounts", DonaRogAppConsts.DbSchema);
            b.ConfigureByConvention();

            // Properties
            b.Property(x => x.AccountName).IsRequired().HasMaxLength(200);
            b.Property(x => x.BankName).HasMaxLength(100);
            b.Property(x => x.Swift).HasMaxLength(11);
            b.Property(x => x.Notes).HasMaxLength(500);

            // IBAN as owned entity (value object)
            b.OwnsOne(x => x.Iban, ib =>
            {
                ib.Property(p => p.Value).HasColumnName("Iban").IsRequired().HasMaxLength(34);
                ib.Property(p => p.CountryCode).HasColumnName("IbanCountryCode").HasMaxLength(2);
                ib.Property(p => p.CheckDigits).HasColumnName("IbanCheckDigits").HasMaxLength(2);
                ib.Property(p => p.BBAN).HasColumnName("IbanBBAN").HasMaxLength(30);
            });

            // Indexes
            b.HasIndex(x => x.IsActive);
            b.HasIndex(x => x.IsDefault);
            b.HasIndex(x => new { x.TenantId, x.IsActive });
        });

        /* Configure Donation Aggregate */

        // DONATION AGGREGATE ROOT
        builder.Entity<Donation>(b =>
        {
            b.ToTable(DonaRogAppConsts.DbTablePrefix + "Donations", DonaRogAppConsts.DbSchema);
            b.ConfigureByConvention();

            // Properties
            b.Property(x => x.Reference).IsRequired().HasMaxLength(50);
            b.Property(x => x.ExternalId).HasMaxLength(100);
            b.Property(x => x.TotalAmount).IsRequired().HasPrecision(18, 2);
            b.Property(x => x.Currency).IsRequired().HasMaxLength(3).HasDefaultValue("EUR");
            b.Property(x => x.Notes).HasMaxLength(1000);
            b.Property(x => x.InternalNotes).HasMaxLength(1000);
            b.Property(x => x.RejectionNotes).HasMaxLength(1000);

            // Relationships
            b.HasOne(x => x.Donor)
                .WithMany()
                .HasForeignKey(x => x.DonorId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasOne(x => x.Campaign)
                .WithMany()
                .HasForeignKey(x => x.CampaignId)
                .OnDelete(DeleteBehavior.SetNull);

            b.HasOne(x => x.BankAccount)
                .WithMany()
                .HasForeignKey(x => x.BankAccountId)
                .OnDelete(DeleteBehavior.SetNull);

            b.HasOne(x => x.ThankYouTemplate)
                .WithMany()
                .HasForeignKey(x => x.ThankYouTemplateId)
                .OnDelete(DeleteBehavior.SetNull);

            b.HasMany(x => x.Projects)
                .WithOne(x => x.Donation)
                .HasForeignKey(x => x.DonationId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasMany(x => x.Documents)
                .WithOne()
                .HasForeignKey(x => x.DonationId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            b.HasIndex(x => x.Reference);
            b.HasIndex(x => x.ExternalId).IsUnique().HasFilter("\"ExternalId\" IS NOT NULL");
            b.HasIndex(x => x.Status);
            b.HasIndex(x => x.Channel);
            b.HasIndex(x => x.DonorId);
            b.HasIndex(x => x.CampaignId);
            b.HasIndex(x => x.DonationDate);
            b.HasIndex(x => x.CreditDate);
            b.HasIndex(x => new { x.Status, x.DonationDate });
            b.HasIndex(x => new { x.DonorId, x.Status });
            b.HasIndex(x => new { x.CampaignId, x.Status });
            b.HasIndex(x => x.TenantId);
        });

        // DONATION PROJECT (Junction Table with Amount)
        builder.Entity<DonationProject>(b =>
        {
            b.ToTable(DonaRogAppConsts.DbTablePrefix + "DonationProjects", DonaRogAppConsts.DbSchema);
            b.ConfigureByConvention();

            // Composite key
            b.HasKey(x => new { x.DonationId, x.ProjectId });

            // Properties
            b.Property(x => x.AllocatedAmount).IsRequired().HasPrecision(18, 2);

            // Relationships
            b.HasOne(x => x.Project)
                .WithMany()
                .HasForeignKey(x => x.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            b.HasIndex(x => x.ProjectId);
            b.HasIndex(x => x.DonationId);
        });

        // DONATION DOCUMENT (Child Entity)
        builder.Entity<DonationDocument>(b =>
        {
            b.ToTable(DonaRogAppConsts.DbTablePrefix + "DonationDocuments", DonaRogAppConsts.DbSchema);
            b.ConfigureByConvention();

            // Properties - nullable for text-only documents
            b.Property(x => x.FileName).HasMaxLength(255);
            b.Property(x => x.FileExtension).HasMaxLength(20);
            b.Property(x => x.MimeType).HasMaxLength(100);
            b.Property(x => x.StoragePath).HasMaxLength(1000);
            b.Property(x => x.TextContent).HasMaxLength(4000);
            b.Property(x => x.Notes).HasMaxLength(500);

            // Indexes
            b.HasIndex(x => x.DonationId);
            b.HasIndex(x => x.DocumentType);
            b.HasIndex(x => x.IsFromExternalFlow);
            b.HasIndex(x => new { x.DonationId, x.CreationTime });
        });

        /* Configure Communications Aggregates */

        // PRINT BATCH AGGREGATE ROOT
        builder.Entity<PrintBatch>(b =>
        {
            b.ToTable(DonaRogAppConsts.DbTablePrefix + "PrintBatches", DonaRogAppConsts.DbSchema);
            b.ConfigureByConvention();

            // Properties
            b.Property(x => x.BatchNumber).IsRequired().HasMaxLength(50);
            b.Property(x => x.Name).HasMaxLength(200);
            b.Property(x => x.PdfFilePath).HasMaxLength(500);
            b.Property(x => x.CancellationReason).HasMaxLength(500);
            b.Property(x => x.TotalDonationAmount).HasPrecision(18, 2);
            b.Property(x => x.FilterJson).HasColumnType("TEXT");
            b.Property(x => x.MinAmount).HasPrecision(18, 2);
            b.Property(x => x.MaxAmount).HasPrecision(18, 2);
            b.Property(x => x.Region).HasMaxLength(128);
            b.Property(x => x.ProjectIds).HasMaxLength(500);
            b.Property(x => x.CampaignIds).HasMaxLength(500);
            b.Property(x => x.Notes).HasMaxLength(1000);

            // Indexes
            b.HasIndex(x => x.BatchNumber).IsUnique();
            b.HasIndex(x => x.Status);
            b.HasIndex(x => x.GeneratedAt);
            b.HasIndex(x => x.PrintedAt);
            b.HasIndex(x => new { x.Status, x.GeneratedAt });
            b.HasIndex(x => x.TenantId);
        });

        // THANK YOU RULE AGGREGATE ROOT
        builder.Entity<ThankYouRule>(b =>
        {
            b.ToTable(DonaRogAppConsts.DbTablePrefix + "ThankYouRules", DonaRogAppConsts.DbSchema);
            b.ConfigureByConvention();

            // Properties
            b.Property(x => x.Name).IsRequired().HasMaxLength(200);
            b.Property(x => x.Description).HasMaxLength(1000);
            b.Property(x => x.MinAmount).HasPrecision(18, 2);
            b.Property(x => x.MaxAmount).HasPrecision(18, 2);

            // Relationships
            b.HasOne(x => x.Recurrence)
                .WithMany()
                .HasForeignKey(x => x.RecurrenceId)
                .OnDelete(DeleteBehavior.SetNull);

            b.HasMany(x => x.TemplateAssociations)
                .WithOne(x => x.Rule)
                .HasForeignKey(x => x.RuleId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            b.HasIndex(x => x.IsActive);
            b.HasIndex(x => new { x.IsActive, x.Priority });
            b.HasIndex(x => x.ProjectId);
            b.HasIndex(x => x.CampaignId);
            b.HasIndex(x => x.RecurrenceId);
            b.HasIndex(x => x.ValidFrom);
            b.HasIndex(x => x.ValidUntil);
            b.HasIndex(x => new { x.ValidFrom, x.ValidUntil });
            b.HasIndex(x => x.TenantId);
        });

        // RULE TEMPLATE ASSOCIATION (Many-to-Many)
        builder.Entity<RuleTemplateAssociation>(b =>
        {
            b.ToTable(DonaRogAppConsts.DbTablePrefix + "RuleTemplateAssociations", DonaRogAppConsts.DbSchema);
            b.ConfigureByConvention();

            // Composite key
            b.HasKey(x => new { x.RuleId, x.TemplateId });

            // Relationships
            b.HasOne(x => x.Template)
                .WithMany()
                .HasForeignKey(x => x.TemplateId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            b.HasIndex(x => x.RuleId);
            b.HasIndex(x => x.TemplateId);
            b.HasIndex(x => new { x.RuleId, x.Priority });
            b.HasIndex(x => new { x.RuleId, x.IsActive });
        });

        // DONOR TEMPLATE USAGE (LRU Tracking)
        builder.Entity<DonorTemplateUsage>(b =>
        {
            b.ToTable(DonaRogAppConsts.DbTablePrefix + "DonorTemplateUsages", DonaRogAppConsts.DbSchema);
            b.ConfigureByConvention();

            // Composite key
            b.HasKey(x => new { x.DonorId, x.TemplateId });

            // Relationships
            b.HasOne(x => x.Donor)
                .WithMany()
                .HasForeignKey(x => x.DonorId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasOne(x => x.Template)
                .WithMany()
                .HasForeignKey(x => x.TemplateId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            b.HasIndex(x => x.DonorId);
            b.HasIndex(x => x.TemplateId);
            b.HasIndex(x => new { x.DonorId, x.LastUsedDate });
            b.HasIndex(x => x.LastUsedDate);
            b.HasIndex(x => x.TenantId);
        });

        // ======================================================================
        // SEGMENTATION RULES
        // ======================================================================
        builder.Entity<SegmentationRule>(b =>
        {
            b.ToTable(DonaRogAppConsts.DbTablePrefix + "SegmentationRules", DonaRogAppConsts.DbSchema);
            b.ConfigureByConvention();

            // Properties
            b.Property(x => x.Name).IsRequired().HasMaxLength(128);
            b.Property(x => x.Description).HasMaxLength(500);
            
            // Decimal precision
            b.Property(x => x.MinTotalDonated).HasPrecision(18, 2);
            b.Property(x => x.MaxTotalDonated).HasPrecision(18, 2);

            // Relationships
            b.HasOne(x => x.Segment)
                .WithMany()
                .HasForeignKey(x => x.SegmentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            b.HasIndex(x => x.IsActive);
            b.HasIndex(x => x.Priority);
            b.HasIndex(x => new { x.IsActive, x.Priority });
            b.HasIndex(x => x.SegmentId);
            b.HasIndex(x => x.TenantId);
        });
    }
}