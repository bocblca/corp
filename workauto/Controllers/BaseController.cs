
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Senparc.Weixin;
using Senparc.Weixin.Entities;

namespace Weixin.Controllers
{
    public class BaseController : Controller
    {

        public BaseController()
        {

        }
        protected string CorpId
        {
            get
            {
                return Config.SenparcWeixinSetting.WeixinCorpId;//与企业微信后台的AppId设置保持一致，区分大小写。
            }
        }

        protected static ISenparcWeixinSettingForWork WorkSetting
        {
            get
            {
                return Config.SenparcWeixinSetting.WorkSetting;
            }
        }
        protected string AppId
        {
            get
            {
                return Config.SenparcWeixinSetting.WeixinAppId;//与微信公众账号后台的AppId设置保持一致，区分大小写。

            }
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            //给模板页 footer 输出使用，根据实际需要配置
            ViewData["CacheType"] = Senparc.CO2NET.Cache.CacheStrategyFactory.GetObjectCacheStrategyInstance().GetType().Name;
            WeixinTrace.SendCustomLog("收到消息", context.Result + "|" + context.HttpContext.Response.Body.ToString() + context.ToString());
            base.OnActionExecuting(context);
        }
    }
}
