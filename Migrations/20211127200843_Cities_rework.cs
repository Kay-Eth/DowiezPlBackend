using Microsoft.EntityFrameworkCore.Migrations;

namespace DowiezPlBackend.Migrations
{
    public partial class Cities_rework : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Cities_CityName_CityDistrict",
                table: "Cities");

            migrationBuilder.DropColumn(
                name: "CityDistrict",
                table: "Cities");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CityDistrict",
                table: "Cities",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Cities_CityName_CityDistrict",
                table: "Cities",
                columns: new[] { "CityName", "CityDistrict" },
                unique: true);
        }
    }
}
