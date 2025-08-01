using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DonaRogApp.Migrations
{
    /// <inheritdoc />
    public partial class Donor_simplified_model_FullAuditedAggregateRoot_fixConvetionName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Donors",
                table: "Donors");

            migrationBuilder.RenameTable(
                name: "Donors",
                newName: "AppDonors");

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "AppDonors",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "AppDonors",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<Guid>(
                name: "DeleterId",
                table: "AppDonors",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletionTime",
                table: "AppDonors",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "AppDonors",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "RawAddress",
                table: "AppDonors",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RawCap",
                table: "AppDonors",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RawComune",
                table: "AppDonors",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_AppDonors",
                table: "AppDonors",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AppDonors",
                table: "AppDonors");

            migrationBuilder.DropColumn(
                name: "DeleterId",
                table: "AppDonors");

            migrationBuilder.DropColumn(
                name: "DeletionTime",
                table: "AppDonors");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "AppDonors");

            migrationBuilder.DropColumn(
                name: "RawAddress",
                table: "AppDonors");

            migrationBuilder.DropColumn(
                name: "RawCap",
                table: "AppDonors");

            migrationBuilder.DropColumn(
                name: "RawComune",
                table: "AppDonors");

            migrationBuilder.RenameTable(
                name: "AppDonors",
                newName: "Donors");

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "Donors",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "Donors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Donors",
                table: "Donors",
                column: "Id");
        }
    }
}
