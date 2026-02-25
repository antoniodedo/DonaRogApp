using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DonaRogApp.Migrations
{
    /// <inheritdoc />
    public partial class AddDonationDocuments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "App_DonationDocuments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DonationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FileExtension = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MimeType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FileSizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    StoragePath = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    DocumentType = table.Column<int>(type: "int", nullable: false),
                    IsFromExternalFlow = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "App_DonationDocuments");
        }
    }
}
