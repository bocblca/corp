using Senparc.Weixin.Entities.TemplateMessage;
using Senparc.Weixin.MP.AdvancedAPIs.TemplateMessage;

namespace workauto.Template
{
    public class TempGoods : TemplateMessageBase
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



        public TempGoods(string _first, string GoodsName,
            string GoodsPrice, string custName, string custAdr, string custDate,
            string _remark,
            string templateId = "TLiY2idqlBtHpeQpuu1Pn8EZTCLvz1LsdjPAiTbGmKk",
            string url = "")
            : base(templateId, url, "预约成功")
        {

            first = new TemplateDataItem(_first);
            keyword1 = new TemplateDataItem(GoodsName);
            keyword2 = new TemplateDataItem(GoodsPrice);
            keyword3 = new TemplateDataItem(custName);//显示为红色
            keyword4 = new TemplateDataItem(custAdr);
            keyword5 = new TemplateDataItem(custDate);
            remark = new TemplateDataItem(_remark);
        }
    }

    public class TempDirect : TemplateMessageBase
    {
        public TemplateDataItem first { get; set; }
        public TemplateDataItem keyword1 { get; set; }
        public TemplateDataItem keyword2 { get; set; }
        public TemplateDataItem keyword3 { get; set; }
        public TemplateDataItem keyword4 { get; set; }
        public TemplateDataItem remark { get; set; }
        /// <summary>
        /// “预约”模板消息数据定义 构造函数



        public TempDirect(string _first, string custName,
            string custTel, string custAdr, string custDate,
            string _remark,
            string templateId = "l4Db2_twOCdoiEIG1tlk7JfTBe3ShacFcFAGrRdRtlg",
            string url = "")
            : base(templateId, url, "预约成功")
        {

            first = new TemplateDataItem(_first);
            keyword1 = new TemplateDataItem(custName);
            keyword2 = new TemplateDataItem(custTel);
            keyword3 = new TemplateDataItem(custAdr);//显示为红色
            keyword4 = new TemplateDataItem(custDate);
            remark = new TemplateDataItem(_remark);
        }
    }


    public class Temppost : TemplateMessageBase
    {
        public TemplateDataItem first { get; set; }
        public TemplateDataItem keyword1 { get; set; }
        public TemplateDataItem keyword2 { get; set; }
        public TemplateDataItem keyword3 { get; set; }
        public TemplateDataItem keyword4 { get; set; }
        public TemplateDataItem remark { get; set; }
        /// <summary>
        /// “预约”模板消息数据定义 构造函数



        public Temppost(string _first, string phonex,
            string postdate, string postnumber, string postname,
            string _remark,
            string templateId = "X9pIwBfMrbZ-cv6a7pV8YGlEIbAxqZ3ZL1QC-OhNKNU",
            string url = "")
            : base(templateId, url, "预约成功")
        {

            first = new TemplateDataItem(_first);
            keyword1 = new TemplateDataItem(phonex);
            keyword2 = new TemplateDataItem(postdate);
            keyword3 = new TemplateDataItem(postnumber);//显示为红色
            keyword4 = new TemplateDataItem(postname);
            remark = new TemplateDataItem(_remark);
        }
    }

    public class Tempcancel : TemplateMessageBase
    {
        public TemplateDataItem first { get; set; }
        public TemplateDataItem keyword1 { get; set; }
        public TemplateDataItem keyword2 { get; set; }
        public TemplateDataItem keyword3 { get; set; }

        public TemplateDataItem remark { get; set; }
        /// <summary>
        /// “预约”模板消息数据定义 构造函数

        public Tempcancel(string _first, string bank,
            string posttype, string posttime,
            string _remark,
            string templateId = "wyyO3CTHdKMfQnsbiE9lQnXHFZNmbiqu3f1xwBZnWkg",
            string url = "")
            : base(templateId, url, "预约取消")
        {
            /* 模板格式      {{first.DATA}}
                用户名：{{keyword1.DATA}}
                订单号：{{keyword2.DATA}}
                订单金额：{{keyword3.DATA}}
                商品信息：{{keyword4.DATA}}
         {{remark.DATA}} 
           */
            first = new TemplateDataItem(_first);
            keyword1 = new TemplateDataItem(bank);
            keyword2 = new TemplateDataItem(posttype);
            keyword3 = new TemplateDataItem(posttime);//显示为红色
            remark = new TemplateDataItem(_remark);
        }
    }

}
