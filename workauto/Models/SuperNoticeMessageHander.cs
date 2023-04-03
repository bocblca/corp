

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




        //审批状态回调


        //public override IWorkResponseMessageBase OnEvent_Open_Approval_Change_Status_ChangeRequest(RequestMessageEvent_OpenApprovalChange requestMessage)
        //{
           
        //    return base.OnEvent_Open_Approval_Change_Status_ChangeRequest(requestMessage);
       
        //}
        public override IWorkResponseMessageBase OnEvent_Open_Approval_Change_Status_ChangeRequest(RequestMessageEvent_OpenApprovalChange requestMessage)
        {

         
            
            try {
                Console.WriteLine("这是自建应用回调...");
                Console.WriteLine(requestMessage.AgentID);
                Console.WriteLine("这是自建应用回调...执行第一次");
                Console.WriteLine(requestMessage.CreateTime);
                Console.WriteLine("这是自建应用回调...执行第二次");
                if (requestMessage.ApprovalInfo != null)
                {
                    Console.WriteLine("approval不为空");
                    Console.WriteLine(requestMessage.ApprovalInfo.ToString());
                }
                else { 
                   Console.WriteLine("approval是空值");
                }
                
                Console.WriteLine("这是自建应用回调...执行第三次");

                Console.WriteLine(requestMessage.ToString());
                Console.WriteLine("这是自建应用回调...执行第四次");
                Console.WriteLine(requestMessage.ToJsonString());
                Console.WriteLine(requestMessage.ApprovalInfo.OpenSpName);
                Console.WriteLine("这是自建应用回调...执行第五次");
            }catch (Exception ex) { 
                Console.WriteLine(ex.Message); 
            }
           
            return DefaultResponseMessage(requestMessage);
        }

        public override IWorkResponseMessageBase OnEvent_Sys_Approval_Change_Status_ChangeRequest(RequestMessageEvent_SysApprovalChange requestMessage)
        {
            Console.WriteLine("这是系统审批回调...");
            Console.WriteLine(requestMessage.ApprovalInfo.Applyer.UserId);
            return base.OnEvent_Sys_Approval_Change_Status_ChangeRequest(requestMessage);
        }




        public override IWorkResponseMessageBase OnEvent_ScancodeWaitmsgRequest(RequestMessageEvent_Scancode_Waitmsg requestMessage)
        {
            var responseMessage = this.CreateResponseMessage<ResponseMessageText>();
            var appKeyx = AccessTokenContainer.BuildingKey(_workSetting);
            Senparc.Weixin.Work.AdvancedAPIs.MassApi.SendTextCard(appKeyx, requestMessage.AgentID.ToString(), "扫描结果", "<div class=\"normal\">物品二维(条)码</div><div class=\"highlight\">" + requestMessage.ScanCodeInfo.ScanResult + "</div>", "https://rcbcybank.com/#/?id=" + requestMessage.ScanCodeInfo.ScanResult + "&user=" + requestMessage.FromUserName, "物品登记", requestMessage.FromUserName);

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
