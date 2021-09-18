using Microsoft.EntityFrameworkCore.Migrations;

namespace Ext_Dynamics_API.Migrations
{
    public partial class courseIdnotak : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_Data_Column_Entries_CourseId",
                table: "Data_Column_Entries");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddUniqueConstraint(
                name: "AK_Data_Column_Entries_CourseId",
                table: "Data_Column_Entries",
                column: "CourseId");
        }
    }
}
