using Microsoft.EntityFrameworkCore.Migrations;

namespace Ext_Dynamics_API.Migrations
{
    public partial class appuseraccount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "App_Users",
                columns: table => new
                {
                    AppUserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AppUserName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserPassword = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_Users", x => x.AppUserId);
                    table.UniqueConstraint("AK_App_Users_AppUserName", x => x.AppUserName);
                });

            migrationBuilder.CreateTable(
                name: "Personal_Access_Tokens",
                columns: table => new
                {
                    PatId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TokenName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccessToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AppUserId = table.Column<int>(type: "int", nullable: false),
                    ApplicationUserAccountAppUserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Personal_Access_Tokens", x => x.PatId);
                    table.ForeignKey(
                        name: "FK_Personal_Access_Tokens_App_Users_ApplicationUserAccountAppUserId",
                        column: x => x.ApplicationUserAccountAppUserId,
                        principalTable: "App_Users",
                        principalColumn: "AppUserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Personal_Access_Tokens_ApplicationUserAccountAppUserId",
                table: "Personal_Access_Tokens",
                column: "ApplicationUserAccountAppUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Personal_Access_Tokens");

            migrationBuilder.DropTable(
                name: "App_Users");
        }
    }
}
