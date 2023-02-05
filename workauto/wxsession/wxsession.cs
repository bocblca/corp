using System.ComponentModel.DataAnnotations;

namespace workapi.wxsession
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
