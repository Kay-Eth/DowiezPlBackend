using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DowiezPlBackend.Migrations
{
    public partial class TransportTest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transports_Cities_EndsInCityId",
                table: "Transports");

            migrationBuilder.DropForeignKey(
                name: "FK_Transports_Cities_StartsInCityId",
                table: "Transports");

            migrationBuilder.AlterColumn<Guid>(
                name: "StartsInCityId",
                table: "Transports",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true)
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<Guid>(
                name: "EndsInCityId",
                table: "Transports",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true)
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AddForeignKey(
                name: "FK_Transports_Cities_EndsInCityId",
                table: "Transports",
                column: "EndsInCityId",
                principalTable: "Cities",
                principalColumn: "CityId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Transports_Cities_StartsInCityId",
                table: "Transports",
                column: "StartsInCityId",
                principalTable: "Cities",
                principalColumn: "CityId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transports_Cities_EndsInCityId",
                table: "Transports");

            migrationBuilder.DropForeignKey(
                name: "FK_Transports_Cities_StartsInCityId",
                table: "Transports");

            migrationBuilder.AlterColumn<Guid>(
                name: "StartsInCityId",
                table: "Transports",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<Guid>(
                name: "EndsInCityId",
                table: "Transports",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AddForeignKey(
                name: "FK_Transports_Cities_EndsInCityId",
                table: "Transports",
                column: "EndsInCityId",
                principalTable: "Cities",
                principalColumn: "CityId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Transports_Cities_StartsInCityId",
                table: "Transports",
                column: "StartsInCityId",
                principalTable: "Cities",
                principalColumn: "CityId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
