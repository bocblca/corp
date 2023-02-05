using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace workapi.Migrations
{
    public partial class subankdata : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Subankdatas",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    sub_id = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SubName = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Dept = table.Column<double>(type: "double", nullable: false),
                    PubDept = table.Column<double>(type: "double", nullable: false),
                    LowDept = table.Column<double>(type: "double", nullable: false),
                    NewDept = table.Column<double>(type: "double", nullable: false),
                    Count01 = table.Column<double>(type: "double", nullable: false),
                    Cunnt02 = table.Column<double>(type: "double", nullable: false),
                    Risk = table.Column<double>(type: "double", nullable: false),
                    Gold = table.Column<double>(type: "double", nullable: false),
                    Ebank0 = table.Column<double>(type: "double", nullable: false),
                    Ebank1 = table.Column<double>(type: "double", nullable: false),
                    Ebank2 = table.Column<double>(type: "double", nullable: false),
                    Ebank3 = table.Column<double>(type: "double", nullable: false),
                    CR01 = table.Column<double>(type: "double", nullable: false),
                    CR02 = table.Column<double>(type: "double", nullable: false),
                    CR03 = table.Column<double>(type: "double", nullable: false),
                    XD01 = table.Column<double>(type: "double", nullable: false),
                    XD02 = table.Column<double>(type: "double", nullable: false),
                    RiskK01 = table.Column<double>(type: "double", nullable: false),
                    Risk02 = table.Column<double>(type: "double", nullable: false),
                    TRAIN01 = table.Column<double>(type: "double", nullable: false),
                    TRAIN02 = table.Column<double>(type: "double", nullable: false),
                    TRAIN03 = table.Column<double>(type: "double", nullable: false),
                    netadd = table.Column<double>(type: "double", nullable: false),
                    netred = table.Column<double>(type: "double", nullable: false),
                    Comp1 = table.Column<double>(type: "double", nullable: false),
                    Comp2 = table.Column<double>(type: "double", nullable: false),
                    Comp3 = table.Column<double>(type: "double", nullable: false),
                    Comp4 = table.Column<double>(type: "double", nullable: false),
                    Comp5 = table.Column<double>(type: "double", nullable: false),
                    Comp6 = table.Column<double>(type: "double", nullable: false),
                    Comp7 = table.Column<double>(type: "double", nullable: false),
                    Comp8 = table.Column<double>(type: "double", nullable: false),
                    Comp9 = table.Column<double>(type: "double", nullable: false),
                    Comp10 = table.Column<double>(type: "double", nullable: false),
                    Comp11 = table.Column<double>(type: "double", nullable: false),
                    Comp12 = table.Column<double>(type: "double", nullable: false),
                    Comp13 = table.Column<double>(type: "double", nullable: false),
                    Comp14 = table.Column<double>(type: "double", nullable: false),
                    Comp15 = table.Column<double>(type: "double", nullable: false),
                    Comp16 = table.Column<double>(type: "double", nullable: false),
                    Comp17 = table.Column<double>(type: "double", nullable: false),
                    nums = table.Column<double>(type: "double", nullable: false),
                    mdate = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subankdatas", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Subankdatas");
        }
    }
}
