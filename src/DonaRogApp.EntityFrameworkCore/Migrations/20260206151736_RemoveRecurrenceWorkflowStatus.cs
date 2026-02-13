using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DonaRogApp.Migrations
{
    /// <inheritdoc />
    public partial class RemoveRecurrenceWorkflowStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_App_Recurrences_IsActive_Status",
                table: "App_Recurrences");

            migrationBuilder.DropIndex(
                name: "IX_App_Recurrences_Status",
                table: "App_Recurrences");

            migrationBuilder.DropColumn(
                name: "ActivatedDate",
                table: "App_Recurrences");

            migrationBuilder.DropColumn(
                name: "CancellationReason",
                table: "App_Recurrences");

            migrationBuilder.DropColumn(
                name: "CancelledDate",
                table: "App_Recurrences");

            migrationBuilder.DropColumn(
                name: "CompletedDate",
                table: "App_Recurrences");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "App_Recurrences");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ActivatedDate",
                table: "App_Recurrences",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CancellationReason",
                table: "App_Recurrences",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CancelledDate",
                table: "App_Recurrences",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedDate",
                table: "App_Recurrences",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "App_Recurrences",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_App_Recurrences_IsActive_Status",
                table: "App_Recurrences",
                columns: new[] { "IsActive", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_App_Recurrences_Status",
                table: "App_Recurrences",
                column: "Status");
        }
    }
}
