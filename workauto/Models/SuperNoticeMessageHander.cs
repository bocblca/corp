using Microsoft.EntityFrameworkCore;
using Mysqldb;
using Newtonsoft.Json;
using Senparc.Weixin.Entities;
using Senparc.Weixin.Work.AdvancedAPIs;
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
        public override async Task<IWorkResponseMessageBase> OnEvent_Open_Approval_Change_Status_ChangeRequestAsync(RequestMessageEvent_OpenApprovalChange requestMessage)
        {
            Console.WriteLine("自建应用回调");
            var tmpdata = requestMessage.ApprovalInfo;
            var xx = JsonConvert.SerializeObject(tmpdata);
            Console.WriteLine(xx);

            var res = _mdata.Supernotices.Where(e => e.NoticeId + 1 == requestMessage.ApprovalInfo.ThirdNo).FirstOrDefault();
            var mtoken = AccessTokenContainer.TryGetToken(_workSetting.WeixinCorpId, _workSetting.WeixinCorpSecret);
            if (res != null)
            
            {
                  Console.WriteLine("分行审批节点");
                //分行审批回调
                if (res.Approverstep == 1)
                {
                    Console.WriteLine("分行审批节点开始");
                    res.Approvals[0].Approvalid = requestMessage.ApprovalInfo.ThirdNo;
                    res.Approvals[0].Approvalname = requestMessage.ApprovalInfo.OpenSpName;
                    res.Approvals[0].Approval_Userid = requestMessage.ApprovalInfo.ApprovalNodes[0].Items[0].ItemUserId;
                    res.Approvals[0].Approvalname = requestMessage.ApprovalInfo.ApprovalNodes[0].Items[0].ItemName;
                    res.Approvals[0].TempId = requestMessage.ApprovalInfo.OpenTemplateId;
                    res.Approvals[0].Approval_memo = requestMessage.ApprovalInfo.ApprovalNodes[0].Items[0].ItemSpeech;
                    res.Approvals[0].Approval_status = requestMessage.ApprovalInfo.ApprovalNodes[0].Items[0].ItemStatus.ToString();

                    res.Approvals[0].Nodestatus = requestMessage.ApprovalInfo.ApprovalNodes[0].Items[0].ItemStatus;
                    res.Approvals[0].Ordertime = requestMessage.ApprovalInfo.ApprovalNodes[0].Items[0].ItemOpTime;
                    if (requestMessage.ApprovalInfo.ApprovalNodes[0].Items[0].ItemStatus == 2)
                    {
                        res.Approverstep = 2;
                        await MassApi.SendTextCardAsync(mtoken, _workSetting.WeixinCorpAgentId, "督办通知单", "支行有督办通知需要处理!编号:" + res.NoticeId, "https://rcbcybank.com/#/noticelist", null, res.Noticedata.Noticebankuserid);
                    }
                    _mdata.Supernotices.Attach(res);
                    _mdata.Entry(res).State = EntityState.Modified;

                    _ = await _mdata.SaveChangesAsync();
                }
                
            }
            var res2 = _mdata.Supernotices.Where(e => e.NoticeId + 2 == requestMessage.ApprovalInfo.ThirdNo).FirstOrDefault();
            if (res2 != null)
            {
                 Console.WriteLine("支行审批节点");
                //支行行审批回调

                if (res2.Approverstep == 3)
                {
                    Console.WriteLine("支行审批节点开始");

                    res2.Approvals[1].Approvalid = requestMessage.ApprovalInfo.ThirdNo;
                    res2.Approvals[1].Approvalname = requestMessage.ApprovalInfo.OpenSpName;
                    res2.Approvals[1].Approval_Userid = requestMessage.ApprovalInfo.ApprovalNodes[0].Items[0].ItemUserId;
                    res2.Approvals[1].Approvalname = requestMessage.ApprovalInfo.ApprovalNodes[0].Items[0].ItemName;
                    res2.Approvals[1].TempId = requestMessage.ApprovalInfo.OpenTemplateId;
                    res2.Approvals[1].Approval_memo = requestMessage.ApprovalInfo.ApprovalNodes[0].Items[0].ItemSpeech;
                    res2.Approvals[1].Approval_status = requestMessage.ApprovalInfo.ApprovalNodes[0].Items[0].ItemStatus.ToString();
                    res2.Approvals[1].Nodestatus = requestMessage.ApprovalInfo.ApprovalNodes[0].Items[0].ItemStatus;
                    res2.Approvals[1].Ordertime = requestMessage.ApprovalInfo.ApprovalNodes[0].Items[0].ItemOpTime;
                    if (requestMessage.ApprovalInfo.ApprovalNodes[0].Items[0].ItemStatus == 2)
                    {
                        res2.Approverstep = 4;
                        await MassApi.SendTextCardAsync(mtoken, _workSetting.WeixinCorpAgentId, "督办通知单", "支行反馈通知单需要处理,编号:" + res2.NoticeId, "https://rcbcybank.com/#/noticereceive?noticeid=" + res2.NoticeId, null, res2.Orderdata.Userid);
                    }
                    _mdata.Supernotices.Attach(res2);
                    _mdata.Entry(res2).State = EntityState.Modified;
                    _ = await _mdata.SaveChangesAsync();
                }
               
            }
            return null;
        }
        public override IWorkResponseMessageBase DefaultResponseMessage(IWorkRequestMessageBase requestMessage)
        {
            return new WorkSuccessResponseMessage();
        }

        
    }
}
