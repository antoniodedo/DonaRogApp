using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DonaRogApp.Migrations
{
    /// <inheritdoc />
    public partial class FixUniqueIndexesForMultiTenancy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_App_Titles_Code",
                table: "App_Titles");

            migrationBuilder.DropIndex(
                name: "IX_App_Tags_Code",
                table: "App_Tags");

            migrationBuilder.DropIndex(
                name: "IX_App_Segments_Code",
                table: "App_Segments");

            migrationBuilder.DropIndex(
                name: "IX_App_Interests_Code",
                table: "App_Interests");

            migrationBuilder.CreateIndex(
                name: "IX_App_Titles_TenantId_Code",
                table: "App_Titles",
                columns: new[] { "TenantId", "Code" },
                unique: true,
                filter: "[TenantId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_App_Tags_TenantId_Code",
                table: "App_Tags",
                columns: new[] { "TenantId", "Code" },
                unique: true,
                filter: "[TenantId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_App_Segments_TenantId_Code",
                table: "App_Segments",
                columns: new[] { "TenantId", "Code" },
                unique: true,
                filter: "[TenantId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_App_Interests_TenantId_Code",
                table: "App_Interests",
                columns: new[] { "TenantId", "Code" },
                unique: true,
                filter: "[TenantId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_App_Titles_TenantId_Code",
                table: "App_Titles");

            migrationBuilder.DropIndex(
                name: "IX_App_Tags_TenantId_Code",
                table: "App_Tags");

            migrationBuilder.DropIndex(
                name: "IX_App_Segments_TenantId_Code",
                table: "App_Segments");

            migrationBuilder.DropIndex(
                name: "IX_App_Interests_TenantId_Code",
                table: "App_Interests");

            migrationBuilder.CreateIndex(
                name: "IX_App_Titles_Code",
                table: "App_Titles",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_App_Tags_Code",
                table: "App_Tags",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_App_Segments_Code",
                table: "App_Segments",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_App_Interests_Code",
                table: "App_Interests",
                column: "Code",
                unique: true);
        }
    }
}
