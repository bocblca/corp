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
}
