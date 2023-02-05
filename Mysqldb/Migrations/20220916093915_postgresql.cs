using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Mysqldb.Migrations
{
    public partial class postgresql : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "accounts",
                columns: table => new
                {
                    Nums = table.Column<string>(type: "text", nullable: false),
                    Address = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Legal = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Range = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Regcapital = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Validdate = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Pname = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Pnums = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Paddress = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Birth = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Dept = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Nation = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Faildate = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Issudate = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Sex = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Phone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_accounts", x => x.Nums);
                });

            migrationBuilder.CreateTable(
                name: "aitokens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AiToken = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Expires = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_aitokens", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    WxUnionid = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Wxopenid = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Appopenid = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Cname = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "asset_states",
                columns: table => new
                {
                    spanid = table.Column<long>(type: "bigint", maxLength: 50, nullable: false),
                    qrcode = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    origin_id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    operator_id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    operator_content = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    target_id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    state = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_asset_states", x => x.spanid);
                });

            migrationBuilder.CreateTable(
                name: "banks",
                columns: table => new
                {
                    bankID = table.Column<string>(type: "text", nullable: false),
                    bankName = table.Column<string>(type: "text", nullable: true),
                    parentID = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_banks", x => x.bankID);
                    table.ForeignKey(
                        name: "FK_banks_banks_parentID",
                        column: x => x.parentID,
                        principalTable: "banks",
                        principalColumn: "bankID");
                });

            migrationBuilder.CreateTable(
                name: "bookings",
                columns: table => new
                {
                    Unionid = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Wxopenid = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CustomerName = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    FirstDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Seconddatae = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Thirddatae = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Forthdatae = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Stage = table.Column<int>(type: "integer", nullable: true),
                    phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    IsProccess = table.Column<bool>(type: "boolean", nullable: false),
                    Straff = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Information = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Nums = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bookings", x => x.Unionid);
                });

            migrationBuilder.CreateTable(
                name: "cloundprinters",
                columns: table => new
                {
                    Code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    straff = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Fprname = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Sprname = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Lon = table.Column<double>(type: "double precision", nullable: false),
                    Lat = table.Column<double>(type: "double precision", nullable: false),
                    Gpsflag = table.Column<bool>(type: "boolean", nullable: false),
                    PrTop = table.Column<int>(type: "integer", nullable: false),
                    PrLeft = table.Column<int>(type: "integer", nullable: false),
                    Regcode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Bankcode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    BankName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cloundprinters", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "contracts",
                columns: table => new
                {
                    number = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    money = table.Column<double>(type: "double precision", nullable: false),
                    num = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    product = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contracts", x => x.number);
                });

            migrationBuilder.CreateTable(
                name: "credits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    fileId = table.Column<int>(type: "integer", nullable: false),
                    token = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_credits", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "employees",
                columns: table => new
                {
                    EmployeeID = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Depart = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    DepartID = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    acces = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_employees", x => x.EmployeeID);
                });

            migrationBuilder.CreateTable(
                name: "gold_TBLs",
                columns: table => new
                {
                    goldID = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    goldName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    goldMoney = table.Column<decimal>(type: "numeric", nullable: false),
                    goldSize = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    goldPro = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    goldType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    remark = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gold_TBLs", x => x.goldID);
                });

            migrationBuilder.CreateTable(
                name: "gps",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Lat = table.Column<double>(type: "double precision", nullable: false),
                    Lon = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gps", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "hangjobs",
                columns: table => new
                {
                    jobid = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    procid = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    date_start = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    date_end = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    date_warn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Ace = table.Column<int>(type: "integer", nullable: false),
                    iswarn = table.Column<bool>(type: "boolean", nullable: false),
                    isdel = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_hangjobs", x => x.jobid);
                });

            migrationBuilder.CreateTable(
                name: "loanProcesses",
                columns: table => new
                {
                    ProcID = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    flag = table.Column<int>(type: "integer", nullable: true),
                    straff1 = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    straff2 = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    straff3 = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    straff4 = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    straff5 = table.Column<string>(type: "text", nullable: true),
                    date11 = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    date12 = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    date21 = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    date22 = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    date31 = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    date32 = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    date41 = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    date42 = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    date51 = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    date52 = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    bankid = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    bankname = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    loantype = table.Column<int>(type: "integer", nullable: true),
                    message = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    parentid = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_loanProcesses", x => x.ProcID);
                });

            migrationBuilder.CreateTable(
                name: "loans",
                columns: table => new
                {
                    LoanID = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Money = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Vdate = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Bankname = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_loans", x => x.LoanID);
                });

            migrationBuilder.CreateTable(
                name: "loanusers",
                columns: table => new
                {
                    straff = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    straff_name = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    unionid = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    bankid = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    IsLogin = table.Column<bool>(type: "boolean", nullable: true),
                    Ace = table.Column<int>(type: "integer", nullable: false),
                    IsValid = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_loanusers", x => x.straff);
                });

            migrationBuilder.CreateTable(
                name: "members",
                columns: table => new
                {
                    userid = table.Column<string>(type: "text", nullable: false),
                    department = table.Column<string>(type: "text", nullable: true),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_members", x => x.userid);
                });

            migrationBuilder.CreateTable(
                name: "printers",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_printers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "rcbstraffs",
                columns: table => new
                {
                    Straff = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    BankId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Name = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Ace = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Unionid = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rcbstraffs", x => x.Straff);
                });

            migrationBuilder.CreateTable(
                name: "Subankdatas",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    sub_id = table.Column<string>(type: "text", nullable: true),
                    SubName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Dept = table.Column<double>(type: "double precision", nullable: false),
                    PubDept = table.Column<double>(type: "double precision", nullable: false),
                    LowDept = table.Column<double>(type: "double precision", nullable: false),
                    NewDept = table.Column<double>(type: "double precision", nullable: false),
                    Count01 = table.Column<double>(type: "double precision", nullable: false),
                    Cunnt02 = table.Column<double>(type: "double precision", nullable: false),
                    Risk = table.Column<double>(type: "double precision", nullable: false),
                    Gold = table.Column<double>(type: "double precision", nullable: false),
                    Ebank0 = table.Column<double>(type: "double precision", nullable: false),
                    Ebank1 = table.Column<double>(type: "double precision", nullable: false),
                    Ebank2 = table.Column<double>(type: "double precision", nullable: false),
                    Ebank3 = table.Column<double>(type: "double precision", nullable: false),
                    CR01 = table.Column<double>(type: "double precision", nullable: false),
                    CR02 = table.Column<double>(type: "double precision", nullable: false),
                    CR03 = table.Column<double>(type: "double precision", nullable: false),
                    XD01 = table.Column<double>(type: "double precision", nullable: false),
                    XD02 = table.Column<double>(type: "double precision", nullable: false),
                    RiskK01 = table.Column<double>(type: "double precision", nullable: false),
                    Risk02 = table.Column<double>(type: "double precision", nullable: false),
                    TRAIN01 = table.Column<double>(type: "double precision", nullable: false),
                    TRAIN02 = table.Column<double>(type: "double precision", nullable: false),
                    TRAIN03 = table.Column<double>(type: "double precision", nullable: false),
                    netadd = table.Column<double>(type: "double precision", nullable: false),
                    netred = table.Column<double>(type: "double precision", nullable: false),
                    Comp1 = table.Column<double>(type: "double precision", nullable: false),
                    Comp2 = table.Column<double>(type: "double precision", nullable: false),
                    Comp3 = table.Column<double>(type: "double precision", nullable: false),
                    Comp4 = table.Column<double>(type: "double precision", nullable: false),
                    Comp5 = table.Column<double>(type: "double precision", nullable: false),
                    Comp6 = table.Column<double>(type: "double precision", nullable: false),
                    Comp7 = table.Column<double>(type: "double precision", nullable: false),
                    Comp8 = table.Column<double>(type: "double precision", nullable: false),
                    Comp9 = table.Column<double>(type: "double precision", nullable: false),
                    Comp10 = table.Column<double>(type: "double precision", nullable: false),
                    Comp11 = table.Column<double>(type: "double precision", nullable: false),
                    Comp12 = table.Column<double>(type: "double precision", nullable: false),
                    Comp13 = table.Column<double>(type: "double precision", nullable: false),
                    Comp14 = table.Column<double>(type: "double precision", nullable: false),
                    Comp15 = table.Column<double>(type: "double precision", nullable: false),
                    Comp16 = table.Column<double>(type: "double precision", nullable: false),
                    Comp17 = table.Column<double>(type: "double precision", nullable: false),
                    nums = table.Column<double>(type: "double precision", nullable: false),
                    mdate = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subankdatas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "unionids",
                columns: table => new
                {
                    WxUnionid = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Wxopenid = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Appopenid = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Gappid = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Xappid = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Subscribe = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_unionids", x => x.WxUnionid);
                });

            migrationBuilder.CreateTable(
                name: "wrokdeparts",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    parentid = table.Column<long>(type: "bigint", nullable: false),
                    order = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_wrokdeparts", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "wxsessions",
                columns: table => new
                {
                    code = table.Column<string>(type: "text", nullable: false),
                    sessionkey = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    unionid = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_wxsessions", x => x.code);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<long>(type: "bigint", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    RoleId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "bankcoords",
                columns: table => new
                {
                    bankID = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    bankName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    LAT = table.Column<double>(type: "double precision", nullable: false),
                    LON = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bankcoords", x => x.bankID);
                    table.ForeignKey(
                        name: "FK_bankcoords_banks_bankID",
                        column: x => x.bankID,
                        principalTable: "banks",
                        principalColumn: "bankID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Straffs",
                columns: table => new
                {
                    straffID = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    openid = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    straffName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    straffTel = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    straffSex = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    straffAge = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    acess = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    aceID = table.Column<int>(type: "integer", nullable: true),
                    bankID = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Straffs", x => x.straffID);
                    table.ForeignKey(
                        name: "FK_Straffs_banks_bankID",
                        column: x => x.bankID,
                        principalTable: "banks",
                        principalColumn: "bankID");
                });

            migrationBuilder.CreateTable(
                name: "Trans_TBLs",
                columns: table => new
                {
                    busID = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    transID = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    prepay_ID = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    openid = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    straffID = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    productID = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    transDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    transType = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    transMoney = table.Column<decimal>(type: "numeric", nullable: false),
                    sendInfo = table.Column<int>(type: "integer", nullable: false),
                    transBack = table.Column<int>(type: "integer", nullable: false),
                    remark = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trans_TBLs", x => x.busID);
                    table.ForeignKey(
                        name: "FK_Trans_TBLs_gold_TBLs_productID",
                        column: x => x.productID,
                        principalTable: "gold_TBLs",
                        principalColumn: "goldID");
                });

            migrationBuilder.CreateTable(
                name: "assets",
                columns: table => new
                {
                    Qrcode = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Pname = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    kind = table.Column<int>(type: "integer", nullable: false),
                    Userid = table.Column<string>(type: "text", nullable: true),
                    Pointid = table.Column<int>(type: "integer", nullable: false),
                    Img = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Money = table.Column<double>(type: "double precision", nullable: true),
                    State = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_assets", x => x.Qrcode);
                    table.ForeignKey(
                        name: "FK_assets_gps_Pointid",
                        column: x => x.Pointid,
                        principalTable: "gps",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "customers",
                columns: table => new
                {
                    openid = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    unionId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    customerName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    customerTel1 = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    customerTel2 = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    customerTel3 = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    customerTel4 = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    customerTel5 = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    customerAdr1 = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    customerAdr2 = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    customerAdr3 = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    customerAdr4 = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    customerAdr5 = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    remark = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    straffID = table.Column<string>(type: "character varying(10)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_customers", x => x.openid);
                    table.ForeignKey(
                        name: "FK_customers_Straffs_straffID",
                        column: x => x.straffID,
                        principalTable: "Straffs",
                        principalColumn: "straffID");
                });

            migrationBuilder.CreateTable(
                name: "Order_TBLs",
                columns: table => new
                {
                    orderID = table.Column<string>(type: "text", nullable: false),
                    openid = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    orderTime = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    orderInfo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    orderDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    straffDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    orderName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    orderTel = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    isCheck = table.Column<int>(type: "integer", nullable: false),
                    straffID = table.Column<string>(type: "character varying(10)", nullable: true),
                    sub = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    remark = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Order_TBLs", x => x.orderID);
                    table.ForeignKey(
                        name: "FK_Order_TBLs_Straffs_straffID",
                        column: x => x.straffID,
                        principalTable: "Straffs",
                        principalColumn: "straffID");
                });

            migrationBuilder.CreateTable(
                name: "express",
                columns: table => new
                {
                    expressID = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    transID = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    phoneLast = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_express", x => x.expressID);
                    table.ForeignKey(
                        name: "FK_express_Trans_TBLs_transID",
                        column: x => x.transID,
                        principalTable: "Trans_TBLs",
                        principalColumn: "busID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_assets_Pointid",
                table: "assets",
                column: "Pointid");

            migrationBuilder.CreateIndex(
                name: "IX_banks_parentID",
                table: "banks",
                column: "parentID");

            migrationBuilder.CreateIndex(
                name: "IX_customers_straffID",
                table: "customers",
                column: "straffID");

            migrationBuilder.CreateIndex(
                name: "IX_express_transID",
                table: "express",
                column: "transID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Order_TBLs_straffID",
                table: "Order_TBLs",
                column: "straffID");

            migrationBuilder.CreateIndex(
                name: "IX_Straffs_bankID",
                table: "Straffs",
                column: "bankID");

            migrationBuilder.CreateIndex(
                name: "IX_Trans_TBLs_productID",
                table: "Trans_TBLs",
                column: "productID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "accounts");

            migrationBuilder.DropTable(
                name: "aitokens");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "asset_states");

            migrationBuilder.DropTable(
                name: "assets");

            migrationBuilder.DropTable(
                name: "bankcoords");

            migrationBuilder.DropTable(
                name: "bookings");

            migrationBuilder.DropTable(
                name: "cloundprinters");

            migrationBuilder.DropTable(
                name: "contracts");

            migrationBuilder.DropTable(
                name: "credits");

            migrationBuilder.DropTable(
                name: "customers");

            migrationBuilder.DropTable(
                name: "employees");

            migrationBuilder.DropTable(
                name: "express");

            migrationBuilder.DropTable(
                name: "hangjobs");

            migrationBuilder.DropTable(
                name: "loanProcesses");

            migrationBuilder.DropTable(
                name: "loans");

            migrationBuilder.DropTable(
                name: "loanusers");

            migrationBuilder.DropTable(
                name: "members");

            migrationBuilder.DropTable(
                name: "Order_TBLs");

            migrationBuilder.DropTable(
                name: "printers");

            migrationBuilder.DropTable(
                name: "rcbstraffs");

            migrationBuilder.DropTable(
                name: "Subankdatas");

            migrationBuilder.DropTable(
                name: "unionids");

            migrationBuilder.DropTable(
                name: "wrokdeparts");

            migrationBuilder.DropTable(
                name: "wxsessions");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "gps");

            migrationBuilder.DropTable(
                name: "Trans_TBLs");

            migrationBuilder.DropTable(
                name: "Straffs");

            migrationBuilder.DropTable(
                name: "gold_TBLs");

            migrationBuilder.DropTable(
                name: "banks");
        }
    }
}
