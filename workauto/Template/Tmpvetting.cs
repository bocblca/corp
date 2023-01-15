using Senparc.Weixin.Entities.TemplateMessage;
using Senparc.Weixin.MP.AdvancedAPIs.TemplateMessage;

namespace Mysqldb
{
    public class TempVetting : TemplateMessageBase
    {
        public TemplateDataItem first { get; set; }
        public TemplateDataItem keyword1 { get; set; }
        public TemplateDataItem keyword2 { get; set; }
        public TemplateDataItem keyword3 { get; set; }
        public TemplateDataItem remark { get; set; }


        public TempVetting(string _first, string procid,
            string mdate, string flags,
            string _remark,
            string templateId = "ALgfq7U1yRg4B0y9naD17sSVMCjlJzrX7-24GsNTR3M",
            string url = null)
            : base(templateId, url, "申请审批")
        {

            first = new TemplateDataItem(_first);
            keyword1 = new TemplateDataItem(procid);
            keyword2 = new TemplateDataItem(mdate);
            keyword3 = new TemplateDataItem(flags, "#ff0000");//显示为红色
            remark = new TemplateDataItem(_remark);

        }
    }
    public class TempVetted : TemplateMessageBase
    {
        public TemplateDataItem first { get; set; }
        public TemplateDataItem keyword1 { get; set; }
        public TemplateDataItem keyword2 { get; set; }
        public TemplateDataItem keyword3 { get; set; }
        public TemplateDataItem remark { get; set; }


        public TempVetted(string _first, string custname,
            string product, string money,
            string _remark,
            string templateId = "btyjvUc_DN_oOt8qDJjEpE3wh0EPejPcwej7AYmFw7I",
            string url = null)
            : base(templateId, url, "审批通过")
        {

            first = new TemplateDataItem(_first);
            keyword1 = new TemplateDataItem(custname);
            keyword2 = new TemplateDataItem(product);
            keyword3 = new TemplateDataItem(money, "#ff0000");//显示为红色
            remark = new TemplateDataItem(_remark);

        }
    }
}