using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace workapi.Migrations
{
    public partial class assetgps : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_assets_Gps_Pointid",
                table: "assets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Gps",
                table: "Gps");

            migrationBuilder.RenameTable(
                name: "Gps",
                newName: "gps");

            migrationBuilder.AddPrimaryKey(
                name: "PK_gps",
                table: "gps",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_assets_gps_Pointid",
                table: "assets",
                column: "Pointid",
                principalTable: "gps",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_assets_gps_Pointid",
                table: "assets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_gps",
                table: "gps");

            migrationBuilder.RenameTable(
                name: "gps",
                newName: "Gps");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Gps",
                table: "Gps",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_assets_Gps_Pointid",
                table: "assets",
                column: "Pointid",
                principalTable: "Gps",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
