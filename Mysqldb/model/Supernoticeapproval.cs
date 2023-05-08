using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NPOI.Util;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mysqldb
{
    public class Supernoticeapproval
    {
        public string  Noticeid { get; set; }

        [Column(TypeName = "jsonb")]
        public List<Approval_userid> Users { get; set; }

    }
    public class Approval_userid { 
      public  string Userid { get; set; }
      public  int Approverstep { get; set; }

      public long  Dt {  get; set; }
     
    }
}
