using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DonaRogApp.Migrations
{
    /// <inheritdoc />
    public partial class RefactorEventsRemoveYearAddValidityRange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_App_Events_EndDate",
                table: "App_Events");

            migrationBuilder.DropIndex(
                name: "IX_App_Events_StartDate",
                table: "App_Events");

            migrationBuilder.DropIndex(
                name: "IX_App_Events_TenantId_Code_Year",
                table: "App_Events");

            migrationBuilder.DropIndex(
                name: "IX_App_Events_Year",
                table: "App_Events");

            migrationBuilder.DropIndex(
                name: "IX_App_Events_Year_Status",
                table: "App_Events");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "App_Events");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "App_Events");

            migrationBuilder.RenameColumn(
                name: "Year",
                table: "App_Events",
                newName: "DaysBeforeEvent");

            migrationBuilder.AddColumn<int>(
                name: "DaysAfterEvent",
                table: "App_Events",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeactivatedDate",
                table: "App_Events",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeactivationReason",
                table: "App_Events",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EventDate",
                table: "App_Events",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "App_Events",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_App_Events_EventDate",
                table: "App_Events",
                column: "EventDate");

            migrationBuilder.CreateIndex(
                name: "IX_App_Events_IsActive",
                table: "App_Events",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_App_Events_IsActive_EventType",
                table: "App_Events",
                columns: new[] { "IsActive", "EventType" });

            migrationBuilder.CreateIndex(
                name: "IX_App_Events_TenantId_Code",
                table: "App_Events",
                columns: new[] { "TenantId", "Code" },
                unique: true,
                filter: "[TenantId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_App_Events_EventDate",
                table: "App_Events");

            migrationBuilder.DropIndex(
                name: "IX_App_Events_IsActive",
                table: "App_Events");

            migrationBuilder.DropIndex(
                name: "IX_App_Events_IsActive_EventType",
                table: "App_Events");

            migrationBuilder.DropIndex(
                name: "IX_App_Events_TenantId_Code",
                table: "App_Events");

            migrationBuilder.DropColumn(
                name: "DaysAfterEvent",
                table: "App_Events");

            migrationBuilder.DropColumn(
                name: "DeactivatedDate",
                table: "App_Events");

            migrationBuilder.DropColumn(
                name: "DeactivationReason",
                table: "App_Events");

            migrationBuilder.DropColumn(
                name: "EventDate",
                table: "App_Events");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "App_Events");

            migrationBuilder.RenameColumn(
                name: "DaysBeforeEvent",
                table: "App_Events",
                newName: "Year");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "App_Events",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "App_Events",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_App_Events_EndDate",
                table: "App_Events",
                column: "EndDate");

            migrationBuilder.CreateIndex(
                name: "IX_App_Events_StartDate",
                table: "App_Events",
                column: "StartDate");

            migrationBuilder.CreateIndex(
                name: "IX_App_Events_TenantId_Code_Year",
                table: "App_Events",
                columns: new[] { "TenantId", "Code", "Year" },
                unique: true,
                filter: "[TenantId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_App_Events_Year",
                table: "App_Events",
                column: "Year");

            migrationBuilder.CreateIndex(
                name: "IX_App_Events_Year_Status",
                table: "App_Events",
                columns: new[] { "Year", "Status" });
        }
    }
}
