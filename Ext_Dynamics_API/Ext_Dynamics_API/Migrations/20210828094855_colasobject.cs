using Microsoft.EntityFrameworkCore.Migrations;

namespace Ext_Dynamics_API.Migrations
{
    public partial class colasobject : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CalcRule",
                table: "Data_Column_Entries");

            migrationBuilder.DropColumn(
                name: "ColMaxValue",
                table: "Data_Column_Entries");

            migrationBuilder.DropColumn(
                name: "ColMinValue",
                table: "Data_Column_Entries");

            migrationBuilder.DropColumn(
                name: "ColumnType",
                table: "Data_Column_Entries");

            migrationBuilder.DropColumn(
                name: "DataType",
                table: "Data_Column_Entries");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Data_Column_Entries");

            migrationBuilder.DropColumn(
                name: "RelatedDataId",
                table: "Data_Column_Entries");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CalcRule",
                table: "Data_Column_Entries",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "ColMaxValue",
                table: "Data_Column_Entries",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "ColMinValue",
                table: "Data_Column_Entries",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "ColumnType",
                table: "Data_Column_Entries",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DataType",
                table: "Data_Column_Entries",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Data_Column_Entries",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "RelatedDataId",
                table: "Data_Column_Entries",
                type: "int",
                nullable: true);
        }
    }
}
