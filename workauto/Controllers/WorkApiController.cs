using Flurl;
using Flurl.Http;
using LiteDB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mysqldb;
using Mysqldb.model;
using Newtonsoft.Json;
using Npoi.Mapper;
using OpenAI_API;
using OpenAI_API.Completions;
using OpenAI_API.Models;
using Senparc.CO2NET.Extensions;
using Senparc.Weixin.Entities;
using Senparc.Weixin.Work.AdvancedAPIs;
using Senparc.Weixin.Work.AdvancedAPIs.MailList;
using Senparc.Weixin.Work.AdvancedAPIs.OA;
using Senparc.Weixin.Work.AdvancedAPIs.OA.OAJson;
using Senparc.Weixin.Work.Containers;
using Senparc.Weixin.Work.Helpers;
using workauto.corp;
using workauto.filter;
using workauto.works;
using Zack.EventBus;

using Config = Senparc.Weixin.Config;
using Image = SixLabors.ImageSharp.Image;
using Hangfire;

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
        public Dictionary<string, string> Corpdic = new() {
            { "electronic","电子设备" },
            {"office","办公用品" },
            {"consumable","低值易耗品" },
            {"news","未使用"},
            {"use","使用中" },
            {"scrap","报废" },
            {"clean","清理" },
            {"Salvage","清理变现"}
        };
        public Dictionary<int, string> Dic_approval = new() {
            {1,"总分行创建" },
            {2,"总分行授权发出" },
            {3,"支行反馈" },
            {4,"支行授权反馈"},
            {5,"总分行接收反馈审核" },
            {6,"总分行接收一级授权" },
            {7,"总行接收二级授权" },
            {8,"总分行接收三级授权"},
            {9,"分行终结授权"},
            {10,"总行终结授权"}
        };

        [HttpGet]
        public void LoadHangJob() {


            RecurringJobOptions jobOptions = new()
            {
                TimeZone = TimeZoneInfo.Local
            };

            DateTimeOffset myoffset = DateTimeOffset.Parse("2022-07-01T00:00");

            var starttime = myoffset.ToUnixTimeSeconds();
            var endtime = new DateTimeOffset(DateTime.UtcNow.AddDays(1)).ToUnixTimeSeconds();
            // var endtime = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();


            RecurringJob.AddOrUpdate("SaveSpnodetail", () => GetDaterangeAsync(starttime, endtime), Cron.Daily(1, 0), jobOptions);

            //var jobid= BackgroundJob.Schedule(()=>GetDaterangeAsync(starttime, endtime), TimeSpan.FromSeconds(5));

            Console.WriteLine("start HangJob...");

        }

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

        [HttpGet]
        [NotTransactional]
        public ActionResult GetFirstdata(string transid)
        {
            var res = _mdata.Firsts.AsNoTracking().AsEnumerable().Where(e => e.Transid == transid && e.Relay.Where(x => x.Ismain == true && x.Leader_approval == 0).Any()).FirstOrDefault();
            if (res == null)
            {
                return Ok("error");
            }
            else
            {

                return Ok(res);
            }
        }
        [HttpGet]
        [NotTransactional]

        public ActionResult GetFirstAlldata()
        {
            var res = _mdata.Firsts.AsNoTracking().AsEnumerable().Select(e => new FirstData() { Transid = e.Transid, Transtime = e.Transtime, Trans_Status = e.Trans_Status, Cust = e.Cust, Relay = e.Relay });

            if (res == null)
            {
                return Ok("error");
            }
            else
            {
                return Ok(res);
            }
        }
        [HttpGet]
        [NotTransactional]
        public ActionResult GetFirstapprovals(string transid)
        {

            var res = _mdata.Firsts.AsNoTracking().Where(e => e.Transid == transid).FirstOrDefault();

            if (res == null)
            {
                return Ok("error");
            }
            else
            {
                var resapproval = new Firstapproval()
                {
                    Transid = res.Transid,
                    Cust = res.Cust,
                    Relay = res.Relay,
                    Transtime = res.Transtime,
                    Trans_Status = res.Trans_Status,
                    Attachs = res.Attachs.Select(e => new Downfile() { Name = e.Name, Size = e.Size, ContentType = e.ContentType }).ToList(),

                };
                return Ok(resapproval);
            }
        }
        [HttpGet]
        [NotTransactional]
        public FileContentResult Attachsurl(string transid, string name)
        {

            var res = _mdata.Firsts.AsNoTracking().Where(e => e.Transid == transid).FirstOrDefault();
            if (res == null)
            {
                return null;
            }
            else
            {
                var filedata = res.Attachs.Where(e => e.Name == name).FirstOrDefault();
                return File(filedata.Data, filedata.ContentType, filedata.Name);
            }
        }


        [HttpPost]

        public async Task<ActionResult> SaveLimitApproval_receiveAsync(LimitReceiveApproval Receivedata)
        {
            var mtoken_super = AccessTokenContainer.TryGetToken(superSetting.WeixinCorpId, superSetting.WeixinCorpSecret);
            var res = _mdata.Limits.Where(e => e.Limitid == Receivedata.Limitid).FirstOrDefault();
            if (res == null)
            {
                return Ok("error");
            }
            else
            {

                res.Relay_approval = Receivedata.Relay_approval;
                res.Relay_approval_time = Receivedata.Relay_approval_time;
                res.Relay_approval_content = Receivedata.Relay_approval_content;

                var wrline = await _mdata.SaveChangesAsync();

                _ = await MassApi.SendTextCardAsync(mtoken_super, superSetting.WeixinCorpAgentId, "时效办结业务审批结果", $"审批人:{res.Relay_leadername}\n业务编号:{res.Limitid}", $"https://rcbcybank.com/#/Limit?limitid={res.Limitid}& reply={res.Userid}", res.Relay_approval_content, res.Userid);
                _ = await MassApi.SendTextCardAsync(mtoken_super, superSetting.WeixinCorpAgentId, "时效办结业务审批结果", $"审批人:{res.Relay_leadername}\n业务编号:{res.Limitid}", $"https://rcbcybank.com/#/Limit?limitid={res.Limitid}& reply={res.Relay_userid}", res.Relay_approval_content, res.Relay_userid);

                return Ok(wrline);
            }

        }

        [HttpPost]

        public async Task<ActionResult> SaveLimitReceiveAsync(LimitReceivedata receivedata)
        {
            var mtoken_super = AccessTokenContainer.TryGetToken(superSetting.WeixinCorpId, superSetting.WeixinCorpSecret);
            var res = _mdata.Limits.Where(e => e.Limitid == receivedata.Limitid).FirstOrDefault();
            if (res == null)
            {
                return Ok("error");
            }
            else
            {
                res.Relay_content = receivedata.Relay_content;
                var wrline = await _mdata.SaveChangesAsync();

                _ = await MassApi.SendTextCardAsync(mtoken_super, superSetting.WeixinCorpAgentId, "时效办结业务需要审批", $"提交人:{res.Relay_username}\n业务编号:{res.Limitid}", $"https://rcbcybank.com/#/Limit?limitid={res.Limitid}&receive={res.Relay_leaderid}", "审批", res.Relay_leaderid);



                return Ok(wrline);

            }


        }


        [HttpPost]

        public async Task<ActionResult> SaveLimitApprovalAsync(Limitupinfo limitupinfo)
        {
            var mtoken_super = AccessTokenContainer.TryGetToken(superSetting.WeixinCorpId, superSetting.WeixinCorpSecret);
            var resapproval = _mdata.Limits.Where(e => e.Limitid == limitupinfo.Limitid).FirstOrDefault();
            if (resapproval == null)
            {
                return Ok("error");
            }
            else
            {
                resapproval.Approval = limitupinfo.Approval;
                resapproval.Approval_content = limitupinfo.Approval_content;
                resapproval.Approval_time = limitupinfo.Approval_time;
                var wrline = await _mdata.SaveChangesAsync();
                _ = await MassApi.SendTextCardAsync(mtoken_super, superSetting.WeixinCorpAgentId, "时效办结业务审批结果", $"审批人:{resapproval.Leadername}\n业务编号:{resapproval.Limitid}", $"https://rcbcybank.com/#/Limit?limitid={resapproval.Limitid}&reply={resapproval.Userid}", resapproval.Approval, resapproval.Userid);

                if (limitupinfo.Approval == "审批完成")
                {
                    _ = await MassApi.SendTextCardAsync(mtoken_super, superSetting.WeixinCorpAgentId, "您有时效办结业务需要处理", $"来自:{resapproval.Departname}\n业务编号:{resapproval.Limitid}", $"https://rcbcybank.com/#/Limit?limitid={resapproval.Limitid}&fromx={resapproval.Departid}", "详情", resapproval.Relay_userid);
                }
                return Ok(wrline);
            }

        }

        [HttpGet]
        [NotTransactional]
        public FileContentResult Attachsurl_limit(string limitid, string name)
        {

            var res = _mdata.Limits.AsNoTracking().Where(e => e.Limitid == limitid).FirstOrDefault();
            if (res == null)
            {
                return null;
            }
            else
            {
                var filedata = res.Attachs.Where(e => e.Name == name).FirstOrDefault();
                return File(filedata.Data, filedata.ContentType, filedata.Name);
            }
        }

        [HttpPost]

        public async Task<ActionResult> SaveFirstApprovalAsync(First_approval first_Approval)
        {

            var mtoken_super = AccessTokenContainer.TryGetToken(superSetting.WeixinCorpId, superSetting.WeixinCorpSecret);
            var res = _mdata.Firsts.Where(e => e.Transid == first_Approval.Transid).FirstOrDefault();
            if (res == null)
            {
                return Ok("error");
            }
            else
            {
                var resRelay = res.Relay.Where(e => e.Leaderid == first_Approval.Leaderid).FirstOrDefault();

                resRelay.Leader_approval = first_Approval.Leader_approval;
                resRelay.Approval_content = first_Approval.Approval_content;
                resRelay.Approval_time = first_Approval.Approval_time;

                if (first_Approval.Isowen)
                {
                    res.Trans_Status.Comptime = first_Approval.Approval_time;
                    res.Trans_Status.Isover = true;
                }
                else
                {
                    res.Trans_Status.Isover = true;
                }
                _mdata.Firsts.Update(res);
                _mdata.Firsts.Attach(res);
                _mdata.Entry(res).State = EntityState.Modified;
                var result = await _mdata.SaveChangesAsync();
                if (result == 1)
                {
                    if (first_Approval.Leader_approval == 1)
                    {



                        if (first_Approval.Isowen)
                        {

                            _ = await MassApi.SendTextCardAsync(mtoken_super, superSetting.WeixinCorpAgentId, "首问负责制业务审批通过-已结束", $"审批人:{resRelay.Leaderid}-{resRelay.Leadername}\n业务编号:{res.Transid}", $"https://rcbcybank.com/#/First?transid={res.Transid}&approval={resRelay.Leaderid}&relay={resRelay.Userid}", "详情", resRelay.Userid);
                        }
                        else
                        {

                            var Relayother = res.Relay.Where(e => e.Ismain == true).FirstOrDefault();

                            _ = await MassApi.SendTextCardAsync(mtoken_super, superSetting.WeixinCorpAgentId, "首问负责制业务审批通过-已转发", $"审批人:{resRelay.Leaderid}-{resRelay.Leadername}\n业务编号:{res.Transid}", $"https://rcbcybank.com/#/First?transid={res.Transid}&approval={resRelay.Leaderid}&relay={resRelay.Userid}", "详情", resRelay.Userid);
                            _ = await MassApi.SendTextCardAsync(mtoken_super, superSetting.WeixinCorpAgentId, "首问负责制业务(转发)需要处理", $"转发人:{resRelay.Departname}-{resRelay.Userid}-{resRelay.Name}\n业务编号:{res.Transid}", $"https://rcbcybank.com/#/First?transid={res.Transid}", "详情", Relayother.Userid);
                        }
                    }
                    else
                    {
                        _ = await MassApi.SendTextCardAsync(mtoken_super, superSetting.WeixinCorpAgentId, "首问负责制业务审批-已驳回", $"审批人:{resRelay.Leaderid}-{resRelay.Leadername}\n业务编号:{res.Transid}", $"https://rcbcybank.com/#/First?transid={res.Transid}&approval={resRelay.Leaderid}&relay={resRelay.Userid}", "详情", resRelay.Userid);

                    }
                }
                return Ok(result);
            }

        }



        [HttpPost]

        public async Task<ActionResult> SaveRelayNewAsync([FromForm] First_upload data)
        {
            var Rdata = JsonConvert.DeserializeObject<First>(data.First);
            var mtoken_super = AccessTokenContainer.TryGetToken(superSetting.WeixinCorpId, superSetting.WeixinCorpSecret);
            if (data.Files != null)
            {
                foreach (var item in data.Files)
                {
                    using var fs = new MemoryStream();
                    await item.CopyToAsync(fs);
                    Rdata.Attachs.Add(new Transfile
                    {

                        Data = fs.ToArray(),
                        Name = item.FileName,
                        ContentType = item.ContentType,
                        Size = fs.Length
                    });
                }
            }


            _mdata.Firsts.Update(Rdata);
            _mdata.Firsts.Attach(Rdata);
            _mdata.Entry(Rdata).State = EntityState.Modified;


            await _mdata.SaveChangesAsync();
            var resRelay = Rdata.Relay.AsEnumerable().Where(e => e.Userid == Rdata.Trans_Status.Userid).FirstOrDefault();

            _ = await MassApi.SendTextCardAsync(mtoken_super, superSetting.WeixinCorpAgentId, "首问负责制业务需要审批", $"业务发起人:{resRelay.Userid}-{resRelay.Name}\n业务编号:{Rdata.Transid}", $"https://rcbcybank.com/#/First?transid={Rdata.Transid}&approval={resRelay.Leaderid}", "审批", resRelay.Leaderid);

            return Ok("success");
        }

        [HttpPost]

        public async Task<ActionResult> SaveFirstNewAsync([FromForm] First_upload data)
        {
            var Rdata = JsonConvert.DeserializeObject<First>(data.First);
            var mtoken_super = AccessTokenContainer.TryGetToken(superSetting.WeixinCorpId, superSetting.WeixinCorpSecret);
            if (data.Files != null)
            {
                foreach (var item in data.Files)
                {
                    using var fs = new MemoryStream();
                    await item.CopyToAsync(fs);
                    Rdata.Attachs.Add(new Transfile
                    {

                        Data = fs.ToArray(),
                        Name = item.FileName,
                        ContentType = item.ContentType,
                        Size = fs.Length
                    });
                }
            }



            _mdata.Firsts.Add(Rdata);

            await _mdata.SaveChangesAsync();

            var resRelay = Rdata.Relay.AsEnumerable().Where(e => e.Userid == Rdata.Trans_Status.Userid).FirstOrDefault();
            _ = await MassApi.SendTextCardAsync(mtoken_super, superSetting.WeixinCorpAgentId, "首问负责制业务需要审批", $"业务发起人:{resRelay.Userid}-{resRelay.Name}\n业务编号:{Rdata.Transid}", $"https://rcbcybank.com/#/First?transid={Rdata.Transid}&approval={resRelay.Leaderid}", "审批", resRelay.Leaderid);

            return Ok("success");
        }
        [HttpGet]
        [NotTransactional]

        public async Task<ActionResult> GetdepartUserList(long departid)
        {
            string AppKey = await AccessTokenContainer.TryGetTokenAsync(workSetting.WeixinCorpId, workSetting.WeixinCorpSecret);
            var res = await MailListApi.GetDepartmentMemberAsync(AppKey, departid, 0);
            var users = res.userlist.AsEnumerable().Where(e => IsLeader(AppKey, e.userid) != true && e.department.Length == 1);
            return Ok(users);

        }

        private Boolean IsLeader(string AppKey, string userid)
        {

            var res = MailListApi.GetMember(AppKey, userid);
            var resa = res.is_leader_in_dept[0];
            if (resa == 1)
            {
                return true;
            }
            else
            {
                return false;
            }

        }



        private static int Getdays(DateTime mdate, int days)
        {


            int addedworkDays = 0;
            var currentday = mdate;
            int mdays = 0;
            while (addedworkDays < days)
            {
                mdays++;
                currentday = currentday.AddDays(1);
                if (!IsHoliday2023(currentday))
                {
                    addedworkDays++;
                }

            }



            return mdays;

        }

        private static bool IsHoliday2023(DateTime mdate)
        {

            var Holidays = new List<string>() {
               new DateTime(2023,6,22).ToShortDateString(),
               new DateTime(2023,6,23).ToShortDateString(),
               new DateTime(2023,6,24).ToShortDateString(),
               new DateTime(2023,9,29).ToShortDateString(),
               new DateTime(2023,9,30).ToShortDateString(),
               new DateTime(2023,10,1).ToShortDateString(),
               new DateTime(2023,10,2).ToShortDateString(),
               new DateTime(2023,10,3).ToShortDateString(),
               new DateTime(2023,10,4).ToShortDateString(),
               new DateTime(2023,10,5).ToShortDateString(),
               new DateTime(2023,10,6).ToShortDateString(),
            };
            var NoHolidays = new List<string>()
            {
                 new DateTime(2023,6,25).ToShortDateString(),
                 new DateTime(2023,10,7).ToShortDateString(),
                 new DateTime(2023,10,8).ToShortDateString(),
            };

            var resDay = mdate.ToShortDateString();

            if (Holidays.Where(e => e.Equals(resDay)).Any())
            {
                return true;
            }
            if (NoHolidays.Where(e => e.Equals(resDay)).Any())
            {
                return false;
            }
            if (mdate.DayOfWeek == DayOfWeek.Sunday || mdate.DayOfWeek == DayOfWeek.Saturday)
            {
                return true;
            }
            return false;


        }

        [HttpGet]
        [NotTransactional]
        public ActionResult GetLimitAlldata()
        {

            var res = _mdata.Limits.AsNoTracking();
            return Ok(res);
        }


        [HttpGet]
        [NotTransactional]
        public ActionResult GetLimitinfo(string limitid)
        {
            var resLimit = _mdata.Limits.Where(e => e.Limitid == limitid).FirstOrDefault();
            if (resLimit == null)
            {
                return Ok("error");
            }
            else
            {
                var resapproval = new Limitapproval()
                {
                    Approval = resLimit.Approval,
                    Limitid = resLimit.Limitid,
                    Userid = resLimit.Userid,
                    Username = resLimit.Username,
                    Departid = resLimit.Departid,
                    Departname = resLimit.Departname,
                    Transtime = resLimit.Transtime,
                    Content = resLimit.Content,
                    Conttype = resLimit.Conttype,
                    Day = resLimit.Day,
                    Detail = resLimit.Detail,
                    Info = resLimit.Info,
                    Leaderid = resLimit.Leaderid,
                    Leadername = resLimit.Leadername,
                    Isover = resLimit.Isover,
                    Overday = resLimit.Overday,
                    Approval_content = resLimit.Approval_content,
                    Approval_time = resLimit.Approval_time,
                    Relay_approval = resLimit.Relay_approval,
                    Relay_approval_time = resLimit.Relay_approval_time,
                    Relay_departid = resLimit.Relay_departid,
                    Relay_leadername = resLimit.Relay_leadername,
                    Relay_approval_content = resLimit.Relay_approval_content,
                    Relay_content = resLimit.Relay_content,
                    Relay_departname = resLimit.Relay_departname,
                    Relay_leaderid = resLimit.Relay_leaderid,
                    Relay_time = resLimit.Relay_time,
                    Relay_userid = resLimit.Relay_userid,
                    Relay_username = resLimit.Relay_username,
                    Attachs = resLimit.Attachs.Select(e => new Downfile() { Name = e.Name, Size = e.Size, ContentType = e.ContentType }).ToList(),
                };
                return Ok(resapproval);
            }

        }

        [HttpPost]

        public async Task<ActionResult> SaveLimitNewAsync([FromForm] Limit_upload data)
        {
            var Rdata = JsonConvert.DeserializeObject<Limit>(data.Limit);
            var mtoken_super = AccessTokenContainer.TryGetToken(superSetting.WeixinCorpId, superSetting.WeixinCorpSecret);
            string AppKey = await AccessTokenContainer.TryGetTokenAsync(workSetting.WeixinCorpId, workSetting.WeixinCorpSecret);

            if (data.Files != null)
            {
                foreach (var item in data.Files)
                {
                    using var fs = new MemoryStream();
                    await item.CopyToAsync(fs);
                    Rdata.Attachs.Add(new Transfile
                    {

                        Data = fs.ToArray(),
                        Name = item.FileName,
                        ContentType = item.ContentType,
                        Size = fs.Length
                    });
                }
            }
            Rdata.Transtime = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

            var days = Getdays(DateTime.Now, Rdata.Day);//此处处理节假日

            Rdata.Overday = new DateTimeOffset(DateTime.UtcNow.AddDays(days)).ToUnixTimeMilliseconds();
            var resuser = await MailListApi.GetMemberAsync(AppKey, Rdata.Userid);
            if (resuser != null)
            {
                Rdata.Username = resuser.name;
                Rdata.Departid = resuser.main_department.ToString();
            }

            var departinfo = await Getdepartmemo(AppKey, Rdata.Departid);
            var departleader = await GetDirect_Leader(Rdata.Userid);
            Rdata.Leaderid = departleader.userid;
            Rdata.Leadername = departleader.name;
            Rdata.Departname = departinfo.department.name;
            var resleader = await MailListApi.GetMemberAsync(AppKey, Rdata.Leaderid);
            if (resleader != null)
            {
                Rdata.Leadername = resleader.name;
            }

            var relaydirtect = await GetDirect_Leader(Rdata.Relay_userid);
            Rdata.Relay_leaderid = relaydirtect.userid;
            Rdata.Relay_leadername = relaydirtect.name;

            _mdata.Limits.Add(Rdata);

            await _mdata.SaveChangesAsync();

            // var resRelay = Rdata.Relay.AsEnumerable().Where(e => e.Userid == Rdata.Trans_Status.Userid).FirstOrDefault();
            _ = await MassApi.SendTextCardAsync(mtoken_super, superSetting.WeixinCorpAgentId, "时效办结业务需要审批", $"业务发起人:{Rdata.Userid}-{Rdata.Username}\n业务编号:{Rdata.Limitid}", $"https://rcbcybank.com/#/Limit?limitid={Rdata.Limitid}&approval={Rdata.Leaderid}", "审批", Rdata.Leaderid);

            return Ok("success");
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
        public string Getspandate(long tmspan, int flag)
        {
            DateTime mydate;
            if (flag == 0)
            {
                mydate = DateTimeOffset.FromUnixTimeSeconds(tmspan).LocalDateTime;
            }
            else
            {
                mydate = DateTimeOffset.FromUnixTimeMilliseconds(tmspan).LocalDateTime;
            }

            return mydate.ToString();
        }
        [HttpGet]
        [NotTransactional]
        public long Getdatespan()

        {

            var tmspan = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();


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
            string AppKey = AccessTokenContainer.BuildingKey(workSetting.WeixinCorpId, workSetting.WeixinCorpSecret);
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
            string AppKey = AccessTokenContainer.BuildingKey(workSetting.WeixinCorpId, workSetting.WeixinCorpSecret);
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
                if (res.Any())
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
        private static async Task<string> GetdepartinfoAsync(string mtoken, string depart_id)
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
        private static async Task<dynamic> Getdepartmemo(string mtoken, string depart_id)
        {
            try
            {


                string mhost = "https://qyapi.weixin.qq.com";
                var res = await mhost.AppendPathSegment("cgi-bin/department/get")
                    .SetQueryParam("access_token", mtoken)
                    .SetQueryParam("id", depart_id)

                    .GetJsonAsync();

                return res;
            }
            catch
            {
                return "error";
            }


        }

        [HttpGet]
        [NotTransactional]
        public async Task<string> Getdepartname(string depart_id)
        {
            try
            {

                var mtoken = AccessTokenContainer.TryGetToken(workSetting.WeixinCorpId, workSetting.WeixinCorpSecret);
                string mhost = "https://qyapi.weixin.qq.com";
                var res = await mhost.AppendPathSegment("cgi-bin/department/get")
                    .SetQueryParam("access_token", mtoken)
                    .SetQueryParam("id", depart_id)

                    .GetJsonAsync<Departinfo>();

                return res.department.name;
            }
            catch
            {
                return null;
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
        public async Task<ActionResult> GetDirect_LeaderAsync(string userid)
        {
            string AppKey = await AccessTokenContainer.TryGetTokenAsync(workSetting.WeixinCorpId, workSetting.WeixinCorpSecret);
            var res = await MailListApi.GetMemberAsync(AppKey, userid);



            if (res.direct_leader.Length == 0)
            {
                return Ok("error");
            }
            else
            {
                var leaderinfo = await MailListApi.GetMemberAsync(AppKey, res.direct_leader[0]);
                return Ok(leaderinfo);
            }


        }
        private async Task<GetMemberResult> GetDirect_Leader(string userid)
        {
            string AppKey = await AccessTokenContainer.TryGetTokenAsync(workSetting.WeixinCorpId, workSetting.WeixinCorpSecret);
            var res = await MailListApi.GetMemberAsync(AppKey, userid);



            if (res.direct_leader.Length == 0)
            {
                return null;
            }
            else
            {
                var leaderinfo = await MailListApi.GetMemberAsync(AppKey, res.direct_leader[0]);
                return leaderinfo;
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

        public async Task<ActionResult> GettaginfoAsync(string userid, int tagid)
        {

            var mtoken = AccessTokenContainer.TryGetToken(workSetting.WeixinCorpId, workSetting.WeixinCorpSecret);

            var tags = await MailListApi.GetTagListAsync(mtoken);

            if (!tags.taglist.Where(e => e.tagid == tagid.ToString()).Any())
            {

                return Ok("error");
            }
            var result = await MailListApi.GetTagMemberAsync(mtoken, tagid);
            var tag = result.userlist.Where(e => e.userid == userid).FirstOrDefault();
            if (tag == null)
            {
                return Ok("error");
            }
            else
            {
                return Ok(tag);
            }

        }

        [HttpGet]
        [NotTransactional]
        public async Task<ActionResult> Gettagid(string userid)
        {
            var mtoken = AccessTokenContainer.TryGetToken(workSetting.WeixinCorpId, workSetting.WeixinCorpSecret);
            //29 全行督办单管理员 30 首问负责制经办人
            var tags = new int[] { 23, 24, 28, 29, 30 };

            foreach (int tag in tags)
            {
                // Console.WriteLine(tag);
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

                _mdata.Supernotices.Update(supernotice);
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

            var xx = new JsonResult(res);

            return Ok(res);

        }

        [HttpGet]
        [NotTransactional]
        public async Task<ActionResult> GetapprovalListAsync()
        {

            var mtoken = AccessTokenContainer.TryGetToken(superSetting.WeixinCorpId, superSetting.WeixinCorpSecret);
            var time1 = DateTimeOffset.Now.AddDays(-20).ToUnixTimeSeconds();
            var time2 = DateTimeOffset.Now.ToUnixTimeSeconds();
            List<GetApprovalInfoRequest_Filter> filters = new();
            GetApprovalInfoRequest_Filter filter = new()
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
        public string StartHangJob() {




            RecurringJobOptions jobOptions = new() {
                TimeZone = TimeZoneInfo.Local
            };

            DateTimeOffset myoffset = DateTimeOffset.Parse("2022-07-01T00:00");

            var starttime = myoffset.ToUnixTimeSeconds();
            var endtime = new DateTimeOffset(DateTime.UtcNow.AddDays(1)).ToUnixTimeSeconds();
            // var endtime = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();


            RecurringJob.AddOrUpdate("SaveSpnodetail", () => GetDaterangeAsync(starttime, endtime), Cron.Daily(1, 0), jobOptions);

            //var jobid= BackgroundJob.Schedule(()=>GetDaterangeAsync(starttime, endtime), TimeSpan.FromSeconds(5));

            Console.WriteLine("start HangJob...");

            return "success start hangfire for spnodetail";
        }

        [HttpGet]
        public ActionResult GetspnoDetail(long starttime, long endtime)
        {
            var resspno = _mdata.Spdatas.AsNoTracking().AsEnumerable().Where(e => DateTimeOffset.Parse(e.Apply_time).ToUnixTimeSeconds() >= starttime && DateTimeOffset.Parse(e.Apply_time).AddDays(-1).ToUnixTimeSeconds() <= endtime);

            return Ok(resspno);

        }

        [HttpGet]
        [NotTransactional]

        public async Task<ActionResult> GetDaterangeAsync(long starttime, long endtime)
        {
            var mtoken = AccessTokenContainer.TryGetToken(superSetting.WeixinCorpId, superSetting.WeixinCorpSecret);
            string AppKey = await AccessTokenContainer.TryGetTokenAsync(workSetting.WeixinCorpId, workSetting.WeixinCorpSecret);

            DateTimeOffset startDate = DateTimeOffset.FromUnixTimeSeconds(starttime);
            DateTimeOffset endDate = DateTimeOffset.FromUnixTimeSeconds(endtime);

            DateTimeOffset currentDate = startDate;

            List<TimeInterval> timeIntervals = new();
            TimeSpan interval = TimeSpan.FromDays(30);
            while (currentDate <= endDate)
            {
                DateTimeOffset startTime = currentDate;
                DateTimeOffset endTime = currentDate + interval;

                if (endTime > endDate) // 检查endTime是否超过设定的结束日期
                {
                    endTime = endDate; // 如果超过，则将endTime设置为结束日期
                }

                timeIntervals.Add(new TimeInterval(startTime, endTime));

                currentDate += interval;
            }


            var tmplist = new List<string>();
            // 打印数组
            foreach (var item in timeIntervals)
            {

                var xx = await GetCorp_approvallistAsync(item.Start.ToUnixTimeSeconds(), item.End.ToUnixTimeSeconds());
                tmplist.AddRange(xx);
                Console.WriteLine($"item.Start:{item.Start}end:{item.End}");
            }

            var spdatas = new List<Spdata>();

            foreach (var item in tmplist)
            {

                var sprecord = await GetApproval_Detail(mtoken, item);
                var resuser = await MailListApi.GetMemberAsync(AppKey, sprecord.Apply_userid);
                var departinfo = await Getdepartmemo(AppKey, sprecord.Apply_departid);



                var tmmdata = new Spdata() {
                    Sp_no = item,
                    Sp_type = sprecord.Sp_type,
                    Apply_time = sprecord.Apply_time,
                    Apply_userid = sprecord.Apply_userid,
                    Username = resuser.name,
                    Apply_departid = sprecord.Apply_departid,
                    Departname = departinfo.department.name,
                    Sp_status = sprecord.Sp_status,
                    Start = sprecord.Start,
                    End = sprecord.End,
                    Duration = sprecord.Duration,


                };
                spdatas.Add(tmmdata);
            }


            await _mdata.AddRangeAsync(spdatas);


            var res = await _mdata.SaveChangesAsync();

            Console.WriteLine($"write record number:{res}");

            return Ok(res);
        }



        private async Task<List<string>> GetCorp_approvallistAsync(long time1, long time2)
        {

            //目前，企业微信只能支持一个月时间跨度

            var mtoken = AccessTokenContainer.TryGetToken(superSetting.WeixinCorpId, superSetting.WeixinCorpSecret);

            List<GetApprovalInfoRequest_Filter> filters = new();
            GetApprovalInfoRequest_Filter filter = new()
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

            string mhost = "https://qyapi.weixin.qq.com";
            var res = await mhost.AppendPathSegment("cgi-bin/oa/getapprovalinfo")

                .SetQueryParam("access_token", mtoken)
                .PostJsonAsync(approval)
                .ReceiveJson<Splistresult>();

            var tmpdata = res.sp_no_list;

            var tmpc = new List<string>();
            tmpc.AddRange(tmpdata);
            approval.cursor = res.next_cursor;
            while (approval.cursor != 0)
            {

                var restmp = await mhost.AppendPathSegment("cgi-bin/oa/getapprovalinfo")

                  .SetQueryParam("access_token", mtoken)
                  .PostJsonAsync(approval)
                  .ReceiveJson<Splistresult>();
                //Splistresult
                approval.cursor = restmp.next_cursor;
                tmpc.AddRange(restmp.sp_no_list);

            }

            var respsql = _mdata.Spdatas.AsNoTracking();

            var reslist = tmpc.Where(e => respsql.Where(x => x.Sp_no == e).FirstOrDefault() == null).ToList();

            return reslist;
        }



        private static async Task<Sprecord> GetApproval_Detail(string mtoken, string spno)
        {

            try
            {


                var spnox = new Sp_no(spno);

                string mhost = "https://qyapi.weixin.qq.com";
                var res = await mhost.AppendPathSegment("cgi-bin/oa/getapprovaldetail")

                    .SetQueryParam("access_token", mtoken)

                    .PostJsonAsync(spnox)
                    .ReceiveJson<Spdetail>();

                //Spdetail
                var tmpdata = res.info.apply_data.contents[0];
                //请假期种类
                var duration = tmpdata.value.vacation.attendance.date_range.new_duration;
                // DateTimeOffset.FromUnixTimeSeconds(res.info.apply_time).LocalDateTime.ToString()

                //DateTimeOffset.FromUnixTimeSeconds(tmpdata.value.vacation.attendance.date_range.new_begin).LocalDateTime.ToString(),
                Sprecord sprecord = new(
                 tmpdata.value.vacation.selector.options[0].value[0].text, DateTimeOffset.FromUnixTimeSeconds(res.info.apply_time).LocalDateTime.ToString(),

                 res.info.applyer.userid,
                 res.info.applyer.partyid,
                 res.info.sp_status switch { 1 => "审批中", 2 => "已通过", 3 => "已驳回", 4 => "已撤销", 6 => "通过后撤销", 7 => "已删除", _ => "未知" }, DateTimeOffset.FromUnixTimeSeconds(tmpdata.value.vacation.attendance.date_range.new_begin).LocalDateTime.ToString(),
                 DateTimeOffset.FromUnixTimeSeconds(tmpdata.value.vacation.attendance.date_range.new_end).LocalDateTime.ToString(),
                 TimeSpan.FromSeconds(duration).TotalDays

                );
                return sprecord;
            }
            catch (Exception ex) {
                Console.WriteLine($"spno error:{spno}");
                return null;
            }




        }
        [HttpPost]
        public async Task<ActionResult> Getsp_Detail(string spno)
        {



            var mtoken = AccessTokenContainer.TryGetToken(superSetting.WeixinCorpId, superSetting.WeixinCorpSecret);
            var spnox = new Sp_no(spno);

            string mhost = "https://qyapi.weixin.qq.com";
            var res = await mhost.AppendPathSegment("cgi-bin/oa/getapprovaldetail")

                .SetQueryParam("access_token", mtoken)

                .PostJsonAsync(spnox)
                .ReceiveJson<Spdetail>();

            return Ok(res);

            //Spdetail
            //var tmpdata = res.info.apply_data.contents[0];
            ////请假期种类
            //var duration = tmpdata.value.vacation.attendance.date_range.new_duration;
            //// DateTimeOffset.FromUnixTimeSeconds(res.info.apply_time).LocalDateTime.ToString()

            ////DateTimeOffset.FromUnixTimeSeconds(tmpdata.value.vacation.attendance.date_range.new_begin).LocalDateTime.ToString(),
            //Sprecord sprecord = new(
            // tmpdata.value.vacation.selector.options[0].value[0].text, DateTimeOffset.FromUnixTimeSeconds(res.info.apply_time).LocalDateTime.ToString(),

            // res.info.applyer.userid,
            // res.info.applyer.partyid,
            // res.info.sp_status switch { 1 => "审批中", 2 => "已通过", 3 => "已驳回", 4 => "已撤销", 6 => "通过后撤销", 7 => "已删除", _ => "未知" }, DateTimeOffset.FromUnixTimeSeconds(tmpdata.value.vacation.attendance.date_range.new_begin).LocalDateTime.ToString(),
            // DateTimeOffset.FromUnixTimeSeconds(tmpdata.value.vacation.attendance.date_range.new_end).LocalDateTime.ToString(),
            // TimeSpan.FromSeconds(duration).TotalDays

            //);
            //return sprecord;





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
        public ActionResult Getbankreceive(string Noticeid, string Userid)
        {

            var res = _mdata.Supernotices.AsNoTracking().Where(e => e.NoticeId == Noticeid && e.Approverstep >= 4).FirstOrDefault();
            var resx = _mdata.Supernoticeapprovals.AsNoTracking().AsEnumerable().Where(e => e.Noticeid == Noticeid && e.Users.Where(b => b.Userid == Userid).FirstOrDefault() != null).FirstOrDefault();
            Receivedata receivedata = new()
            {
                supernotice = res,
                Approvalstatus = 0,
            };

            if (res != null && resx == null)
            {
                return Ok(receivedata);
            }
            else
            {
                receivedata.Approvalstatus = 1;
                return Ok(receivedata);
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
        public async Task<ActionResult> Approval_sendmsgAsync(Approvalmsg approvalmsg)
        {
            var mtoken = AccessTokenContainer.TryGetToken(superSetting.WeixinCorpId, superSetting.WeixinCorpSecret);
            var ressuper = _mdata.Supernotices.Where(e => e.NoticeId == approvalmsg.Noticeid && e.Approverstep >= 4).FirstOrDefault();
            var parentId = await GetuserLeaderAsync(approvalmsg.Userid);
            var userinfo = await MailListApi.GetMemberAsync(mtoken, approvalmsg.Userid);
            string Username = userinfo.name;
            var departlevel = await GetbanklevelAsync(ressuper.Orderdata.Userid);
            int Mapprovalstep;
            if (parentId == "error")
            {
                if (departlevel == "7")
                {
                    Mapprovalstep = 10;
                }
                else
                {
                    Mapprovalstep = 9;
                }
                _ = await MassApi.SendTextCardAsync(mtoken, superSetting.WeixinCorpAgentId, "督办单全流程审核已完成", $"督办单编号:{approvalmsg.Noticeid}", $"https://rcbcybank.com/#/Super", "详情", ressuper.Noticedata.Noticebankuserid);
            }
            else
            {
                _ = await MassApi.SendTextCardAsync(mtoken, superSetting.WeixinCorpAgentId, "支行返回督办单审核已通过", $"上一级审批人:{approvalmsg.Userid}-{Username}\n 督办单编号:{approvalmsg.Noticeid}", $"https://rcbcybank.com/#/Noticereceive?noticeid={approvalmsg.Noticeid}", "审批", parentId);
                Mapprovalstep = ressuper.Approverstep + 1;
            }

            var resapproval = _mdata.Supernoticeapprovals.AsEnumerable().Where(e => e.Noticeid == approvalmsg.Noticeid).FirstOrDefault();
            List<Approval_userid> Musers = new()
                    {
                       new Approval_userid() {
                          Userid = approvalmsg.Userid,
                          Approverstep = Mapprovalstep,
                          Dt = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds()
                       }
                    };

            Supernoticeapproval approvalinfo = new()
            {
                Noticeid = approvalmsg.Noticeid,
                Users = Musers

            };


            if (resapproval == null)
            {

                var resnotice1 = _mdata.Supernotices.Where(e => e.NoticeId == approvalmsg.Noticeid).FirstOrDefault();
                resnotice1.Approverstep = Mapprovalstep;
                _mdata.Supernotices.Attach(resnotice1);
                _mdata.Entry(resnotice1).State = EntityState.Modified;
                _mdata.Supernoticeapprovals.Add(approvalinfo);
                _ = await _mdata.SaveChangesAsync();
            }
            else
            {

                if (resapproval.Users.Where(e => e.Userid == approvalmsg.Userid).FirstOrDefault() == null)
                {
                    resapproval.Users.Add(approvalinfo.Users[0]);
                    _mdata.Supernoticeapprovals.Attach(resapproval);
                    _mdata.Entry(resapproval).State = EntityState.Modified;

                    var resnotice2 = _mdata.Supernotices.Where(e => e.NoticeId == approvalmsg.Noticeid).FirstOrDefault();
                    resnotice2.Approverstep = Mapprovalstep;
                    _mdata.Supernotices.Attach(resnotice2);
                    _mdata.Entry(resnotice2).State = EntityState.Modified;
                }


                _ = await _mdata.SaveChangesAsync();

            }


            return Ok("success");
        }

        [HttpGet]

        public ActionResult Getorder_list(string departid)
        {

            var res = _mdata.Supernotices.Where(e => e.Orderdata.Departid == departid);
            if (res.Any())
            {
                return Ok(res);

            }
            else
            {

                return Ok("error");
            }
        }
        [HttpGet]

        public ActionResult Getorder_alllist()
        {

            var res = _mdata.Supernotices;
            if (res.Any())
            {
                return Ok(res);

            }
            else
            {

                return Ok("error");
            }
        }
        private static string GetunixtimeStr(long timestr)
        {

            if (timestr == 0)
            {
                return "null";
            }
            else
            {
                var res = DateTimeOffset.FromUnixTimeMilliseconds(timestr).LocalDateTime;
                return res.ToString();
            }

        }
        [HttpGet]
        [NotTransactional]
        public ActionResult GetFirstExcel()
        {

            var res = _mdata.Firsts.AsNoTracking();

            var mapper = new Mapper();
            mapper.Map<First>("编号", e => e.Transid)
            .Format<First>("yyyy-mm-dd hh:mm", e => e.Transtime)
            .Ignore<First>(e => e.Copypersion)
             .Ignore<First>(e => e.Attachs)
            .Map<First>("业务发起时间", e => e.Transtime, null, (c, t) =>
            {
                First first = t as First;
                c.CurrentValue = GetunixtimeStr(first.Transtime);
                return true;
            })
            //.Format<First>("yyyy-mm-dd hh:mm",e=>e.Transtime)
            .Map<First>("完成状态", e => e.Trans_Status, null, (c, t) =>
            {
                First first = t as First;
                c.CurrentValue = first.Trans_Status.Isover switch { true => "已完成", false => "未完成" };
                Console.WriteLine("success");
                return true;
            })
             .Map<First>("客户信息", e => e.Cust, null, (c, t) =>
             {
                 First first = t as First;
                 c.CurrentValue = first.Cust.Name + " 诉求:" + first.Cust.Content;
                 return true;
             })
             .Map<First>("流转详情", e => e.Relay, null, (c, t) =>
             {
                 First first = t as First;
                 string infotxt = "";
                 int i = 0;
                 foreach (var item in first.Relay)
                 {
                     i++;
                     infotxt += $"{i}--部门:" + item.Departname + " 处理意见:" + item.Content + " 时间:" + GetunixtimeStr(item.Transtime) + "\r\n";
                 }
                 c.CurrentValue = infotxt;
                 return true;
             });


            MemoryStream stream = new();

            mapper.Save(stream, res, "sheet1", overwrite: true);

            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Firstinfo.xlsx");
        }

        [HttpGet]
        [NotTransactional]
        public ActionResult GetHolidayExcel(long starttime, long endtime) {

            var resspno = _mdata.Spdatas.AsNoTracking().AsEnumerable().Where(e => DateTimeOffset.Parse(e.Apply_time).ToUnixTimeSeconds() >= starttime && DateTimeOffset.Parse(e.Apply_time).ToUnixTimeSeconds() <= endtime);
            var mapper = new Mapper();
            MemoryStream stream = new();

            mapper.Map<Spdata>("审批编号", e => e.Sp_no)
                  .Map<Spdata>("机构", e => e.Departname)
                  .Map<Spdata>("姓名", e => e.Username)
                  .Map<Spdata>("类型", e => e.Sp_type)
                  .Map<Spdata>("申请时间", e => e.Apply_time)
                  .Map<Spdata>("时长(天数)", e => e.Duration)
                  .Map<Spdata>("开始时间", e => e.Start)
                  .Map<Spdata>("结束时间", e => e.End)
                  .Map<Spdata>("审批状态", e => e.Sp_status)
                  .Map<Spdata>("申请人编号", e => e.Apply_userid)
                  .Map<Spdata>("机构号", e => e.Apply_departid);

            mapper.Save(stream, resspno, "sheet1", overwrite: true);

            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Holidayinfo.xlsx");

        }

        [HttpGet]
        [NotTransactional]
        public ActionResult GetLimitExcel()
        {
            var mapper = new Mapper();
            MemoryStream stream = new();
            var resexcel = _mdata.Limits.AsNoTracking().Select(e => new LimitExcel
            {
                Departname = e.Departname,
                Relay_departname = e.Relay_departname,
                Relay_leadername = e.Relay_leadername,
                Leadername = e.Leadername,
                Conttype = e.Conttype,
                Day = e.Day,
                Detail = e.Detail,
                Info = e.Info,
                Isover = e.Isover,
                Limitid = e.Limitid,
                Relay_approval_time = e.Relay_approval_time,
                Transtime = e.Transtime,
                Username = e.Username
            });
            mapper.Map<LimitExcel>("是否办结", e => e.Isover, null, (c, t) =>
            {
                LimitExcel limit = t as LimitExcel;
                c.CurrentValue = limit.Isover switch { true => "已办强", false => "未办结" };

                return true;
            });
            mapper.Map<LimitExcel>("发起时间", e => e.Transtime, null, (c, t) =>
            {
                LimitExcel limit = t as LimitExcel;
                c.CurrentValue = GetunixtimeStr(limit.Transtime);

                return true;
            });
            mapper.Map<LimitExcel>("办结时间", e => e.Relay_approval_time, null, (c, t) =>
            {
                LimitExcel limit = t as LimitExcel;
                if (limit.Relay_approval_time == 0)
                {
                    c.CurrentValue = "";
                }
                else
                {
                    c.CurrentValue = GetunixtimeStr(limit.Relay_approval_time);
                }
                return true;
            });
            mapper.Save(stream, resexcel, "sheet1", overwrite: true);
            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Limitinfo.xlsx");
        }

        [HttpGet]

        public ActionResult GetSuperexcel(string departid)
        {
            IEnumerable<Supernotice> res;
            if (departid == "0")
            {
                res = _mdata.Supernotices.AsEnumerable();
            }
            else
            {
                res = _mdata.Supernotices.Where(e => e.Orderdata.Departid == departid).AsEnumerable();

            }
            if (res.Any())
            {
                var mapper = new Mapper();
                MemoryStream stream = new();

                var result = res.Select(e => new SuperExcel
                {
                    NoticeId = e.NoticeId,
                    Notice_departname = e.Noticedata.Bankdepartname,
                    Orderdata_departname = e.Orderdata.Departname,
                    Orderdata_task = e.Orderdata.Task,
                    Orderdata_taskinfo = e.Orderdata.Taskinfo,
                    Noticedata_applyinfo = e.Noticedata.Applyinfo,
                    Orderdata_name = e.Orderdata.Username,
                    Ordertime = DateTimeOffset.FromUnixTimeMilliseconds(e.Orderdata.Ordertime).LocalDateTime.ToString(),
                    Noticetime = DateTimeOffset.FromUnixTimeMilliseconds(e.Noticedata.Checkdate).LocalDateTime.ToString(),
                    Approval_status = Dic_approval[e.Approverstep]
                });
                mapper.Save(stream, result, "sheet1", overwrite: true);
                //application/vnd.ms-excel 
                //application/vnd.openxmlformats-officedocument.spreadsheetml.sheet
                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "SuperList.xlsx");
            }
            else
            {
                return Ok("error");
            }
        }
        [HttpGet]
        [NotTransactional]
        public ActionResult Getpersonholiday(string userid) {
            var personinfo = _mdata.Spdatas.AsNoTracking().Where(e => e.Apply_userid == userid);
            return Ok(personinfo);

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
