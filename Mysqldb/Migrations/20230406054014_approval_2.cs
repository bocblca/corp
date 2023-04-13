using Microsoft.EntityFrameworkCore.Migrations;
using Senparc.Weixin.Work.Entities;

#nullable disable

namespace Mysqldb.Migrations
{
    /// <inheritdoc />
    public partial class approval_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Supernotices",
                columns: table => new
                {
                    Noticeid = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Superinfo = table.Column<RequestMessageEvent_OpenApprovalChange>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Supernotices", x => x.Noticeid);
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
