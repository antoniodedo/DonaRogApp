using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DonaRogApp.Migrations
{
    /// <inheritdoc />
    public partial class RemoveEventType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_App_Events_EventType",
                table: "App_Events");

            migrationBuilder.DropIndex(
                name: "IX_App_Events_IsActive_EventType",
                table: "App_Events");

            migrationBuilder.DropColumn(
                name: "EventType",
                table: "App_Events");

            migrationBuilder.CreateIndex(
                name: "IX_App_Events_IsActive_Status",
                table: "App_Events",
                columns: new[] { "IsActive", "Status" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_App_Events_IsActive_Status",
                table: "App_Events");

            migrationBuilder.AddColumn<int>(
                name: "EventType",
                table: "App_Events",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_App_Events_EventType",
                table: "App_Events",
                column: "EventType");

            migrationBuilder.CreateIndex(
                name: "IX_App_Events_IsActive_EventType",
                table: "App_Events",
                columns: new[] { "IsActive", "EventType" });
        }
    }
}
