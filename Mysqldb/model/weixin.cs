using Newtonsoft.Json;
using Npoi.Mapper.Attributes;
using System.ComponentModel;



using System.ComponentModel.DataAnnotations;

namespace Mysqldb
{

    public record postJsonBank(string branchName);
    public class Subankdata
    {
        public long Id { get; set; }
        [Column("序号")]
        public string? sub_id { get; set; }
        [Column("名称")]
        public string? SubName { get; set; }
        [Column("储蓄存款月日均增幅")]
        public double Dept { get; set; }
        [Column("存款偏离度")]
        public double PubDept { get; set; }
        [Column("个人客户贷款增量")]
        public double LowDept { get; set; }

        [Column("个贷新产品增量")]
        public double NewDept { get; set; }
        [Column("整村授信农户信息采集率")]
        public double Count01 { get; set; }
        [Column("整村授信农户新增贷款累放完成率")]
        public double Cunnt02 { get; set; }

        [Column("代理保险")]
        public double Risk { get; set; }
        [Column("代理贵金属")]
        public double Gold { get; set; } //以上普惠金融指标
        //电子金融
        [Column("个人手机银行")]
        public double Ebank0 { get; set; }
        [Column("收单商户活跃户数（存量+新增）")]
        public double Ebank1 { get; set; }

        [Column("短信银行")]
        public double Ebank2 { get; set; }
        [Column("扣分")]
        public double Ebank3 { get; set; }

        //公司业务
        [Column("对公贷款净增投放")]
        public double CR01 { get; set; }
        [Column("对公存款日均增量")]
        public double CR02 { get; set; }
        [Column("对公中收")]
        public double CR03 { get; set; }
        //信贷综合

        [Column("实收利息")]
        public double XD01 { get; set; }
        [Column("到期贷款回收率")]
        public double XD02 { get; set; }


        //风险
        [Column("表内不良贷款清收")]
        public double RiskK01 { get; set; }
        [Column("表外债权清收")]
        public double Risk02 { get; set; }

        //教育培训 train

        [Column("金信网络学院学习地图模块")]
        public double TRAIN01 { get; set; }


        [Column("金信网络学院完成学时学分")]
        public double TRAIN02 { get; set; }

        [Column("课程开发")]
        public double TRAIN03 { get; set; }

        //网点转型

        [Column("课程加分")]
        public double netadd { get; set; }
        [Column("课程扣分")]
        public double netred { get; set; }



        //内控合规
        [Column("负债管理专业")]
        public double Comp1 { get; set; }
        [Column("信贷管理专业")]
        public double Comp2 { get; set; }
        [Column("风险管理专业")]
        public double Comp3 { get; set; }
        [Column("运营管理专业")]
        public double Comp4 { get; set; }
        [Column("安全保卫专业")]
        public double Comp5 { get; set; }
        [Column("计财专业")]
        public double Comp6 { get; set; }
        [Column("电子金融专业")]
        public double Comp7 { get; set; }
        [Column("消保专业")]
        public double Comp8 { get; set; }
        [Column("信息科技专业")]
        public double Comp9 { get; set; }
        [Column("法律合规专业")]
        public double Comp10 { get; set; }
        [Column("舆情及声誉风险防控")]
        public double Comp11 { get; set; }
        [Column("其他专业")]
        public double Comp12 { get; set; }
        [Column("案件防控")]
        public double Comp13 { get; set; }
        [Column("监管处罚")]


        public double Comp14 { get; set; }
        [Column("党建（扣分）")]
        public double Comp15 { get; set; }
        [Column("党风廉政（扣分）")]
        public double Comp16 { get; set; }
        [Column("其他（阶段性整村授信）")]
        public double Comp17 { get; set; }
        [Column("得分")]
        public double nums { get; set; }

        public string mdate { get; set; } = "2021-01";

    }
    public class Bank
    {


        public string bankID { get; set; }
        public string bankName { get; set; }
        public string? parentID { get; set; }
        //public List<Straff> straff { get; set; }

        public Bank parent { get; set; }

        public List<Bank> Children { get; set; } = new List<Bank>();

        public bankcoord bankcoord { get; set; }

    }

    //快递数据库类结构
    public class Express
    {

        public string transID { get; set; }
        public string expressID { get; set; }
        public string phoneLast { get; set; }
        public Trans_TBL trans { get; set; }
    }




    public class Trans_TBL
    {


        public string busID { get; set; }
        public string transID { get; set; }

        public string prepay_ID { get; set; }
        public string openid { get; set; }
        public string straffID { get; set; }
        public string productID { get; set; }
        public DateTime transDate { get; set; }
        public string transType { get; set; }
        public decimal transMoney { get; set; }
        public int sendInfo { get; set; }
        public int transBack { get; set; }
        public string remark { get; set; }
        public Express express { get; set; }
        public gold_TBL goods { get; set; }

    }

    //goldID goldName    goldMoney goldSize    goldPro goldType    remark

    public class gold_TBL
    {


        public string goldID { get; set; }
        public string goldName { get; set; }

        public decimal goldMoney { get; set; }
        public string goldSize { get; set; }
        public string goldPro { get; set; }
        public string goldType { get; set; }
        public string remark { get; set; }

    }

    public class bankcoord
    {


        public string bankID { get; set; }
        public string bankName { get; set; }

        public double LAT { get; set; }
        public double LON { get; set; }
        public Bank bank { get; set; }

    }

    public class SqlOrder
    {
        [Key]
        public string orderID { get; set; }
        public string orderInfo { get; set; }
        public string orderTime { get; set; }

