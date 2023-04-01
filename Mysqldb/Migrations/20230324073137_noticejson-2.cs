using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Mysqldb;

#nullable disable

namespace Mysqldb.Migrations
{
    /// <inheritdoc />
    public partial class noticejson2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<List<Approval>>(
                name: "Approvals",
                table: "Supernotices",
                type: "jsonb",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Approvals",
                table: "Supernotices");
        }
    }
}
