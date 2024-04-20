using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VeriBot.Migrations
{
    /// <inheritdoc />
    public partial class Addchannelforselfrolemessage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "SelfRolesAssignmentMessageChannelId",
                table: "Guilds",
                type: "numeric(20,0)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SelfRolesAssignmentMessageChannelId",
                table: "Guilds");
        }
    }
}
