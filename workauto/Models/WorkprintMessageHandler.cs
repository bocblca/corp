using Mysqldb;
using Senparc.Weixin;
using Senparc.Weixin.Entities;
using Senparc.Weixin.Work.AdvancedAPIs;
using Senparc.Weixin.Work.Containers;
using Senparc.Weixin.Work.Entities;
using Senparc.Weixin.Work.MessageHandlers;

namespace workapi.Models
{
    public class WorkprintMessageHandler : WorkMessageHandler<WorkCustomMessageContext>
    {
        /// <summary>
        /// 为中间件提供生成当前类的委托
        /// </summary>
        /// 
        private Wxusers wxusers;
        public static Func<Stream, PostModel, int, IServiceProvider, WorkprintMessageHandler> GenerateMessageHandler =
            (stream, postModel, maxRecordCount, serviceProvider) => new WorkprintMessageHandler(stream, postModel, maxRecordCount, serviceProvider);

        readonly ISenparcWeixinSettingForWork _workSetting;

        public WorkprintMessageHandler(Stream inputStream, PostModel postModel, int maxRecordCount = 0, IServiceProvider serviceProvider = null)
            : base(inputStream, postModel, maxRecordCount, serviceProvider: serviceProvider)
        {
            _workSetting = Senparc.Weixin.Config.SenparcWeixinSetting.Items["workprint"];
            wxusers = serviceProvider.GetRequiredService<Wxusers>();
        }

        public override IWorkResponseMessageBase OnTextRequest(RequestMessageText requestMessage)
        {

            var responseMessage = this.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "您发送了消息(workprint)：" + requestMessage.Content;

            //发送一条客服消息
            var weixinSetting = Config.SenparcWeixinSetting.WorkSetting;
            var appKey = AccessTokenContainer.BuildingKey(weixinSetting.WeixinCorpId, weixinSetting.WeixinCorpSecret);
            MassApi.SendText(appKey, weixinSetting.WeixinCorpAgentId, "这是一条客服消息，对应您发送的消息：" + requestMessage.Content, OpenId);

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

        public override IWorkResponseMessageBase OnEvent_LocationRequest(RequestMessageEvent_Location requestMessage)
        {
            var responseMessage = this.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = string.Format("位置坐标 {0} - {1}", requestMessage.Latitude, requestMessage.Longitude);
            return responseMessage;
        }

        public override IWorkResponseMessageBase OnEvent_EnterAgentRequest(RequestMessageEvent_Enter_Agent requestMessage)
        {
            var responseMessage = this.CreateResponseMessage<ResponseMessageText>();

            responseMessage.Content = "欢迎进入应用！现在时间是：" + SystemTime.Now.DateTime.ToString();
            return responseMessage;
        }

        /// <summary>
        /// 新增成员
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override async Task<IWorkResponseMessageBase> OnEvent_ChangeContactCreateUserRequestAsync(RequestMessageEvent_Change_Contact_User_Create requestMessage)
        {

            var members = new Member { userid = requestMessage.UserID, name = requestMessage.Name, department = requestMessage.Department };
            wxusers.Add(members);
            _ = await wxusers.SaveChangesAsync();

            return base.OnEvent_ChangeContactCreateUserRequest(requestMessage);
        }


        /// <summary>
        /// 新增部门
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override async Task<IWorkResponseMessageBase> OnEvent_ChangeContactCreatePartyRequestAsync(RequestMessageEvent_Change_Contact_Party_Create requestMessage)
        {

            var departs = new Workdepart { id = requestMessage.Id, name = requestMessage.Name, order = requestMessage.Order, parentid = requestMessage.ParentId };
            wxusers.Add(departs);
            _ = await wxusers.SaveChangesAsync();
            return base.OnEvent_ChangeContactCreatePartyRequest(requestMessage);
        }


        /// <summary>
        /// 修改部门信息
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override async Task<IWorkResponseMessageBase> OnEvent_ChangeContactUpdatePartyRequestAsync(RequestMessageEvent_Change_Contact_Party_Update requestMessage)
        {

            var res = wxusers.wrokdeparts.Where(e => e.id == requestMessage.Id).FirstOrDefault();
            if (res != null)
            {
                if (requestMessage.ParentId != 0)
                {
                    res.parentid = requestMessage.ParentId;

                }
                if (requestMessage.Name != null)
                {
                    res.name = requestMessage.Name;
                }
                _ = await wxusers.SaveChangesAsync();


            }





            return base.OnEvent_ChangeContactUpdatePartyRequest(requestMessage);
        }

        /// <summary>
        /// 删除部门
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override async Task<IWorkResponseMessageBase> OnEvent_ChangeContactDeletePartyRequestAsync(RequestMessageEvent_Change_Contact_Party_Base requestMessage)
        {

            var res = wxusers.wrokdeparts.Where(e => e.id == requestMessage.Id).FirstOrDefault();
            wxusers.wrokdeparts.Remove(res);
            _ = await wxusers.SaveChangesAsync();
            return base.OnEvent_ChangeContactDeletePartyRequest(requestMessage);
        }


        /// <summary>
        /// 删除成员 
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override async Task<IWorkResponseMessageBase> OnEvent_ChangeContactDeleteUserRequestAsync(RequestMessageEvent_Change_Contact_User_Base requestMessage)
        {

            var res = wxusers.members.Where(e => e.userid == requestMessage.UserID).FirstOrDefault();
            wxusers.members.Remove(res);
            _ = await wxusers.SaveChangesAsync();

            return base.OnEvent_ChangeContactDeleteUserRequest(requestMessage);
        }

        /// <summary>
        /// 更新成员
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override async Task<IWorkResponseMessageBase> OnEvent_ChangeContactUpdateUserRequestAsync(RequestMessageEvent_Change_Contact_User_Update requestMessage)
        {
            //发送消息
            // ChatApi.SendChatSimpleMessage(AccessTokenContainer.BuildingKey(_workSetting.WeixinCorpId, _workSetting.WeixinCorpSecret), "001", ChatMsgType.text, $"用户信息已被修改：{requestMessage.ToJson(true)}", 1);



            if (requestMessage.Name != null && requestMessage.DepartmentIdList != null)
            {
                var res = wxusers.members.Where(e => e.userid == requestMessage.UserID).FirstOrDefault();
                if (res != null)
                {
                    res.name = requestMessage.Name;
                    res.department = string.Join(",", requestMessage.DepartmentIdList);
                    _ = await wxusers.SaveChangesAsync();
                }

            }
            else
            {
                if (requestMessage.Name != null)
                {
                    var res = wxusers.members.Where(e => e.userid == requestMessage.UserID).FirstOrDefault();
                    if (res != null)
                    {
                        res.name = requestMessage.Name;

                        _ = await wxusers.SaveChangesAsync();
                    }
                }
                if (requestMessage.Department != null)
                {
                    var res = wxusers.members.Where(e => e.userid == requestMessage.UserID).FirstOrDefault();
                    if (res != null)
                    {

                        res.department = requestMessage.Department;

                        _ = await wxusers.SaveChangesAsync();
                    }
                }
            }




            return base.OnEvent_ChangeContactUpdateUserRequest(requestMessage);
        }



        public override IWorkResponseMessageBase DefaultResponseMessage(IWorkRequestMessageBase requestMessage)
        {

            var responseMessage = this.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "这是一条没有找到合适回复信息的默认消息。";
            return responseMessage;
        }
    }
}
