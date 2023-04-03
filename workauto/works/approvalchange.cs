using Senparc.Weixin.Work.Entities;
using Senparc.Weixin.Work;
using Senparc.NeuChar.Entities;
using System.Xml.Serialization;

namespace workauto.works
{
    public class Approvalchange : RequestMessageEvent_OpenApprovalChange
    {
        public override Event Event => Event.OPEN_APPROVAL_CHANGE;

        //
        // 摘要:
        //     审批信息
        public new Approvalinfo ApprovalInfo { get; set; }


    }

    public class Approvalinfo {
        public string ThirdNo { get; set; }

        //
        // 摘要:
        //     审批模板名称
        public string OpenSpName { get; set; }

        //
        // 摘要:
        //     审批模板id
        public string OpenTemplateId { get; set; }

        //
        // 摘要:
        //     申请单当前审批状态：1-审批中；2-已通过；3-已驳回；4-已撤销
        public byte OpenSpStatus { get; set; }

        //
        // 摘要:
        //     提交申请时间
        public uint ApplyTime { get; set; }

        //
        // 摘要:
        //     提交者姓名
        public string ApplyUserName { get; set; }

        //
        // 摘要:
        //     提交者userid
        public string ApplyUserId { get; set; }

        //
        // 摘要:
        //     提交者所在部门
        public string ApplyUserParty { get; set; }

        //
        // 摘要:
        //     提交者头像
        public string ApplyUserImage { get; set; }

        //
        // 摘要:
        //     审批流程信息
        [XmlArrayItem("ApprovalNode", IsNullable = false)]
        public OpenApprovalNode[] ApprovalNodes { get; set; }

        //
        // 摘要:
        //     抄送信息，可能有多个抄送人
        [XmlArrayItem("NotifyNode", IsNullable = false)]
        public OpenApprovalNotifyNode[] NotifyNodes { get; set; }

        //
        // 摘要:
        //     当前审批节点：0-第一个审批节点；1-第二个审批节点…以此类推
        public byte ApproverStep { get; set; }

    }  
}
