using Microsoft.AspNetCore.Mvc;
using Senparc.Weixin;

namespace workapi.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class hldrcb : ControllerBase
    {
        public static readonly string Token = Config.SenparcWeixinSetting.Items["second"].Token;
        [HttpGet]

        public ActionResult Msg(string signature, string timestamp, string nonce, string echostr)
        {


            return Content(echostr); //返回随机字符串则表示验证通过

        }
    }
}
