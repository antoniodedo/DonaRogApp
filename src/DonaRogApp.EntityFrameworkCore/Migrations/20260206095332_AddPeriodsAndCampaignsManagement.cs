using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DonaRogApp.Migrations
{
    /// <inheritdoc />
    public partial class AddPeriodsAndCampaignsManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DonorId1",
                table: "App_DonorStatusHistories",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "App_Events",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Year = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EventType = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
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
                    table.PrimaryKey("PK_App_Events", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "App_Campaigns",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CampaignType = table.Column<int>(type: "int", nullable: false),
                    Channel = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    EventId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExtractionScheduledDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExtractionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DispatchScheduledDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DispatchDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EventDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    YearlySequenceNumber = table.Column<int>(type: "int", nullable: true),
                    PostalCodeSequence = table.Column<int>(type: "int", nullable: true),
                    PostalCodeYearSuffix = table.Column<int>(type: "int", nullable: true),
                    MailchimpCampaignId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    MailchimpListId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    SmsProviderId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    TotalCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalRaised = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TargetDonorCount = table.Column<int>(type: "int", nullable: false),
                    ExtractedDonorCount = table.Column<int>(type: "int", nullable: false),
                    DispatchedCount = table.Column<int>(type: "int", nullable: false),
                    ResponseCount = table.Column<int>(type: "int", nullable: false),
                    ResponseRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DonationCount = table.Column<int>(type: "int", nullable: false),
                    AverageDonation = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ConversionRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ROI = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
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
                    table.PrimaryKey("PK_App_Campaigns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_App_Campaigns_App_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "App_Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "App_CampaignDonors",
                columns: table => new
                {
                    CampaignId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DonorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TrackingCode = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TrackingCampaignId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TrackingDonorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TrackingHash = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    ExtractedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DispatchedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OpenedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ClickedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ResponseType = table.Column<int>(type: "int", nullable: false),
                    DonationAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DonationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    RemovedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_CampaignDonors", x => new { x.CampaignId, x.DonorId });
                    table.ForeignKey(
                        name: "FK_App_CampaignDonors_App_Campaigns_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "App_Campaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_App_CampaignDonors_App_Donors_DonorId",
                        column: x => x.DonorId,
                        principalTable: "App_Donors",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_App_DonorStatusHistories_DonorId1",
                table: "App_DonorStatusHistories",
                column: "DonorId1");

            migrationBuilder.CreateIndex(
                name: "IX_App_CampaignDonors_CampaignId_ResponseType",
                table: "App_CampaignDonors",
                columns: new[] { "CampaignId", "ResponseType" });

            migrationBuilder.CreateIndex(
                name: "IX_App_CampaignDonors_ClickedAt",
                table: "App_CampaignDonors",
                column: "ClickedAt");

            migrationBuilder.CreateIndex(
                name: "IX_App_CampaignDonors_DispatchedAt",
                table: "App_CampaignDonors",
                column: "DispatchedAt");

            migrationBuilder.CreateIndex(
                name: "IX_App_CampaignDonors_DonationDate",
                table: "App_CampaignDonors",
                column: "DonationDate");

            migrationBuilder.CreateIndex(
                name: "IX_App_CampaignDonors_DonorId",
                table: "App_CampaignDonors",
                column: "DonorId");

            migrationBuilder.CreateIndex(
                name: "IX_App_CampaignDonors_ExtractedAt",
                table: "App_CampaignDonors",
                column: "ExtractedAt");

            migrationBuilder.CreateIndex(
                name: "IX_App_CampaignDonors_OpenedAt",
                table: "App_CampaignDonors",
                column: "OpenedAt");

            migrationBuilder.CreateIndex(
                name: "IX_App_CampaignDonors_ResponseType",
                table: "App_CampaignDonors",
                column: "ResponseType");

            migrationBuilder.CreateIndex(
                name: "IX_App_Campaigns_CampaignType",
                table: "App_Campaigns",
                column: "CampaignType");

            migrationBuilder.CreateIndex(
                name: "IX_App_Campaigns_Channel",
                table: "App_Campaigns",
                column: "Channel");

            migrationBuilder.CreateIndex(
                name: "IX_App_Campaigns_DispatchDate",
                table: "App_Campaigns",
                column: "DispatchDate");

            migrationBuilder.CreateIndex(
                name: "IX_App_Campaigns_EventId",
                table: "App_Campaigns",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_App_Campaigns_Status",
                table: "App_Campaigns",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_App_Campaigns_TenantId_Year_Code",
                table: "App_Campaigns",
                columns: new[] { "TenantId", "Year", "Code" },
                unique: true,
                filter: "[TenantId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_App_Campaigns_Year",
                table: "App_Campaigns",
                column: "Year");

            migrationBuilder.CreateIndex(
                name: "IX_App_Campaigns_Year_Status",
                table: "App_Campaigns",
                columns: new[] { "Year", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_App_Events_EndDate",
                table: "App_Events",
                column: "EndDate");

            migrationBuilder.CreateIndex(
                name: "IX_App_Events_EventType",
                table: "App_Events",
                column: "EventType");

            migrationBuilder.CreateIndex(
                name: "IX_App_Events_StartDate",
                table: "App_Events",
                column: "StartDate");

            migrationBuilder.CreateIndex(
                name: "IX_App_Events_Status",
                table: "App_Events",
                column: "Status");

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

            migrationBuilder.AddForeignKey(
                name: "FK_App_DonorStatusHistories_App_Donors_DonorId1",
                table: "App_DonorStatusHistories",
                column: "DonorId1",
                principalTable: "App_Donors",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_App_DonorStatusHistories_App_Donors_DonorId1",
                table: "App_DonorStatusHistories");

            migrationBuilder.DropTable(
                name: "App_CampaignDonors");

            migrationBuilder.DropTable(
                name: "App_Campaigns");

            migrationBuilder.DropTable(
                name: "App_Events");

            migrationBuilder.DropIndex(
                name: "IX_App_DonorStatusHistories_DonorId1",
                table: "App_DonorStatusHistories");

            migrationBuilder.DropColumn(
                name: "DonorId1",
                table: "App_DonorStatusHistories");
        }
    }
}
