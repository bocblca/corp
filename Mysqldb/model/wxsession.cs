using System.ComponentModel.DataAnnotations;

namespace Mysqldb
{
    public class Wxsession
    {
        [Key]
        public string code { get; set; }
        [MaxLength(100)]
        public string? sessionkey { get; set; }
        [MaxLength(100)]
        public string? unionid { get; set; }
    }
}
