using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DonaRogApp.Migrations
{
    /// <inheritdoc />
    public partial class AddDonorAggregate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "App_Interests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Category = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Icon = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ColorCode = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    DonorCount = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_Interests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "App_Segments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ColorCode = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: true),
                    Icon = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsSystem = table.Column<bool>(type: "bit", nullable: false),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_Segments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "App_Tags",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ColorCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Category = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UsageCount = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsSystem = table.Column<bool>(type: "bit", nullable: false),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_Tags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "App_Titles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Abbreviation = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    AssociatedGender = table.Column<int>(type: "int", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_Titles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "App_Donors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DonorCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SubjectType = table.Column<int>(type: "int", nullable: false),
                    TitleId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TitleId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    MiddleName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Gender = table.Column<int>(type: "int", nullable: true),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BirthPlace = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CompanyName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    OrganizationType = table.Column<int>(type: "int", nullable: true),
                    LegalForm = table.Column<int>(type: "int", nullable: true),
                    BusinessSector = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    TaxCode = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    VatNumber = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false),
                    Origin = table.Column<int>(type: "int", nullable: true),
                    TotalDonated = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DonationCount = table.Column<int>(type: "int", nullable: false),
                    AverageDonationAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FirstDonationAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    LastDonationAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    FirstDonationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastDonationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FirstConversionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RecencyScore = table.Column<int>(type: "int", nullable: false),
                    FrequencyScore = table.Column<int>(type: "int", nullable: false),
                    MonetaryScore = table.Column<int>(type: "int", nullable: false),
                    RfmSegment = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PreferredLanguage = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false, defaultValue: "IT"),
                    PreferredFrequency = table.Column<int>(type: "int", nullable: true),
                    PreferredChannel = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LettersSentCount = table.Column<int>(type: "int", nullable: false),
                    EmailsSentCount = table.Column<int>(type: "int", nullable: false),
                    LastThankYouLetterDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastEmailSentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PrivacyConsent = table.Column<bool>(type: "bit", nullable: false),
                    PrivacyConsentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NewsletterConsent = table.Column<bool>(type: "bit", nullable: false),
                    NewsletterConsentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PhoneConsent = table.Column<bool>(type: "bit", nullable: false),
                    MailConsent = table.Column<bool>(type: "bit", nullable: false),
                    ProfilingConsent = table.Column<bool>(type: "bit", nullable: false),
                    ThirdPartyConsent = table.Column<bool>(type: "bit", nullable: false),
                    IsAnonymized = table.Column<bool>(type: "bit", nullable: false),
                    AnonymizationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_Donors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_App_Donors_App_Titles_TitleId",
                        column: x => x.TitleId,
                        principalTable: "App_Titles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_App_Donors_App_Titles_TitleId1",
                        column: x => x.TitleId1,
                        principalTable: "App_Titles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "App_Communications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DonorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Category = table.Column<int>(type: "int", nullable: true),
                    TemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DonationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CampaignId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDelivered = table.Column<bool>(type: "bit", nullable: false),
                    DeliveredDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsOpened = table.Column<bool>(type: "bit", nullable: false),
                    OpenedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OpenCount = table.Column<int>(type: "int", nullable: false),
                    IsClicked = table.Column<bool>(type: "bit", nullable: false),
                    ClickedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ClickCount = table.Column<int>(type: "int", nullable: false),
                    IsFailed = table.Column<bool>(type: "bit", nullable: false),
                    FailureReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FailureDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Recipient = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Body = table.Column<string>(type: "TEXT", nullable: true),
                    ExternalId = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    SentByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_Communications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_App_Communications_App_Donors_DonorId",
                        column: x => x.DonorId,
                        principalTable: "App_Donors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "App_DonorAddresses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DonorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Street = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    City = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Province = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Region = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    PostalCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false),
                    VerifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_DonorAddresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_App_DonorAddresses_App_Donors_DonorId",
                        column: x => x.DonorId,
                        principalTable: "App_Donors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "App_DonorContacts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DonorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PhoneCountryCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    PhoneNationalNumber = table.Column<string>(type: "nvarchar(14)", maxLength: 14, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false),
                    VerifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateAdded = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_DonorContacts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_App_DonorContacts_App_Donors_DonorId",
                        column: x => x.DonorId,
                        principalTable: "App_Donors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "App_DonorEmails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DonorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmailAddress = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false),
                    VerifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VerificationToken = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    BounceCount = table.Column<int>(type: "int", nullable: false),
                    LastBounceDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastBounceReason = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    IsInvalid = table.Column<bool>(type: "bit", nullable: false),
                    DateAdded = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_DonorEmails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_App_DonorEmails_App_Donors_DonorId",
                        column: x => x.DonorId,
                        principalTable: "App_Donors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "App_DonorInterests",
                columns: table => new
                {
                    DonorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InterestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DonorId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    InterestId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    InterestedSince = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RemovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    InterestLevel = table.Column<int>(type: "int", nullable: false),
                    EngagementScore = table.Column<int>(type: "int", nullable: false),
                    DonationCountOnTopic = table.Column<int>(type: "int", nullable: false),
                    TotalDonatedOnTopic = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CommunicationsReceivedOnTopic = table.Column<int>(type: "int", nullable: false),
                    CommunicationsEngagedOnTopic = table.Column<int>(type: "int", nullable: false),
                    DiscoveryMethod = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_DonorInterests", x => new { x.DonorId, x.InterestId });
                    table.ForeignKey(
                        name: "FK_App_DonorInterests_App_Donors_DonorId",
                        column: x => x.DonorId,
                        principalTable: "App_Donors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_App_DonorInterests_App_Donors_DonorId1",
                        column: x => x.DonorId1,
                        principalTable: "App_Donors",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_App_DonorInterests_App_Interests_InterestId",
                        column: x => x.InterestId,
                        principalTable: "App_Interests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_App_DonorInterests_App_Interests_InterestId1",
                        column: x => x.InterestId1,
                        principalTable: "App_Interests",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "App_DonorNotes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DonorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    InteractionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsImportant = table.Column<bool>(type: "bit", nullable: false),
                    IsPrivate = table.Column<bool>(type: "bit", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_DonorNotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_App_DonorNotes_App_Donors_DonorId",
                        column: x => x.DonorId,
                        principalTable: "App_Donors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "App_DonorSegments",
                columns: table => new
                {
                    DonorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SegmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DonorId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SegmentId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RemovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AssignmentNotes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsAutomatic = table.Column<bool>(type: "bit", nullable: false),
                    AutomaticReason = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    AddedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_DonorSegments", x => new { x.DonorId, x.SegmentId });
                    table.ForeignKey(
                        name: "FK_App_DonorSegments_App_Donors_DonorId",
                        column: x => x.DonorId,
                        principalTable: "App_Donors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_App_DonorSegments_App_Donors_DonorId1",
                        column: x => x.DonorId1,
                        principalTable: "App_Donors",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_App_DonorSegments_App_Segments_SegmentId",
                        column: x => x.SegmentId,
                        principalTable: "App_Segments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_App_DonorSegments_App_Segments_SegmentId1",
                        column: x => x.SegmentId1,
                        principalTable: "App_Segments",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "App_DonorTags",
                columns: table => new
                {
                    DonorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TagId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DonorId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TagId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TaggedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RemovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TaggingReason = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    IsAutomatic = table.Column<bool>(type: "bit", nullable: false),
                    AssignedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_DonorTags", x => new { x.DonorId, x.TagId });
                    table.ForeignKey(
                        name: "FK_App_DonorTags_App_Donors_DonorId",
                        column: x => x.DonorId,
                        principalTable: "App_Donors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_App_DonorTags_App_Donors_DonorId1",
                        column: x => x.DonorId1,
                        principalTable: "App_Donors",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_App_DonorTags_App_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "App_Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_App_DonorTags_App_Tags_TagId1",
                        column: x => x.TagId1,
                        principalTable: "App_Tags",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_App_Communications_DonorId_SentDate",
                table: "App_Communications",
                columns: new[] { "DonorId", "SentDate" });

            migrationBuilder.CreateIndex(
                name: "IX_App_Communications_IsDelivered",
                table: "App_Communications",
                column: "IsDelivered");

            migrationBuilder.CreateIndex(
                name: "IX_App_Communications_IsFailed",
                table: "App_Communications",
                column: "IsFailed");

            migrationBuilder.CreateIndex(
                name: "IX_App_Communications_Type",
                table: "App_Communications",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_App_DonorAddresses_DonorId_StartDate",
                table: "App_DonorAddresses",
                columns: new[] { "DonorId", "StartDate" });

            migrationBuilder.CreateIndex(
                name: "IX_App_DonorAddresses_IsVerified",
                table: "App_DonorAddresses",
                column: "IsVerified");

            migrationBuilder.CreateIndex(
                name: "IX_App_DonorContacts_DonorId",
                table: "App_DonorContacts",
                column: "DonorId");

            migrationBuilder.CreateIndex(
                name: "IX_App_DonorContacts_IsVerified",
                table: "App_DonorContacts",
                column: "IsVerified");

            migrationBuilder.CreateIndex(
                name: "IX_App_DonorEmails_DonorId_EmailAddress",
                table: "App_DonorEmails",
                columns: new[] { "DonorId", "EmailAddress" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_App_DonorEmails_IsInvalid",
                table: "App_DonorEmails",
                column: "IsInvalid");

            migrationBuilder.CreateIndex(
                name: "IX_App_DonorEmails_IsVerified",
                table: "App_DonorEmails",
                column: "IsVerified");

            migrationBuilder.CreateIndex(
                name: "IX_App_DonorInterests_DonorId_RemovedAt",
                table: "App_DonorInterests",
                columns: new[] { "DonorId", "RemovedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_App_DonorInterests_DonorId1",
                table: "App_DonorInterests",
                column: "DonorId1");

            migrationBuilder.CreateIndex(
                name: "IX_App_DonorInterests_InterestId",
                table: "App_DonorInterests",
                column: "InterestId");

            migrationBuilder.CreateIndex(
                name: "IX_App_DonorInterests_InterestId1",
                table: "App_DonorInterests",
                column: "InterestId1");

            migrationBuilder.CreateIndex(
                name: "IX_App_DonorNotes_DonorId",
                table: "App_DonorNotes",
                column: "DonorId");

            migrationBuilder.CreateIndex(
                name: "IX_App_Donors_Category",
                table: "App_Donors",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_App_Donors_DonorCode",
                table: "App_Donors",
                column: "DonorCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_App_Donors_Status",
                table: "App_Donors",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_App_Donors_TenantId",
                table: "App_Donors",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_App_Donors_TitleId",
                table: "App_Donors",
                column: "TitleId");

            migrationBuilder.CreateIndex(
                name: "IX_App_Donors_TitleId1",
                table: "App_Donors",
                column: "TitleId1");

            migrationBuilder.CreateIndex(
                name: "IX_App_DonorSegments_DonorId_RemovedAt",
                table: "App_DonorSegments",
                columns: new[] { "DonorId", "RemovedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_App_DonorSegments_DonorId1",
                table: "App_DonorSegments",
                column: "DonorId1");

            migrationBuilder.CreateIndex(
                name: "IX_App_DonorSegments_SegmentId",
                table: "App_DonorSegments",
                column: "SegmentId");

            migrationBuilder.CreateIndex(
                name: "IX_App_DonorSegments_SegmentId1",
                table: "App_DonorSegments",
                column: "SegmentId1");

            migrationBuilder.CreateIndex(
                name: "IX_App_DonorTags_DonorId_RemovedAt",
                table: "App_DonorTags",
                columns: new[] { "DonorId", "RemovedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_App_DonorTags_DonorId1",
                table: "App_DonorTags",
                column: "DonorId1");

            migrationBuilder.CreateIndex(
                name: "IX_App_DonorTags_TagId",
                table: "App_DonorTags",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_App_DonorTags_TagId1",
                table: "App_DonorTags",
                column: "TagId1");

            migrationBuilder.CreateIndex(
                name: "IX_App_Interests_Code",
                table: "App_Interests",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_App_Interests_IsActive",
                table: "App_Interests",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_App_Segments_Code",
                table: "App_Segments",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_App_Segments_IsActive",
                table: "App_Segments",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_App_Segments_IsSystem",
                table: "App_Segments",
                column: "IsSystem");

            migrationBuilder.CreateIndex(
                name: "IX_App_Tags_Code",
                table: "App_Tags",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_App_Tags_IsActive",
                table: "App_Tags",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_App_Tags_IsSystem",
                table: "App_Tags",
                column: "IsSystem");

            migrationBuilder.CreateIndex(
                name: "IX_App_Titles_Code",
                table: "App_Titles",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_App_Titles_IsActive",
                table: "App_Titles",
                column: "IsActive");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "App_Communications");

            migrationBuilder.DropTable(
                name: "App_DonorAddresses");

            migrationBuilder.DropTable(
                name: "App_DonorContacts");

            migrationBuilder.DropTable(
                name: "App_DonorEmails");

            migrationBuilder.DropTable(
                name: "App_DonorInterests");

            migrationBuilder.DropTable(
                name: "App_DonorNotes");

            migrationBuilder.DropTable(
                name: "App_DonorSegments");

            migrationBuilder.DropTable(
                name: "App_DonorTags");

            migrationBuilder.DropTable(
                name: "App_Interests");

            migrationBuilder.DropTable(
                name: "App_Segments");

            migrationBuilder.DropTable(
                name: "App_Donors");

            migrationBuilder.DropTable(
                name: "App_Tags");

            migrationBuilder.DropTable(
                name: "App_Titles");
        }
    }
}
