
using Mysqldb;
using Senparc.Weixin.Entities;
using Senparc.Weixin.Work.Containers;
using Senparc.Weixin.Work.Entities;
using Senparc.Weixin.Work.MessageHandlers;
namespace workapi.Models
{
    public class SuperNoticeMessageHandler : WorkMessageHandler<WorkCustomMessageContext>
    {
        /// <summary>
        /// 为中间件提供生成当前类的委托
        /// </summary>
        public static Func<Stream, PostModel, int, IServiceProvider, SuperNoticeMessageHandler> GenerateMessageHandler =
            (stream, postModel, maxRecordCount, serviceProvider) => new SuperNoticeMessageHandler(stream, postModel, maxRecordCount, serviceProvider);

        private readonly ISenparcWeixinSettingForWork _workSetting;
        private readonly Wxusers _mdata;

        public SuperNoticeMessageHandler(Stream inputStream, PostModel postModel, int maxRecordCount = 0, IServiceProvider serviceProvider = null)
            : base(inputStream, postModel, maxRecordCount, serviceProvider: serviceProvider)
        {
            _workSetting = Senparc.Weixin.Config.SenparcWeixinSetting.Items["supernotice"];
            _mdata = serviceProvider!.GetRequiredService<Wxusers>();
            

        }

        public override async Task<IWorkResponseMessageBase> OnTextRequestAsync(RequestMessageText requestMessage)
        {

            var responseMessage = this.CreateResponseMessage<ResponseMessageText>();


            //发送一条客服消息
            //var weixinSetting = Config.SenparcWeixinSetting.WorkSetting;
            //var appKey = AccessTokenContainer.BuildingKey(weixinSetting.WeixinCorpId, weixinSetting.WeixinCorpSecret);
            //MassApi.SendText(appKey, weixinSetting.WeixinCorpAgentId, "这是一条客服消息，对应您发送的消息：" + requestMessage.Content, OpenId);

           
           

            return responseMessage;
        }

    

        //审批状态回调
        public override async Task<IWorkResponseMessageBase> OnEvent_Open_Approval_Change_Status_ChangeRequestAsync(RequestMessageEvent_OpenApprovalChange requestMessage)
        {
            
           
            var res=_mdata.Supernotices.Where(e=>e.NoticeId==requestMessage.ApprovalInfo.ThirdNo).FirstOrDefault();
            Console.WriteLine(requestMessage.ToJsonString());
            if (res == null) {
               
                res.Approvals[0].Approval_Userid = requestMessage.ApprovalInfo.ApplyUserId;
                //res.Approvals[0].Approval_memo = requestMessage.ApprovalInfo.ApprovalNodes[0].Items[0].ItemSpeech;
                res.Approvals[0].Approval_status = requestMessage.ApprovalInfo.ApproverStep.ToString();
                await _mdata.SaveChangesAsync();
            }
           
            return base.OnEvent_Open_Approval_Change_Status_ChangeRequest(requestMessage);
        }
        public override IWorkResponseMessageBase OnEvent_ScancodeWaitmsgRequest(RequestMessageEvent_Scancode_Waitmsg requestMessage)
        {
            var responseMessage = this.CreateResponseMessage<ResponseMessageText>();
            var appKeyx = AccessTokenContainer.BuildingKey(_workSetting);
            var resa = Senparc.Weixin.Work.AdvancedAPIs.MassApi.SendTextCard(appKeyx, requestMessage.AgentID.ToString(), "扫描结果", "<div class=\"normal\">物品二维(条)码</div><div class=\"highlight\">" + requestMessage.ScanCodeInfo.ScanResult + "</div>", "https://rcbcybank.com/#/?id=" + requestMessage.ScanCodeInfo.ScanResult + "&user=" + requestMessage.FromUserName, "物品登记", requestMessage.FromUserName);

            //responseMessage.Content = "扫描end";

            return responseMessage;
            //return base.OnEvent_ScancodeWaitmsgRequest(requestMessage);
        }
        //public override IWorkResponseMessageBase OnEvent_LocationRequest(RequestMessageEvent_Location requestMessage)
        //{
        //   // var responseMessage = this.CreateResponseMessage<ResponseMessageText>();
        //    //responseMessage.Content = string.Format("位置坐标 {0} - {1}", requestMessage.Latitude, requestMessage.Longitude);
        //    return null;
        //}

        public override IWorkResponseMessageBase DefaultResponseMessage(IWorkRequestMessageBase requestMessage)
        {

            return new WorkSuccessResponseMessage();
        }

        //public override IWorkResponseMessageBase OnEvent_EnterAgentRequest(RequestMessageEvent_Enter_Agent requestMessage)
        //{
        //    var responseMessage = this.CreateResponseMessage<ResponseMessageText>();

        //    responseMessage.Content = "欢迎进入应用！现在时间是：" + SystemTime.Now.DateTime.ToString();
        //    return responseMessage;
        //}



        //public override IWorkResponseMessageBase DefaultResponseMessage(IWorkRequestMessageBase requestMessage)
        //{

        //   // var responseMessage = this.CreateResponseMessage<ResponseMessageText>();
        //   // responseMessage.Content = "这是一条没有找到合适回复信息的默认消息。";
        //  //  return responseMessage;
        //    return null;
        //}
    }
}
