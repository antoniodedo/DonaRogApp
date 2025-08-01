using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DonaRogApp.Migrations
{
    /// <inheritdoc />
    public partial class change_DbTablePrefix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AppDonors",
                table: "AppDonors");

            migrationBuilder.RenameTable(
                name: "AppDonors",
                newName: "App_Donors");

            migrationBuilder.AddPrimaryKey(
                name: "PK_App_Donors",
                table: "App_Donors",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_App_Donors",
                table: "App_Donors");

            migrationBuilder.RenameTable(
                name: "App_Donors",
                newName: "AppDonors");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AppDonors",
                table: "AppDonors",
                column: "Id");
        }
    }
}
