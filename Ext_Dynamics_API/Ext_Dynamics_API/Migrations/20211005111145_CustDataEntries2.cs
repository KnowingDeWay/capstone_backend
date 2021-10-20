using Microsoft.EntityFrameworkCore.Migrations;

namespace Ext_Dynamics_API.Migrations
{
    public partial class CustDataEntries2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CourseId",
                table: "User_Custom_Data_Entries",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CourseId",
                table: "Custom_Data_Scopes",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CourseId",
                table: "User_Custom_Data_Entries");

            migrationBuilder.DropColumn(
                name: "CourseId",
                table: "Custom_Data_Scopes");
        }
    }
}
