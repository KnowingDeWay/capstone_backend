using Microsoft.EntityFrameworkCore.Migrations;

namespace Ext_Dynamics_API.Migrations
{
    public partial class nullablerowversion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn("RowVersion", "Token_Entries");
            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Token_Entries",
                type: "rowversion",
                rowVersion: true,
                nullable: true);

            migrationBuilder.DropColumn("RowVersion", "Personal_Access_Tokens");
            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Personal_Access_Tokens",
                type: "rowversion",
                rowVersion: true,
                nullable: true);

            migrationBuilder.DropColumn("RowVersion", "App_Users");
            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "App_Users",
                type: "rowversion",
                rowVersion: true,
                nullable: true);


            migrationBuilder.AlterColumn<int>(
                name: "RelatedDataId",
                table: "Data_Column_Entries",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Data_Column_Entries_RelatedDataId",
                table: "Data_Column_Entries",
                column: "RelatedDataId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_Data_Column_Entries_RelatedDataId",
                table: "Data_Column_Entries");

            migrationBuilder.AlterColumn<int>(
                name: "RelatedDataId",
                table: "Data_Column_Entries",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
