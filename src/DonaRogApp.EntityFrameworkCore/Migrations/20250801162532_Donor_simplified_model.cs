using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DonaRogApp.Migrations
{
    /// <inheritdoc />
    public partial class Donor_simplified_model : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_Donors_DonorId",
                table: "Addresses");

            migrationBuilder.DropForeignKey(
                name: "FK_DonorRelationships_Donors_DonorId",
                table: "DonorRelationships");

            migrationBuilder.DropForeignKey(
                name: "FK_DonorTags_Donors_DonorId",
                table: "DonorTags");

            migrationBuilder.DropForeignKey(
                name: "FK_Emails_Donors_DonorId",
                table: "Emails");

            migrationBuilder.DropForeignKey(
                name: "FK_Notes_Donors_DonorId",
                table: "Notes");

            migrationBuilder.DropForeignKey(
                name: "FK_PhoneNumbers_Donors_DonorId",
                table: "PhoneNumbers");

            migrationBuilder.DropIndex(
                name: "IX_PhoneNumbers_DonorId",
                table: "PhoneNumbers");

            migrationBuilder.DropIndex(
                name: "IX_Notes_DonorId",
                table: "Notes");

            migrationBuilder.DropIndex(
                name: "IX_Emails_DonorId",
                table: "Emails");

            migrationBuilder.DropIndex(
                name: "IX_DonorTags_DonorId",
                table: "DonorTags");

            migrationBuilder.DropIndex(
                name: "IX_DonorRelationships_DonorId",
                table: "DonorRelationships");

            migrationBuilder.DropIndex(
                name: "IX_Addresses_DonorId",
                table: "Addresses");

            migrationBuilder.DropColumn(
                name: "ContactPreference",
                table: "Donors");

            migrationBuilder.DropColumn(
                name: "EmailDeliveryEnabled",
                table: "Donors");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "Donors");

            migrationBuilder.DropColumn(
                name: "IsOrganization",
                table: "Donors");

            migrationBuilder.DropColumn(
                name: "MiddleName",
                table: "Donors");

            migrationBuilder.DropColumn(
                name: "OrganizationName",
                table: "Donors");

            migrationBuilder.DropColumn(
                name: "PaperDeliveryEnabled",
                table: "Donors");

            migrationBuilder.DropColumn(
                name: "Score",
                table: "Donors");

            migrationBuilder.DropColumn(
                name: "SecondLastName",
                table: "Donors");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Donors");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Donors");

            migrationBuilder.DropColumn(
                name: "WantsThanks",
                table: "Donors");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ContactPreference",
                table: "Donors",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "EmailDeliveryEnabled",
                table: "Donors",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Gender",
                table: "Donors",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsOrganization",
                table: "Donors",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "MiddleName",
                table: "Donors",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationName",
                table: "Donors",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PaperDeliveryEnabled",
                table: "Donors",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Score",
                table: "Donors",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "SecondLastName",
                table: "Donors",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Donors",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Donors",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "WantsThanks",
                table: "Donors",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_PhoneNumbers_DonorId",
                table: "PhoneNumbers",
                column: "DonorId");

            migrationBuilder.CreateIndex(
                name: "IX_Notes_DonorId",
                table: "Notes",
                column: "DonorId");

            migrationBuilder.CreateIndex(
                name: "IX_Emails_DonorId",
                table: "Emails",
                column: "DonorId");

            migrationBuilder.CreateIndex(
                name: "IX_DonorTags_DonorId",
                table: "DonorTags",
                column: "DonorId");

            migrationBuilder.CreateIndex(
                name: "IX_DonorRelationships_DonorId",
                table: "DonorRelationships",
                column: "DonorId");

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_DonorId",
                table: "Addresses",
                column: "DonorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_Donors_DonorId",
                table: "Addresses",
                column: "DonorId",
                principalTable: "Donors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DonorRelationships_Donors_DonorId",
                table: "DonorRelationships",
                column: "DonorId",
                principalTable: "Donors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DonorTags_Donors_DonorId",
                table: "DonorTags",
                column: "DonorId",
                principalTable: "Donors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Emails_Donors_DonorId",
                table: "Emails",
                column: "DonorId",
                principalTable: "Donors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Notes_Donors_DonorId",
                table: "Notes",
                column: "DonorId",
                principalTable: "Donors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PhoneNumbers_Donors_DonorId",
                table: "PhoneNumbers",
                column: "DonorId",
                principalTable: "Donors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
