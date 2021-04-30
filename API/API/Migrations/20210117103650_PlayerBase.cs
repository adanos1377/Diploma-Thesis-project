using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Migrations
{
    public partial class PlayerBase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "test");

            migrationBuilder.CreateTable(
                name: "Players",
                schema: "test",
                columns: table => new
                {
                    PlayerId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NickName = table.Column<string>(maxLength: 16, nullable: false),
                    SkillRating = table.Column<double>(nullable: false),
                    GamesPlayed = table.Column<int>(nullable: false),
                    GamesWon = table.Column<int>(nullable: false),
                    GamesTied = table.Column<int>(nullable: false),
                    GamesLost = table.Column<int>(nullable: false),
                    WinRate = table.Column<double>(nullable: false),
                    Rank = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.PlayerId);
                });

            migrationBuilder.CreateTable(
                name: "Solo Matches",
                schema: "test",
                columns: table => new
                {
                    GameId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Players = table.Column<string>(nullable: true),
                    RankedGame = table.Column<bool>(nullable: false),
                    GameDate = table.Column<DateTime>(nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    Scores = table.Column<string>(nullable: false),
                    Finished = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Solo Matches", x => x.GameId);
                });

            migrationBuilder.CreateTable(
                name: "Team Matches",
                schema: "test",
                columns: table => new
                {
                    GameId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Teams = table.Column<string>(nullable: true),
                    RankedGame = table.Column<bool>(nullable: false),
                    GameDate = table.Column<DateTime>(nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    Scores = table.Column<string>(nullable: false),
                    Finished = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Team Matches", x => x.GameId);
                });

            migrationBuilder.CreateTable(
                name: "Teams",
                schema: "test",
                columns: table => new
                {
                    TeamID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PlayersID = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teams", x => x.TeamID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Players_NickName",
                schema: "test",
                table: "Players",
                column: "NickName",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Players",
                schema: "test");

            migrationBuilder.DropTable(
                name: "Solo Matches",
                schema: "test");

            migrationBuilder.DropTable(
                name: "Team Matches",
                schema: "test");

            migrationBuilder.DropTable(
                name: "Teams",
                schema: "test");
        }
    }
}
