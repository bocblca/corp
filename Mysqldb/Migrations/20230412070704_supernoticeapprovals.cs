using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Mysqldb;

#nullable disable

namespace Mysqldb.Migrations
{
    /// <inheritdoc />
    public partial class supernoticeapprovals : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Approverstep",
                table: "Supernoticeapprovals");

            migrationBuilder.DropColumn(
                name: "Userid",
                table: "Supernoticeapprovals");

            migrationBuilder.AddColumn<List<Approval_userid>>(
                name: "Users",
                table: "Supernoticeapprovals",
                type: "jsonb",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Users",
                table: "Supernoticeapprovals");

            migrationBuilder.AddColumn<int>(
                name: "Approverstep",
                table: "Supernoticeapprovals",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Userid",
                table: "Supernoticeapprovals",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }
    }
}
