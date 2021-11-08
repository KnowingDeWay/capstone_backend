using Microsoft.EntityFrameworkCore.Migrations;

namespace Ext_Dynamics_API.Migrations
{
    public partial class ScopeCanvasStudentId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CanvasStudentId",
                table: "Custom_Data_Scopes",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CanvasStudentId",
                table: "Custom_Data_Scopes");
        }
    }
}
