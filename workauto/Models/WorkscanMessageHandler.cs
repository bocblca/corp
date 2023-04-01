
using K4os.Hash.xxHash;
using Mysqldb;
using NPOI.SS.Formula.Functions;
using Senparc.Weixin.Entities;
using Senparc.Weixin.Exceptions;
using Senparc.Weixin.Work;
using Senparc.Weixin.Work.AdvancedAPIs;
using Senparc.Weixin.Work.CommonAPIs;
using Senparc.Weixin.Work.Containers;
using Senparc.Weixin.Work.Entities;
using Senparc.Weixin.Work.Entities.Menu;
using Senparc.Weixin.Work.MessageHandlers;

namespace workapi.Models
{
    public class WorkscanMessageHandler : WorkMessageHandler<WorkCustomMessageContext>
    {
        /// <summary>
        /// 为中间件提供生成当前类的委托
        /// </summary>
        public static Func<Stream, PostModel, int, IServiceProvider, WorkscanMessageHandler> GenerateMessageHandler =
            (stream, postModel, maxRecordCount, serviceProvider) => new WorkscanMessageHandler(stream, postModel, maxRecordCount, serviceProvider);

        private readonly ISenparcWeixinSettingForWork _workSetting;
   
        public WorkscanMessageHandler(Stream inputStream, PostModel postModel, int maxRecordCount = 0, IServiceProvider serviceProvider = null)
            : base(inputStream, postModel, maxRecordCount, serviceProvider: serviceProvider)
        {
            _workSetting = Senparc.Weixin.Config.SenparcWeixinSetting.Items["workscan"];
            

        }

        public override async Task<IWorkResponseMessageBase> OnTextRequestAsync(RequestMessageText requestMessage)
        {

            var responseMessage = this.CreateResponseMessage<ResponseMessageText>();


            //发送一条客服消息
            //var weixinSetting = Config.SenparcWeixinSetting.WorkSetting;
            //var appKey = AccessTokenContainer.BuildingKey(weixinSetting.WeixinCorpId, weixinSetting.WeixinCorpSecret);
            //MassApi.SendText(appKey, weixinSetting.WeixinCorpAgentId, "这是一条客服消息，对应您发送的消息：" + requestMessage.Content, OpenId);

            if (requestMessage.Content == "999")
            {
                try
                {
                    var aToken = AccessTokenContainer.GetToken(_workSetting.WeixinCorpId, _workSetting.WeixinCorpSecret);
                    // var res = CommonApi.ConvertToUserId(aToken, OpenId);
                    //textcard消息

                    var members = await MailListApi.GetDepartmentMemberAsync(aToken, 105, 0);
                    string[] users = new string[members.userlist.Count];
                    int i = 0;
                    foreach (var ulist in members.userlist)
                    {
                        users[i] = ulist.userid;
                        i++;
                    }
                    
                    var result = await ChatApi.CreateChatAsync(aToken, "6420006", "会计运营部test", "642005", users);

                    var res = await ChatApi.SendChatSimpleMessageAsync(aToken, "6420006", ChatMsgType.text, "test app msg");

                    responseMessage.Content = "您的ID:" + OpenId + "|创新部门群|" + res.errmsg;
                }
                catch (Exception e){
                    responseMessage.Content = e.Message;
                }


            }

            if (requestMessage.Content == "999999")
            {


                //创新菜单
                ButtonGroup bg = new ButtonGroup();

                var subButton = new SubButton()
                {
                    name = "资产扫描"
                };


                subButton.sub_button.Add(new SingleScancodeWaitmsgButton()
                {
                    key = "SingleScancodeWaitmsg",
                    name = "二维码扫描"


                });
                subButton.sub_button.Add(new SingleViewButton()
                {
                    url = "https://rcbcybank.com/#/Manager",
                    name = "资产管理"
                });
                //subButton.sub_button.Add(new SinglePicPhotoOrAlbumButton()
                //{
                //    key = "SubClickRoot_Pic_Photo_Or_Album",
                //    name = "测试拍照"
                //});
                bg.button.Add(subButton);
                var appKey = AccessTokenContainer.BuildingKey(_workSetting);

                int agentId;
                if (!int.TryParse(_workSetting.WeixinCorpAgentId, out agentId))
                {
                    throw new WeixinException("WeixinCorpAgentId 必须为整数！");
                }




                var result = await CommonApi.CreateMenuAsync(appKey, agentId, bg);
                responseMessage.Content = "创建菜单：" + result.errmsg + "|" + result.errcode + "|" + agentId + "|" + appKey;
            }



            return responseMessage;
        }

        public override IWorkResponseMessageBase OnImageRequest(RequestMessageImage requestMessage)
        {
            var responseMessage = CreateResponseMessage<ResponseMessageImage>();
            responseMessage.Image.MediaId = requestMessage.MediaId;

            return responseMessage;
        }

        public override IWorkResponseMessageBase OnEvent_ClickRequest(RequestMessageEvent_Click requestMessage)
        {
            var reponseMessage = CreateResponseMessage<ResponseMessageText>();

            if (requestMessage.EventKey == "SubClickRoot_Text")
            {
                reponseMessage.Content = "您点击了【返回文本】按钮";
            }
            else
            {
                reponseMessage.Content = "您点击了其他事件按钮";
            }

            return reponseMessage;
        }
        public override IWorkResponseMessageBase OnEvent_ScancodePushRequest(RequestMessageEvent_Scancode_Push requestMessage)
        {
            var responseMessage = this.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "scan1" + requestMessage.ScanCodeInfo.ScanResult;

            return responseMessage;
            //return base.OnEvent_ScancodePushRequest(requestMessage);
        }
        public override IWorkResponseMessageBase OnEvent_ScancodeWaitmsgRequest(RequestMessageEvent_Scancode_Waitmsg requestMessage)
        {
            var responseMessage = this.CreateResponseMessage<ResponseMessageText>();
            var appKeyx = AccessTokenContainer.BuildingKey(_workSetting);
            MassApi.SendTextCard(appKeyx, requestMessage.AgentID.ToString(), "扫描结果", "<div class=\"normal\">物品二维(条)码</div><div class=\"highlight\">" + requestMessage.ScanCodeInfo.ScanResult + "</div>", "https://rcbcybank.com/#/?id=" + requestMessage.ScanCodeInfo.ScanResult + "&user=" + requestMessage.FromUserName, "物品登记",requestMessage.FromUserName);
           
            //responseMessage.Content = "扫描end";

            return responseMessage;
            //return base.OnEvent_ScancodeWaitmsgRequest(requestMessage);
        }
        public override IWorkResponseMessageBase OnEvent_LocationRequest(RequestMessageEvent_Location requestMessage)
        {
            var responseMessage = this.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = string.Format("位置坐标 {0} - {1}", requestMessage.Latitude, requestMessage.Longitude);
            return responseMessage;
        }

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
