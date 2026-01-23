using DonaRogApp.Domain.Donors.Entities;
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
    public DbSet<Communication> Communications { get; set; }

    #endregion

    #region Donor Many-to-Many Junctions

    public DbSet<DonorSegment> DonorSegments { get; set; }
    public DbSet<DonorTag> DonorTags { get; set; }
    public DbSet<DonorInterest> DonorInterests { get; set; }

    #endregion

    #region Shared Entities (Lookup Tables)

    public DbSet<Title> Titles { get; set; }
    public DbSet<Segment> Segments { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<Interest> Interests { get; set; }

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

            b.HasIndex(x => new { x.DonorId, x.SentDate });
            b.HasIndex(x => x.Type);
            b.HasIndex(x => x.IsDelivered);
            b.HasIndex(x => x.IsFailed);
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

            b.HasIndex(x => x.Code).IsUnique();
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

            b.HasIndex(x => x.Code).IsUnique();
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

            b.HasIndex(x => x.Code).IsUnique();
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

            b.HasIndex(x => x.Code).IsUnique();
            b.HasIndex(x => x.IsActive);
        });
    }
}