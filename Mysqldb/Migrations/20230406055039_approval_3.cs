using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Mysqldb;

#nullable disable

namespace Mysqldb.Migrations
{
    /// <inheritdoc />
    public partial class approval_3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Noticeid",
                table: "Supernotices",
                newName: "NoticeId");

            migrationBuilder.RenameColumn(
                name: "Superinfo",
                table: "Supernotices",
                newName: "Orderdata");

            migrationBuilder.AddColumn<List<Approval>>(
                name: "Approvals",
                table: "Supernotices",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<Noticedata>(
                name: "Noticedata",
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

            migrationBuilder.DropColumn(
                name: "Noticedata",
                table: "Supernotices");

            migrationBuilder.RenameColumn(
                name: "NoticeId",
                table: "Supernotices",
                newName: "Noticeid");

            migrationBuilder.RenameColumn(
                name: "Orderdata",
                table: "Supernotices",
                newName: "Superinfo");
        }
    }
}
