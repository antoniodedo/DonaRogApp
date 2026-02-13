using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DonaRogApp.Migrations
{
    /// <inheritdoc />
    public partial class AddMailConsentDateAndPrivacyRevokedDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "MailConsentDate",
                table: "App_Donors",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PrivacyConsentRevokedDate",
                table: "App_Donors",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MailConsentDate",
                table: "App_Donors");

            migrationBuilder.DropColumn(
                name: "PrivacyConsentRevokedDate",
                table: "App_Donors");
        }
    }
}
