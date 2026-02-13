using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DonaRogApp.Migrations
{
    /// <inheritdoc />
    public partial class ChangeRecurrenceDateToDayMonth : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_App_Recurrences_RecurrenceDate",
                table: "App_Recurrences");

            migrationBuilder.DropColumn(
                name: "RecurrenceDate",
                table: "App_Recurrences");

            migrationBuilder.AddColumn<int>(
                name: "RecurrenceDay",
                table: "App_Recurrences",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RecurrenceMonth",
                table: "App_Recurrences",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_App_Recurrences_RecurrenceMonth_RecurrenceDay",
                table: "App_Recurrences",
                columns: new[] { "RecurrenceMonth", "RecurrenceDay" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_App_Recurrences_RecurrenceMonth_RecurrenceDay",
                table: "App_Recurrences");

            migrationBuilder.DropColumn(
                name: "RecurrenceDay",
                table: "App_Recurrences");

            migrationBuilder.DropColumn(
                name: "RecurrenceMonth",
                table: "App_Recurrences");

            migrationBuilder.AddColumn<DateTime>(
                name: "RecurrenceDate",
                table: "App_Recurrences",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_App_Recurrences_RecurrenceDate",
                table: "App_Recurrences",
                column: "RecurrenceDate");
        }
    }
}
