using Senparc.Weixin.Entities.TemplateMessage;
using Senparc.Weixin.MP.AdvancedAPIs.TemplateMessage;

namespace workapi.Kpi
{
    public record Kpiinfo(string Subname, string Mdate);
    public record Sendmsg(string mdate);
    public record Rmsg(string Msg, string Code, Resdata Data);
    public record Resdata(double Dept, double Ebanknum, double Crnum, double Xdnum, double Risknum, double Kpinums, string Kdate);

    public class TempBkmsg : TemplateMessageBase
    {
        public TemplateDataItem first { get; set; }
        public TemplateDataItem keyword1 { get; set; }
        public TemplateDataItem keyword2 { get; set; }
        public TemplateDataItem keyword3 { get; set; }
        public TemplateDataItem keyword4 { get; set; }
        public TemplateDataItem keyword5 { get; set; }
        public TemplateDataItem remark { get; set; }
        /// <summary>
        /// “预约”模板消息数据定义 构造函数



        public TempBkmsg(string _first, string SysName,
            string ServName, string TodoTitle, string CustName, string Steps,
            string _remark,
            string templateId = "TYROSV2t-8ESwJ5jA4LzmVA9Wm6TGbH3jJIN3OkhZ68",
            string url = null)
            : base(templateId, url, "预约成功")
        {

            first = new TemplateDataItem(_first);
            keyword1 = new TemplateDataItem(SysName);
            keyword2 = new TemplateDataItem(ServName);
            keyword3 = new TemplateDataItem(TodoTitle, "#ff0000");//显示为红色
            keyword4 = new TemplateDataItem(CustName);
            keyword5 = new TemplateDataItem(Steps);
            remark = new TemplateDataItem(_remark);

        }
    }

    public class TempKpi : TemplateMessageBase
    {
        public TemplateDataItem first { get; set; }
        public TemplateDataItem keyword1 { get; set; }
        public TemplateDataItem keyword2 { get; set; }
        public TemplateDataItem keyword3 { get; set; }
        public TemplateDataItem keyword4 { get; set; }
        public TemplateDataItem remark { get; set; }
        /// <summary>
        /// “预约”模板消息数据定义 构造函数

        public TempKpi(string _first, string Kpidate, string Kpisef, string Kpitype, string Kpicontent,
            string _remark,
            string templateId = "MhiEX0hd2t1LuSPhiCaXmnI8g03k767Ee5ATLDkbVao",
            string url = "https://rcbcybank.com")

            : base(templateId, url, "绩效考核通知")
        {

            first = new TemplateDataItem(_first);
            keyword1 = new TemplateDataItem(Kpidate);
            keyword2 = new TemplateDataItem(Kpisef);
            keyword3 = new TemplateDataItem(Kpitype);
            keyword4 = new TemplateDataItem(Kpicontent, "#ff0000");//显示为红色
            remark = new TemplateDataItem(_remark);

        }
    }



}
