using System.ComponentModel.DataAnnotations;

namespace workapi.loanmanager
{
    public class Loanuser
    {

        [Key]
        public string straff { get; set; } = "100000";
        public string? straff_name { get; set; }
        public string? unionid { get; set; }
        public string? bankid { get; set; }
        public Boolean? IsLogin { get; set; } = false;
        public int Ace { get; set; } = 1; //角色 1、信贷员 2、支行行长 3、分行行长 4、总行审批人员 5、总行审批主管
        public Boolean IsValid { get; set; } = true; //单角色如审批人员是否在岗

    }
}
