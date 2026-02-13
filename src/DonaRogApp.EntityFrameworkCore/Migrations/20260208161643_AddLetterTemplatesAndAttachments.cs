using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DonaRogApp.Migrations
{
    /// <inheritdoc />
    public partial class AddLetterTemplatesAndAttachments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "App_LetterTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Content = table.Column<string>(type: "TEXT", nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false),
                    Language = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false, defaultValue: "it"),
                    CommunicationType = table.Column<int>(type: "int", nullable: true),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RecurrenceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    MinAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    MaxAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    IsForNewDonor = table.Column<bool>(type: "bit", nullable: false),
                    IsPlural = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    CcEmails = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    BccEmails = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UsageCount = table.Column<int>(type: "int", nullable: false),
                    LastUsedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Version = table.Column<int>(type: "int", nullable: false),
                    PreviousVersionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Tags = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
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
                    table.PrimaryKey("PK_App_LetterTemplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_App_LetterTemplates_App_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "App_Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_App_LetterTemplates_App_Recurrences_RecurrenceId",
                        column: x => x.RecurrenceId,
                        principalTable: "App_Recurrences",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "App_TemplateAttachments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
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
                name: "IX_App_LetterTemplates_ProjectId",
                table: "App_LetterTemplates",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_App_LetterTemplates_RecurrenceId",
                table: "App_LetterTemplates",
                column: "RecurrenceId");

            migrationBuilder.CreateIndex(
                name: "IX_App_LetterTemplates_TenantId_Category_Language",
                table: "App_LetterTemplates",
                columns: new[] { "TenantId", "Category", "Language" });

            migrationBuilder.CreateIndex(
                name: "IX_App_TemplateAttachments_TemplateId",
                table: "App_TemplateAttachments",
                column: "TemplateId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "App_TemplateAttachments");

            migrationBuilder.DropTable(
                name: "App_LetterTemplates");
        }
    }
}
