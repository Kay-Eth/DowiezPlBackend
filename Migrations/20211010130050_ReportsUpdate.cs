using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DowiezPlBackend.Migrations
{
    public partial class ReportsUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "OperatorId",
                table: "Reports",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_OperatorId",
                table: "Reports",
                column: "OperatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_AspNetUsers_OperatorId",
                table: "Reports",
                column: "OperatorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reports_AspNetUsers_OperatorId",
                table: "Reports");

            migrationBuilder.DropIndex(
                name: "IX_Reports_OperatorId",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "OperatorId",
                table: "Reports");
        }
    }
}
