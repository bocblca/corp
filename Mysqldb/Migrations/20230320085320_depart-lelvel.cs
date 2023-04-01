using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mysqldb.Migrations
{
    /// <inheritdoc />
    public partial class departlelvel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "level",
                table: "wrokdeparts",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "level",
                table: "wrokdeparts");
        }
    }
}
