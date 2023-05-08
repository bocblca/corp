using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Senparc.Weixin.Work.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mysqldb
{


    public class Supernotice
    {
        public string NoticeId { get; set; }
        [Column(TypeName = "jsonb")]
        public Noticedata Noticedata { get; set; }
        [Column(TypeName = "jsonb")]
        public Orderdata Orderdata { get; set; }

        [Column(TypeName = "jsonb")]
        public List<Approval> Approvals { get; set; }

        //1、总分行创建 2、总分行授权发出 3、支行反馈 4、支行授权反馈
        //5、总分行接收反馈审核 6、总分行接收一级授权 7、总行接收二级授权 8、总分行接收三级授权 9、分行终结授权 10、总行终结授权
        public int Approverstep { get; set; }= 0;

       

    }
    public class Approval
    {
        public string Approvalid { get; set; }
        public string Approvalname { get; set; }
        public long Ordertime { get; set; }

        public string TempId { get; set; }

        public string Approval_memo { get; set; }

        public Approval_extdata Approval_extData { get; set; }

        public string Approval_status { get; set; }

        public string Approval_Userid { get; set; }

        public int Nodestatus { get; set; }
    }

    public class Approval_extdata
    {
        public List<FieldList> FieldList { get; set; }
    }
    public class FieldList
    {

        public string Title { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
    }
    public class Noticedata
    {
        public string Bankdepartid { get; set; }
        public string Bankdepartname { get; set; }
        public string Leaderid { get; set; }
        public string Leadername { get; set; }
        public string Leaderdepart { get; set; }
        public string Leaderposition { get; set; }
        public string Noticebankuserid { get; set; }
        public string Noticebankusername { get; set; }
        public long Checkdate { get; set; }
        public string Applyinfo { get; set; }
    }
    public class Orderdata
    {
        public string Userid { get; set; }
        public string Username { get; set; }
        public string Departid { get; set; }
        public string Avatar { get; set; }
        public string Departname { get; set; }
        public string Phone { get; set; }
        public string Task { get; set; }
        public string Taskinfo { get; set; }

        public long Ordertime { get; set; }
    }
}

