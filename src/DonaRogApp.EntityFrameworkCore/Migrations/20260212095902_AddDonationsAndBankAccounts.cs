using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DonaRogApp.Migrations
{
    /// <inheritdoc />
    public partial class AddDonationsAndBankAccounts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "App_BankAccounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AccountName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Iban = table.Column<string>(type: "nvarchar(34)", maxLength: 34, nullable: false),
                    IbanCountryCode = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    IbanCheckDigits = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    IbanBBAN = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    BankName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Swift = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
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
                    table.PrimaryKey("PK_App_BankAccounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "App_Donations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Reference = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ExternalId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DonorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CampaignId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    BankAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ThankYouTemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Channel = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false, defaultValue: "EUR"),
                    DonationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreditDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionReason = table.Column<int>(type: "int", nullable: true),
                    RejectionNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    RejectedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    VerifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VerifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    InternalNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
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
                name: "App_DonationProjects",
                columns: table => new
                {
                    DonationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AllocatedAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
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
                filter: "[ExternalId] IS NOT NULL");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "App_DonationProjects");

            migrationBuilder.DropTable(
                name: "App_Donations");

            migrationBuilder.DropTable(
                name: "App_BankAccounts");
        }
    }
}
