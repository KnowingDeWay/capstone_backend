using Microsoft.EntityFrameworkCore.Migrations;

namespace Ext_Dynamics_API.Migrations
{
    public partial class CustDataEntry : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_User_Custom_Data_Entries_ItemName",
                table: "User_Custom_Data_Entries");

            migrationBuilder.DropColumn(
                name: "Scope",
                table: "User_Custom_Data_Entries");

            migrationBuilder.DropColumn(
                name: "Namespace",
                table: "Custom_Data_Scopes");

            migrationBuilder.RenameColumn(
                name: "CanvasUserId",
                table: "User_Custom_Data_Entries",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "CanvasUserId",
                table: "Custom_Data_Scopes",
                newName: "UserId");

            migrationBuilder.AlterColumn<string>(
                name: "ItemName",
                table: "User_Custom_Data_Entries",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<int>(
                name: "ScopeId",
                table: "User_Custom_Data_Entries",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_User_Custom_Data_Entries_ScopeId",
                table: "User_Custom_Data_Entries",
                column: "ScopeId");

            migrationBuilder.CreateIndex(
                name: "IX_Custom_Data_Scopes_UserId",
                table: "Custom_Data_Scopes",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Custom_Data_Scopes_App_Users_UserId",
                table: "Custom_Data_Scopes",
                column: "UserId",
                principalTable: "App_Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_User_Custom_Data_Entries_Custom_Data_Scopes_ScopeId",
                table: "User_Custom_Data_Entries",
                column: "ScopeId",
                principalTable: "Custom_Data_Scopes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Custom_Data_Scopes_App_Users_UserId",
                table: "Custom_Data_Scopes");

            migrationBuilder.DropForeignKey(
                name: "FK_User_Custom_Data_Entries_Custom_Data_Scopes_ScopeId",
                table: "User_Custom_Data_Entries");

            migrationBuilder.DropIndex(
                name: "IX_User_Custom_Data_Entries_ScopeId",
                table: "User_Custom_Data_Entries");

            migrationBuilder.DropIndex(
                name: "IX_Custom_Data_Scopes_UserId",
                table: "Custom_Data_Scopes");

            migrationBuilder.DropColumn(
                name: "ScopeId",
                table: "User_Custom_Data_Entries");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "User_Custom_Data_Entries",
                newName: "CanvasUserId");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Custom_Data_Scopes",
                newName: "CanvasUserId");

            migrationBuilder.AlterColumn<string>(
                name: "ItemName",
                table: "User_Custom_Data_Entries",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "Scope",
                table: "User_Custom_Data_Entries",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Namespace",
                table: "Custom_Data_Scopes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_User_Custom_Data_Entries_ItemName",
                table: "User_Custom_Data_Entries",
                column: "ItemName");
        }
    }
}
