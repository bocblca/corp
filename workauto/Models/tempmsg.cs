using Mysqldb;
using Senparc.Weixin.MP.MessageContexts;
namespace workapi.Models
{
    public class tempmsg : DefaultMpMessageContext
    {
        public Wxusers mdata { get; set; }
    }
}
