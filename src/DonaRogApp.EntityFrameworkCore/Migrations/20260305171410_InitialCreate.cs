using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DonaRogApp.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AbpAuditLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ApplicationName = table.Column<string>(type: "character varying(96)", maxLength: 96, nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    TenantName = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    ImpersonatorUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    ImpersonatorUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ImpersonatorTenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    ImpersonatorTenantName = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    ExecutionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExecutionDuration = table.Column<int>(type: "integer", nullable: false),
                    ClientIpAddress = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    ClientName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    ClientId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    CorrelationId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    BrowserInfo = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    HttpMethod = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
                    Url = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Exceptions = table.Column<string>(type: "text", nullable: true),
                    Comments = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    HttpStatusCode = table.Column<int>(type: "integer", nullable: true),
                    ExtraProperties = table.Column<string>(type: "text", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpAuditLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpBackgroundJobs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ApplicationName = table.Column<string>(type: "character varying(96)", maxLength: 96, nullable: true),
                    JobName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    JobArgs = table.Column<string>(type: "character varying(1048576)", maxLength: 1048576, nullable: false),
                    TryCount = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)0),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    NextTryTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastTryTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsAbandoned = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    Priority = table.Column<byte>(type: "smallint", nullable: false, defaultValue: (byte)15),
                    ExtraProperties = table.Column<string>(type: "text", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpBackgroundJobs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpBlobContainers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    ExtraProperties = table.Column<string>(type: "text", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpBlobContainers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpClaimTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Required = table.Column<bool>(type: "boolean", nullable: false),
                    IsStatic = table.Column<bool>(type: "boolean", nullable: false),
                    Regex = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    RegexDescription = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Description = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ValueType = table.Column<int>(type: "integer", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExtraProperties = table.Column<string>(type: "text", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpClaimTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpFeatureGroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    ExtraProperties = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpFeatureGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpFeatures",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GroupName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    ParentName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    DisplayName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    DefaultValue = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    IsVisibleToClients = table.Column<bool>(type: "boolean", nullable: false),
                    IsAvailableToHost = table.Column<bool>(type: "boolean", nullable: false),
                    AllowedProviders = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ValueType = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    ExtraProperties = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpFeatures", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpFeatureValues",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Value = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    ProviderName = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    ProviderKey = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpFeatureValues", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpLinkUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SourceUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    SourceTenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    TargetUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    TargetTenantId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpLinkUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpOrganizationUnits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: true),
                    Code = table.Column<string>(type: "character varying(95)", maxLength: 95, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    EntityVersion = table.Column<int>(type: "integer", nullable: false),
                    ExtraProperties = table.Column<string>(type: "text", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpOrganizationUnits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AbpOrganizationUnits_AbpOrganizationUnits_ParentId",
                        column: x => x.ParentId,
                        principalTable: "AbpOrganizationUnits",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AbpPermissionGrants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    ProviderName = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    ProviderKey = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpPermissionGrants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpPermissionGroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    ExtraProperties = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpPermissionGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpPermissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GroupName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    ParentName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    DisplayName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    IsEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    MultiTenancySide = table.Column<byte>(type: "smallint", nullable: false),
                    Providers = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    StateCheckers = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ExtraProperties = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpPermissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false),
                    IsStatic = table.Column<bool>(type: "boolean", nullable: false),
                    IsPublic = table.Column<bool>(type: "boolean", nullable: false),
                    EntityVersion = table.Column<int>(type: "integer", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExtraProperties = table.Column<string>(type: "text", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpSecurityLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    ApplicationName = table.Column<string>(type: "character varying(96)", maxLength: 96, nullable: true),
                    Identity = table.Column<string>(type: "character varying(96)", maxLength: 96, nullable: true),
                    Action = table.Column<string>(type: "character varying(96)", maxLength: 96, nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    TenantName = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    ClientId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    CorrelationId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    ClientIpAddress = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    BrowserInfo = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExtraProperties = table.Column<string>(type: "text", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpSecurityLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SessionId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Device = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    DeviceInfo = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClientId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    IpAddresses = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    SignedIn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastAccessed = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ExtraProperties = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpSessions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpSettingDefinitions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    DefaultValue = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    IsVisibleToClients = table.Column<bool>(type: "boolean", nullable: false),
                    Providers = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    IsInherited = table.Column<bool>(type: "boolean", nullable: false),
                    IsEncrypted = table.Column<bool>(type: "boolean", nullable: false),
                    ExtraProperties = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpSettingDefinitions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Value = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                    ProviderName = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    ProviderKey = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpTenants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    NormalizedName = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    EntityVersion = table.Column<int>(type: "integer", nullable: false),
                    ExtraProperties = table.Column<string>(type: "text", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpTenants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpUserDelegations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    SourceUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    TargetUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    StartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpUserDelegations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    Surname = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    PasswordHash = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    SecurityStamp = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    IsExternal = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    PhoneNumber = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    ShouldChangePasswordOnNextLogin = table.Column<bool>(type: "boolean", nullable: false),
                    EntityVersion = table.Column<int>(type: "integer", nullable: false),
                    LastPasswordChangeTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ExtraProperties = table.Column<string>(type: "text", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "App_BankAccounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    AccountName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Iban = table.Column<string>(type: "character varying(34)", maxLength: 34, nullable: false),
                    IbanCountryCode = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    IbanCheckDigits = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    IbanBBAN = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    BankName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Swift = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false),
                    Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ExtraProperties = table.Column<string>(type: "text", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_BankAccounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "App_Interests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Category = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Icon = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ColorCode = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: true),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    DonorCount = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    ExtraProperties = table.Column<string>(type: "text", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_Interests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "App_LetterTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Content = table.Column<string>(type: "TEXT", nullable: false),
                    TemplateType = table.Column<int>(type: "integer", nullable: false),
                    TemplateFilePath = table.Column<string>(type: "text", nullable: true),
                    TemplateFileName = table.Column<string>(type: "text", nullable: true),
                    TemplateFileSizeBytes = table.Column<long>(type: "bigint", nullable: true),
                    TemplateFileUploadedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Category = table.Column<int>(type: "integer", nullable: false),
                    Language = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: false, defaultValue: "it"),
                    CommunicationType = table.Column<int>(type: "integer", nullable: true),
                    EmailSubject = table.Column<string>(type: "text", nullable: true),
                    IsPlural = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false),
                    CcEmails = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    BccEmails = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    UsageCount = table.Column<int>(type: "integer", nullable: false),
                    LastUsedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    PreviousVersionId = table.Column<Guid>(type: "uuid", nullable: true),
                    Tags = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ExtraProperties = table.Column<string>(type: "text", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_LetterTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "App_PrintBatches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    BatchNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ReadyAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    GenerationStartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    GeneratedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    GeneratedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    DownloadedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DownloadedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    PrintedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PrintedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CancelledAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CancellationReason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    PdfFilePath = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    PdfFileSizeBytes = table.Column<long>(type: "bigint", nullable: true),
                    TotalLetters = table.Column<int>(type: "integer", nullable: false),
                    TotalDonationAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    FilterJson = table.Column<string>(type: "TEXT", nullable: true),
                    MinAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    MaxAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    Region = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    ProjectIds = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CampaignIds = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    ExtraProperties = table.Column<string>(type: "text", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_PrintBatches", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "App_Projects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Category = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TargetAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false, defaultValue: "EUR"),
                    ResponsiblePerson = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ResponsibleEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ResponsiblePhone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    MainImageUrl = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    ThumbnailUrl = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Location = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Latitude = table.Column<decimal>(type: "numeric(10,7)", precision: 10, scale: 7, nullable: true),
                    Longitude = table.Column<decimal>(type: "numeric(10,7)", precision: 10, scale: 7, nullable: true),
                    TotalAmountRaised = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalDonationsCount = table.Column<int>(type: "integer", nullable: false),
                    AverageDonation = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    LastDonationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ExtraProperties = table.Column<string>(type: "text", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_Projects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "App_Recurrences",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Code = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    RecurrenceDay = table.Column<int>(type: "integer", nullable: true),
                    RecurrenceMonth = table.Column<int>(type: "integer", nullable: true),
                    DaysBeforeRecurrence = table.Column<int>(type: "integer", nullable: false),
                    DaysAfterRecurrence = table.Column<int>(type: "integer", nullable: false),
                    Notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    DeactivatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeactivationReason = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    ExtraProperties = table.Column<string>(type: "text", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_Recurrences", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "App_Segments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ColorCode = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: true),
                    Icon = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsSystem = table.Column<bool>(type: "boolean", nullable: false),
                    ExtraProperties = table.Column<string>(type: "text", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_Segments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "App_Tags",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    Code = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    ColorCode = table.Column<string>(type: "text", nullable: true),
                    Category = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    UsageCount = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsSystem = table.Column<bool>(type: "boolean", nullable: false),
                    ExtraProperties = table.Column<string>(type: "text", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_Tags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "App_Titles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Abbreviation = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    AssociatedGender = table.Column<int>(type: "integer", nullable: true),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    ExtraProperties = table.Column<string>(type: "text", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_Titles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OpenIddictApplications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ApplicationType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ClientId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ClientSecret = table.Column<string>(type: "text", nullable: true),
                    ClientType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ConsentType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    DisplayName = table.Column<string>(type: "text", nullable: true),
                    DisplayNames = table.Column<string>(type: "text", nullable: true),
                    JsonWebKeySet = table.Column<string>(type: "text", nullable: true),
                    Permissions = table.Column<string>(type: "text", nullable: true),
                    PostLogoutRedirectUris = table.Column<string>(type: "text", nullable: true),
                    Properties = table.Column<string>(type: "text", nullable: true),
                    RedirectUris = table.Column<string>(type: "text", nullable: true),
                    Requirements = table.Column<string>(type: "text", nullable: true),
                    Settings = table.Column<string>(type: "text", nullable: true),
                    ClientUri = table.Column<string>(type: "text", nullable: true),
                    LogoUri = table.Column<string>(type: "text", nullable: true),
                    ExtraProperties = table.Column<string>(type: "text", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpenIddictApplications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OpenIddictScopes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Descriptions = table.Column<string>(type: "text", nullable: true),
                    DisplayName = table.Column<string>(type: "text", nullable: true),
                    DisplayNames = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Properties = table.Column<string>(type: "text", nullable: true),
                    Resources = table.Column<string>(type: "text", nullable: true),
                    ExtraProperties = table.Column<string>(type: "text", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpenIddictScopes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpAuditLogActions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    AuditLogId = table.Column<Guid>(type: "uuid", nullable: false),
                    ServiceName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    MethodName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Parameters = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    ExecutionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExecutionDuration = table.Column<int>(type: "integer", nullable: false),
                    ExtraProperties = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpAuditLogActions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AbpAuditLogActions_AbpAuditLogs_AuditLogId",
                        column: x => x.AuditLogId,
                        principalTable: "AbpAuditLogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AbpEntityChanges",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AuditLogId = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    ChangeTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ChangeType = table.Column<byte>(type: "smallint", nullable: false),
                    EntityTenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    EntityId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    EntityTypeFullName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    ExtraProperties = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpEntityChanges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AbpEntityChanges_AbpAuditLogs_AuditLogId",
                        column: x => x.AuditLogId,
                        principalTable: "AbpAuditLogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AbpBlobs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ContainerId = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Content = table.Column<byte[]>(type: "bytea", maxLength: 2147483647, nullable: true),
                    ExtraProperties = table.Column<string>(type: "text", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpBlobs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AbpBlobs_AbpBlobContainers_ContainerId",
                        column: x => x.ContainerId,
                        principalTable: "AbpBlobContainers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AbpOrganizationUnitRoles",
                columns: table => new
                {
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    OrganizationUnitId = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpOrganizationUnitRoles", x => new { x.OrganizationUnitId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AbpOrganizationUnitRoles_AbpOrganizationUnits_OrganizationU~",
                        column: x => x.OrganizationUnitId,
                        principalTable: "AbpOrganizationUnits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AbpOrganizationUnitRoles_AbpRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AbpRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AbpRoleClaims",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    ClaimType = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    ClaimValue = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AbpRoleClaims_AbpRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AbpRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AbpTenantConnectionStrings",
                columns: table => new
                {
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Value = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpTenantConnectionStrings", x => new { x.TenantId, x.Name });
                    table.ForeignKey(
                        name: "FK_AbpTenantConnectionStrings_AbpTenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "AbpTenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AbpUserClaims",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    ClaimType = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    ClaimValue = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AbpUserClaims_AbpUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AbpUserLogins",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LoginProvider = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    ProviderKey = table.Column<string>(type: "character varying(196)", maxLength: 196, nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpUserLogins", x => new { x.UserId, x.LoginProvider });
                    table.ForeignKey(
                        name: "FK_AbpUserLogins_AbpUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AbpUserOrganizationUnits",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    OrganizationUnitId = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpUserOrganizationUnits", x => new { x.OrganizationUnitId, x.UserId });
                    table.ForeignKey(
                        name: "FK_AbpUserOrganizationUnits_AbpOrganizationUnits_OrganizationU~",
                        column: x => x.OrganizationUnitId,
                        principalTable: "AbpOrganizationUnits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AbpUserOrganizationUnits_AbpUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AbpUserRoles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AbpUserRoles_AbpRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AbpRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AbpUserRoles_AbpUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AbpUserTokens",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LoginProvider = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AbpUserTokens_AbpUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "App_TemplateAttachments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TemplateId = table.Column<Guid>(type: "uuid", nullable: false),
                    FileName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    FilePath = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_TemplateAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_App_TemplateAttachments_App_LetterTemplates_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "App_LetterTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "App_ProjectDocuments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    FileName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    FileUrl = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    FileType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_ProjectDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_App_ProjectDocuments_App_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "App_Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "App_Campaigns",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    Code = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    CampaignType = table.Column<int>(type: "integer", nullable: false),
                    Channel = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    RecurrenceId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExtractionScheduledDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ExtractionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DispatchScheduledDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DispatchDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RecurrenceDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    YearlySequenceNumber = table.Column<int>(type: "integer", nullable: true),
                    PostalCodeSequence = table.Column<int>(type: "integer", nullable: true),
                    PostalCodeYearSuffix = table.Column<int>(type: "integer", nullable: true),
                    MailchimpCampaignId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    MailchimpListId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    SmsProviderId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    TotalCost = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalRaised = table.Column<decimal>(type: "numeric", nullable: false),
                    TargetDonorCount = table.Column<int>(type: "integer", nullable: false),
                    ExtractedDonorCount = table.Column<int>(type: "integer", nullable: false),
                    DispatchedCount = table.Column<int>(type: "integer", nullable: false),
                    ResponseCount = table.Column<int>(type: "integer", nullable: false),
                    ResponseRate = table.Column<decimal>(type: "numeric", nullable: false),
                    DonationCount = table.Column<int>(type: "integer", nullable: false),
                    AverageDonation = table.Column<decimal>(type: "numeric", nullable: false),
                    ConversionRate = table.Column<decimal>(type: "numeric", nullable: false),
                    ROI = table.Column<decimal>(type: "numeric", nullable: false),
                    ExtraProperties = table.Column<string>(type: "text", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_Campaigns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_App_Campaigns_App_Recurrences_RecurrenceId",
                        column: x => x.RecurrenceId,
                        principalTable: "App_Recurrences",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "App_ThankYouRules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Priority = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    MinAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    MaxAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    IsFirstDonation = table.Column<bool>(type: "boolean", nullable: true),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: true),
                    CampaignId = table.Column<Guid>(type: "uuid", nullable: true),
                    DonorCategory = table.Column<int>(type: "integer", nullable: true),
                    SubjectType = table.Column<int>(type: "integer", nullable: true),
                    RecurrenceId = table.Column<Guid>(type: "uuid", nullable: true),
                    ValidFrom = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ValidUntil = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreateThankYou = table.Column<bool>(type: "boolean", nullable: false),
                    SuggestedChannel = table.Column<int>(type: "integer", nullable: true),
                    SuggestedTemplateId = table.Column<Guid>(type: "uuid", nullable: true),
                    ExtraProperties = table.Column<string>(type: "text", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_ThankYouRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_App_ThankYouRules_App_Recurrences_RecurrenceId",
                        column: x => x.RecurrenceId,
                        principalTable: "App_Recurrences",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "App_SegmentationRules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Priority = table.Column<int>(type: "integer", nullable: false),
                    SegmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    MinRecencyScore = table.Column<int>(type: "integer", nullable: true),
                    MaxRecencyScore = table.Column<int>(type: "integer", nullable: true),
                    MinFrequencyScore = table.Column<int>(type: "integer", nullable: true),
                    MaxFrequencyScore = table.Column<int>(type: "integer", nullable: true),
                    MinMonetaryScore = table.Column<int>(type: "integer", nullable: true),
                    MaxMonetaryScore = table.Column<int>(type: "integer", nullable: true),
                    MinTotalDonated = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    MaxTotalDonated = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    MinDonationCount = table.Column<int>(type: "integer", nullable: true),
                    MaxDonationCount = table.Column<int>(type: "integer", nullable: true),
                    MinDaysSinceLastDonation = table.Column<int>(type: "integer", nullable: true),
                    MaxDaysSinceLastDonation = table.Column<int>(type: "integer", nullable: true),
                    FirstDonationAfter = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FirstDonationBefore = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastDonationAfter = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastDonationBefore = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ExtraProperties = table.Column<string>(type: "text", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_SegmentationRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_App_SegmentationRules_App_Segments_SegmentId",
                        column: x => x.SegmentId,
                        principalTable: "App_Segments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "App_Donors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    DonorCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    SubjectType = table.Column<int>(type: "integer", nullable: false),
                    TitleId = table.Column<Guid>(type: "uuid", nullable: true),
                    TitleId1 = table.Column<Guid>(type: "uuid", nullable: true),
                    FirstName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    MiddleName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    LastName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Gender = table.Column<int>(type: "integer", nullable: true),
                    BirthDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    BirthPlace = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    CompanyName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    OrganizationType = table.Column<int>(type: "integer", nullable: true),
                    LegalForm = table.Column<int>(type: "integer", nullable: true),
                    BusinessSector = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    TaxCode = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
                    VatNumber = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Category = table.Column<int>(type: "integer", nullable: false),
                    Origin = table.Column<int>(type: "integer", nullable: true),
                    TotalDonated = table.Column<decimal>(type: "numeric", nullable: false),
                    DonationCount = table.Column<int>(type: "integer", nullable: false),
                    AverageDonationAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    FirstDonationAmount = table.Column<decimal>(type: "numeric", nullable: true),
                    LastDonationAmount = table.Column<decimal>(type: "numeric", nullable: true),
                    FirstDonationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastDonationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FirstConversionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RecencyScore = table.Column<int>(type: "integer", nullable: false),
                    FrequencyScore = table.Column<int>(type: "integer", nullable: false),
                    MonetaryScore = table.Column<int>(type: "integer", nullable: false),
                    RfmSegment = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    PreferredLanguage = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: false, defaultValue: "IT"),
                    PreferredFrequency = table.Column<int>(type: "integer", nullable: true),
                    PreferredChannel = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    LettersSentCount = table.Column<int>(type: "integer", nullable: false),
                    EmailsSentCount = table.Column<int>(type: "integer", nullable: false),
                    LastThankYouLetterDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastEmailSentDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PrivacyConsent = table.Column<bool>(type: "boolean", nullable: false),
                    PrivacyConsentDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    NewsletterConsent = table.Column<bool>(type: "boolean", nullable: false),
                    NewsletterConsentDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PhoneConsent = table.Column<bool>(type: "boolean", nullable: false),
                    MailConsent = table.Column<bool>(type: "boolean", nullable: false),
                    MailConsentDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PrivacyConsentRevokedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ProfilingConsent = table.Column<bool>(type: "boolean", nullable: false),
                    ThirdPartyConsent = table.Column<bool>(type: "boolean", nullable: false),
                    IsAnonymized = table.Column<bool>(type: "boolean", nullable: false),
                    AnonymizationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PreferredThankYouChannel = table.Column<int>(type: "integer", nullable: true),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    ExtraProperties = table.Column<string>(type: "text", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
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
                name: "OpenIddictAuthorizations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ApplicationId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Properties = table.Column<string>(type: "text", nullable: true),
                    Scopes = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Subject = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: true),
                    Type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ExtraProperties = table.Column<string>(type: "text", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpenIddictAuthorizations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OpenIddictAuthorizations_OpenIddictApplications_Application~",
                        column: x => x.ApplicationId,
                        principalTable: "OpenIddictApplications",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AbpEntityPropertyChanges",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    EntityChangeId = table.Column<Guid>(type: "uuid", nullable: false),
                    NewValue = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    OriginalValue = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    PropertyName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    PropertyTypeFullName = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpEntityPropertyChanges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AbpEntityPropertyChanges_AbpEntityChanges_EntityChangeId",
                        column: x => x.EntityChangeId,
                        principalTable: "AbpEntityChanges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "App_RuleTemplateAssociations",
                columns: table => new
                {
                    RuleId = table.Column<Guid>(type: "uuid", nullable: false),
                    TemplateId = table.Column<Guid>(type: "uuid", nullable: false),
                    Priority = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_RuleTemplateAssociations", x => new { x.RuleId, x.TemplateId });
                    table.ForeignKey(
                        name: "FK_App_RuleTemplateAssociations_App_LetterTemplates_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "App_LetterTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_App_RuleTemplateAssociations_App_ThankYouRules_RuleId",
                        column: x => x.RuleId,
                        principalTable: "App_ThankYouRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "App_CampaignDonors",
                columns: table => new
                {
                    CampaignId = table.Column<Guid>(type: "uuid", nullable: false),
                    DonorId = table.Column<Guid>(type: "uuid", nullable: false),
                    TrackingCode = table.Column<Guid>(type: "uuid", nullable: true),
                    TrackingCampaignId = table.Column<Guid>(type: "uuid", nullable: true),
                    TrackingDonorId = table.Column<Guid>(type: "uuid", nullable: true),
                    TrackingHash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    ExtractedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DispatchedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    OpenedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ClickedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ResponseType = table.Column<int>(type: "integer", nullable: false),
                    DonationAmount = table.Column<decimal>(type: "numeric", nullable: true),
                    DonationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    RemovedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_CampaignDonors", x => new { x.CampaignId, x.DonorId });
                    table.ForeignKey(
                        name: "FK_App_CampaignDonors_App_Campaigns_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "App_Campaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_App_CampaignDonors_App_Donors_DonorId",
                        column: x => x.DonorId,
                        principalTable: "App_Donors",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "App_Communications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    DonorId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Subject = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Category = table.Column<int>(type: "integer", nullable: true),
                    TemplateId = table.Column<Guid>(type: "uuid", nullable: true),
                    DonationId = table.Column<Guid>(type: "uuid", nullable: true),
                    CampaignId = table.Column<Guid>(type: "uuid", nullable: true),
                    SentDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDelivered = table.Column<bool>(type: "boolean", nullable: false),
                    DeliveredDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsOpened = table.Column<bool>(type: "boolean", nullable: false),
                    OpenedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    OpenCount = table.Column<int>(type: "integer", nullable: false),
                    IsClicked = table.Column<bool>(type: "boolean", nullable: false),
                    ClickedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ClickCount = table.Column<int>(type: "integer", nullable: false),
                    IsFailed = table.Column<bool>(type: "boolean", nullable: false),
                    FailureReason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    FailureDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Recipient = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Body = table.Column<string>(type: "TEXT", nullable: true),
                    ExternalId = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    SentByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    PrintBatchId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsPrinted = table.Column<bool>(type: "boolean", nullable: false),
                    PrintedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
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
                    table.ForeignKey(
                        name: "FK_App_Communications_App_PrintBatches_PrintBatchId",
                        column: x => x.PrintBatchId,
                        principalTable: "App_PrintBatches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "App_Donations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    Reference = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ExternalId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    DonorId = table.Column<Guid>(type: "uuid", nullable: false),
                    CampaignId = table.Column<Guid>(type: "uuid", nullable: true),
                    BankAccountId = table.Column<Guid>(type: "uuid", nullable: true),
                    ThankYouTemplateId = table.Column<Guid>(type: "uuid", nullable: true),
                    Channel = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false, defaultValue: "EUR"),
                    DonationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreditDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RejectionReason = table.Column<int>(type: "integer", nullable: true),
                    RejectionNotes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    RejectedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RejectedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    VerifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    VerifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    InternalNotes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    ExtraProperties = table.Column<string>(type: "text", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_Donations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_App_Donations_App_BankAccounts_BankAccountId",
                        column: x => x.BankAccountId,
                        principalTable: "App_BankAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_App_Donations_App_Campaigns_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "App_Campaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_App_Donations_App_Donors_DonorId",
                        column: x => x.DonorId,
                        principalTable: "App_Donors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_App_Donations_App_LetterTemplates_ThankYouTemplateId",
                        column: x => x.ThankYouTemplateId,
                        principalTable: "App_LetterTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "App_DonorAddresses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    DonorId = table.Column<Guid>(type: "uuid", nullable: false),
                    Street = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    City = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Province = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Region = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    PostalCode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Country = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsVerified = table.Column<bool>(type: "boolean", nullable: false),
                    VerifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
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
                name: "App_DonorAttachments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    DonorId = table.Column<Guid>(type: "uuid", nullable: false),
                    FileName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    FileExtension = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    MimeType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    FileSizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    BlobName = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    AttachmentType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_DonorAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_App_DonorAttachments_App_Donors_DonorId",
                        column: x => x.DonorId,
                        principalTable: "App_Donors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "App_DonorContacts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    DonorId = table.Column<Guid>(type: "uuid", nullable: false),
                    PhoneCountryCode = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    PhoneNationalNumber = table.Column<string>(type: "character varying(14)", maxLength: 14, nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false),
                    IsVerified = table.Column<bool>(type: "boolean", nullable: false),
                    VerifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DateAdded = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
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
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    DonorId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmailAddress = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false),
                    IsVerified = table.Column<bool>(type: "boolean", nullable: false),
                    VerifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    VerificationToken = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    BounceCount = table.Column<int>(type: "integer", nullable: false),
                    LastBounceDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastBounceReason = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    IsInvalid = table.Column<bool>(type: "boolean", nullable: false),
                    DateAdded = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
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
                    DonorId = table.Column<Guid>(type: "uuid", nullable: false),
                    InterestId = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    DonorId1 = table.Column<Guid>(type: "uuid", nullable: true),
                    InterestId1 = table.Column<Guid>(type: "uuid", nullable: true),
                    InterestedSince = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RemovedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    InterestLevel = table.Column<int>(type: "integer", nullable: false),
                    EngagementScore = table.Column<int>(type: "integer", nullable: false),
                    DonationCountOnTopic = table.Column<int>(type: "integer", nullable: false),
                    TotalDonatedOnTopic = table.Column<decimal>(type: "numeric", nullable: false),
                    CommunicationsReceivedOnTopic = table.Column<int>(type: "integer", nullable: false),
                    CommunicationsEngagedOnTopic = table.Column<int>(type: "integer", nullable: false),
                    DiscoveryMethod = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true)
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
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    DonorId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Content = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Category = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    InteractionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsImportant = table.Column<bool>(type: "boolean", nullable: false),
                    IsPrivate = table.Column<bool>(type: "boolean", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
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
                    DonorId = table.Column<Guid>(type: "uuid", nullable: false),
                    SegmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    DonorId1 = table.Column<Guid>(type: "uuid", nullable: true),
                    SegmentId1 = table.Column<Guid>(type: "uuid", nullable: true),
                    AssignedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RemovedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AssignmentNotes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsAutomatic = table.Column<bool>(type: "boolean", nullable: false),
                    AutomaticReason = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    AddedByUserId = table.Column<Guid>(type: "uuid", nullable: true)
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
                name: "App_DonorStatusHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    DonorId = table.Column<Guid>(type: "uuid", nullable: false),
                    OldStatus = table.Column<int>(type: "integer", nullable: false),
                    NewStatus = table.Column<int>(type: "integer", nullable: false),
                    Note = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ChangedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DonorId1 = table.Column<Guid>(type: "uuid", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_DonorStatusHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_App_DonorStatusHistories_App_Donors_DonorId",
                        column: x => x.DonorId,
                        principalTable: "App_Donors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_App_DonorStatusHistories_App_Donors_DonorId1",
                        column: x => x.DonorId1,
                        principalTable: "App_Donors",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "App_DonorTags",
                columns: table => new
                {
                    DonorId = table.Column<Guid>(type: "uuid", nullable: false),
                    TagId = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    DonorId1 = table.Column<Guid>(type: "uuid", nullable: true),
                    TagId1 = table.Column<Guid>(type: "uuid", nullable: true),
                    TaggedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RemovedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TaggingReason = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Priority = table.Column<int>(type: "integer", nullable: false),
                    IsAutomatic = table.Column<bool>(type: "boolean", nullable: false),
                    AssignedByUserId = table.Column<Guid>(type: "uuid", nullable: true)
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

            migrationBuilder.CreateTable(
                name: "App_DonorTemplateUsages",
                columns: table => new
                {
                    DonorId = table.Column<Guid>(type: "uuid", nullable: false),
                    TemplateId = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastUsedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UsageCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_DonorTemplateUsages", x => new { x.DonorId, x.TemplateId });
                    table.ForeignKey(
                        name: "FK_App_DonorTemplateUsages_App_Donors_DonorId",
                        column: x => x.DonorId,
                        principalTable: "App_Donors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_App_DonorTemplateUsages_App_LetterTemplates_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "App_LetterTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OpenIddictTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ApplicationId = table.Column<Guid>(type: "uuid", nullable: true),
                    AuthorizationId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ExpirationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Payload = table.Column<string>(type: "text", nullable: true),
                    Properties = table.Column<string>(type: "text", nullable: true),
                    RedemptionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ReferenceId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Subject = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: true),
                    Type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ExtraProperties = table.Column<string>(type: "text", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpenIddictTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OpenIddictTokens_OpenIddictApplications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "OpenIddictApplications",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OpenIddictTokens_OpenIddictAuthorizations_AuthorizationId",
                        column: x => x.AuthorizationId,
                        principalTable: "OpenIddictAuthorizations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "App_DonationDocuments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DonationId = table.Column<Guid>(type: "uuid", nullable: false),
                    FileName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    FileExtension = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    MimeType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    FileSizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    StoragePath = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    TextContent = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    DocumentType = table.Column<int>(type: "integer", nullable: false),
                    IsFromExternalFlow = table.Column<bool>(type: "boolean", nullable: false),
                    Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_DonationDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_App_DonationDocuments_App_Donations_DonationId",
                        column: x => x.DonationId,
                        principalTable: "App_Donations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "App_DonationProjects",
                columns: table => new
                {
                    DonationId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    AllocatedAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_DonationProjects", x => new { x.DonationId, x.ProjectId });
                    table.ForeignKey(
                        name: "FK_App_DonationProjects_App_Donations_DonationId",
                        column: x => x.DonationId,
                        principalTable: "App_Donations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_App_DonationProjects_App_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "App_Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AbpAuditLogActions_AuditLogId",
                table: "AbpAuditLogActions",
                column: "AuditLogId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpAuditLogActions_TenantId_ServiceName_MethodName_Executio~",
                table: "AbpAuditLogActions",
                columns: new[] { "TenantId", "ServiceName", "MethodName", "ExecutionTime" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpAuditLogs_TenantId_ExecutionTime",
                table: "AbpAuditLogs",
                columns: new[] { "TenantId", "ExecutionTime" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpAuditLogs_TenantId_UserId_ExecutionTime",
                table: "AbpAuditLogs",
                columns: new[] { "TenantId", "UserId", "ExecutionTime" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpBackgroundJobs_IsAbandoned_NextTryTime",
                table: "AbpBackgroundJobs",
                columns: new[] { "IsAbandoned", "NextTryTime" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpBlobContainers_TenantId_Name",
                table: "AbpBlobContainers",
                columns: new[] { "TenantId", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpBlobs_ContainerId",
                table: "AbpBlobs",
                column: "ContainerId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpBlobs_TenantId_ContainerId_Name",
                table: "AbpBlobs",
                columns: new[] { "TenantId", "ContainerId", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpEntityChanges_AuditLogId",
                table: "AbpEntityChanges",
                column: "AuditLogId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpEntityChanges_TenantId_EntityTypeFullName_EntityId",
                table: "AbpEntityChanges",
                columns: new[] { "TenantId", "EntityTypeFullName", "EntityId" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpEntityPropertyChanges_EntityChangeId",
                table: "AbpEntityPropertyChanges",
                column: "EntityChangeId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpFeatureGroups_Name",
                table: "AbpFeatureGroups",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AbpFeatures_GroupName",
                table: "AbpFeatures",
                column: "GroupName");

            migrationBuilder.CreateIndex(
                name: "IX_AbpFeatures_Name",
                table: "AbpFeatures",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AbpFeatureValues_Name_ProviderName_ProviderKey",
                table: "AbpFeatureValues",
                columns: new[] { "Name", "ProviderName", "ProviderKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AbpLinkUsers_SourceUserId_SourceTenantId_TargetUserId_Targe~",
                table: "AbpLinkUsers",
                columns: new[] { "SourceUserId", "SourceTenantId", "TargetUserId", "TargetTenantId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AbpOrganizationUnitRoles_RoleId_OrganizationUnitId",
                table: "AbpOrganizationUnitRoles",
                columns: new[] { "RoleId", "OrganizationUnitId" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpOrganizationUnits_Code",
                table: "AbpOrganizationUnits",
                column: "Code");

            migrationBuilder.CreateIndex(
                name: "IX_AbpOrganizationUnits_ParentId",
                table: "AbpOrganizationUnits",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpPermissionGrants_TenantId_Name_ProviderName_ProviderKey",
                table: "AbpPermissionGrants",
                columns: new[] { "TenantId", "Name", "ProviderName", "ProviderKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AbpPermissionGroups_Name",
                table: "AbpPermissionGroups",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AbpPermissions_GroupName",
                table: "AbpPermissions",
                column: "GroupName");

            migrationBuilder.CreateIndex(
                name: "IX_AbpPermissions_Name",
                table: "AbpPermissions",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AbpRoleClaims_RoleId",
                table: "AbpRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpRoles_NormalizedName",
                table: "AbpRoles",
                column: "NormalizedName");

            migrationBuilder.CreateIndex(
                name: "IX_AbpSecurityLogs_TenantId_Action",
                table: "AbpSecurityLogs",
                columns: new[] { "TenantId", "Action" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpSecurityLogs_TenantId_ApplicationName",
                table: "AbpSecurityLogs",
                columns: new[] { "TenantId", "ApplicationName" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpSecurityLogs_TenantId_Identity",
                table: "AbpSecurityLogs",
                columns: new[] { "TenantId", "Identity" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpSecurityLogs_TenantId_UserId",
                table: "AbpSecurityLogs",
                columns: new[] { "TenantId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpSessions_Device",
                table: "AbpSessions",
                column: "Device");

            migrationBuilder.CreateIndex(
                name: "IX_AbpSessions_SessionId",
                table: "AbpSessions",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpSessions_TenantId_UserId",
                table: "AbpSessions",
                columns: new[] { "TenantId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpSettingDefinitions_Name",
                table: "AbpSettingDefinitions",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AbpSettings_Name_ProviderName_ProviderKey",
                table: "AbpSettings",
                columns: new[] { "Name", "ProviderName", "ProviderKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenants_Name",
                table: "AbpTenants",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenants_NormalizedName",
                table: "AbpTenants",
                column: "NormalizedName");

            migrationBuilder.CreateIndex(
                name: "IX_AbpUserClaims_UserId",
                table: "AbpUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpUserLogins_LoginProvider_ProviderKey",
                table: "AbpUserLogins",
                columns: new[] { "LoginProvider", "ProviderKey" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpUserOrganizationUnits_UserId_OrganizationUnitId",
                table: "AbpUserOrganizationUnits",
                columns: new[] { "UserId", "OrganizationUnitId" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpUserRoles_RoleId_UserId",
                table: "AbpUserRoles",
                columns: new[] { "RoleId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpUsers_Email",
                table: "AbpUsers",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_AbpUsers_NormalizedEmail",
                table: "AbpUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_AbpUsers_NormalizedUserName",
                table: "AbpUsers",
                column: "NormalizedUserName");

            migrationBuilder.CreateIndex(
                name: "IX_AbpUsers_UserName",
                table: "AbpUsers",
                column: "UserName");

            migrationBuilder.CreateIndex(
                name: "IX_App_BankAccounts_IsActive",
                table: "App_BankAccounts",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_App_BankAccounts_IsDefault",
                table: "App_BankAccounts",
                column: "IsDefault");

            migrationBuilder.CreateIndex(
                name: "IX_App_BankAccounts_TenantId_IsActive",
                table: "App_BankAccounts",
                columns: new[] { "TenantId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_App_CampaignDonors_CampaignId_ResponseType",
                table: "App_CampaignDonors",
                columns: new[] { "CampaignId", "ResponseType" });

            migrationBuilder.CreateIndex(
                name: "IX_App_CampaignDonors_ClickedAt",
                table: "App_CampaignDonors",
                column: "ClickedAt");

            migrationBuilder.CreateIndex(
                name: "IX_App_CampaignDonors_DispatchedAt",
                table: "App_CampaignDonors",
                column: "DispatchedAt");

            migrationBuilder.CreateIndex(
                name: "IX_App_CampaignDonors_DonationDate",
                table: "App_CampaignDonors",
                column: "DonationDate");

            migrationBuilder.CreateIndex(
                name: "IX_App_CampaignDonors_DonorId",
                table: "App_CampaignDonors",
                column: "DonorId");

            migrationBuilder.CreateIndex(
                name: "IX_App_CampaignDonors_ExtractedAt",
                table: "App_CampaignDonors",
                column: "ExtractedAt");

            migrationBuilder.CreateIndex(
                name: "IX_App_CampaignDonors_OpenedAt",
                table: "App_CampaignDonors",
                column: "OpenedAt");

            migrationBuilder.CreateIndex(
                name: "IX_App_CampaignDonors_ResponseType",
                table: "App_CampaignDonors",
                column: "ResponseType");

            migrationBuilder.CreateIndex(
                name: "IX_App_Campaigns_CampaignType",
                table: "App_Campaigns",
                column: "CampaignType");

            migrationBuilder.CreateIndex(
                name: "IX_App_Campaigns_Channel",
                table: "App_Campaigns",
                column: "Channel");

            migrationBuilder.CreateIndex(
                name: "IX_App_Campaigns_DispatchDate",
                table: "App_Campaigns",
                column: "DispatchDate");

            migrationBuilder.CreateIndex(
                name: "IX_App_Campaigns_RecurrenceId",
                table: "App_Campaigns",
                column: "RecurrenceId");

            migrationBuilder.CreateIndex(
                name: "IX_App_Campaigns_Status",
                table: "App_Campaigns",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_App_Campaigns_TenantId_Year_Code",
                table: "App_Campaigns",
                columns: new[] { "TenantId", "Year", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_App_Campaigns_Year",
                table: "App_Campaigns",
                column: "Year");

            migrationBuilder.CreateIndex(
                name: "IX_App_Campaigns_Year_Status",
                table: "App_Campaigns",
                columns: new[] { "Year", "Status" });

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
                name: "IX_App_Communications_IsPrinted",
                table: "App_Communications",
                column: "IsPrinted");

            migrationBuilder.CreateIndex(
                name: "IX_App_Communications_PrintBatchId",
                table: "App_Communications",
                column: "PrintBatchId");

            migrationBuilder.CreateIndex(
                name: "IX_App_Communications_Status",
                table: "App_Communications",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_App_Communications_Type",
                table: "App_Communications",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_App_DonationDocuments_DocumentType",
                table: "App_DonationDocuments",
                column: "DocumentType");

            migrationBuilder.CreateIndex(
                name: "IX_App_DonationDocuments_DonationId",
                table: "App_DonationDocuments",
                column: "DonationId");

            migrationBuilder.CreateIndex(
                name: "IX_App_DonationDocuments_DonationId_CreationTime",
                table: "App_DonationDocuments",
                columns: new[] { "DonationId", "CreationTime" });

            migrationBuilder.CreateIndex(
                name: "IX_App_DonationDocuments_IsFromExternalFlow",
                table: "App_DonationDocuments",
                column: "IsFromExternalFlow");

            migrationBuilder.CreateIndex(
                name: "IX_App_DonationProjects_DonationId",
                table: "App_DonationProjects",
                column: "DonationId");

            migrationBuilder.CreateIndex(
                name: "IX_App_DonationProjects_ProjectId",
                table: "App_DonationProjects",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_App_Donations_BankAccountId",
                table: "App_Donations",
                column: "BankAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_App_Donations_CampaignId",
                table: "App_Donations",
                column: "CampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_App_Donations_CampaignId_Status",
                table: "App_Donations",
                columns: new[] { "CampaignId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_App_Donations_Channel",
                table: "App_Donations",
                column: "Channel");

            migrationBuilder.CreateIndex(
                name: "IX_App_Donations_CreditDate",
                table: "App_Donations",
                column: "CreditDate");

            migrationBuilder.CreateIndex(
                name: "IX_App_Donations_DonationDate",
                table: "App_Donations",
                column: "DonationDate");

            migrationBuilder.CreateIndex(
                name: "IX_App_Donations_DonorId",
                table: "App_Donations",
                column: "DonorId");

            migrationBuilder.CreateIndex(
                name: "IX_App_Donations_DonorId_Status",
                table: "App_Donations",
                columns: new[] { "DonorId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_App_Donations_ExternalId",
                table: "App_Donations",
                column: "ExternalId",
                unique: true,
                filter: "\"ExternalId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_App_Donations_Reference",
                table: "App_Donations",
                column: "Reference");

            migrationBuilder.CreateIndex(
                name: "IX_App_Donations_Status",
                table: "App_Donations",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_App_Donations_Status_DonationDate",
                table: "App_Donations",
                columns: new[] { "Status", "DonationDate" });

            migrationBuilder.CreateIndex(
                name: "IX_App_Donations_TenantId",
                table: "App_Donations",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_App_Donations_ThankYouTemplateId",
                table: "App_Donations",
                column: "ThankYouTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_App_DonorAddresses_DonorId_StartDate",
                table: "App_DonorAddresses",
                columns: new[] { "DonorId", "StartDate" });

            migrationBuilder.CreateIndex(
                name: "IX_App_DonorAddresses_IsVerified",
                table: "App_DonorAddresses",
                column: "IsVerified");

            migrationBuilder.CreateIndex(
                name: "IX_App_DonorAttachments_AttachmentType",
                table: "App_DonorAttachments",
                column: "AttachmentType");

            migrationBuilder.CreateIndex(
                name: "IX_App_DonorAttachments_CreationTime",
                table: "App_DonorAttachments",
                column: "CreationTime");

            migrationBuilder.CreateIndex(
                name: "IX_App_DonorAttachments_DonorId",
                table: "App_DonorAttachments",
                column: "DonorId");

            migrationBuilder.CreateIndex(
                name: "IX_App_DonorAttachments_DonorId_DisplayOrder",
                table: "App_DonorAttachments",
                columns: new[] { "DonorId", "DisplayOrder" });

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
                name: "IX_App_DonorStatusHistories_DonorId_ChangedAt",
                table: "App_DonorStatusHistories",
                columns: new[] { "DonorId", "ChangedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_App_DonorStatusHistories_DonorId1",
                table: "App_DonorStatusHistories",
                column: "DonorId1");

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
                name: "IX_App_DonorTemplateUsages_DonorId",
                table: "App_DonorTemplateUsages",
                column: "DonorId");

            migrationBuilder.CreateIndex(
                name: "IX_App_DonorTemplateUsages_DonorId_LastUsedDate",
                table: "App_DonorTemplateUsages",
                columns: new[] { "DonorId", "LastUsedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_App_DonorTemplateUsages_LastUsedDate",
                table: "App_DonorTemplateUsages",
                column: "LastUsedDate");

            migrationBuilder.CreateIndex(
                name: "IX_App_DonorTemplateUsages_TemplateId",
                table: "App_DonorTemplateUsages",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_App_DonorTemplateUsages_TenantId",
                table: "App_DonorTemplateUsages",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_App_Interests_IsActive",
                table: "App_Interests",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_App_Interests_TenantId_Code",
                table: "App_Interests",
                columns: new[] { "TenantId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_App_LetterTemplates_Category",
                table: "App_LetterTemplates",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_App_LetterTemplates_Category_Language_IsActive",
                table: "App_LetterTemplates",
                columns: new[] { "Category", "Language", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_App_LetterTemplates_IsActive",
                table: "App_LetterTemplates",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_App_LetterTemplates_IsDefault",
                table: "App_LetterTemplates",
                column: "IsDefault");

            migrationBuilder.CreateIndex(
                name: "IX_App_LetterTemplates_Language",
                table: "App_LetterTemplates",
                column: "Language");

            migrationBuilder.CreateIndex(
                name: "IX_App_LetterTemplates_LastUsedDate",
                table: "App_LetterTemplates",
                column: "LastUsedDate");

            migrationBuilder.CreateIndex(
                name: "IX_App_LetterTemplates_TenantId_Category_Language",
                table: "App_LetterTemplates",
                columns: new[] { "TenantId", "Category", "Language" });

            migrationBuilder.CreateIndex(
                name: "IX_App_PrintBatches_BatchNumber",
                table: "App_PrintBatches",
                column: "BatchNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_App_PrintBatches_GeneratedAt",
                table: "App_PrintBatches",
                column: "GeneratedAt");

            migrationBuilder.CreateIndex(
                name: "IX_App_PrintBatches_PrintedAt",
                table: "App_PrintBatches",
                column: "PrintedAt");

            migrationBuilder.CreateIndex(
                name: "IX_App_PrintBatches_Status",
                table: "App_PrintBatches",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_App_PrintBatches_Status_GeneratedAt",
                table: "App_PrintBatches",
                columns: new[] { "Status", "GeneratedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_App_PrintBatches_TenantId",
                table: "App_PrintBatches",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_App_ProjectDocuments_ProjectId",
                table: "App_ProjectDocuments",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_App_ProjectDocuments_ProjectId_DisplayOrder",
                table: "App_ProjectDocuments",
                columns: new[] { "ProjectId", "DisplayOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_App_Projects_Category",
                table: "App_Projects",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_App_Projects_EndDate",
                table: "App_Projects",
                column: "EndDate");

            migrationBuilder.CreateIndex(
                name: "IX_App_Projects_StartDate",
                table: "App_Projects",
                column: "StartDate");

            migrationBuilder.CreateIndex(
                name: "IX_App_Projects_Status",
                table: "App_Projects",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_App_Projects_Status_Category",
                table: "App_Projects",
                columns: new[] { "Status", "Category" });

            migrationBuilder.CreateIndex(
                name: "IX_App_Projects_TenantId_Code",
                table: "App_Projects",
                columns: new[] { "TenantId", "Code" },
                unique: true,
                filter: "\"TenantId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_App_Recurrences_IsActive",
                table: "App_Recurrences",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_App_Recurrences_RecurrenceMonth_RecurrenceDay",
                table: "App_Recurrences",
                columns: new[] { "RecurrenceMonth", "RecurrenceDay" });

            migrationBuilder.CreateIndex(
                name: "IX_App_Recurrences_TenantId_Code",
                table: "App_Recurrences",
                columns: new[] { "TenantId", "Code" },
                unique: true,
                filter: "\"Code\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_App_RuleTemplateAssociations_RuleId",
                table: "App_RuleTemplateAssociations",
                column: "RuleId");

            migrationBuilder.CreateIndex(
                name: "IX_App_RuleTemplateAssociations_RuleId_IsActive",
                table: "App_RuleTemplateAssociations",
                columns: new[] { "RuleId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_App_RuleTemplateAssociations_RuleId_Priority",
                table: "App_RuleTemplateAssociations",
                columns: new[] { "RuleId", "Priority" });

            migrationBuilder.CreateIndex(
                name: "IX_App_RuleTemplateAssociations_TemplateId",
                table: "App_RuleTemplateAssociations",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_App_SegmentationRules_IsActive",
                table: "App_SegmentationRules",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_App_SegmentationRules_IsActive_Priority",
                table: "App_SegmentationRules",
                columns: new[] { "IsActive", "Priority" });

            migrationBuilder.CreateIndex(
                name: "IX_App_SegmentationRules_Priority",
                table: "App_SegmentationRules",
                column: "Priority");

            migrationBuilder.CreateIndex(
                name: "IX_App_SegmentationRules_SegmentId",
                table: "App_SegmentationRules",
                column: "SegmentId");

            migrationBuilder.CreateIndex(
                name: "IX_App_SegmentationRules_TenantId",
                table: "App_SegmentationRules",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_App_Segments_IsActive",
                table: "App_Segments",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_App_Segments_IsSystem",
                table: "App_Segments",
                column: "IsSystem");

            migrationBuilder.CreateIndex(
                name: "IX_App_Segments_TenantId_Code",
                table: "App_Segments",
                columns: new[] { "TenantId", "Code" },
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
                name: "IX_App_Tags_TenantId_Code",
                table: "App_Tags",
                columns: new[] { "TenantId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_App_TemplateAttachments_TemplateId",
                table: "App_TemplateAttachments",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_App_ThankYouRules_CampaignId",
                table: "App_ThankYouRules",
                column: "CampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_App_ThankYouRules_IsActive",
                table: "App_ThankYouRules",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_App_ThankYouRules_IsActive_Priority",
                table: "App_ThankYouRules",
                columns: new[] { "IsActive", "Priority" });

            migrationBuilder.CreateIndex(
                name: "IX_App_ThankYouRules_ProjectId",
                table: "App_ThankYouRules",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_App_ThankYouRules_RecurrenceId",
                table: "App_ThankYouRules",
                column: "RecurrenceId");

            migrationBuilder.CreateIndex(
                name: "IX_App_ThankYouRules_TenantId",
                table: "App_ThankYouRules",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_App_ThankYouRules_ValidFrom",
                table: "App_ThankYouRules",
                column: "ValidFrom");

            migrationBuilder.CreateIndex(
                name: "IX_App_ThankYouRules_ValidFrom_ValidUntil",
                table: "App_ThankYouRules",
                columns: new[] { "ValidFrom", "ValidUntil" });

            migrationBuilder.CreateIndex(
                name: "IX_App_ThankYouRules_ValidUntil",
                table: "App_ThankYouRules",
                column: "ValidUntil");

            migrationBuilder.CreateIndex(
                name: "IX_App_Titles_IsActive",
                table: "App_Titles",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_App_Titles_TenantId_Code",
                table: "App_Titles",
                columns: new[] { "TenantId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OpenIddictApplications_ClientId",
                table: "OpenIddictApplications",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_OpenIddictAuthorizations_ApplicationId_Status_Subject_Type",
                table: "OpenIddictAuthorizations",
                columns: new[] { "ApplicationId", "Status", "Subject", "Type" });

            migrationBuilder.CreateIndex(
                name: "IX_OpenIddictScopes_Name",
                table: "OpenIddictScopes",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_OpenIddictTokens_ApplicationId_Status_Subject_Type",
                table: "OpenIddictTokens",
                columns: new[] { "ApplicationId", "Status", "Subject", "Type" });

            migrationBuilder.CreateIndex(
                name: "IX_OpenIddictTokens_AuthorizationId",
                table: "OpenIddictTokens",
                column: "AuthorizationId");

            migrationBuilder.CreateIndex(
                name: "IX_OpenIddictTokens_ReferenceId",
                table: "OpenIddictTokens",
                column: "ReferenceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AbpAuditLogActions");

            migrationBuilder.DropTable(
                name: "AbpBackgroundJobs");

            migrationBuilder.DropTable(
                name: "AbpBlobs");

            migrationBuilder.DropTable(
                name: "AbpClaimTypes");

            migrationBuilder.DropTable(
                name: "AbpEntityPropertyChanges");

            migrationBuilder.DropTable(
                name: "AbpFeatureGroups");

            migrationBuilder.DropTable(
                name: "AbpFeatures");

            migrationBuilder.DropTable(
                name: "AbpFeatureValues");

            migrationBuilder.DropTable(
                name: "AbpLinkUsers");

            migrationBuilder.DropTable(
                name: "AbpOrganizationUnitRoles");

            migrationBuilder.DropTable(
                name: "AbpPermissionGrants");

            migrationBuilder.DropTable(
                name: "AbpPermissionGroups");

            migrationBuilder.DropTable(
                name: "AbpPermissions");

            migrationBuilder.DropTable(
                name: "AbpRoleClaims");

            migrationBuilder.DropTable(
                name: "AbpSecurityLogs");

            migrationBuilder.DropTable(
                name: "AbpSessions");

            migrationBuilder.DropTable(
                name: "AbpSettingDefinitions");

            migrationBuilder.DropTable(
                name: "AbpSettings");

            migrationBuilder.DropTable(
                name: "AbpTenantConnectionStrings");

            migrationBuilder.DropTable(
                name: "AbpUserClaims");

            migrationBuilder.DropTable(
                name: "AbpUserDelegations");

            migrationBuilder.DropTable(
                name: "AbpUserLogins");

            migrationBuilder.DropTable(
                name: "AbpUserOrganizationUnits");

            migrationBuilder.DropTable(
                name: "AbpUserRoles");

            migrationBuilder.DropTable(
                name: "AbpUserTokens");

            migrationBuilder.DropTable(
                name: "App_CampaignDonors");

            migrationBuilder.DropTable(
                name: "App_Communications");

            migrationBuilder.DropTable(
                name: "App_DonationDocuments");

            migrationBuilder.DropTable(
                name: "App_DonationProjects");

            migrationBuilder.DropTable(
                name: "App_DonorAddresses");

            migrationBuilder.DropTable(
                name: "App_DonorAttachments");

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
                name: "App_DonorStatusHistories");

            migrationBuilder.DropTable(
                name: "App_DonorTags");

            migrationBuilder.DropTable(
                name: "App_DonorTemplateUsages");

            migrationBuilder.DropTable(
                name: "App_ProjectDocuments");

            migrationBuilder.DropTable(
                name: "App_RuleTemplateAssociations");

            migrationBuilder.DropTable(
                name: "App_SegmentationRules");

            migrationBuilder.DropTable(
                name: "App_TemplateAttachments");

            migrationBuilder.DropTable(
                name: "OpenIddictScopes");

            migrationBuilder.DropTable(
                name: "OpenIddictTokens");

            migrationBuilder.DropTable(
                name: "AbpBlobContainers");

            migrationBuilder.DropTable(
                name: "AbpEntityChanges");

            migrationBuilder.DropTable(
                name: "AbpTenants");

            migrationBuilder.DropTable(
                name: "AbpOrganizationUnits");

            migrationBuilder.DropTable(
                name: "AbpRoles");

            migrationBuilder.DropTable(
                name: "AbpUsers");

            migrationBuilder.DropTable(
                name: "App_PrintBatches");

            migrationBuilder.DropTable(
                name: "App_Donations");

            migrationBuilder.DropTable(
                name: "App_Interests");

            migrationBuilder.DropTable(
                name: "App_Tags");

            migrationBuilder.DropTable(
                name: "App_Projects");

            migrationBuilder.DropTable(
                name: "App_ThankYouRules");

            migrationBuilder.DropTable(
                name: "App_Segments");

            migrationBuilder.DropTable(
                name: "OpenIddictAuthorizations");

            migrationBuilder.DropTable(
                name: "AbpAuditLogs");

            migrationBuilder.DropTable(
                name: "App_BankAccounts");

            migrationBuilder.DropTable(
                name: "App_Campaigns");

            migrationBuilder.DropTable(
                name: "App_Donors");

            migrationBuilder.DropTable(
                name: "App_LetterTemplates");

            migrationBuilder.DropTable(
                name: "OpenIddictApplications");

            migrationBuilder.DropTable(
                name: "App_Recurrences");

            migrationBuilder.DropTable(
                name: "App_Titles");
        }
    }
}
