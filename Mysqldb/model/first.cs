using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mysqldb.model
{
    public class First_upload {
        public IFormFile[] Files { get; set; } 
        public string First { get; set; }
    }

    public class Firstapproval {
        public string Transid { get; set; }
        //处理时间
        public long Transtime { get; set; }

        public Cust Cust { get; set; }

        public List<Employeeinfo> Relay { get; set; }

        public Trans_status Trans_Status { get; set; }

        public List<Downfile> Attachs { get; set; }
    }

    public class Downfile{
       
        public string ContentType { get; set; }
        public string Name { get; set; }
        public long Size { get; set; }
    }
    public class First
    {
        [MaxLength(100)]
        [Key]
        public string Transid { get; set; }
        //处理时间
       
        public long Transtime { get; set; }

        //抄送人
        public string Copypersion { get; set; }
        [Column(TypeName = "jsonb")]
        public Cust Cust { get; set; }
        
        //流转列表lists
        [Column(TypeName = "jsonb")]
        public List<Employeeinfo> Relay { get; set;  }
        [Column(TypeName = "jsonb")]
      
        public Trans_status Trans_Status { get; set; }
        [Column(TypeName = "jsonb")]
        //附件
        public List<Transfile> Attachs { get; set; }
    }

    public class Transfile
    {
        public byte[]  Data { get; set; }
        public string ContentType { get; set; }
        public  string Name { get; set; }

        public long Size { get; set; }
    } 


    public class Trans_status { 
       public bool Isover { get; set; }
       public string Userid { get; set; }
       public string Name { get; set; }
      
       public long Comptime { get; set; }
    }
    public class Employeeinfo { 
       
        public string Userid { get; set; }
        public string Name { get; set; }

        public Boolean Ismain { get; set; }
        public string Leaderid { get; set; }

        public string Leadername { get; set; }

        //领导审批意见

        //0、未审批 1、同意 2、驳回 
        public int Leader_approval { get; set; }

        //领导审批意见
        public string Approval_content { get; set; }
        // 转给其部门
        public string Relay_userid { get; set; }


        public string Departid { get; set; }

        public string Departname { get; set; }
        //员工处理意见
        public string Content { get; set; }
       
        //处理时间
        public long Transtime { get; set; }
      
        public long Approval_time { get; set; }
    }
    public class Cust { 
      public string Name { get; set; }
      public string Corp { get; set; }
      public string Phone { get; set; }
      //诉求内容
      public string Content { get; set; }
      //客户意见
      public string Suggest { get; set; }

        //1来电、2来信、3来访、4网上投诉、5直接面交
      public int Conttype { get; set; }

    }
}
