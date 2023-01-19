using Flurl;
using Flurl.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mysqldb;
using Senparc.CO2NET.Extensions;
using Senparc.Weixin;
using Senparc.Weixin.Entities;
using Senparc.Weixin.Work.AdvancedAPIs;
using Senparc.Weixin.Work.AdvancedAPIs.MailList;
using Senparc.Weixin.Work.Containers;
using Senparc.Weixin.Work.Helpers;
using SixLabors.ImageSharp;

namespace workauto
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class WorkApiController : ControllerBase
    {
       
      
        public static readonly string Token = Config.SenparcWeixinSetting.WorkSetting.WeixinCorpToken;//与企业微信账号后台的Token设置保持一致，区分大小写。
        public static readonly string EncodingAESKey = Config.SenparcWeixinSetting.WorkSetting.WeixinCorpEncodingAESKey;//与微信企业账号后台的EncodingAESKey设置保持一致，区分大小写。
        public static readonly string CorpId = Config.SenparcWeixinSetting.WorkSetting.WeixinCorpId;//与微信企业账号后台的CorpId设置保持一致，区分大小写。
        public static readonly string corpSecret = Config.SenparcWeixinSetting.WorkSetting.WeixinCorpSecret;
        public readonly ISenparcWeixinSettingForWork workSetting = Senparc.Weixin.Config.SenparcWeixinSetting.Items["workscan"];
        private readonly Wxusers _mdata;
        private readonly ILogger _logger;
    
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

        public WorkApiController(Wxusers mdata, ILogger<WorkApiController> logger)
        {
            _mdata = mdata;
            _logger = logger;
          
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
                   // _mdata.asset_States.Add(state_info);
                    _mdata.asset_States.Update(state_info);

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
                    //_mdata.asset_States.Add(state_info);
                    _mdata.asset_States.Update(state_info);

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
                    //_mdata.asset_States.Add(state_info);
                    
                    _mdata.asset_States.Update(state_info);
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
                    //_mdata.asset_States.Add(state_info);
                    _mdata.asset_States.Update(state_info);


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

                //_mdata.asset_States.Add(state_info);
                _mdata.asset_States.Update(state_info);
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
        public ActionResult GetUrlBase(string returnUrl)
        {


            var oauthUrl =
                OAuth2Api.GetCode(workSetting.WeixinCorpId, returnUrl.UrlEncode(),
                    null, null);//snsapi_base方式回调地址

            return Ok(oauthUrl);
        }
        [HttpGet]
        public async Task<ActionResult> GetUseridAsync(string code)
        {

            var appKey = AccessTokenContainer.BuildingKey(workSetting);
            var accessToken = await AccessTokenContainer.GetTokenAsync(workSetting.WeixinCorpId, workSetting.WeixinCorpSecret);
            //获取用户信息 测试链接：https://open.work.weixin.qq.com/wwopen/devtool/interface?doc_id=10019
            var oauthResult = await OAuth2Api.GetUserIdAsync(accessToken, code);
            var userId = oauthResult.UserId;
            GetMemberResult result = await MailListApi.GetMemberAsync(appKey, userId);
            return Ok(result);
        }
        [HttpGet]
        public async Task<ActionResult> GetjsApiUiPackageAsync(string httpsurl)
        {

            // 获取 JsApiTicket（保密信息，不可外传）
            var jsApiTicket = await JsApiTicketContainer.GetTicketAsync(workSetting.WeixinCorpId, workSetting.WeixinCorpSecret, false);
            // 获取 UI 打包信息
            var jsApiUiPackage = await JSSDKHelper.GetJsApiUiPackageAsync(workSetting.WeixinCorpId, workSetting.WeixinCorpSecret, httpsurl, jsApiTicket, false);

            return Ok(jsApiUiPackage);
        }
        [HttpGet]
        public async Task<ActionResult> GetagentjsApiUiPackageAsync(string httpsurl)
        {


            // 获取 JsApiTicket（保密信息，不可外传）
            var agentConfigJsApiTicket = await JsApiTicketContainer.GetTicketAsync(workSetting.WeixinCorpId, workSetting.WeixinCorpSecret, true);
            var agentJsApiUiPackage = await JSSDKHelper.GetJsApiUiPackageAsync(workSetting.WeixinCorpId, workSetting.WeixinCorpSecret, httpsurl, agentConfigJsApiTicket, true);

            return Ok(agentJsApiUiPackage);
        }

        [HttpGet]
        public async Task<ActionResult> Getdepart()
        {
            string AppKey = AccessTokenContainer.BuildingKey(CorpId, corpSecret);

            var departs = await MailListApi.GetDepartmentListAsync(AppKey);
            //var members = await MailListApi.GetDepartmentMemberAsync(AppKey, 1, 1);
            //var members1 = await MailListApi.GetDepartmentMemberInfoAsync(AppKey, 1, 1);
            if (departs.department != null)
            {


                foreach (var dp in departs.department)
                {
                    _mdata.wrokdeparts.Add(new Workdepart { id = dp.id, name = dp.name, order = dp.order, parentid = dp.parentid });

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
        public List<string> Getparentdeparts(long departid)
        {
            //string AppKey = AccessTokenContainer.BuildingKey(CorpId, corpSecret);
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
        public string Getspandate(long tmspan)
        {
            var mydate = DateTimeOffset.FromUnixTimeMilliseconds(tmspan).LocalDateTime;
            return mydate.ToString();
        }
        [HttpGet]
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
        public async Task<ActionResult<GetDepartmentListResult>> GetmaindepartAsync(long departid)
        {
            string AppKey = AccessTokenContainer.BuildingKey(CorpId, corpSecret);
            GetDepartmentListResult res = await MailListApi.GetDepartmentListAsync(AppKey, departid);

            return res;

        }
        [HttpGet]
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
        public async Task<ActionResult<GetMemberResult>> GetUserinfosAsync(string userid)
        {
            string AppKey = AccessTokenContainer.BuildingKey(CorpId, corpSecret);
            var res = await MailListApi.GetMemberAsync(AppKey, userid);

            return res;

        }
        [HttpGet]
        public async Task<ActionResult> GetMembers()
        {

            _logger.LogWarning("获取warning级别以上信息:members alll bank");
            string AppKey = AccessTokenContainer.BuildingKey(CorpId, corpSecret);

            //var departs = await MailListApi.GetDepartmentListAsync(AppKey);
            var members = await MailListApi.GetDepartmentMemberAsync(AppKey, 1, 1);
            //var members1 = await MailListApi.GetDepartmentMemberInfoAsync(AppKey, 1, 1);
            if (members.userlist != null)
            {


                foreach (var user in members.userlist)
                {

                    _mdata.members.Add(new Member { userid = user.userid, name = user.name, department = string.Join(",", user.department) });

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
        public async Task<ActionResult> Getdeptlist(long dept_id)
        {
            string AppKey = AccessTokenContainer.BuildingKey(CorpId, corpSecret);
            var res = await MailListApi.GetDepartmentListAsync(AppKey, dept_id);

            return Ok(res.department);


        }
        [HttpGet]
        public async Task<ActionResult> Getbranchdeptlist(long dept_id, long pid)
        {
            string AppKey = AccessTokenContainer.BuildingKey(CorpId, corpSecret);
            var res = await MailListApi.GetDepartmentListAsync(AppKey, dept_id);
            var branch = res.department.Where(e => e.parentid == pid);
            return Ok(branch);


        }
        [HttpGet]

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

        [HttpGet]
        public async Task<ActionResult> GetMembersbydeptAsync(long dept_id, int son)
        {
            string AppKey = AccessTokenContainer.BuildingKey(CorpId, corpSecret);

            //var departs = await MailListApi.GetDepartmentListAsync(AppKey);
            var members = await MailListApi.GetDepartmentMemberAsync(AppKey, dept_id, son);
            //var members1 = await MailListApi.GetDepartmentMemberInfoAsync(AppKey, 1, 1);
            return Ok(members.userlist);


        }
        [HttpGet]
        public async Task<ActionResult> Get_assetMembers(long dept_id, int son = 1)
        {
            string AppKey = AccessTokenContainer.BuildingKey(CorpId, corpSecret);
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
        public async Task<ActionResult> Set_all_enable(long departid)
        {
            string AppKey = AccessTokenContainer.BuildingKey(CorpId, corpSecret);
            var mtoken = AccessTokenContainer.TryGetToken(CorpId, corpSecret);

            var members_result = await MailListApi.GetDepartmentMemberAsync(AppKey, departid, 1);
            var members = members_result.userlist;

            foreach (var memx in members)
            {

                // var memberlists = await MailListApi.GetMemberAsync(AppKey, memx.userid);
                //此处判断可可以去除总行、分行
                var updateuserx = new Customupdateuser()
                {

                    userid = memx.userid,
                    enable = 1
                };
                _ = await MailListApi.UpdateMemberAsync(AppKey, updateuserx);




            }

            return Ok("success");
        }

        [HttpPost]
        public async Task<ActionResult> Set_all_direct_leaderAsync(long departid)
        {
            string AppKey = AccessTokenContainer.BuildingKey(CorpId, corpSecret);
            var mtoken = AccessTokenContainer.TryGetToken(CorpId, corpSecret);

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
                    var updateuserx = new Customupdateuser()
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
        public async Task<ActionResult> Setdepartleader(string depart_id)
        {

            var mtoken = AccessTokenContainer.TryGetToken(CorpId, corpSecret);
            //string AppKey = AccessTokenContainer.BuildingKey(CorpId, corpSecret);

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
    }
}
