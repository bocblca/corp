using Mysqldb;
using Npoi.Mapper.Attributes;
using NPOI.SS.UserModel;

namespace workauto.corp
{
    public class Approvalmsg
    {
        public string Userid { get; set; }
        public string Noticeid { get; set; }
    }
    public class TimeInterval
    {
        public DateTimeOffset Start { get; set; }
        public DateTimeOffset End { get; set; }

        public TimeInterval(DateTimeOffset start, DateTimeOffset end)
        {
            Start = start;
            End = end;
        }
    }

    public record Spnoinfo(string sp_no);
    public class Receivedata 
    {
      public Supernotice supernotice { get; set; }
      public int Approvalstatus { get; set; }
    }
    public class SuperExcel {
        [Column("督办单编号")]
       
        public string NoticeId { get; set; }
        [Column("被检机构")]
        public string Notice_departname { get; set; }
        [Column("督办机构")]
        public string Orderdata_departname { get;set; }
        [Column("督办事项")]
        public string Orderdata_task { get; set; }
        [Column("督办详情")]
        public string Orderdata_taskinfo { get;set; }
        [Column("反馈详情")]
        public string Noticedata_applyinfo { get; set; }
        [Column("督办联系人")]
        public string Orderdata_name { get;set; }
        [Column("督办时间")]
        public string Ordertime { get; set; }
        [Column("反馈时间")]
        public string Noticetime { get; set; }
        
        [Column("审批状态")]
        public string Approval_status { get; set; }

    }
    public class LimitExcel
    {
        [Column("业务编号")]
        public string Limitid { get; set; }
        [Column("发起时间")]
        public long Transtime { get; set; }
        [Column("发起部门")]
        public string Departname { get; set; }
        [Column("发起人")]
        public string Username { get; set; }
        [Column("发起审批人")]
        public string Leadername { get; set; }
        [Column("接收部门")]
        public string Relay_departname { get; set; }
        [Column("办结审批人")]
        public string Relay_leadername { get; set; }
        [Column("限时事项")]
        public string Conttype { get; set; }
        [Column("时效子项")]
        public string Detail { get; set; }
        [Column("限时天数")]
        public int Day { get; set; }
        [Column("时效说明")]
        public string Info { get; set; }
        [Column("是否办结")]
        public Boolean Isover { get; set; }
        [Column("办结时间")]
        public long Relay_approval_time { get; set; }   
      
    }
    public record Sprecord(string Sp_type,string Apply_time, string Apply_userid,string Apply_departid, string Sp_status, string Start,string End, double Duration);
    public record Sp_no(string sp_no);

    //public record Spdata(string Sp_no, string Sp_type, string Apply_time, string Apply_userid, string Apply_departid, string Sp_status, string Start, string End, double Duration);
}
