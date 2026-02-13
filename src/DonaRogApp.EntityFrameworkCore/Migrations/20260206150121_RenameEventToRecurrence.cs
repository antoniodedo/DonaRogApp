using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DonaRogApp.Migrations
{
    /// <inheritdoc />
    public partial class RenameEventToRecurrence : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_App_Campaigns_App_Events_EventId",
                table: "App_Campaigns");

            migrationBuilder.DropTable(
                name: "App_Events");

            migrationBuilder.RenameColumn(
                name: "EventId",
                table: "App_Campaigns",
                newName: "RecurrenceId");

            migrationBuilder.RenameColumn(
                name: "EventDate",
                table: "App_Campaigns",
                newName: "RecurrenceDate");

            migrationBuilder.RenameIndex(
                name: "IX_App_Campaigns_EventId",
                table: "App_Campaigns",
                newName: "IX_App_Campaigns_RecurrenceId");

            migrationBuilder.CreateTable(
                name: "App_Recurrences",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    RecurrenceDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DaysBeforeRecurrence = table.Column<int>(type: "int", nullable: false),
                    DaysAfterRecurrence = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DeactivatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeactivationReason = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ActivatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CancelledDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CancellationReason = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
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
                    table.PrimaryKey("PK_App_Recurrences", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_App_Recurrences_IsActive",
                table: "App_Recurrences",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_App_Recurrences_IsActive_Status",
                table: "App_Recurrences",
                columns: new[] { "IsActive", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_App_Recurrences_RecurrenceDate",
                table: "App_Recurrences",
                column: "RecurrenceDate");

            migrationBuilder.CreateIndex(
                name: "IX_App_Recurrences_Status",
                table: "App_Recurrences",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_App_Recurrences_TenantId_Code",
                table: "App_Recurrences",
                columns: new[] { "TenantId", "Code" },
                unique: true,
                filter: "[TenantId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_App_Campaigns_App_Recurrences_RecurrenceId",
                table: "App_Campaigns",
                column: "RecurrenceId",
                principalTable: "App_Recurrences",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_App_Campaigns_App_Recurrences_RecurrenceId",
                table: "App_Campaigns");

            migrationBuilder.DropTable(
                name: "App_Recurrences");

            migrationBuilder.RenameColumn(
                name: "RecurrenceId",
                table: "App_Campaigns",
                newName: "EventId");

            migrationBuilder.RenameColumn(
                name: "RecurrenceDate",
                table: "App_Campaigns",
                newName: "EventDate");

            migrationBuilder.RenameIndex(
                name: "IX_App_Campaigns_RecurrenceId",
                table: "App_Campaigns",
                newName: "IX_App_Campaigns_EventId");

            migrationBuilder.CreateTable(
                name: "App_Events",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActivatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CancellationReason = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    CancelledDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CompletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DaysAfterEvent = table.Column<int>(type: "int", nullable: false),
                    DaysBeforeEvent = table.Column<int>(type: "int", nullable: false),
                    DeactivatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeactivationReason = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    EventDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_Events", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_App_Events_EventDate",
                table: "App_Events",
                column: "EventDate");

            migrationBuilder.CreateIndex(
                name: "IX_App_Events_IsActive",
                table: "App_Events",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_App_Events_IsActive_Status",
                table: "App_Events",
                columns: new[] { "IsActive", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_App_Events_Status",
                table: "App_Events",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_App_Events_TenantId_Code",
                table: "App_Events",
                columns: new[] { "TenantId", "Code" },
                unique: true,
                filter: "[TenantId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_App_Campaigns_App_Events_EventId",
                table: "App_Campaigns",
                column: "EventId",
                principalTable: "App_Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
