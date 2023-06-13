using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Mysqldb.model;

#nullable disable

namespace Mysqldb.Migrations
{
    /// <inheritdoc />
    public partial class limit1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "limits",
                columns: table => new
                {
                    Limitid = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Userid = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Username = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Transtime = table.Column<long>(type: "bigint", nullable: false),
                    Leaderid = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Leadername = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Approval = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Approval_content = table.Column<string>(type: "text", nullable: true),
                    Relay_userid = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Relay_content = table.Column<string>(type: "text", nullable: true),
                    Relay_time = table.Column<long>(type: "bigint", nullable: false),
                    Relay_departid = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Relay_departname = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Relay_approval = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Relay_approval_content = table.Column<string>(type: "text", nullable: true),
                    Relay_leaderid = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Relay_approval_time = table.Column<long>(type: "bigint", nullable: false),
                    Departname = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Departid = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Conttype = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Detail = table.Column<string>(type: "text", nullable: true),
                    Day = table.Column<int>(type: "integer", nullable: false),
                    Overday = table.Column<int>(type: "integer", nullable: false),
                    Info = table.Column<string>(type: "text", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    Isover = table.Column<bool>(type: "boolean", nullable: false),
                    Attachs = table.Column<List<Transfile>>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_limits", x => x.Limitid);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "limits");
        }
    }
}
