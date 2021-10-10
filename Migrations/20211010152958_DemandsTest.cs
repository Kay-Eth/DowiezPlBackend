using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DowiezPlBackend.Migrations
{
    public partial class DemandsTest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Demands_Cities_DestinationCityId",
                table: "Demands");

            migrationBuilder.AlterColumn<Guid>(
                name: "DestinationCityId",
                table: "Demands",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true)
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AddForeignKey(
                name: "FK_Demands_Cities_DestinationCityId",
                table: "Demands",
                column: "DestinationCityId",
                principalTable: "Cities",
                principalColumn: "CityId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Demands_Cities_DestinationCityId",
                table: "Demands");

            migrationBuilder.AlterColumn<Guid>(
                name: "DestinationCityId",
                table: "Demands",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AddForeignKey(
                name: "FK_Demands_Cities_DestinationCityId",
                table: "Demands",
                column: "DestinationCityId",
                principalTable: "Cities",
                principalColumn: "CityId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