        public DateTime orderDate { get; set; }

        public DateTime straffDate { get; set; }
        public string orderName { get; set; }
        public string orderTel { get; set; }
        public int isCheck { get; set; }
        public string straffID { get; set; }

        public string straffTel { get; set; }
        public string straffName { get; set; }
        public string sub { get; set; }

        public string acess { get; set; }
        public string branch { get; set; }

    }

    public class sqlorderace
    {
        [Key]
        public string orderID { get; set; }
        public string openid { get; set; }
        public string orderTime { get; set; }
        public string orderInfo { get; set; }
        public DateTime orderDate { get; set; }
        public DateTime straffDate { get; set; }



        public string orderName { get; set; }
        public string orderTel { get; set; }
        public int isCheck { get; set; }
        public string straffID { get; set; }
        public string straffName { get; set; }
        public string sub { get; set; }
        public string remark { get; set; }

        public int acessID { get; set; }
        public int range { get; set; }
        public string branch { get; set; }


    }



    public class Order_TBL
    {

        public string orderID { get; set; }
        public string openid { get; set; } //客户微信ID
        public string orderTime { get; set; } //预约时间
        public string orderInfo { get; set; }
        public DateTime orderDate { get; set; } //业务发起时间
        public DateTime straffDate { get; set; } //业务受理时间
        public string orderName { get; set; }
        public string orderTel { get; set; }
        public int isCheck { get; set; }
        public string straffID { get; set; }

        public Straff straff { get; set; }

        public string sub { get; set; }
        public string remark { get; set; }

    }


    public class customers
    {
        public string openid { get; set; }
        public string unionId { get; set; }
        public string customerName { get; set; }
        public string customerTel1 { get; set; }
        public string customerTel2 { get; set; }
        public string customerTel3 { get; set; }
        public string customerTel4 { get; set; }
        public string customerTel5 { get; set; }
        public string customerAdr1 { get; set; }
        public string customerAdr2 { get; set; }
        public string customerAdr3 { get; set; }
        public string customerAdr4 { get; set; }
        public string customerAdr5 { get; set; }
        public string remark { get; set; }
        public string straffID { get; set; }
        public Straff straff { get; set; }


    }
    public class Credittoken
    {
        public int Id { get; set; }
        public int fileId { get; set; }
        public string? token { get; set; }
    }
    public class creditInfo
    {
        public string returnCode { get; set; }
        public string description { get; set; }

        public mresult result;

    }
    public class mresult
    {
        public string phoneNumber { get; set; }
        public string grade { get; set; }
        public string creditscore { get; set; }
    }
    public class creditphone
    {
        public string phoneNumber { get; set; }
        public string productId;
        public string orderNo;
        public creditphone()
        {
            productId = "credit001";
            orderNo = "ORD00000001";
        }
    }
    public class Mobilephone
    {
        public string phone { get; set; }
    }
    public class Mtoken
    {
        public string status { get; set; }
        public string objs { get; set; }
        public string message { get; set; }
        public string data { get; set; }

    }

    public class tokendata
    {
        public string userame { get; set; }
        public string password { get; set; }
        public string appno { get; set; }
        public string appkey { get; set; }
        public string orderid { get; set; }

    }
    public class Loan
    {

        public string xm { get; set; }


        public string phone { get; set; }

        public string post { get; set; }

        public string loanx { get; set; }
    }


    public class Straff
    {

        public string openid { get; set; } //微信ID

        public string straffID { get; set; } //员工柜员号
        public string straffName { get; set; } //员工姓名
        public string straffTel { get; set; } //电话
        public string straffSex { get; set; } //性别
        public string straffAge { get; set; } //年龄

        public string acess { get; set; } //职务

        public int? aceID { get; set; } //权限 0、1、2
        public Bank bank { get; set; }

        public List<Order_TBL> order { get; set; }
    }
    public class Userinfo
    {

        public string Userid { get; set; }

        [JsonProperty(PropertyName = "姓名")]


        public string Username { get; set; }

        public string Nick { get; set; }

        [DisplayName("部门")]
        public string Depart { get; set; }

        [DisplayName("预约银行")]
        public string Bank { get; set; }

        [JsonProperty(PropertyName = "预约时间")]

        [DisplayName("预约时间")]
        public string Post { get; set; }

        [DisplayName("关注时间")]

        public DateTime? Signtime { get; set; }

        [JsonProperty(PropertyName = "预约编号")]

        [DisplayName("预约编号")]
        public string Acceptmsg { get; set; }

        [JsonProperty(PropertyName = "手机号码")]
        [DisplayName("预约手机")]
        public string phone { get; set; }

        [DisplayName("总行")]
        public string headbank { get; set; }

        [DisplayName("所属银行")]
        public string workbank { get; set; }

        [DisplayName("职务")]
        public string posit { get; set; }


        [JsonProperty(PropertyName = "预约金额")]

        [DisplayName("金额")]
        public int? money { get; set; }


        [DisplayName("签到")]
        public int? signid { get; set; }


        [DisplayName("签到时间")]
        public DateTime? Signdate { get; set; }


        [DisplayName("经度")]

        public double? lon { get; set; }

        [DisplayName("维度")]
        public double? lat { get; set; }

        [DisplayName("行号")]
        public string bkno { get; set; }

    }
    public class Openidinfo
    {
        public string openid { get; set; }
        public string mopenid { get; set; }

        public string bankno { get; set; }
    }

  


    public class Postresult
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public MyRole Role { get; set; }
    }

    public enum MyRole
    {
        Admin, User, Guest
    }
}


