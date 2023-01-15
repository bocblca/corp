using System.ComponentModel.DataAnnotations;

namespace workapi.loanmanager
{
    public class LoanProcess
    {
        [Key]
        public string ProcID { get; set; } = "10000";
        public int? flag { get; set; } //处理状态 1-支行发起 2-支行审批 3- 分行审批 4-总行审批 5-总行主管审批 6-总行审批通过返回支行 7-总行审批未通过退回 8-总行审批通过进入贷审会 9-放款
        public string? straff1 { get; set; } //发起人
        public string? straff2 { get; set; } //支行审批人
        public string? straff3 { get; set; }//总行审批人
        public string? straff4 { get; set; } //总行审批主管
        public string straff5 { get; set; } //分行行长
        public DateTime? date11 { get; set; }  //业务发起时间
        public DateTime? date12 { get; set; }  //提交审批时间
        public DateTime? date21 { get; set; }  //支行审受理批时间
        public DateTime? date22 { get; set; }  //支行审完成批时间
        public DateTime? date31{ get; set; }   //分行审批受理时间
        public DateTime? date32 { get; set; }   //分行行审批完成时间
        public DateTime? date41 { get; set; }  //总行审批受理时间
        public DateTime? date42{ get; set; }  //总行审批完成时间
        public DateTime? date51 { get; set; } //总行主管审批受理时间
        public DateTime? date52 { get; set; } //总行主管审批完成时间
        public string? bankid { get; set; } //支行机构号
        public string? bankname { get; set; } //机构名称
        public int? loantype { get; set; } = 1; //业务属性 1-对私贷款 2-对公贷款
        public string? message { get; set; } //退回理由或留言

        public string? parentid { get; set; } //分行机构号

       

    }
}
