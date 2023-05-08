using Microsoft.EntityFrameworkCore.Migrations;
using Mysqldb.model;

#nullable disable

namespace Mysqldb.Migrations
{
    /// <inheritdoc />
    public partial class first2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Firstuser",
                table: "Firsts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Employeeinfo>(
                name: "Firstuser",
                table: "Firsts",
                type: "jsonb",
                nullable: true);
        }
    }
}
