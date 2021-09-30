using Microsoft.EntityFrameworkCore.Migrations;

namespace Ext_Dynamics_API.Migrations
{
    public partial class nullablecolsdatacolentry : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn("RowVersion", "Data_Column_Entries");
            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Data_Column_Entries",
                type: "rowversion",
                rowVersion: true,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CalcRule",
                table: "Data_Column_Entries",
                type: "nvarchar(max)",
                nullable: true,
                oldNullable: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn("RowVersion", "Data_Column_Entries");
            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Token_Entries",
                type: "rowversion",
                rowVersion: true,
                nullable: false);

            migrationBuilder.AlterColumn<string>(
            name: "CalcRule",
            table: "Data_Column_Entries",
            type: "nvarchar(max)",
            nullable: false,
            oldNullable: true);
        }
    }
}
