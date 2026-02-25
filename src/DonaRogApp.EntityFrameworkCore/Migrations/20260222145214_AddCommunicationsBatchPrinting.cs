using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DonaRogApp.Migrations
{
    /// <inheritdoc />
    public partial class AddCommunicationsBatchPrinting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EmailSubject",
                table: "App_LetterTemplates",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TemplateFileName",
                table: "App_LetterTemplates",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TemplateFilePath",
                table: "App_LetterTemplates",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "TemplateFileSizeBytes",
                table: "App_LetterTemplates",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TemplateFileUploadedAt",
                table: "App_LetterTemplates",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TemplateType",
                table: "App_LetterTemplates",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PreferredThankYouChannel",
                table: "App_Donors",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPrinted",
                table: "App_Communications",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "PrintBatchId",
                table: "App_Communications",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PrintedAt",
                table: "App_Communications",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "App_Communications",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "App_PrintBatches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    BatchNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ReadyAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GenerationStartedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GeneratedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GeneratedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DownloadedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DownloadedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PrintedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PrintedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CancelledAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CancellationReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PdfFilePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PdfFileSizeBytes = table.Column<long>(type: "bigint", nullable: true),
                    TotalLetters = table.Column<int>(type: "int", nullable: false),
                    TotalDonationAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    FilterJson = table.Column<string>(type: "TEXT", nullable: true),
                    MinAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    MaxAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    Region = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ProjectIds = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CampaignIds = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
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
                    table.PrimaryKey("PK_App_PrintBatches", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "App_ThankYouRules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    MinAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    MaxAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    IsFirstDonation = table.Column<bool>(type: "bit", nullable: true),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CampaignId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DonorCategory = table.Column<int>(type: "int", nullable: true),
                    SubjectType = table.Column<int>(type: "int", nullable: true),
                    CreateThankYou = table.Column<bool>(type: "bit", nullable: false),
                    SuggestedChannel = table.Column<int>(type: "int", nullable: true),
                    SuggestedTemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
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
                    table.PrimaryKey("PK_App_ThankYouRules", x => x.Id);
                });

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
                name: "IX_App_ThankYouRules_TenantId",
                table: "App_ThankYouRules",
                column: "TenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_App_Communications_App_PrintBatches_PrintBatchId",
                table: "App_Communications",
                column: "PrintBatchId",
                principalTable: "App_PrintBatches",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_App_Communications_App_PrintBatches_PrintBatchId",
                table: "App_Communications");

            migrationBuilder.DropTable(
                name: "App_PrintBatches");

            migrationBuilder.DropTable(
                name: "App_ThankYouRules");

            migrationBuilder.DropIndex(
                name: "IX_App_Communications_IsPrinted",
                table: "App_Communications");

            migrationBuilder.DropIndex(
                name: "IX_App_Communications_PrintBatchId",
                table: "App_Communications");

            migrationBuilder.DropIndex(
                name: "IX_App_Communications_Status",
                table: "App_Communications");

            migrationBuilder.DropColumn(
                name: "EmailSubject",
                table: "App_LetterTemplates");

            migrationBuilder.DropColumn(
                name: "TemplateFileName",
                table: "App_LetterTemplates");

            migrationBuilder.DropColumn(
                name: "TemplateFilePath",
                table: "App_LetterTemplates");

            migrationBuilder.DropColumn(
                name: "TemplateFileSizeBytes",
                table: "App_LetterTemplates");

            migrationBuilder.DropColumn(
                name: "TemplateFileUploadedAt",
                table: "App_LetterTemplates");

            migrationBuilder.DropColumn(
                name: "TemplateType",
                table: "App_LetterTemplates");

            migrationBuilder.DropColumn(
                name: "PreferredThankYouChannel",
                table: "App_Donors");

            migrationBuilder.DropColumn(
                name: "IsPrinted",
                table: "App_Communications");

            migrationBuilder.DropColumn(
                name: "PrintBatchId",
                table: "App_Communications");

            migrationBuilder.DropColumn(
                name: "PrintedAt",
                table: "App_Communications");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "App_Communications");
        }
    }
}
