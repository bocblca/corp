using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mysqldb.Migrations
{
    /// <inheritdoc />
    public partial class Spdatas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "spdatas",
                columns: table => new
                {
                    Sp_no = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Sp_type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Apply_time = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Apply_userid = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Username = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Apply_departid = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Departname = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Sp_status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Start = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    End = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Duration = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_spdatas", x => x.Sp_no);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "spdatas");
        }
    }
}
