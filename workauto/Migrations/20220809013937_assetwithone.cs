using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace workapi.Migrations
{
    public partial class assetwithone : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_assets_Gps_Pointid",
                table: "assets");

            migrationBuilder.AlterColumn<int>(
                name: "Pointid",
                table: "assets",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_assets_Gps_Pointid",
                table: "assets",
                column: "Pointid",
                principalTable: "Gps",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_assets_Gps_Pointid",
                table: "assets");

            migrationBuilder.AlterColumn<int>(
                name: "Pointid",
                table: "assets",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_assets_Gps_Pointid",
                table: "assets",
                column: "Pointid",
                principalTable: "Gps",
                principalColumn: "id");
        }
    }
}
