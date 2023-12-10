using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace VeriBot.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuditLog",
                columns: table => new
                {
                    RowId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Who = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    WhoName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    What = table.Column<string>(type: "text", nullable: false),
                    WhereGuildId = table.Column<decimal>(type: "numeric(20,0)", nullable: true),
                    WhereGuildName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    WhereChannelId = table.Column<decimal>(type: "numeric(20,0)", nullable: true),
                    WhereChannelName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    When = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLog", x => x.RowId);
                });

            migrationBuilder.CreateTable(
                name: "CommandStatistics",
                columns: table => new
                {
                    RowId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CommandName = table.Column<string>(type: "text", nullable: true),
                    UsageCount = table.Column<long>(type: "bigint", nullable: false),
                    LastUsed = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommandStatistics", x => x.RowId);
                });

            migrationBuilder.CreateTable(
                name: "Guesses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    PuzzleLevel = table.Column<int>(type: "integer", nullable: false),
                    GuessContent = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Guesses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Guilds",
                columns: table => new
                {
                    RowId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DiscordId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    BotAddedTo = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CommandPrefix = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true, defaultValue: "+"),
                    LevelAnnouncementChannelId = table.Column<decimal>(type: "numeric(20,0)", nullable: true),
                    GoodBotVotes = table.Column<int>(type: "integer", nullable: false),
                    BadBotVotes = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    DadJokesEnabled = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Guilds", x => x.RowId);
                });

            migrationBuilder.CreateTable(
                name: "LoggedErrors",
                columns: table => new
                {
                    RowId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Message = table.Column<string>(type: "text", nullable: true),
                    StackTrace = table.Column<string>(type: "text", nullable: true),
                    SourceMethod = table.Column<string>(type: "text", nullable: true),
                    FullDetail = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoggedErrors", x => x.RowId);
                });

            migrationBuilder.CreateTable(
                name: "Pets",
                columns: table => new
                {
                    RowId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OwnerDiscordId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    Name = table.Column<string>(type: "character varying(70)", maxLength: 70, nullable: true),
                    Priority = table.Column<int>(type: "integer", nullable: false),
                    EarnedXp = table.Column<double>(type: "double precision", nullable: false),
                    CurrentLevel = table.Column<int>(type: "integer", nullable: false),
                    BornAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FoundAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Species = table.Column<int>(type: "integer", nullable: false),
                    Size = table.Column<int>(type: "integer", nullable: false),
                    Rarity = table.Column<int>(type: "integer", nullable: false),
                    IsCorrupt = table.Column<bool>(type: "boolean", nullable: false),
                    IsDead = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pets", x => x.RowId);
                });

            migrationBuilder.CreateTable(
                name: "PuzzleProgress",
                columns: table => new
                {
                    UserId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    CurrentLevel = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PuzzleProgress", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "UserAudits",
                columns: table => new
                {
                    RowId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GuildDiscordId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    CurrentRankRoleName = table.Column<string>(type: "text", nullable: true),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DiscordId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    MessageCount = table.Column<long>(type: "bigint", nullable: false),
                    TotalMessageLength = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    TimeSpentInVoiceSeconds = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    TimeSpentMutedSeconds = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    TimeSpentDeafenedSeconds = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    TimeSpentStreamingSeconds = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    TimeSpentOnVideoSeconds = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    TimeSpentAfkSeconds = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    TimeSpentDisconnectedSeconds = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    GuildRowId = table.Column<long>(type: "bigint", nullable: false),
                    MessageXpEarned = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    VoiceXpEarned = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    MutedXpEarned = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    DeafenedXpEarned = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    StreamingXpEarned = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    VideoXpEarned = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    DisconnectedXpEarned = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    CurrentLevel = table.Column<int>(type: "integer", nullable: false),
                    CurrentRankRoleRowId = table.Column<long>(type: "bigint", nullable: true),
                    ActivityStreakXpEarned = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    ConsecutiveDaysActive = table.Column<int>(type: "integer", nullable: false),
                    LastActiveDay = table.Column<DateOnly>(type: "date", nullable: false),
                    OptedOutOfMentions = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAudits", x => x.RowId);
                });

            migrationBuilder.CreateTable(
                name: "RankRoles",
                columns: table => new
                {
                    RowId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleDiscordId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    RoleName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    GuildRowId = table.Column<long>(type: "bigint", nullable: false),
                    LevelRequired = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RankRoles", x => x.RowId);
                    table.ForeignKey(
                        name: "FK_RankRoles_Guilds_GuildRowId",
                        column: x => x.GuildRowId,
                        principalTable: "Guilds",
                        principalColumn: "RowId");
                });

            migrationBuilder.CreateTable(
                name: "SelfRoles",
                columns: table => new
                {
                    RowId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    DiscordRoleId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    GuildRowId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SelfRoles", x => x.RowId);
                    table.ForeignKey(
                        name: "FK_SelfRoles_Guilds_GuildRowId",
                        column: x => x.GuildRowId,
                        principalTable: "Guilds",
                        principalColumn: "RowId");
                });

            migrationBuilder.CreateTable(
                name: "PetAttributes",
                columns: table => new
                {
                    RowId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PetId = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PetAttributes", x => x.RowId);
                    table.ForeignKey(
                        name: "FK_PetAttributes_Pets_PetId",
                        column: x => x.PetId,
                        principalTable: "Pets",
                        principalColumn: "RowId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PetBonuses",
                columns: table => new
                {
                    RowId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PetId = table.Column<long>(type: "bigint", nullable: false),
                    BonusType = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PetBonuses", x => x.RowId);
                    table.ForeignKey(
                        name: "FK_PetBonuses_Pets_PetId",
                        column: x => x.PetId,
                        principalTable: "Pets",
                        principalColumn: "RowId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    RowId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserFirstSeen = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    MutedStartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeafenedStartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    StreamingStartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    VideoStartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    VoiceStartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AfkStartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DisconnectedStartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastActivity = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastMessageSent = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastXpEarningMessage = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DiscordId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    MessageCount = table.Column<long>(type: "bigint", nullable: false),
                    TotalMessageLength = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    TimeSpentInVoiceSeconds = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    TimeSpentMutedSeconds = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    TimeSpentDeafenedSeconds = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    TimeSpentStreamingSeconds = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    TimeSpentOnVideoSeconds = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    TimeSpentAfkSeconds = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    TimeSpentDisconnectedSeconds = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    GuildRowId = table.Column<long>(type: "bigint", nullable: false),
                    MessageXpEarned = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    VoiceXpEarned = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    MutedXpEarned = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    DeafenedXpEarned = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    StreamingXpEarned = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    VideoXpEarned = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    DisconnectedXpEarned = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    CurrentLevel = table.Column<int>(type: "integer", nullable: false),
                    CurrentRankRoleRowId = table.Column<long>(type: "bigint", nullable: true),
                    ActivityStreakXpEarned = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    ConsecutiveDaysActive = table.Column<int>(type: "integer", nullable: false),
                    LastActiveDay = table.Column<DateOnly>(type: "date", nullable: false),
                    OptedOutOfMentions = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.RowId);
                    table.ForeignKey(
                        name: "FK_Users_Guilds_GuildRowId",
                        column: x => x.GuildRowId,
                        principalTable: "Guilds",
                        principalColumn: "RowId");
                    table.ForeignKey(
                        name: "FK_Users_RankRoles_CurrentRankRoleRowId",
                        column: x => x.CurrentRankRoleRowId,
                        principalTable: "RankRoles",
                        principalColumn: "RowId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Triggers",
                columns: table => new
                {
                    RowId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TriggerText = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    ExactMatch = table.Column<bool>(type: "boolean", nullable: false),
                    Response = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    GuildRowId = table.Column<long>(type: "bigint", nullable: false),
                    CreatorRowId = table.Column<long>(type: "bigint", nullable: false),
                    ChannelDiscordId = table.Column<decimal>(type: "numeric(20,0)", nullable: true),
                    TimesActivated = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Triggers", x => x.RowId);
                    table.ForeignKey(
                        name: "FK_Triggers_Guilds_GuildRowId",
                        column: x => x.GuildRowId,
                        principalTable: "Guilds",
                        principalColumn: "RowId");
                    table.ForeignKey(
                        name: "FK_Triggers_Users_CreatorRowId",
                        column: x => x.CreatorRowId,
                        principalTable: "Users",
                        principalColumn: "RowId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CommandStatistics_CommandName",
                table: "CommandStatistics",
                column: "CommandName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Guilds_DiscordId",
                table: "Guilds",
                column: "DiscordId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PetAttributes_PetId",
                table: "PetAttributes",
                column: "PetId");

            migrationBuilder.CreateIndex(
                name: "IX_PetBonuses_PetId",
                table: "PetBonuses",
                column: "PetId");

            migrationBuilder.CreateIndex(
                name: "IX_Pets_OwnerDiscordId",
                table: "Pets",
                column: "OwnerDiscordId");

            migrationBuilder.CreateIndex(
                name: "IX_RankRoles_GuildRowId",
                table: "RankRoles",
                column: "GuildRowId");

            migrationBuilder.CreateIndex(
                name: "IX_SelfRoles_GuildRowId",
                table: "SelfRoles",
                column: "GuildRowId");

            migrationBuilder.CreateIndex(
                name: "IX_Triggers_CreatorRowId",
                table: "Triggers",
                column: "CreatorRowId");

            migrationBuilder.CreateIndex(
                name: "IX_Triggers_GuildRowId",
                table: "Triggers",
                column: "GuildRowId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_CurrentRankRoleRowId",
                table: "Users",
                column: "CurrentRankRoleRowId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_GuildRowId",
                table: "Users",
                column: "GuildRowId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLog");

            migrationBuilder.DropTable(
                name: "CommandStatistics");

            migrationBuilder.DropTable(
                name: "Guesses");

            migrationBuilder.DropTable(
                name: "LoggedErrors");

            migrationBuilder.DropTable(
                name: "PetAttributes");

            migrationBuilder.DropTable(
                name: "PetBonuses");

            migrationBuilder.DropTable(
                name: "PuzzleProgress");

            migrationBuilder.DropTable(
                name: "SelfRoles");

            migrationBuilder.DropTable(
                name: "Triggers");

            migrationBuilder.DropTable(
                name: "UserAudits");

            migrationBuilder.DropTable(
                name: "Pets");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "RankRoles");

            migrationBuilder.DropTable(
                name: "Guilds");
        }
    }
}
