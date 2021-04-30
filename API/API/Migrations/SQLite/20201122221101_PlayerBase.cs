using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Migrations.SQLite
{
    public partial class PlayerBase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PlayersDB",
                columns: table => new
                {
                    Nickname = table.Column<string>(nullable: false),
                    Email = table.Column<string>(maxLength: 250, nullable: false),
                    Password = table.Column<string>(nullable: false),
                    Localization = table.Column<string>(nullable: false),
                    Adress = table.Column<string>(nullable: false),
                    Rank = table.Column<string>(nullable: true),
                    WinRate = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayersDB", x => x.Nickname);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlayersDB");
        }
    }
}
