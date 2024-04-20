using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VeriBot.Migrations
{
    /// <inheritdoc />
    public partial class Addemojireactionforselfroles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "EmojiId",
                table: "SelfRoles",
                type: "numeric(20,0)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SelfRolesAssignmentMessageId",
                table: "Guilds",
                type: "numeric(20,0)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmojiId",
                table: "SelfRoles");

            migrationBuilder.DropColumn(
                name: "SelfRolesAssignmentMessageId",
                table: "Guilds");
        }
    }
}
