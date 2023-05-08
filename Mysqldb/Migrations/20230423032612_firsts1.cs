using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mysqldb.Migrations
{
    /// <inheritdoc />
    public partial class firsts1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Copypersion",
                table: "Firsts",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Copypersion",
                table: "Firsts");
        }
    }
}
