using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DowiezPlBackend.Migrations
{
    public partial class ReportsAndGroups : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ReportedDemandDemandId",
                table: "Reports",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<Guid>(
                name: "ReportedGroupGroupId",
                table: "Reports",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<Guid>(
                name: "ReportedTransportTransportId",
                table: "Reports",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<string>(
                name: "GroupPassword",
                table: "Groups",
                type: "varchar(256)",
                maxLength: 256,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_ReportedDemandDemandId",
                table: "Reports",
                column: "ReportedDemandDemandId");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_ReportedGroupGroupId",
                table: "Reports",
                column: "ReportedGroupGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_ReportedTransportTransportId",
                table: "Reports",
                column: "ReportedTransportTransportId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_Demands_ReportedDemandDemandId",
                table: "Reports",
                column: "ReportedDemandDemandId",
                principalTable: "Demands",
                principalColumn: "DemandId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_Groups_ReportedGroupGroupId",
                table: "Reports",
                column: "ReportedGroupGroupId",
                principalTable: "Groups",
                principalColumn: "GroupId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_Transports_ReportedTransportTransportId",
                table: "Reports",
                column: "ReportedTransportTransportId",
                principalTable: "Transports",
                principalColumn: "TransportId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reports_Demands_ReportedDemandDemandId",
                table: "Reports");

            migrationBuilder.DropForeignKey(
                name: "FK_Reports_Groups_ReportedGroupGroupId",
                table: "Reports");

            migrationBuilder.DropForeignKey(
                name: "FK_Reports_Transports_ReportedTransportTransportId",
                table: "Reports");

            migrationBuilder.DropIndex(
                name: "IX_Reports_ReportedDemandDemandId",
                table: "Reports");

            migrationBuilder.DropIndex(
                name: "IX_Reports_ReportedGroupGroupId",
                table: "Reports");

            migrationBuilder.DropIndex(
                name: "IX_Reports_ReportedTransportTransportId",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "ReportedDemandDemandId",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "ReportedGroupGroupId",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "ReportedTransportTransportId",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "GroupPassword",
                table: "Groups");
        }
    }
}
