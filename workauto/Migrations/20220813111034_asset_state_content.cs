using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace workapi.Migrations
{
    public partial class asset_state_content : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "operator_content",
                table: "asset_states",
                type: "varchar(255)",
                maxLength: 255,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "operator_content",
                table: "asset_states");
        }
    }
}
