using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Ext_Dynamics_API.Migrations
{
    public partial class loginlogout : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Personal_Access_Tokens_App_Users_ApplicationUserAccountAppUserId",
                table: "Personal_Access_Tokens");

            migrationBuilder.DropIndex(
                name: "IX_Personal_Access_Tokens_ApplicationUserAccountAppUserId",
                table: "Personal_Access_Tokens");

            migrationBuilder.DropColumn(
                name: "ApplicationUserAccountAppUserId",
                table: "Personal_Access_Tokens");

            migrationBuilder.RenameColumn(
                name: "PatId",
                table: "Personal_Access_Tokens",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "AppUserId",
                table: "App_Users",
                newName: "Id");

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Personal_Access_Tokens",
                type: "rowversion",
                rowVersion: true,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "App_Users",
                type: "rowversion",
                rowVersion: true,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Token_Entries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EncodedToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AppUserId = table.Column<int>(type: "int", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Token_Entries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Token_Entries_App_Users_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "App_Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Personal_Access_Tokens_AppUserId",
                table: "Personal_Access_Tokens",
                column: "AppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Token_Entries_AppUserId",
                table: "Token_Entries",
                column: "AppUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Personal_Access_Tokens_App_Users_AppUserId",
                table: "Personal_Access_Tokens",
                column: "AppUserId",
                principalTable: "App_Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Personal_Access_Tokens_App_Users_AppUserId",
                table: "Personal_Access_Tokens");

            migrationBuilder.DropTable(
                name: "Token_Entries");

            migrationBuilder.DropIndex(
                name: "IX_Personal_Access_Tokens_AppUserId",
                table: "Personal_Access_Tokens");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Personal_Access_Tokens");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "App_Users");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Personal_Access_Tokens",
                newName: "PatId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "App_Users",
                newName: "AppUserId");

            migrationBuilder.AddColumn<int>(
                name: "ApplicationUserAccountAppUserId",
                table: "Personal_Access_Tokens",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Personal_Access_Tokens_ApplicationUserAccountAppUserId",
                table: "Personal_Access_Tokens",
                column: "ApplicationUserAccountAppUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Personal_Access_Tokens_App_Users_ApplicationUserAccountAppUserId",
                table: "Personal_Access_Tokens",
                column: "ApplicationUserAccountAppUserId",
                principalTable: "App_Users",
                principalColumn: "AppUserId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
