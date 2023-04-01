using Microsoft.EntityFrameworkCore.Migrations;
using Mysqldb;

#nullable disable

namespace Mysqldb.Migrations
{
    /// <inheritdoc />
    public partial class noticejson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Supernotices",
                columns: table => new
                {
                    NoticeId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Noticedata = table.Column<Noticedata>(type: "jsonb", nullable: true),
                    Orderdata = table.Column<Orderdata>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Supernotices", x => x.NoticeId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Supernotices");
        }
    }
}
