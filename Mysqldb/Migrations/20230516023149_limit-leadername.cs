using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mysqldb.Migrations
{
    /// <inheritdoc />
    public partial class limitleadername : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Relay_leadername",
                table: "limits",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Relay_leadername",
                table: "limits");
        }
    }
}
