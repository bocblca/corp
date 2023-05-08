using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Mysqldb.model;

#nullable disable

namespace Mysqldb.Migrations
{
    /// <inheritdoc />
    public partial class firsts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Firsts",
                columns: table => new
                {
                    Transid = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Transtime = table.Column<long>(type: "bigint", nullable: false),
                    Cust = table.Column<Cust>(type: "jsonb", nullable: true),
                    Firstuser = table.Column<Employeeinfo>(type: "jsonb", nullable: true),
                    Relay = table.Column<List<Employeeinfo>>(type: "jsonb", nullable: true),
                    Trans_Status = table.Column<Trans_status>(type: "jsonb", nullable: true),
                    Attachs = table.Column<List<Transfile>>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Firsts", x => x.Transid);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Firsts");
        }
    }
}
