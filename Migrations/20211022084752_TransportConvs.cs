using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DowiezPlBackend.Migrations
{
    public partial class TransportConvs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ConversationId",
                table: "Transports",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_Transports_ConversationId",
                table: "Transports",
                column: "ConversationId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Transports_Conversations_ConversationId",
                table: "Transports",
                column: "ConversationId",
                principalTable: "Conversations",
                principalColumn: "ConversationId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transports_Conversations_ConversationId",
                table: "Transports");

            migrationBuilder.DropIndex(
                name: "IX_Transports_ConversationId",
                table: "Transports");

            migrationBuilder.DropColumn(
                name: "ConversationId",
                table: "Transports");
        }
    }
}
