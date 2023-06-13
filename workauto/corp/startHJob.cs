
using Flurl;
using Flurl.Http;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Mysqldb;
using Mysqldb.model;
using Senparc.Weixin.Entities;
using Senparc.Weixin.Work.AdvancedAPIs;
using Senparc.Weixin.Work.AdvancedAPIs.OA.OAJson;
using Senparc.Weixin.Work.Containers;
using Zack.Commons;
using Config = Senparc.Weixin.Config;
namespace workauto.corp
{
    public  class StartHJob 
    {

        private readonly Wxusers _mdata;
        public readonly ISenparcWeixinSettingForWork superSetting = Config.SenparcWeixinSetting.Items["supernotice"];
        public readonly ISenparcWeixinSettingForWork workSetting = Config.SenparcWeixinSetting.WorkSetting;

        public StartHJob(Wxusers mdata)
        {
            _mdata = mdata;
           
        }

        public string Starthangfirejob() {

            RecurringJobOptions jobOptions = new()
            {
                TimeZone = TimeZoneInfo.Local
            };

            DateTimeOffset myoffset = DateTimeOffset.Parse("2022-07-01T00:00");

            var starttime = myoffset.ToUnixTimeSeconds();
           // var endtime = new DateTimeOffset(DateTime.UtcNow.AddDays(1)).ToUnixTimeSeconds();
            // var endtime = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();


            RecurringJob.AddOrUpdate("SaveSpnodetail", () => GetDaterangeAsync(starttime), Cron.Daily(1, 0), jobOptions);

            //var jobid= BackgroundJob.Schedule(()=>GetDaterangeAsync(starttime, endtime), TimeSpan.FromSeconds(5));

            Console.WriteLine("start HangJob...");

            return "success start hangfire for spnodetail";
        }

        public async Task<string> GetDaterangeAsync(long starttime)
        {
            var mtoken = AccessTokenContainer.TryGetToken(superSetting.WeixinCorpId, superSetting.WeixinCorpSecret);
            string AppKey = await AccessTokenContainer.TryGetTokenAsync(workSetting.WeixinCorpId, workSetting.WeixinCorpSecret);
            var endtime = new DateTimeOffset(DateTime.UtcNow.AddDays(1)).ToUnixTimeSeconds();
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



                var tmmdata = new Spdata()
                {
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

            return "success";
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
            catch (Exception ex)
            {
                Console.WriteLine($"spno error:{spno}");
                return null;
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
        class AutoInitcs : IModuleInitializer
        {
            public void Initialize(IServiceCollection services)
            {
                services.AddScoped<StartHJob>();
            }
        }
    }

}
