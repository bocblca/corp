using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mysqldb.model
{
    public class Limit_upload
    {
        public IFormFile[] Files { get; set; }
        public string Limit { get; set; }
    }

    public class LimitReceiveApproval {

       public string  Limitid { get; set; }
       public long  Relay_approval_time { get; set; }
       public string  Relay_approval { get;set; }
       public string  Relay_approval_content { get; set; }

    }
    public class LimitReceivedata { 
       public string Limitid { get; set; }
       public string Relay_content { get; set; }
    }
    public class Limitupinfo { 
       public string Limitid { get; set; }
       public string Approval { get; set; }
       public string Approval_content { get; set; }

       public long Approval_time { get; set; }
    }
    public class Limitapproval {
        public string Limitid { get; set; }
        public string Userid { get; set; }
        public string Username { get; set; }
        public long Transtime { get; set; }
        public string Leaderid { get; set; }
        public string Leadername { get; set; }
        public string Approval { get; set; }
        public string Approval_content { get; set; }
        public long  Approval_time { get;set; }
        public string Relay_userid { get; set; }
        public string Relay_username { get; set; }
        public string Relay_content { get; set; }
        public long Relay_time { get; set; }
        public string Relay_departid { get; set; }
        public string Relay_departname { get; set; }
        public string Relay_approval { get; set; }
        public string Relay_approval_content { get; set; }
        public string Relay_leaderid { get; set; }
        public string Relay_leadername { get; set; }
        public long Relay_approval_time { get; set; }
        public string Departname { get; set; }
        public string Departid { get; set; }
        public string Conttype { get; set; }
        public string Detail { get; set; }
        public int Day { get; set; }
        public long Overday { get; set; }
        public string Info { get; set; }
        public string Content { get; set; }
        public Boolean Isover { get; set; }
        [Column(TypeName = "jsonb")]
        //附件
        public List<Downfile> Attachs { get; set; }

    }
    public class Limit
    {
        public string Limitid { get; set; }
        public string Userid { get; set; }
        public string Username { get; set; }
        public long Transtime { get; set; }
        public string Leaderid { get; set; }
        public string Leadername { get; set; }
        public string Approval { get; set; }
        public string Approval_content { get; set; }
        public long Approval_time { get; set; }
        public string Relay_userid { get; set; }
        public string Relay_username { get; set; }
        public string Relay_content { get; set; }
        public long Relay_time { get; set; }
        public string Relay_departid { get; set; }
        public string Relay_departname { get; set; }
        public string Relay_approval { get; set; }
        public string Relay_approval_content { get; set; }
        public string Relay_leaderid { get; set; }
        public string Relay_leadername { get;set; }
        public long Relay_approval_time { get; set; }
        public string Departname { get; set; }
        public string Departid { get; set; }
        public string Conttype { get; set; }
        public string Detail { get; set; }
        public int Day { get; set; }
        public long Overday { get; set; }
        public string Info { get; set; }
        public string Content { get; set; }
        public Boolean Isover { get; set; }
        [Column(TypeName = "jsonb")]
        //附件
        public List<Transfile> Attachs { get; set; }
    }
   

}
