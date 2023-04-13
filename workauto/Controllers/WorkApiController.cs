using Flurl;
using Flurl.Http;
using LiteDB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using Mysqldb;
using Mysqldb.Migrations;
using NPOI.OpenXmlFormats.Dml;
using OpenAI_API;
using OpenAI_API.Completions;
using OpenAI_API.Models;
using Senparc.CO2NET.Extensions;
using Senparc.Weixin.Entities;
using Senparc.Weixin.Work.AdvancedAPIs;
using Senparc.Weixin.Work.AdvancedAPIs.External;
using Senparc.Weixin.Work.AdvancedAPIs.MailList;
using Senparc.Weixin.Work.AdvancedAPIs.OA;
using Senparc.Weixin.Work.AdvancedAPIs.OA.OAJson;
using Senparc.Weixin.Work.Containers;
using Senparc.Weixin.Work.Entities.Request.KF;
using Senparc.Weixin.Work.Helpers;
using TencentCloud.Cvm.V20170312.Models;
using workauto.corp;
using workauto.filter;
using workauto.works;
using Zack.EventBus;

using Config = Senparc.Weixin.Config;
using Image = SixLabors.ImageSharp.Image;

namespace workauto
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class WorkApiController : ControllerBase
    {



        public readonly ISenparcWeixinSettingForWork scanSetting = Config.SenparcWeixinSetting.Items["workscan"];
        public readonly ISenparcWeixinSettingForWork superSetting = Config.SenparcWeixinSetting.Items["supernotice"];
        public readonly ISenparcWeixinSettingForWork workSetting = Config.SenparcWeixinSetting.WorkSetting;
        private readonly Wxusers _mdata;
        private readonly ILogger _logger;
        private readonly IEventBus eventBus;
        public Dictionary<string, string> Corpdic = new Dictionary<string, string>() {
            { "electronic","电子设备" },
            {"office","办公用品" },
            {"consumable","低值易耗品" },
            {"news","未使用"},
            {"use","使用中" },
            {"scrap","报废" },
            {"clean","清理" },
            {"Salvage","清理变现"}
        };

        public WorkApiController(Wxusers mdata, ILogger<WorkApiController> logger, IEventBus eventBus)
        {
            _mdata = mdata;
            _logger = logger;
            this.eventBus = eventBus;

        }

        public OpenAIAPI api = new("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");



        [HttpPost]

        public async Task<IActionResult> Chatmsg(string msg)
        {
            var response = HttpContext.Response;
            response.Headers.Add("Content-Type", "text/event-stream");

            await foreach (var token in api.Completions.StreamCompletionEnumerableAsync(new CompletionRequest(msg, Model.DavinciText, max_tokens: 1000, temperature: 0.4)))
            {
                if (token.ToString().Trim().Length > 0)
                {
                    var message = "data:{ \"words\": \"" + token + "\"}\n\n";

                    Console.Write(message);
                    Console.Write("===========");

                    // Console.Write(message);
                    await response.WriteAsync(message);
                    await response.Body.FlushAsync();

                }

            }

            return new EmptyResult();

        }




        [HttpGet]

        public IActionResult Sendmsg()
        {
            eventBus.Publish("OrderCreated", 8888);
            return Ok("success");

        }



        [HttpPost]
        public async Task<ActionResult> EditAssetAllAsync([FromForm] Masset masset)
        {

            var res = _mdata.assets.Where(e => e.Qrcode == masset.Qrcode).FirstOrDefault();

            if (res == null)
            {
                return Ok("保存失败,编号不存在,errcode:1003");
            }
            else
            {

                using var memoryStream = new MemoryStream();
                await masset.file.CopyToAsync(memoryStream);
                var imgbyte = memoryStream.ToArray();
                var imagefrom = Image.Load(imgbyte);

                var file_name = GettmSpan().ToString() + ".jpg";
                await ImageExtensions.SaveAsJpegAsync(imagefrom, "wwwroot/assetimg/" + file_name);
                string fileurl = "https://rcbcybank.com/image/assetimg/" + file_name;
                var tempuserid = res.Userid;
                res.Pname = masset.Pname;
                res.kind = (Mysqldb.Kinds)(Kinds)masset.kind;
                res.State = (Mysqldb.States)(States)masset.state;
                res.Userid = masset.Userid;
                res.Point = new Mysqldb.Gps()
                {
                    Lat = masset.Lat,
                    Lon = masset.Lon,
                };
                res.Money = masset.money;
                res.Img = fileurl;





                if (masset.Userid == tempuserid)
                {
                    var state_info = new asset_state()
                    {
                        spanid = GettmSpan(),
                        qrcode = masset.Qrcode,
                        operator_id = masset.operator_id,

                        operator_content = "edit allinfo: user no change image change ",

                        target_id = masset.Userid,
                        state = (int)res.State,

                    };
                    _mdata.asset_States.Add(state_info);
                    //_mdata.asset_States.Update(state_info);



                }
                else
                {

                    var state_info = new asset_state()
                    {
                        spanid = GettmSpan(),
                        qrcode = masset.Qrcode,
                        origin_id = tempuserid,
                        target_id = masset.Userid,
                        operator_id = masset.operator_id,

                        operator_content = "edit allinfo:includs user and image",


                        state = (int)res.State,

                    };
                    _mdata.asset_States.Add(state_info);

                    //_mdata.asset_States.Update(state_info);


                }

                _ = await _mdata.SaveChangesAsync();

                return Ok("数据更新成功");

            }







        }
        [HttpPost]

        public async Task<ActionResult> EditAssetdata(Nasset asset)
        {
            var res = _mdata.assets.Where(e => e.Qrcode == asset.Qrcode).FirstOrDefault();
            if (res == null)
            {
                return Ok("更新数据失败,errcode:1001");
            }
            else
            {

                var tempuserid = res.Userid;
                res.Pname = asset.Pname;
                res.kind = (Kinds)asset.kind;
                res.State = (States)asset.state;
                res.Userid = asset.Userid;
                res.Point = new Gps()
                {
                    Lat = asset.Lat,
                    Lon = asset.Lon,
                };

                res.Money = asset.money;

                if (asset.Userid == tempuserid)
                {
                    var state_info = new asset_state()
                    {
                        spanid = GettmSpan(),
                        qrcode = asset.Qrcode,
                        operator_id = asset.operator_id,

                        operator_content = "edit userinfo: only info no image",
                        target_id = asset.Userid,

                        state = (int)res.State,

                    };
                    _mdata.asset_States.Add(state_info);

                    //_mdata.asset_States.Update(state_info);
                }
                else
                {

                    var state_info = new asset_state()
                    {
                        spanid = GettmSpan(),
                        qrcode = asset.Qrcode,
                        origin_id = tempuserid,
                        target_id = asset.Userid,
                        operator_id = asset.operator_id,

                        operator_content = "edit userinfo: only info cluds user and  no image",


                        state = (int)res.State,

                    };
                    _mdata.asset_States.Add(state_info);
                    //_mdata.asset_States.Update(state_info);


                }





                _ = await _mdata.SaveChangesAsync();
                return Ok("数据更新成功");
            }


        }
        [HttpPost]
        public async Task<ActionResult> EditAssetFileAsync([FromForm] FileAsset fileAsset)
        {
            var res = _mdata.assets.Where(e => e.Qrcode == fileAsset.qrcode).FirstOrDefault();
            if (res == null)
            {
                return Ok("保存失败,编号不存在,errcode:1002");
            }
            else
            {
                using var memoryStream = new MemoryStream();
                await fileAsset.file.CopyToAsync(memoryStream);
                var imgbyte = memoryStream.ToArray();
                var imagefrom = Image.Load(imgbyte);

                var file_name = GettmSpan().ToString() + ".jpg";
                await ImageExtensions.SaveAsJpegAsync(imagefrom, "wwwroot/assetimg/" + file_name);
                string fileurl = "https://rcbcybank.com/image/assetimg/" + file_name;

                res.Img = fileurl;
                //_ = await _mdata.SaveChangesAsync();

                var state_info = new asset_state()
                {
                    spanid = GettmSpan(),
                    qrcode = fileAsset.qrcode,
                    operator_id = fileAsset.operator_id,
                    target_id = res.Userid,
                    operator_content = "edit image:" + res.Img,
                    state = (int)res.State,

                };

                _mdata.asset_States.Add(state_info);
                //_mdata.asset_States.Update(state_info);
                _ = await _mdata.SaveChangesAsync();
                return Ok("资产图片更新成功");
            }
        }

        [HttpPost]
        public async Task<ActionResult> SaveAssetnew([FromForm] Masset masset)
        {
            try
            {
                using var memoryStream = new MemoryStream();
                await masset.file.CopyToAsync(memoryStream);
                var imgbyte = memoryStream.ToArray();
                var imagefrom = Image.Load(imgbyte);

                var file_time = GettmSpan();
                var file_name = file_time.ToString() + ".jpg";
                await ImageExtensions.SaveAsJpegAsync(imagefrom, "wwwroot/assetimg/" + file_name);
                string fileurl = "https://rcbcybank.com/image/assetimg/" + file_name;



                var workinfo = new Asset()
                {
                    Qrcode = masset.Qrcode,
                    Pname = masset.Pname,
                    kind = (Kinds)masset.kind,
                    State = (States)masset.state,
                    Point = new Gps
                    {
                        Lat = masset.Lat,
                        Lon = masset.Lon
                    },
                    Money = masset.money,
                    Userid = masset.Userid,
                    Img = fileurl

                };
                _mdata.assets.Add(workinfo);
                var state_info = new asset_state()
                {
                    spanid = file_time,
                    qrcode = masset.Qrcode,
                    target_id = masset.Userid,
                    operator_id = masset.operator_id,
                    origin_id = null,
                    state = masset.state,
                    operator_content = "add new info:all"
                };
                _mdata.asset_States.Add(state_info);

                var res = await _mdata.SaveChangesAsync();
                return Ok("资产信息登记成功");
            }
            catch
            {
                return Ok("资产信息登记失败");

            }

        }

        [HttpGet]
        [NotTransactional]
        public ActionResult Getasset_state(string qrcode)
        {
            var resdata = _mdata.asset_States.Where(e => e.qrcode == qrcode);
            if (resdata == null)
            {
                return Ok("nodata");
            }
            else
            {
                if (resdata.Any())
                {

                    return Ok(resdata);
                }
                else
                {
                    return Ok("nodata");
                }

            }
        }

        [HttpGet]
        [NotTransactional]
        public ActionResult GetUrlBase(string returnUrl)
        {


            var oauthUrl = OAuth2Api.GetCode(scanSetting.WeixinCorpId, returnUrl.UrlEncode(), "blc", superSetting.WeixinCorpAgentId, "code", "snsapi_privateinfo");//snsapi_base方式回调地址

            return Ok(oauthUrl);
        }

        [HttpGet]
        [NotTransactional]
        public ActionResult GetUrlBase_super(string returnUrl)
        {


            var oauthUrl = OAuth2Api.GetCode(superSetting.WeixinCorpId, returnUrl.UrlEncode(), "blc", superSetting.WeixinCorpAgentId, "code", "snsapi_privateinfo");//snsapi_base方式回调地址

            return Ok(oauthUrl);
        }
        [HttpGet]
        [NotTransactional]
        public async Task<ActionResult> GetUseridAsync(string code)
        {


            var accessToken = await AccessTokenContainer.TryGetTokenAsync(scanSetting.WeixinCorpId, scanSetting.WeixinCorpSecret);

            var oauthResult = await OAuth2Api.GetUserIdAsync(accessToken, code);
            var res = OAuth2Api.GetUserDetail(accessToken, oauthResult.user_ticket);


            return Ok(res);



        }
        [HttpGet]
        [NotTransactional]
        public async Task<ActionResult> GetUserid_superAsync(string code)
        {


            var accessToken = await AccessTokenContainer.TryGetTokenAsync(superSetting.WeixinCorpId, superSetting.WeixinCorpSecret);

            var oauthResult = await OAuth2Api.GetUserIdAsync(accessToken, code);
            var res = OAuth2Api.GetUserDetail(accessToken, oauthResult.user_ticket);


            return Ok(res);
        }
        [HttpGet]
        public async Task<ActionResult> GetjsApiUiPackageAsync(string httpsurl)
        {

            // 获取 JsApiTicket（保密信息，不可外传）
            var jsApiTicket = await JsApiTicketContainer.GetTicketAsync(scanSetting.WeixinCorpId, scanSetting.WeixinCorpSecret, false);
            // 获取 UI 打包信息
            var jsApiUiPackage = await JSSDKHelper.GetJsApiUiPackageAsync(scanSetting.WeixinCorpId, scanSetting.WeixinCorpSecret, httpsurl, jsApiTicket, false);

            return Ok(jsApiUiPackage);
        }
        [HttpGet]
        public async Task<ActionResult> GetjsApiUiPackage_superAsync(string httpsurl)
        {

            // 获取 JsApiTicket（保密信息，不可外传）
            var jsApiTicket = await JsApiTicketContainer.GetTicketAsync(superSetting.WeixinCorpId, superSetting.WeixinCorpSecret, false);
            // 获取 UI 打包信息
            var jsApiUiPackage = await JSSDKHelper.GetJsApiUiPackageAsync(superSetting.WeixinCorpId, superSetting.WeixinCorpSecret, httpsurl, jsApiTicket, false);

            return Ok(jsApiUiPackage);
        }
        [HttpGet]
        [NotTransactional]
        public async Task<ActionResult> GetagentjsApiUiPackageAsync(string httpsurl)
        {


            // 获取 JsApiTicket（保密信息，不可外传）
            var agentConfigJsApiTicket = await JsApiTicketContainer.GetTicketAsync(scanSetting.WeixinCorpId, scanSetting.WeixinCorpSecret, true);
            var agentJsApiUiPackage = await JSSDKHelper.GetJsApiUiPackageAsync(scanSetting.WeixinCorpId, scanSetting.WeixinCorpSecret, httpsurl, agentConfigJsApiTicket, true);

            return Ok(agentJsApiUiPackage);
        }
        [HttpGet]
        [NotTransactional]
        public async Task<ActionResult> GetagentjsApiUiPackage_superAsync(string httpsurl)
        {


            // 获取 JsApiTicket（保密信息，不可外传）
            var agentConfigJsApiTicket = await JsApiTicketContainer.GetTicketAsync(superSetting.WeixinCorpId, superSetting.WeixinCorpSecret, true);
            var agentJsApiUiPackage = await JSSDKHelper.GetJsApiUiPackageAsync(superSetting.WeixinCorpId, superSetting.WeixinCorpSecret, httpsurl, agentConfigJsApiTicket, true);

            return Ok(agentJsApiUiPackage);
        }


        [HttpGet]
        [NotTransactional]
        public async Task<ActionResult> Getdepart()
        {
            string AppKey = AccessTokenContainer.BuildingKey(workSetting.WeixinCorpId, workSetting.WeixinCorpSecret);

            var departs = await MailListApi.GetDepartmentListAsync(AppKey);
            int mlevel;
            //var members = await MailListApi.GetDepartmentMemberAsync(AppKey, 1, 1);
            //var members1 = await MailListApi.GetDepartmentMemberInfoAsync(AppKey, 1, 1);
            if (departs.department != null)
            {


                foreach (var dp in departs.department)
                {
                    Console.WriteLine(dp.name + "==" + dp.id);
                    var members = await MailListApi.GetDepartmentMemberAsync(AppKey, dp.id, 0);

                    foreach (var ulist in members.userlist)
                    {
                        var leader = await MailListApi.GetMemberAsync(AppKey, ulist.userid);

                        if (leader.is_leader_in_dept[0] == 1)
                        {
                            Console.WriteLine(leader.userid + "===" + leader.name);
                        }

                    }
                    mlevel = dp.parentid switch
                    {
                        0 => 0,
                        7 => 1,
                        _ => 2,
                    };
                    _mdata.wrokdeparts.Add(new Workdepart { id = dp.id, name = dp.name, order = dp.order, parentid = dp.parentid, level = mlevel });

                }

                _ = await _mdata.SaveChangesAsync();
                return Ok(departs.department.Count);

            }
            else
            {

                return Ok("error");
            }




        }
        [HttpGet]
        [NotTransactional]
        public List<string> Getparentdeparts(long departid)
        {
            //string AppKey = AccessTokenContainer.BuildingKey(scanSetting.WeixinCorpId, scanSetting.WeixinCorpSecret);
            List<string> departs = new();
            long tmpid;
            var res = _mdata.wrokdeparts.AsNoTracking().Where(e => e.id == departid).FirstOrDefault();
            if (res != null)
            {
                tmpid = res.parentid;
                for (int i = 0; i <= 10; i++)
                {

                    var res1 = _mdata.wrokdeparts.AsNoTracking().Where(e => e.id == tmpid).FirstOrDefault();
                    if (res1 != null)
                    {
                        tmpid = res1.parentid;
                        departs.Add(res1.name);
                    }
                    else
                    {
                        break;
                    }
                }


            }



            return departs;

        }

        [HttpGet]
        [NotTransactional]
        public Selectoption[] GetKinds()
        {
            var mlen = Enum.GetNames(typeof(Kinds)).Length;
            Selectoption[] noptions = new Selectoption[mlen];

            for (int i = 0; i < mlen; i++)
            {
                noptions[i] = new Selectoption
                {
                    value = i,
                    label = Corpdic[Enum.GetName(typeof(Kinds), i) ?? ""]
                };


            }
            return noptions;
            /*
            Selectoption[] moptions = new Selectoption[] {
                new Selectoption {value = (int)Kinds.electronic, label = Corpdic[Kinds.electronic.ToString()] },
                new Selectoption {value = (int)Kinds.office, label = Corpdic[Kinds.office.ToString()] },
                new Selectoption {value = (int)Kinds.consumable, label = Corpdic[Kinds.consumable.ToString()] },
            };

            return moptions;  */

        }

        [HttpGet]
        [NotTransactional]
        public string Getspandate(long tmspan)
        {
            var mydate = DateTimeOffset.FromUnixTimeMilliseconds(tmspan).LocalDateTime;
            return mydate.ToString();
        }
        [HttpGet]
        [NotTransactional]
        public long Getdatespan()

        {

            var tmspan = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
            return tmspan;
        }
        private long GettmSpan()
        {
            //获取时间戳
            var tmspan = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
            //时间戳转本地时间
            // var mydate=DateTimeOffset.FromUnixTimeMilliseconds(tmspan).LocalDateTime;

            return tmspan;
        }

        [HttpGet]
        [NotTransactional]
        public Selectoption[] GetStates()
        {

            var mlen = Enum.GetNames(typeof(States)).Length;
            Selectoption[] noptions = new Selectoption[mlen];

            for (int i = 0; i < mlen; i++)
            {
                noptions[i] = new Selectoption
                {
                    value = i,
                    label = Corpdic[Enum.GetName(typeof(States), i) ?? ""]
                };


            }
            return noptions;
            /*
              Selectoption[] moptions = new Selectoption[] {



                  new Selectoption {value = (int)States.news, label = Corpdic[States.news.ToString()] },
                  new Selectoption {value = (int)States.use, label = Corpdic[States.use.ToString()] },
                  new Selectoption {value = (int)States.scrap, label = Corpdic[States.scrap.ToString()] },
                  new Selectoption {value = (int)States.clean, label = Corpdic[States.clean.ToString()] },
                  new Selectoption {value = (int)States.Salvage, label = Corpdic[States.Salvage.ToString()] },
              };

              return moptions;  */

        }

        [HttpGet]
        [NotTransactional]
        public async Task<ActionResult<GetDepartmentListResult>> GetmaindepartAsync(long departid)
        {
            string AppKey = AccessTokenContainer.BuildingKey(scanSetting.WeixinCorpId, scanSetting.WeixinCorpSecret);
            GetDepartmentListResult res = await MailListApi.GetDepartmentListAsync(AppKey, departid);

            return res;

        }
        [HttpGet]
        [NotTransactional]
        public ActionResult GetAsset(string qrcode)
        {

            var res = _mdata.assets.Include(e => e.Point).Where(e => e.Qrcode == qrcode).FirstOrDefault();
            var asset = new Asset
            {
                Qrcode = "0"
            };
            if (res == null)
            {
                return Ok(asset);
            }
            else
            {
                return Ok(res);
            }

        }



        [HttpGet]
        [NotTransactional]
        public async Task<ActionResult<GetMemberResult>> GetUserinfosAsync(string userid)
        {
            string AppKey = await AccessTokenContainer.TryGetTokenAsync(workSetting.WeixinCorpId, workSetting.WeixinCorpSecret);
            var res = await MailListApi.GetMemberAsync(AppKey, userid);


            return res;

        }
        [HttpGet]
        [NotTransactional]
        public async Task<ActionResult> GetMembers()
        {

            _logger.LogWarning("获取warning级别以上信息:members alll bank");
            string AppKey = AccessTokenContainer.BuildingKey(scanSetting.WeixinCorpId, scanSetting.WeixinCorpSecret);

            //var departs = await MailListApi.GetDepartmentListAsync(AppKey);
            var members = await MailListApi.GetDepartmentMemberAsync(AppKey, 1, 1);
            //var members1 = await MailListApi.GetDepartmentMemberInfoAsync(AppKey, 1, 1);
            if (members.userlist != null)
            {


                foreach (var user in members.userlist)
                {

                    _mdata.members.Add(new Member { userid = user.userid, name = user.name, department = string.Join(",", user.department) });
                    Console.WriteLine(user.userid + "====" + user.name);

                }

                _ = await _mdata.SaveChangesAsync();
                return Ok(members.userlist.Count);

            }
            else
            {
                return Ok("error");
            }




        }
        [HttpGet]
        [NotTransactional]
        public async Task<ActionResult> Getdeptlist(long dept_id)
        {
            string AppKey = AccessTokenContainer.BuildingKey(scanSetting.WeixinCorpId, scanSetting.WeixinCorpSecret);
            var res = await MailListApi.GetDepartmentListAsync(AppKey, dept_id);

            return Ok(res.department);


        }
        [HttpGet]
        [NotTransactional]
        public async Task<ActionResult> Getbranchdeptlist(long dept_id, long pid)
        {
            string AppKey = AccessTokenContainer.BuildingKey(scanSetting.WeixinCorpId, scanSetting.WeixinCorpSecret);
            var res = await MailListApi.GetDepartmentListAsync(AppKey, dept_id);
            var branch = res.department.Where(e => e.parentid == pid);
            return Ok(branch);


        }
        [HttpGet]
        [NotTransactional]
        public ActionResult Getasset_total()
        {
            try
            {
                var resdata = _mdata.assets.AsEnumerable().GroupBy(e => new { e.kind, e.State }).Select(q => new
                {

                    kind = Corpdic[Enum.GetName(typeof(Kinds), q.Key.kind) ?? ""],
                    state = Corpdic[Enum.GetName(typeof(States), q.Key.State) ?? ""],
                    money = q.Sum(x => x.Money),
                    sums = q.Count()

                });
                return Ok(resdata);
            }
            catch
            {

                return Ok("error");
            }


        }
        [HttpPost]
        [NotTransactional]
        public async Task<ActionResult> CreateChat()
        {
            string AppKey = AccessTokenContainer.BuildingKey(scanSetting.WeixinCorpId, scanSetting.WeixinCorpSecret);
            // var members= await MailListApi.GetDepartmentMemberAsync(AppKey, dept_id, son);

            var departs = MailListApi.GetDepartmentListAsync(AppKey);

            var xx = await MailListApi.GetMemberAsync(AppKey, "642005");



            return Ok(xx.is_leader_in_dept[0]);

        }

        [HttpGet]
        [NotTransactional]
        public async Task<ActionResult> GetMembersbydeptAsync(long dept_id, int son)
        {
            string AppKey = AccessTokenContainer.BuildingKey(scanSetting.WeixinCorpId, scanSetting.WeixinCorpSecret);

            //var departs = await MailListApi.GetDepartmentListAsync(AppKey);
            var members = await MailListApi.GetDepartmentMemberAsync(AppKey, dept_id, son);
            //var members1 = await MailListApi.GetDepartmentMemberInfoAsync(AppKey, 1, 1);
            return Ok(members.userlist);


        }
        [HttpGet]
        [NotTransactional]
        public async Task<ActionResult> Get_assetMembers(long dept_id, int son = 1)
        {
            string AppKey = AccessTokenContainer.BuildingKey(scanSetting.WeixinCorpId, scanSetting.WeixinCorpSecret);
            var Members = await MailListApi.GetDepartmentMemberAsync(AppKey, dept_id, son);
            var resdata = Members.userlist;
            if (resdata != null)
            {

                var res = _mdata.assets.Include(e => e.Point).AsEnumerable().Where(e => resdata.Where(x => x.userid == e.Userid).FirstOrDefault() != null);
                if (res.Count() > 0)
                {
                    return Ok(res);
                }
                else
                {

                    return Ok("erroruser");

                }

            }
            else
            {
                return Ok("erroruser");
            }

        }

        [HttpPost]
        [NotTransactional]
        public async Task<ActionResult> Set_all_enable(long departid)
        {
            string AppKey = AccessTokenContainer.BuildingKey(workSetting.WeixinCorpId, workSetting.WeixinCorpSecret);
            var mtoken = AccessTokenContainer.TryGetToken(workSetting.WeixinCorpId, workSetting.WeixinCorpSecret);

            var members_result = await MailListApi.GetDepartmentMemberAsync(AppKey, departid, 1);
            var members = members_result.userlist;

            foreach (var memx in members)
            {

                // var memberlists = await MailListApi.GetMemberAsync(AppKey, memx.userid);
                //此处判断可可以去除总行、分行
                var updateuserx = new Senparc.Weixin.Work.AdvancedAPIs.MailList.Member.MemberUpdateRequest()
                {

                    userid = memx.userid,
                    enable = 1
                };
                _ = await MailListApi.UpdateMemberAsync(AppKey, updateuserx);




            }

            return Ok("success");
        }

        [HttpPost]
        [NotTransactional]
        public async Task<ActionResult> Set_all_direct_leaderAsync(long departid)
        {
            string AppKey = AccessTokenContainer.BuildingKey(workSetting.WeixinCorpId, workSetting.WeixinCorpSecret);
            var mtoken = AccessTokenContainer.TryGetToken(workSetting.WeixinCorpId, workSetting.WeixinCorpSecret);

            var members_result = await MailListApi.GetDepartmentMemberAsync(AppKey, departid, 1);
            var members = members_result.userlist;

            foreach (var memx in members)
            {
                var memberlists = await MailListApi.GetMemberAsync(AppKey, memx.userid);
                var leader_id = await GetdepartinfoAsync(mtoken, memberlists.main_department.ToString());
                if (memberlists.is_leader_in_dept[0] == 0 && leader_id != "error" && memberlists.main_department != 0 && memberlists.main_department != 1
                    && memberlists.main_department != 8 && memberlists.main_department != 11 && memberlists.main_department != 7 &&
                     memberlists.main_department != 13 && memberlists.main_department != 33 && memberlists.main_department != 29 && memberlists.main_department != 81)
                {   //此处判断可可以去除总行、分行
                    var updateuserx = new Senparc.Weixin.Work.AdvancedAPIs.MailList.Member.MemberUpdateRequest()
                    {
                        direct_leader = new string[] { leader_id },
                        userid = memberlists.userid,
                        enable = 1
                    };
                    _ = await MailListApi.UpdateMemberAsync(AppKey, updateuserx);



                }



            }




            return Ok("success");
        }
        [HttpGet]
        [NotTransactional]
        public async Task<ActionResult> Setdepartleader(string depart_id)
        {

            var mtoken = AccessTokenContainer.TryGetToken(workSetting.WeixinCorpId, workSetting.WeixinCorpSecret);
            //string AppKey = AccessTokenContainer.BuildingKey(scanSetting.WeixinCorpId, scanSetting.WeixinCorpSecret);

            var leader = await GetdepartinfoAsync(mtoken, depart_id);

            return Ok(leader);
        }
        private async Task<string> GetdepartinfoAsync(string mtoken, string depart_id)
        {
            try
            {


                string mhost = "https://qyapi.weixin.qq.com";
                var res = await mhost.AppendPathSegment("cgi-bin/department/get")
                    .SetQueryParam("access_token", mtoken)
                    .SetQueryParam("id", depart_id)

                    .GetJsonAsync();

                return res.department.department_leader[0];
            }
            catch
            {
                return "error";
            }


        }
        [HttpGet]
        [NotTransactional]
        public ActionResult Bankdepartinfo(long depart_id)
        {

            var res = _mdata.wrokdeparts.Where(e => e.id == depart_id).FirstOrDefault();
            if (res == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(res);
            }


        }
        [HttpGet]
        [NotTransactional]
        public async Task<ActionResult> Getdepartleader(long depart_id)
        {
            var mtoken = AccessTokenContainer.TryGetToken(superSetting.WeixinCorpId, superSetting.WeixinCorpSecret);
            string mhost = "https://qyapi.weixin.qq.com";
            Departuserinfos userinfo = await mhost.AppendPathSegment("cgi-bin/user/list")
                .SetQueryParam("access_token", mtoken)
                .SetQueryParam("department_id", depart_id)
                .SetQueryParam("fetch_child", 0)

                .GetJsonAsync<Departuserinfos>();

            var result = userinfo.userlist.Where(e => e.isleader == 1 && e.main_department == depart_id).FirstOrDefault();


            return Ok(result);


        }

        [HttpGet]
        [NotTransactional]
        public async Task<ActionResult> Gettags()
        {
            var mtoken = AccessTokenContainer.TryGetToken(workSetting.WeixinCorpId, workSetting.WeixinCorpSecret);
            var result = await MailListApi.GetTagListAsync(mtoken);

            return Ok(result);
        }
        [HttpGet]
        [NotTransactional]
        public async Task<ActionResult> Gettagid(string userid)
        {
            var mtoken = AccessTokenContainer.TryGetToken(workSetting.WeixinCorpId, workSetting.WeixinCorpSecret);
            var tags = new int[] { 23, 24 };

            foreach (int tag in tags)
            {
                Console.WriteLine(tag);
                var result = await MailListApi.GetTagMemberAsync(mtoken, tag);
                var res = result.userlist.Where(e => e.userid == userid).FirstOrDefault();
                if (res != null)
                {

                    return Ok(tag);
                }

            }

            return Ok(0);

        }

        [HttpGet]
        [NotTransactional]
        public async Task<ActionResult> Getnoticebankuser(long depart_id, int tagid)
        {
            //tagid  24--机构督办员
            var mtoken = AccessTokenContainer.TryGetToken(superSetting.WeixinCorpId, superSetting.WeixinCorpSecret);

            var result = await MailListApi.GetTagMemberAsync(mtoken, tagid);
            // var result=await MailListApi.GetTagListAsync(mtoken);
            var users = await MailListApi.GetDepartmentMemberAsync(mtoken, depart_id, 0);


            var res = users.userlist.Where(e => result.userlist.Where(x => x.userid == e.userid).FirstOrDefault() != null);
            return Ok(res);


        }
        [HttpGet]
        [NotTransactional]
        public async Task<ActionResult> Getdepartfromuserid(string userid)
        {
            var mtoken = AccessTokenContainer.TryGetToken(superSetting.WeixinCorpId, superSetting.WeixinCorpSecret);
            var res = await MailListApi.GetMemberAsync(mtoken, userid);
            var result = _mdata.wrokdeparts.Where(e => e.id == res.main_department).FirstOrDefault();
            return Ok(result);
        }
        [HttpPost]
        public async Task<ActionResult> SaveNoticedata(Supernotice supernotice)
        {
            var result = _mdata.Supernotices.AsNoTracking().Where(e => e.NoticeId == supernotice.NoticeId).FirstOrDefault();
            if (result != null)
            {

                // _mdata.Supernotices.Update(supernotice);
                _mdata.Supernotices.Attach(supernotice);
                _mdata.Entry(supernotice).State = EntityState.Modified;
                // _mdata.Entry(supernotice).Property(x => x.Noticedata.Applyinfo).IsModified = true;
            }
            else
            {
                _mdata.Supernotices.Add(supernotice);
            }
            // _mdata.Supernotices.Add(supernotice);
            var res = await _mdata.SaveChangesAsync();

            return Ok("success");
        }
        [HttpPost]
        public async Task<ActionResult> Saverbank_replay(Banknotice banknotice)
        {
            var result = _mdata.Supernotices.Where(e => e.NoticeId == banknotice.noticeid).FirstOrDefault();
            if (result != null)
            {
                // _mdata.Supernotices.Update(supernotice);
                result.Noticedata.Applyinfo = banknotice.replayinfo;
                result.Approverstep = 3;


                result.Approvals.Add(banknotice.approval);


                _mdata.Supernotices.Attach(result);
                _mdata.Entry(result).State = EntityState.Modified;
                var res = await _mdata.SaveChangesAsync();
                return Ok("success");
            }
            else
            {
                return Ok("error");
            }

        }
        [HttpPost]
        [NotTransactional]
        public async Task<ActionResult> Setworkshow(Workshow workshow)
        {

            var mtoken = AccessTokenContainer.TryGetToken(superSetting.WeixinCorpId, superSetting.WeixinCorpSecret);
            string mhost = "https://qyapi.weixin.qq.com";

            var res = await mhost.AppendPathSegment("cgi-bin/agent/set_workbench_template")
                    .SetQueryParam("access_token", mtoken)
                    .PostJsonAsync(workshow)
                    .ReceiveJson<Workshow_result>();


            return Ok(res);

        }
        [HttpGet]
        [NotTransactional]
        public ActionResult Getbanknoticelist(string Noticebankuserid)
        {


            var res = _mdata.Supernotices.Where(e => e.Noticedata.Noticebankuserid == Noticebankuserid && e.Noticedata.Applyinfo == "" && (e.Approverstep == 2 || e.Approverstep == 5)).OrderByDescending(e => e.NoticeId);

            return Ok(res);

        }
        [HttpGet]
        [NotTransactional]
        public ActionResult Getbanknoticelist_none(string Noticebankuserid)
        {


            var res = _mdata.Supernotices.Where(e => e.Noticedata.Noticebankuserid == Noticebankuserid && e.Noticedata.Applyinfo != "").OrderByDescending(e => e.NoticeId);

            return Ok(res);

        }

        [HttpGet]
        [NotTransactional]
        public async Task<ActionResult> GetapprovalListAsync()
        {

            var mtoken = AccessTokenContainer.TryGetToken(superSetting.WeixinCorpId, superSetting.WeixinCorpSecret);
            var time1 = DateTimeOffset.Now.AddDays(-10).ToUnixTimeSeconds();
            var time2 = DateTimeOffset.Now.ToUnixTimeSeconds();
            List<GetApprovalInfoRequest_Filter> filters = new List<Senparc.Weixin.Work.AdvancedAPIs.OA.OAJson.GetApprovalInfoRequest_Filter>();
            GetApprovalInfoRequest_Filter filter = new Senparc.Weixin.Work.AdvancedAPIs.OA.OAJson.GetApprovalInfoRequest_Filter()
            {
                key = "record_type",
                value = "1"
            };
            filters.Add(filter);
            GetApprovalInfoRequest approval = new()
            {
                starttime = time1.ToString(),
                endtime = time2.ToString(),
                cursor = 0,
                size = 100,
                filters = filters

            };

            var result = await OaApi.GetApprovalInfoAsync(mtoken, approval);

            return Ok(result);
        }
        [HttpGet]
        [NotTransactional]
        public async Task<ActionResult> GetapprovaldetailAsync(string Spno)
        {
            var mtoken = AccessTokenContainer.TryGetToken(superSetting.WeixinCorpId, superSetting.WeixinCorpSecret);
            var res = await OaApi.GetApprovalDetailAsync(mtoken, Spno);
            return Ok(res);
        }

        [HttpGet]
        [NotTransactional]
        //根据上一级部门判断确定员工是否是总行员工
        private async Task<string> GetbanklevelAsync(string userid)
        {
            var mtoken = AccessTokenContainer.TryGetToken(workSetting.WeixinCorpId, workSetting.WeixinCorpSecret);
            var Memberinfo = await MailListApi.GetMemberAsync(mtoken, userid);
            var Departid = Memberinfo.main_department;

            var mhost = "https://qyapi.weixin.qq.com/cgi-bin/department/get";

            var res = await mhost
                .SetQueryParam("access_token", mtoken)
                .SetQueryParam("id", Departid)
                .GetJsonAsync<dynamic>();
            return res.department.parentid;

        }
        [HttpGet]
        [NotTransactional]
        public ActionResult Getbankreceive(string Noticeid)
        {

            var res = _mdata.Supernotices.Where(e => e.NoticeId == Noticeid && e.Approverstep == 4).FirstOrDefault();

            if (res == null)
            {
                return Ok("error");
            }
            else
            {
                return Ok(res);
            }

        }

        private async Task<string> GetuserLeaderAsync(string userid)
        {
            var mtoken = AccessTokenContainer.TryGetToken(workSetting.WeixinCorpId, workSetting.WeixinCorpSecret);

            var res = await MailListApi.GetMemberAsync(mtoken, userid);

            if (res.direct_leader.Length == 0)
            {
                return "error";
            }
            else
            {
                return res.direct_leader[0];
            }

        }
        [HttpPost]
        [NotTransactional]

        //这逻辑有点绕啊
        public async Task<ActionResult> Approval_sendmsgAsync(string Userid, string Username, string Noticeid)
        {
            var mtoken = AccessTokenContainer.TryGetToken(superSetting.WeixinCorpId, superSetting.WeixinCorpSecret);
            var ressuper = _mdata.Supernotices.Where(e => e.NoticeId == Noticeid && e.Approverstep >= 4).FirstOrDefault();
            var parentId = await GetuserLeaderAsync(Userid);

            var departlevel = await GetbanklevelAsync(ressuper.Orderdata.Userid);
            int Mapprovalstep;
            if (parentId == "error")
            {
                if (departlevel == "7")
                {
                    Mapprovalstep = 30;
                }
                else
                {
                    Mapprovalstep = 20;
                }
            }
            else
            {
                _ = await MassApi.SendTextCardAsync(mtoken, superSetting.WeixinCorpAgentId, "支行返回督办单审核已通过", $"上一级审批人:{Userid}-{Username}\n 督办单编号:{Noticeid}", $"https://rcbcybank.com/#/Noticereceive?noticeid={Noticeid}", "审批", parentId);
                Mapprovalstep = ressuper.Approverstep + 1;
            }
            
            var resapproval = _mdata.Supernoticeapprovals.AsEnumerable().Where(e => e.Noticeid == Noticeid).FirstOrDefault();
            List<Approval_userid> Musers = new()
                    {
                       new Approval_userid() {
                          Userid = Userid,
                          Approverstep = Mapprovalstep
                       }
                    };

            Supernoticeapproval approvalinfo = new()
            {
                Noticeid = Noticeid,
                Users = Musers
            };


            if (resapproval == null)
            {

                var resnotice1 = _mdata.Supernotices.Where(e => e.NoticeId == Noticeid).FirstOrDefault();
                resnotice1.Approverstep = Mapprovalstep;
                _mdata.Supernotices.Attach(resnotice1);
                _mdata.Entry(resnotice1).State = EntityState.Modified;
                _mdata.Supernoticeapprovals.Add(approvalinfo);
                _ = await _mdata.SaveChangesAsync();
            }
            else
            {

                if (resapproval.Users.Where(e => e.Userid == Userid).FirstOrDefault() == null)
                {
                    resapproval.Users.Add(approvalinfo.Users[0]);
                    _mdata.Supernoticeapprovals.Attach(resapproval);
                    _mdata.Entry(resapproval).State = EntityState.Modified;

                    var resnotice2 = _mdata.Supernotices.Where(e => e.NoticeId == Noticeid).FirstOrDefault();
                    resnotice2.Approverstep = Mapprovalstep;
                    _mdata.Supernotices.Attach(resnotice2);
                    _mdata.Entry(resnotice2).State = EntityState.Modified;
                }


                _ = await _mdata.SaveChangesAsync();

            }


            return Ok("success2");
        }


    }
    //[HttpPost]
    //[NotTransactional]

    //public async Task<ActionResult> SetdirectLeaderAsync(string userid)
    //{
    //    var mtoken = AccessTokenContainer.TryGetToken(workSetting.WeixinCorpId, workSetting.WeixinCorpSecret);
    //    var updateuserx = new Senparc.Weixin.Work.AdvancedAPIs.MailList.Member.MemberUpdateRequest()
    //    {
    //        direct_leader = new string[] { "642005", "636323" },
    //        userid = userid,
    //        enable = 1
    //    };
    //    var res= await MailListApi.UpdateMemberAsync(mtoken, updateuserx);

    //    return Ok(res);

    //}
    //}

}
