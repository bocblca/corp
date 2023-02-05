using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Mysqldb;
using Newtonsoft.Json;
using Npoi.Mapper;
using Senparc.CO2NET.Extensions;
using Senparc.Weixin;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.AdvancedAPIs.TemplateMessage;
using Senparc.Weixin.WxOpen.AdvancedAPIs.Sns;
using Senparc.Weixin.WxOpen.AdvancedAPIs.WxApp;
using Senparc.Weixin.WxOpen.Helpers;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Processing;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using workapi.creditloan;
using workapi.JWT;
using workapi.Kpi;

namespace workapi.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class WxController : ControllerBase
    {
        public static readonly string mToken = Config.SenparcWeixinSetting.Items["softrcb"].WxOpenToken;
        public static readonly string EncodingAESKey = Config.SenparcWeixinSetting.Items["softrcb"].WxOpenEncodingAESKey;
        public static readonly string WxOpenAppId = Config.SenparcWeixinSetting.Items["softrcb"].WxOpenAppId;
        public static readonly string WxOpenAppSecret = Config.SenparcWeixinSetting.Items["softrcb"].WxOpenAppSecret;

        public static readonly string mToken2 = Config.SenparcWeixinSetting.Items["softrcb2"].WxOpenToken;
        public static readonly string EncodingAESKey2 = Config.SenparcWeixinSetting.Items["softrcb2"].WxOpenEncodingAESKey;
        public static readonly string WxOpenAppId2 = Config.SenparcWeixinSetting.Items["softrcb2"].WxOpenAppId;
        public static readonly string WxOpenAppSecret2 = Config.SenparcWeixinSetting.Items["softrcb2"].WxOpenAppSecret;

        public static readonly string mToken3 = Config.SenparcWeixinSetting.Items["softbc"].WxOpenToken;
        public static readonly string EncodingAESKey3 = Config.SenparcWeixinSetting.Items["softbc"].WxOpenEncodingAESKey;
        public static readonly string WxOpenAppId3 = Config.SenparcWeixinSetting.Items["softbc"].WxOpenAppId;
        public static readonly string WxOpenAppSecret3 = Config.SenparcWeixinSetting.Items["softbc"].WxOpenAppSecret;

        public static readonly string mToken4 = Config.SenparcWeixinSetting.Items["softloan"].WxOpenToken;
        public static readonly string EncodingAESKey4 = Config.SenparcWeixinSetting.Items["softloan"].WxOpenEncodingAESKey;
        public static readonly string WxOpenAppId4 = Config.SenparcWeixinSetting.Items["softloan"].WxOpenAppId;
        public static readonly string WxOpenAppSecret4 = Config.SenparcWeixinSetting.Items["softloan"].WxOpenAppSecret;

        // GET: api/<WxController>

        private readonly Wxusers _mdata;
        private readonly UserManager<User> userManager;
        private readonly RoleManager<myRole> roleManager;


        private Wxusers wxusers;

        public WxController(Wxusers mdata, UserManager<User> userManager, RoleManager<myRole> roleManager, Wxusers wxusers)
        {
            _mdata = mdata;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.wxusers = wxusers;
        }



        [HttpGet]
        public async Task DoWarn(string procid)
        {

            var res = _mdata.hangjobs.Where(e => e.procid == procid).FirstOrDefault();
            if (res != null)
            {
                res.date_warn = DateTime.Now;
                res.iswarn = true;
                _ = await _mdata.SaveChangesAsync();

                var resn = _mdata.loanusers.Where(e => e.Ace >= 4 && e.unionid != null).ToList(); //发送提示给行领导
                foreach (var item in resn)
                {
                    string tmpunionid = _mdata.loanusers.AsNoTracking().Where(e => e.straff == item.straff).FirstOrDefault().unionid;
                    string tmpopenid = _mdata.wxunionids.AsNoTracking().Where(e => e.WxUnionid == tmpunionid).FirstOrDefault().Wxopenid;
                    await sendwarnmsg(procid, tmpopenid);
                }
            }

        }


        private async Task sendwarnmsg(string procid, string mopenid)
        {
            var myminiProgram = new TemplateModel_MiniProgram()
            {
                appid = WxOpenAppId4,
                pagepath = "pages/index/index"
            };
            var mdata = new TempVetting("贷款审批超时2小时提醒", procid, DateTime.Now.ToString(), "待总行审批", "该笔贷款审批已超时,详情进入小程序查看");
            _ = await TemplateApi.SendTemplateMessageAsync(Config.SenparcWeixinSetting.Items["second"].WeixinAppId, mopenid, mdata, myminiProgram);//使用异步方法
        }


        [HttpGet]
        public async Task DelWarn(string procid)
        {
            var res = _mdata.hangjobs.Where(e => e.procid == procid).FirstOrDefault();
            if (res != null)
            {
                res.isdel = true;
                res.date_end = DateTime.Now;
                _ = await _mdata.SaveChangesAsync();
                BackgroundJob.Delete(res.jobid); //删除警告进程
            }
        }
        [HttpGet]
        public async Task<ActionResult> ChangeFlagAsync(string procid, int flag, string? message = "")
        {
            var res = _mdata.loanProcesses.Where(e => e.ProcID == procid).FirstOrDefault();
            if (res == null)
            {
                return BadRequest();
            }
            else
            {
                res.flag = flag;
                switch (flag)
                {
                    case 4:   //提交总行审批
                        res.date41 = DateTime.Now;
                        res.message = null;
                        var jobid = BackgroundJob.Schedule(() => DoWarn(procid), TimeSpan.FromHours(2));  //超时通知hangfire进程推送
                        //写入datajob表
                        var loanjob = new hangjob
                        {
                            procid = procid,
                            jobid = jobid,
                            date_start = DateTime.Now,
                            iswarn = false,
                            Ace = 10,
                            isdel = false
                        };
                        _mdata.hangjobs.Add(loanjob);
                        _ = await _mdata.SaveChangesAsync();
                        break;
                    case 5:  //提交总行主管审批
                        res.date42 = DateTime.Now;
                        res.date51 = DateTime.Now;
                        _ = await _mdata.SaveChangesAsync();
                        break;
                    case 6: //总行审批完成
                        res.date52 = DateTime.Now;
                        _ = await _mdata.SaveChangesAsync();
                        await DelWarn(procid);  //审批流转,删除hangfire进程
                        break;
                    case 2:  //提交支行行长审批
                        res.date21 = DateTime.Now;
                        _ = await _mdata.SaveChangesAsync();
                        break;
                    case 3:  //提交分行长审批
                        res.date22 = DateTime.Now;
                        res.date31 = DateTime.Now;
                        _ = await _mdata.SaveChangesAsync();
                        break;
                    case 7:  //退回审批，把所有审批时间置空
                        res.date41 = null;
                        res.date42 = null;
                        res.date51 = null;
                        res.date52 = null;
                        res.message = message;
                        _ = await _mdata.SaveChangesAsync();
                        await DelWarn(procid);  //审批流转,删除hangfire进程
                        break;
                    case 9:   //审批完成
                        if (res.date22 == null)  //如果date22为空，说明无需要分行行长审批
                        {
                            res.date22 = DateTime.Now;
                        }
                        else
                        {
                            res.date32 = DateTime.Now; //如果date22不为空，说明需要分行行长审批
                        }
                        _ = await _mdata.SaveChangesAsync();
                        break;
                    default:

                        break;
                }

                return Ok("success");
            }
        }
        [HttpGet]
        public ActionResult GetContract(string number)
        {
            var res = _mdata.contracts.Where(e => e.number == number).FirstOrDefault();
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
        public ActionResult GetLoans(string number)
        {
            var res = _mdata.Loans.Where(e => e.LoanID == number).FirstOrDefault();
            if (res == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(res);
            }
        }
        private string Getbankname(string bankid)
        {
            var res = _mdata.banks.Where(e => e.bankID == bankid).FirstOrDefault();
            if (res == null)
            {
                return "error";
            }
            else
            {
                return res.bankName;
            }
        }
        [HttpGet]
        public ActionResult Getloanprocess(string procid)
        {
            var res = _mdata.loanProcesses.AsNoTracking().Where(e => e.ProcID == procid).FirstOrDefault();
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
        public ActionResult Getovering()
        {
            var resm = _mdata.hangjobs.AsNoTracking().Where(e => e.iswarn == true && e.isdel == false).ToList();
            if (resm == null)
            {
                return Ok("error");
            }
            else
            {
                return Ok(resm);
            }

        }
        [HttpGet]
        public ActionResult Getovered()
        {
            var resm = _mdata.hangjobs.AsNoTracking().Where(e => e.iswarn == true && e.isdel == true).ToList();
            if (resm == null)
            {
                return Ok("error");
            }
            else
            {
                return Ok(resm);
            }


        }


        [HttpPost]
        public ActionResult Getprocess(Loanuser loanuser)
        {
            if (loanuser.Ace <= 3)
            {

                var resm = _mdata.loanProcesses.AsNoTracking().Where(e => (e.bankid == loanuser.bankid || e.parentid == loanuser.bankid) && e.flag != 9 && e.flag != 10).ToList();
                return Ok(resm);
            }
            else
            {
                var resn = _mdata.loanProcesses.AsNoTracking().Where(e => e.flag != 9 && e.flag != 10).ToList();
                return Ok(resn);
            }


        }
        //ip 地址验证 



        [HttpPost]
        public ActionResult Getallprocess(Loanuser loanuser)
        {
            if (loanuser.Ace >= 4)
            {
                var resn = _mdata.loanProcesses.AsNoTracking().Where(e => e.flag == 9).ToList();
                return Ok(resn);
            }
            else
            {
                var resm = _mdata.loanProcesses.AsNoTracking().Where(e => (e.bankid == loanuser.bankid || e.parentid == loanuser.bankid) && e.flag == 9).ToList();
                return Ok(resm);
            }

        }
        [HttpPost]
        //保存授信合同信息到数据库表contract
        public async Task<IActionResult> SaveContract(Contract contract)
        {
            var res = _mdata.contracts.AsNoTracking().Where(e => e.number == contract.number).FirstOrDefault();
            if (res == null)
            {
                _mdata.contracts.Add(contract);
                var resn = await _mdata.SaveChangesAsync();
                return Ok("success");

            }
            else
            {
                return BadRequest();
            }
        }
        [HttpPost]
        //保存授信合同信息到数据库表contract
        public async Task<IActionResult> Saveloan(Loans loans)
        {
            var res = _mdata.Loans.AsNoTracking().Where(e => e.LoanID == loans.LoanID).FirstOrDefault();
            if (res == null)
            {
                _mdata.Loans.Add(loans);
                var resn = await _mdata.SaveChangesAsync();
                return Ok("success");

            }
            else
            {
                return BadRequest();
            }
        }
        [HttpGet]
        public ActionResult getfbank(string bankid)
        {
            var res = _mdata.banks.Where(e => e.bankID == bankid).FirstOrDefault();
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
        //保存业务发起审批信息到数据库表loanprocess
        public async Task<ActionResult> Sendvetting_contract(string straff, string procid)
        {

            string bankid = _mdata.loanusers.AsNoTracking().Where(e => e.straff == straff).FirstOrDefault().bankid;
            string parentid = _mdata.banks.AsNoTracking().Where(e => e.bankID == bankid).FirstOrDefault().parentID;
            string straff2 = _mdata.loanusers.AsNoTracking().Where(e => e.bankid == bankid && e.Ace == 2).FirstOrDefault().straff;
            //straff3是分行行长，需要从bank表里通过分支行父子关系确认
            string parentbankid = _mdata.banks.Where(e => e.bankID == bankid).FirstOrDefault().parentID;
            string straff3 = _mdata.loanusers.AsNoTracking().Where(e => e.bankid == parentbankid && e.Ace == 3).FirstOrDefault().straff;
            string straff4 = _mdata.loanusers.AsNoTracking().Where(e => e.Ace == 4 && e.IsValid == true).OrderBy(x => EF.Functions.Random()).FirstOrDefault().straff;
            string straff5 = _mdata.loanusers.AsNoTracking().Where(e => e.Ace == 5).FirstOrDefault().straff;
            LoanProcess loanProcess = new();
            loanProcess.ProcID = procid;
            loanProcess.straff1 = straff;
            loanProcess.straff2 = straff2;
            loanProcess.straff3 = straff3;
            loanProcess.straff4 = straff4;
            loanProcess.straff5 = straff5;
            loanProcess.date11 = DateTime.Now;
            loanProcess.date12 = DateTime.Now;
            loanProcess.date41 = DateTime.Now;
            loanProcess.flag = 4;
            loanProcess.bankid = bankid;
            loanProcess.parentid = parentid;
            loanProcess.bankname = Getbankname(bankid);
            loanProcess.loantype = 1;

            _mdata.loanProcesses.Add(loanProcess);
            _ = await _mdata.SaveChangesAsync();

            var jobid = BackgroundJob.Schedule(() => DoWarn(procid), TimeSpan.FromHours(3));  //超时通知hangfire进程推送
            //写入datajob表
            var loanjob = new hangjob
            {
                procid = procid,
                jobid = jobid,
                date_start = DateTime.Now,
                iswarn = false,
                Ace = 10,
                isdel = false
            };
            _mdata.hangjobs.Add(loanjob);
            _ = _mdata.SaveChangesAsync();
            return Ok(straff4);
        }
        [HttpGet]
        //保存业务发起审批信息到数据库表loanprocess
        public async Task<ActionResult> Sendvetting_loan(string straff, string procid)
        {

            string bankid = _mdata.loanusers.AsNoTracking().Where(e => e.straff == straff).FirstOrDefault().bankid;
            string straff2 = _mdata.loanusers.AsNoTracking().Where(e => e.bankid == bankid && e.Ace == 2).FirstOrDefault().straff;
            string parentbankid = _mdata.banks.Where(e => e.bankID == bankid).FirstOrDefault().parentID;
            string straff3 = _mdata.loanusers.AsNoTracking().Where(e => e.bankid == parentbankid && e.Ace == 3).FirstOrDefault().straff;
            string straff4 = _mdata.loanusers.AsNoTracking().Where(e => e.Ace == 4 && e.IsValid == true).FirstOrDefault().straff;
            string straff5 = _mdata.loanusers.AsNoTracking().Where(e => e.Ace == 5).FirstOrDefault().straff;
            LoanProcess loanProcess = new();
            loanProcess.ProcID = procid;
            loanProcess.straff1 = straff;
            loanProcess.straff2 = straff2;
            loanProcess.straff3 = straff3;
            loanProcess.straff4 = straff4;
            loanProcess.straff5 = straff5;
            loanProcess.date11 = DateTime.Now;
            loanProcess.date12 = DateTime.Now;
            loanProcess.date41 = DateTime.Now;
            loanProcess.flag = 4;
            loanProcess.bankid = bankid;
            loanProcess.bankname = Getbankname(bankid);
            loanProcess.loantype = 2;
            _mdata.loanProcesses.Add(loanProcess);
            _ = await _mdata.SaveChangesAsync();
            var jobid = BackgroundJob.Schedule(() => DoWarn(procid), TimeSpan.FromHours(3));  //超时通知hangfire进程推送
            //写入datajob表
            var loanjob = new hangjob
            {
                procid = procid,
                jobid = jobid,
                date_start = DateTime.Now,
                iswarn = false,
                Ace = 10,
                isdel = false
            };
            _mdata.hangjobs.Add(loanjob);
            _ = _mdata.SaveChangesAsync();
            return Ok(straff4);
        }
        [HttpGet]
        public async Task<ActionResult> Sendmsgtostraff_superAsync(string procid)
        {
            string straff = _mdata.loanProcesses.Where(e => e.ProcID == procid).FirstOrDefault().straff5;
            string unionid = _mdata.loanusers.Where(e => e.straff == straff).FirstOrDefault().unionid;
            string mopenid = _mdata.wxunionids.Where(e => e.WxUnionid == unionid).FirstOrDefault().Wxopenid;

            var myminiProgram = new TemplateModel_MiniProgram()
            {
                appid = WxOpenAppId4,
                pagepath = "pages/index/index"
            };
            var mdata = new TempVetting("您有一笔贷款审批需要处理", procid, DateTime.Now.ToString(), "待总行主管审批", "请于2小时内处理,详情进入小程序查看");
            var mres = await TemplateApi.SendTemplateMessageAsync(Config.SenparcWeixinSetting.Items["second"].WeixinAppId, mopenid, mdata, myminiProgram);//使用异步方法
            return Ok("success");
        }
        [HttpGet]
        public async Task<ActionResult> Sendmsgtostraff_agreeAsync(string procid)
        {
            string straff = _mdata.loanProcesses.Where(e => e.ProcID == procid).FirstOrDefault().straff1;
            string unionid = _mdata.loanusers.Where(e => e.straff == straff).FirstOrDefault().unionid;
            string mopenid = _mdata.wxunionids.Where(e => e.WxUnionid == unionid).FirstOrDefault().Wxopenid;

            var myminiProgram = new TemplateModel_MiniProgram()
            {
                appid = WxOpenAppId4,
                pagepath = "pages/index/index"
            };
            var mdata = new TempVetting("您有一笔贷款审批需要处理", procid, DateTime.Now.ToString(), "总行审批已通过", "请尽快放款,详情进入小程序查看");
            var mres = await TemplateApi.SendTemplateMessageAsync(Config.SenparcWeixinSetting.Items["second"].WeixinAppId, mopenid, mdata, myminiProgram);//使用异步方法
            return Ok("success");
        }
        [HttpGet]
        public async Task<ActionResult> Sendmsgtostraff_refuseAsync(string procid)
        {
            string straff = _mdata.loanProcesses.Where(e => e.ProcID == procid).FirstOrDefault().straff1;
            string unionid = _mdata.loanusers.Where(e => e.straff == straff).FirstOrDefault().unionid;
            string mopenid = _mdata.wxunionids.Where(e => e.WxUnionid == unionid).FirstOrDefault().Wxopenid;

            var myminiProgram = new TemplateModel_MiniProgram()
            {
                appid = WxOpenAppId4,
                pagepath = "pages/index/index"
            };
            var mdata = new TempVetting("您有一笔贷款审批需要处理", procid, DateTime.Now.ToString(), "总行审批未通过", "请尽快处理,详情进入小程序查看");
            var mres = await TemplateApi.SendTemplateMessageAsync(Config.SenparcWeixinSetting.Items["second"].WeixinAppId, mopenid, mdata, myminiProgram);//使用异步方法
            return Ok("success");
        }
        [HttpGet]
        public async Task<ActionResult> Sendmsgtostraff_subupAsync(string procid)
        {
            string straff = _mdata.loanProcesses.Where(e => e.ProcID == procid).FirstOrDefault().straff3;
            string unionid = _mdata.loanusers.Where(e => e.straff == straff).FirstOrDefault().unionid;
            string mopenid = _mdata.wxunionids.Where(e => e.WxUnionid == unionid).FirstOrDefault().Wxopenid;


            var myminiProgram = new TemplateModel_MiniProgram()
            {
                appid = WxOpenAppId4,
                pagepath = "pages/index/index"
            };
            var mdata = new TempVetting("您有一笔贷款审批需要处理", procid, DateTime.Now.ToString(), "待支行审批", "总行已通过,详情进入小程序查看");
            var mres = await TemplateApi.SendTemplateMessageAsync(Config.SenparcWeixinSetting.Items["second"].WeixinAppId, mopenid, mdata, myminiProgram);//使用异步方法
                                                                                                                                                          // Console.WriteLine(mres);


            return Ok("success");
        }
        [HttpGet]
        public async Task<ActionResult> Sendmsgtostraff_branchagreeAsync(string procid)
        {
            string straff = _mdata.loanProcesses.Where(e => e.ProcID == procid).FirstOrDefault().straff1;
            string unionid = _mdata.loanusers.Where(e => e.straff == straff).FirstOrDefault().unionid;
            string mopenid = _mdata.wxunionids.Where(e => e.WxUnionid == unionid).FirstOrDefault().Wxopenid;


            var myminiProgram = new TemplateModel_MiniProgram()
            {
                appid = WxOpenAppId4,
                pagepath = "pages/index/index"
            };
            var mdata = new TempVetting("您有一笔贷款审批需要处理", procid, DateTime.Now.ToString(), "最终审批通过,业务完成", "分行已通过,详情进入小程序查看");
            var mres = await TemplateApi.SendTemplateMessageAsync(Config.SenparcWeixinSetting.Items["second"].WeixinAppId, mopenid, mdata, myminiProgram);//使用异步方法
                                                                                                                                                          // Console.WriteLine(mres);


            return Ok("success");
        }
        [HttpGet]
        public async Task<ActionResult> Sendmsgtostraff_subnextAsync(string procid)
        {
            string straff = _mdata.loanProcesses.Where(e => e.ProcID == procid).FirstOrDefault().straff3;
            string unionid = _mdata.loanusers.Where(e => e.straff == straff).FirstOrDefault().unionid;
            string mopenid = _mdata.wxunionids.Where(e => e.WxUnionid == unionid).FirstOrDefault().Wxopenid;


            var myminiProgram = new TemplateModel_MiniProgram()
            {
                appid = WxOpenAppId4,
                pagepath = "pages/index/index"
            };
            var mdata = new TempVetting("您有一笔贷款审批需要处理", procid, DateTime.Now.ToString(), "待分行审批", "支行审批已通过,详情进入小程序查看");
            var mres = await TemplateApi.SendTemplateMessageAsync(Config.SenparcWeixinSetting.Items["second"].WeixinAppId, mopenid, mdata, myminiProgram);//使用异步方法
                                                                                                                                                          // Console.WriteLine(mres);


            return Ok("success");
        }
        [HttpGet]
        public async Task<ActionResult> Sendmsgtostraff_subagreeAsync(string procid)
        {
            string straff = _mdata.loanProcesses.Where(e => e.ProcID == procid).FirstOrDefault().straff1;
            string unionid = _mdata.loanusers.Where(e => e.straff == straff).FirstOrDefault().unionid;
            string mopenid = _mdata.wxunionids.Where(e => e.WxUnionid == unionid).FirstOrDefault().Wxopenid;


            var myminiProgram = new TemplateModel_MiniProgram()
            {
                appid = WxOpenAppId4,
                pagepath = "pages/index/index"
            };
            var mdata = new TempVetting("您有一笔贷款审批需要处理", procid, DateTime.Now.ToString(), "最终审批通过,业务完成", "支行审批已通过,详情进入小程序查看");
            var mres = await TemplateApi.SendTemplateMessageAsync(Config.SenparcWeixinSetting.Items["second"].WeixinAppId, mopenid, mdata, myminiProgram);//使用异步方法
                                                                                                                                                          // Console.WriteLine(mres);


            return Ok("success");
        }
        [HttpGet]
        public async Task<ActionResult> Sendmsgtostraff_headupAsync(string procid)
        {
            string straff = _mdata.loanProcesses.Where(e => e.ProcID == procid).FirstOrDefault().straff3;
            string unionid = _mdata.loanusers.Where(e => e.straff == straff).FirstOrDefault().unionid;
            string mopenid = _mdata.wxunionids.Where(e => e.WxUnionid == unionid).FirstOrDefault().Wxopenid;


            var myminiProgram = new TemplateModel_MiniProgram()
            {
                appid = WxOpenAppId4,
                pagepath = "pages/index/index"
            };
            var mdata = new TempVetting("您有一笔贷款审批需要处理", procid, DateTime.Now.ToString(), "待总行审批", "请在2个小时内处理,详情进入小程序查看");
            var mres = await TemplateApi.SendTemplateMessageAsync(Config.SenparcWeixinSetting.Items["second"].WeixinAppId, mopenid, mdata, myminiProgram);//使用异步方法
                                                                                                                                                          // Console.WriteLine(mres);


            return Ok("success");
        }

        [HttpGet]
        public async Task<ActionResult> Cancelbooking(string unionid)
        {
            var res = _mdata.bookings.Where(e => e.Unionid == unionid).FirstOrDefault();
            if (res == null)
            {
                return BadRequest();
            }
            else
            {
                res.Nums = "-1";
                var resn = await _mdata.SaveChangesAsync();
                return Ok(resn);
            }
        }

        [HttpGet]
        public ActionResult Checksubscribe(string unionid)
        {
            var res = _mdata.wxunionids.Where(e => e.WxUnionid == unionid).FirstOrDefault();
            if (res == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(res.Wxopenid);
            }
        }
        [HttpGet]
        public ActionResult Getstrafface(string straff)
        {
            var res = _mdata.loanusers.Where(e => e.straff == straff).FirstOrDefault();
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
        public ActionResult Getstraffname(string straff)
        {
            var res = _mdata.loanusers.Where(e => e.straff == straff).FirstOrDefault();
            if (res == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(res.straff_name);
            }
        }
        [HttpGet]
        public async Task<ActionResult> StraffLoginAsync(string straff, string unionid)
        {

            var res = _mdata.loanusers.Where(e => e.straff == straff).FirstOrDefault();
            if (res == null)
            {
                return BadRequest("tokenerror");
            }
            else
            {
                res.unionid = unionid;
                res.IsLogin = true;
                _ = await _mdata.SaveChangesAsync();
                return Ok(res);
            }
        }


        private async Task<string> MergeImageAsync(string straff, string backfile, string picbase64, int x, int y)
        {
            string picadd;
            if (System.IO.File.Exists("wwwroot/img" + straff + ".jpg"))
            {
                if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "RCB")
                {
                    picadd = "https://rcbcybank.com/img" + straff + ".jpg";
                }
                else
                {
                    picadd = "https://rcbcybank.com/image/img" + straff + ".jpg";
                }

                return picadd;
            }


            byte[] outputbytes = Convert.FromBase64String(picbase64);

            var imagesTemle = Image.Load(backfile, out IImageFormat format);
            var outputImg = Image.Load(outputbytes);


            //进行多图片处理
            imagesTemle.Mutate(a =>
            {
                //还是合并 
                a.DrawImage(outputImg, new Point(x, y), 1);
            });
            await imagesTemle.SaveAsJpegAsync("wwwroot/img" + straff + ".jpg");

            //strRet = imagesTemle.ToBase64String(format);
            string picaddress;
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "RCB")
            {
                picaddress = "https://rcbcybank.com/img" + straff + ".jpg";
            }
            else
            {
                picaddress = "https://rcbcybank.com/image/img" + straff + ".jpg";
            }

            return picaddress;

        }
        private async Task<string> MergeImage2Async(string straff, string backfile, string picbase64, int x, int y)
        {
            string picadd;
            if (System.IO.File.Exists("wwwroot/pic" + straff + ".jpg"))
            {

                if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "RCB")
                {
                    picadd = "https://rcbcybank.com/pic" + straff + ".jpg";
                }
                else
                {
                    picadd = "https://rcbcybank.com/image/pic" + straff + ".jpg";
                }

                return picadd;
            }
            FontCollection collection = new();
            FontFamily family = collection.Add("STZHONGS.TTF");
            //FontFamily family2 = collection.Add("STZHONGS.TTF");
            Font font = family.CreateFont(40, FontStyle.BoldItalic);
            Font font2 = family.CreateFont(80, FontStyle.BoldItalic);
            Font font3 = family.CreateFont(50, FontStyle.BoldItalic);
            //Font font4=family2.CreateFont(80,FontStyle.Regular);
            byte[] outputbytes = Convert.FromBase64String(picbase64);

            var imagesTemle = Image.Load(backfile, out IImageFormat format);
            var outputImg = Image.Load(outputbytes);
            var rcbstraff = _mdata.rcbstraffs.Where(e => e.Straff == straff).FirstOrDefault();
            string departid = rcbstraff.BankId;
            var rcbbank = _mdata.employees.Where(e => e.DepartID == departid).FirstOrDefault();
            string bankname = rcbbank.Depart;
            string ace = rcbstraff.Ace;
            string straffname = rcbstraff.Name;


            if (straffname.Length == 2)
            {
                straffname = straffname.Substring(0, 1) + " " + straffname.Substring(1, 1);
            }
            string phone = rcbstraff.phone;

            if (phone.Length == 11)
            {
                phone = phone.Substring(0, 3) + " " + phone.Substring(3, 4) + " " + phone.Substring(7, 4);
            }


            //进行多图片处理
            imagesTemle.Mutate(a =>
            {
                //合并 
                a.DrawImage(outputImg, new Point(x, y), 1);
                a.DrawText(bankname, font, Color.FromRgb(51, 123, 241), new PointF(530, 350));
                a.DrawText(ace, font, Color.FromRgb(51, 123, 241), new PointF(530, 400));
                a.DrawText(straffname, font2, Color.Black, new PointF(720, 350));
                a.DrawText(phone, font3, Color.Black, new PointF(530, 460));
            });

            await imagesTemle.SaveAsJpegAsync("wwwroot/pic" + straff + ".jpg");
            string picaddress;
            //strRet = imagesTemle.ToBase64String(format);
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "RCB")
            {
                picaddress = "https://rcbcybank.com/pic" + straff + ".jpg";
            }
            else
            {
                picaddress = "https://rcbcybank.com/image/pic" + straff + ".jpg";
            }

            return picaddress;

        }

        [HttpGet]
        public async Task<ActionResult> GetmyQrCodeAsync(string straff)
        {


            var ms = new MemoryStream();
            string page = "pages/index/index";
            string scene = straff;
            //LineColor lineColor = null;
            _ = await WxAppApi
                .GetWxaCodeUnlimitAsync(WxOpenAppId3, ms, scene, page, lineColor: null, width: 200);
            ms.Position = 0;
            var imgBase64 = Convert.ToBase64String(ms.GetBuffer());
            var res = MergeImage2Async(straff, "personback.jpg", imgBase64, 150, 220);


            return Ok(res);
        }





        [HttpGet]
        public async Task<ActionResult> GetQrCodeAsync(string straff)
        {


            var ms = new MemoryStream();
            string page = "pages/index/index";
            string scene = straff;
            //LineColor lineColor = null;
            _ = await WxAppApi
                .GetWxaCodeUnlimitAsync(WxOpenAppId3, ms, scene, page, lineColor: null, width: 200);
            ms.Position = 0;
            var imgBase64 = Convert.ToBase64String(ms.GetBuffer());
            var res = MergeImageAsync(straff, "picback.jpg", imgBase64, 510, 1770);

            return Ok(res);
        }


        [HttpGet]
        public async Task<ActionResult> SetunionidAsync(string code)
        {
            var res = SnsApi.JsCode2Json(WxOpenAppId4, WxOpenAppSecret4, code);
            var user = _mdata.wxunionids.Where(e => e.WxUnionid == res.unionid).FirstOrDefault();
            if (user != null)
            {
                user.Xappid = res.openid;
            }
            else
            {
                var muser = new wxunionid
                {
                    WxUnionid = res.unionid,
                    Xappid = res.openid,

                };
                _mdata.wxunionids.Add(muser);
            }

            var tmp = await _mdata.SaveChangesAsync();
            return Ok(res.unionid);
        }

        [HttpGet]

        public async Task<ActionResult> SavekeyAsync(string wxcode)
        {
            var res = SnsApi.JsCode2Json(WxOpenAppId, WxOpenAppSecret, wxcode);
            var reskey = _mdata.wxsessions.Where(e => e.code == wxcode).FirstOrDefault();
            if (reskey != null)
            {
                reskey.sessionkey = res.session_key;
                reskey.unionid = res.unionid;
            }
            else
            {
                _mdata.Add(new Wxsession { code = wxcode, sessionkey = res.session_key, unionid = res.unionid });
            }

            _ = await _mdata.SaveChangesAsync();

            return Ok(res.session_key);

        }

        [HttpGet]
        public ActionResult Getstraffunionid(string straff)
        {
            var res = _mdata.rcbstraffs.Where(e => e.Straff == straff).AsNoTracking().FirstOrDefault();
            if (res == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(res.Unionid);
            }
        }

        [HttpGet]

        public async Task<ActionResult> UpdateunionidAsync(string code, string straff)
        {
            var res = SnsApi.JsCode2Json(WxOpenAppId3, WxOpenAppSecret3, code);
            if (res != null)
            {
                var resn = _mdata.rcbstraffs.Where(e => e.Straff == straff).FirstOrDefault();
                resn.Unionid = res.unionid;
                _ = await _mdata.SaveChangesAsync();
                return Ok("sucess");
            }
            else
            {
                return BadRequest("error");
            }


        }


        [HttpGet]
        public async Task<ActionResult> Savesession(string wxcode, string straff)
        {




            var res = SnsApi.JsCode2Json(WxOpenAppId3, WxOpenAppSecret3, wxcode);





            var reskey = _mdata.wxsessions.Where(e => e.code == wxcode).FirstOrDefault();
            if (reskey != null)
            {
                reskey.sessionkey = res.session_key;
                reskey.unionid = res.unionid;
            }
            else
            {
                _mdata.Add(new Wxsession { code = wxcode, sessionkey = res.session_key, unionid = res.unionid });
            }

            _ = await _mdata.SaveChangesAsync();




            var customer = _mdata.bookings.Where(e => e.Unionid == res.unionid).AsNoTracking().FirstOrDefault();
            if (customer != null)
            {
                _mdata.bookings.Remove(customer);
                _ = await _mdata.SaveChangesAsync();
            }

            if (straff == "1001")
            {
                straff = Getstraff();
            }

            var rcbcus = new Booking
            {
                Unionid = res.unionid,
                Wxopenid = res.openid,
                IsProccess = false,
                Stage = 0,
                Straff = straff
            };
            _mdata.bookings.Add(rcbcus);
            var result = await _mdata.SaveChangesAsync();
            return Ok(result);
        }

        [HttpPost]
        public ActionResult<string> Sendtemp(Smsinfo smsinfo)
        {

            var res = SmsCloud.Sms(smsinfo);
            return Ok(res);

        }

        private string Smssend(Smsinfo smsinfo)
        {
            var res = SmsCloud.Sms(smsinfo);
            return res;
        }
        [HttpGet]
        public ActionResult gettime()
        {

            var res = _mdata.bookings.Where(e => e.Straff == "636515").FirstOrDefault();
            //本地时间格式
            string str1 = res.FirstDate.ToString();
            string str2 = res.FirstDate.ToString();


            return Ok($"str1--{str1}str2--{str2}");


        }
        [HttpPost]
        public async Task<ActionResult> SavecustAsync(EditCust editCust)
        {

            var res = _mdata.bookings.Where(e => e.Unionid == editCust.unionid).FirstOrDefault();
            if (res == null)
            {
                return BadRequest("no customer");
            }
            else
            {
                switch (editCust.stage)
                {
                    case 1:
                        res.Stage = 1;
                        res.Seconddatae = editCust.cdate;
                        res.Information = editCust.information;
                        break;
                    case 2:
                        res.Stage = 2;
                        res.Thirddatae = editCust.cdate;
                        break;
                    case 3:
                        res.Stage = 3;
                        res.FirstDate = editCust.cdate;
                        break;
                }
                var result = await _mdata.SaveChangesAsync();
                return Ok(result);

            }
        }
        [HttpGet]
        public ActionResult Getcustomer(string unionid)
        {
            var res = _mdata.bookings.Where(e => e.Unionid == unionid).FirstOrDefault();
            if (res == null)
            {
                return BadRequest("no customer");
            }

            var mstraff = _mdata.rcbstraffs.Where(e => e.Straff == res.Straff).FirstOrDefault();

            var result = new CustomerInfo(res, mstraff);
            return Ok(result);

        }
        [HttpGet]
        public ActionResult Getstraffinfo(string straff)
        {
            var BookingsAll = _mdata.bookings;
            var MyBookings = _mdata.bookings.Where(e => e.Straff == straff && e.Nums != "-1");
            if (BookingsAll == null || MyBookings == null)
            {
                return BadRequest("no customer");
            }

            var mstraff = _mdata.rcbstraffs.Where(e => e.Straff == straff).FirstOrDefault();

            var result = new Straffinfo(BookingsAll, MyBookings, mstraff);

            return Ok(result);

        }
        [HttpGet]
        public ActionResult Getcorp(string nums)
        {
            var res = _mdata.accounts.Where(e => e.Nums == nums).FirstOrDefault();
            if (res == null)
            {
                return NotFound("card error nums");
            }
            else
            {
                return Ok(res);
            }
        }
        private string Getstraff()
        {
            var res = _mdata.rcbstraffs.Where(e => e.Ace != "行长" && e.BankId == "6107").OrderBy(c => EF.Functions.Random()).FirstOrDefault();
            return res.Straff;
        }

        private static string WechatDecrypt(string encryptedData, string encryptIv, string sessionKey)
        {
            //base64解码为字节数组
            var encryptData = Convert.FromBase64String(encryptedData);
            var key = Convert.FromBase64String(sessionKey);
            var iv = Convert.FromBase64String(encryptIv);

            //创建aes对象
            var aes = Aes.Create();

            if (aes == null)
            {
                throw new InvalidOperationException("未能获取Aes算法实例");
            }
            //设置模式为CBC
            aes.Mode = CipherMode.CBC;
            //设置Key大小
            aes.KeySize = 128;
            //设置填充
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = key;
            aes.IV = iv;

            //创建解密器
            var de = aes.CreateDecryptor(key, iv);
            //解密数据
            var decodeByteData = de.TransformFinalBlock(encryptData, 0, encryptData.Length);
            //转换为字符串
            Byte[] ThisByte = new Byte[decodeByteData.Length];
            Buffer.BlockCopy(decodeByteData, 30, ThisByte, 0, 1);
            var data = Encoding.UTF8.GetString(decodeByteData);

            return data;
        }




        [HttpPost]
        public async Task<ActionResult> bookingAsync(Lonainfo loaninfo)
        {
            //var res = SnsApi.JsCode2Json(WxOpenAppId3, WxOpenAppSecret3, loaninfo.code);
            //如果是柜员,直接注册进入


            var reslogin = _mdata.rcbstraffs.Where(e => e.Straff == loaninfo.name).FirstOrDefault();
            if (reslogin != null)
            {

                var rest = _mdata.bookings.Where(e => e.CustomerName == null).FirstOrDefault();
                if (rest != null)
                {
                    _mdata.bookings.Remove(rest);
                }

                _ = await _mdata.SaveChangesAsync();

                return Ok(reslogin.Straff);
            }
            string unionid = _mdata.wxsessions.Where(e => e.code == loaninfo.code).FirstOrDefault().unionid;

            var res = _mdata.wxsessions.Where(e => e.code == loaninfo.code).FirstOrDefault();
            var tmpdata = WechatDecrypt(loaninfo.encryptedData, loaninfo.iv, res.sessionkey);

            var phoneinfo = JsonConvert.DeserializeObject<Phoneinfo>(tmpdata);


            var rcbcus = _mdata.bookings.Where(e => e.Unionid == unionid).FirstOrDefault();
            rcbcus.CustomerName = loaninfo.name;
            rcbcus.phone = phoneinfo.phoneNumber;
            rcbcus.Nums = loaninfo.nums;
            rcbcus.FirstDate = DateTime.Now;
            rcbcus.IsProccess = false;
            rcbcus.Stage = 0;
            var resb = await _mdata.SaveChangesAsync();
            //发送短信通知给客户经理（随机确定客户经理）
            var straffphone = _mdata.rcbstraffs.Where(e => e.Straff == rcbcus.Straff).AsNoTracking().FirstOrDefault();
            if (straffphone != null)
            {
                Smsinfo smsinfo = new(rcbcus.CustomerName, rcbcus.phone, straffphone.phone);
                var res2 = SmsCloud.Sms(smsinfo);
                var sendinfo = new Bookinfo(rcbcus, res2);
            }
            return Ok(rcbcus);


        }


        [HttpGet]
        public ActionResult GetAccount(string code)
        {
            var res = _mdata.accounts.Where(e => e.Nums == code).FirstOrDefault();
            if (res == null)
            {
                return BadRequest("无打印信息");
            }
            else
            {
                return Ok(res);
            }
        }

        [HttpGet]
        public ActionResult Printsettings(string code)
        {
            var res = _mdata.cloudprinters.Where(e => e.Code == code).FirstOrDefault();
            if (res == null)
            {
                return BadRequest("云打印未配置");
            }
            else
            {
                return Ok(res);
            }
        }

        [HttpPost]
        public async Task<ActionResult> SetAccountAsync(Account Acc)
        {
            var res = _mdata.accounts.Where(e => e.Nums == Acc.Nums).AsNoTracking().FirstOrDefault();
            if (res != null)
            {
                var result = await _mdata.DeleteRangeAsync<Account>(b => b.Nums == Acc.Nums);
            }
            _mdata.accounts.Add(Acc);

            var res2 = await _mdata.SaveChangesAsync();
            return Ok(res2);
        }

        [HttpPost]

        public async Task<ActionResult> SetprintAsync(Clouddatas cdata)
        {
            var res = _mdata.cloudprinters.Where(e => e.Code == cdata.Cpr.Code).AsNoTracking().FirstOrDefault();
            if (res == null)
            {
                _mdata.Add(cdata.Cpr);
            }
            else
            {
                var entry = _mdata.Entry<Cloudprinter>(cdata.Cpr);
                entry.State = EntityState.Modified;
                entry.Property("Code").IsModified = false;

            }
            var res3 = _mdata.printers.Where(e => e.Code == cdata.Cpr.Code).FirstOrDefault();
            if (res3 != null)
            {
                var res2 = await _mdata.DeleteRangeAsync<Printers>(b => b.Code == cdata.Cpr.Code);
            }

            foreach (var item in cdata.Printers)
            {
                _mdata.Add(item);

            }

            var result = await _mdata.SaveChangesAsync();

            return Ok(result);

        }





        [HttpPost]
        public async Task<ActionResult> tempupdateAsync()
        {




            var res = await userManager.GetUsersInRoleAsync("kpi");

            foreach (var user in res)
            {
                Console.WriteLine(user.Wxopenid);
            }

            return Ok(res);
        }
        [Authorize]
        [HttpPost]
        public async Task<ActionResult> RmuserAsync(string UserName)
        {
            var usn = this.User.FindFirst(ClaimTypes.Name)!.Value;
            var muser = await userManager.FindByNameAsync(usn);
            var res = userManager.DeleteAsync(muser);
            return Ok(res.Result);
        }


        [Authorize]
        [HttpGet]
        public async Task<ActionResult> checksubscribeAsync()
        {
            var usn = this.User.FindFirst(ClaimTypes.Name)!.Value;
            var muser = await userManager.FindByNameAsync(usn);
            string tmpopenid = muser.Wxopenid ?? "";
            var res = _mdata.wxunionids.Where(e => e.Wxopenid == tmpopenid).FirstOrDefault();
            if (res == null)
            {
                return Ok(false);
            }
            else
            {
                return Ok(res.Subscribe);
            }

        }


        [Authorize]
        [HttpGet]
        public async Task<ActionResult> CheckuserAsync()
        {
            var usn = this.User.FindFirst(ClaimTypes.Name)!.Value;
            var usr = await userManager.FindByNameAsync(usn);
            var res = await userManager.IsInRoleAsync(usr, "kpimain");



            return Ok(res);


        }
        /*
        [HttpPost]
        public ActionResult getdraf()
        {

            var accesstoken = AccessTokenContainer.TryGetAccessToken(Config.SenparcWeixinSetting.Items["second"].WeixinAppId, Config.SenparcWeixinSetting.Items["second"].WeixinAppSecret);

            return Ok(accesstoken);

        } 
        [HttpPost]
        public ActionResult getdraf2()
        {

            var accesstoken = AccessTokenContainer.TryGetAccessToken(Config.SenparcWeixinSetting.Items["softrcb2"].WxOpenAppId, Config.SenparcWeixinSetting.Items["softrcb2"].WxOpenAppSecret);

            return Ok(accesstoken);

        } */

        private static string BuildToken(IEnumerable<Claim> claims, JWTOptions options)
        {
            DateTime expires = DateTime.Now.AddSeconds(options.ExpireSeconds);
            byte[] keyBytes = Encoding.UTF8.GetBytes(options.SigningKey);
            var secKey = new SymmetricSecurityKey(keyBytes);
            var credentials = new SigningCredentials(secKey,
                SecurityAlgorithms.HmacSha256Signature);
            var tokenDescriptor = new JwtSecurityToken(expires: expires,
                signingCredentials: credentials, claims: claims);
            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }


        [HttpPost]
        public async Task<ActionResult> AddroleAsync(string Username, string roleName)
        {
            User muser = new() { UserName = Username };
            var urs = await userManager.FindByNameAsync(Username);
            if (urs == null)
            {
                return BadRequest($"{Username}不存在");
            }
            bool roleExists = await roleManager.RoleExistsAsync(roleName);
            if (!roleExists)
            {
                myRole myrole = new myRole { Name = roleName };
                var r = await roleManager.CreateAsync(myrole);
                if (!r.Succeeded)
                {
                    return BadRequest(r.Errors);
                }
            }
            var res = await userManager.AddToRoleAsync(urs, roleName);
            return Ok(res);
        }

        [HttpPost]
        public async Task<ActionResult> AdduserAsync(string Username)
        {

            User muser = new() { UserName = Username };

            var success = await userManager.FindByNameAsync(Username);
            if (success == null)
            {
                var res = await userManager.CreateAsync(muser, "123456");
                if (!res.Succeeded)
                {
                    return BadRequest("useradd error");
                }
            }
            else
            {
                return Ok("user is already exited");
            }

            return Ok("user add success");
        }
        [HttpGet]
        public async Task<IActionResult> Gettoken(string userName,
                    [FromServices] IOptions<JWTOptions> jwtOptions)
        {

            var user = await userManager.FindByNameAsync(userName);

            if (user == null)
            {
                return BadRequest("Failed");
            }
            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
            claims.Add(new Claim(ClaimTypes.Name, user.UserName));
            var roles = await userManager.GetRolesAsync(user);
            foreach (string role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            string jwtToken = BuildToken(claims, jwtOptions.Value);
            return Ok(jwtToken);
        }

        [HttpGet]
        public ActionResult Kpidata(int data, string mdate, string SubName = "one")
        {

            if (SubName == "one")
            {
                var Kdata = data switch
                {
                    1 => _mdata.subankdatas.Where(b => b.mdate == mdate).Select(e => new { e.SubName, e.nums }).OrderByDescending(b => b.nums).ToList().Take(5),
                    2 => _mdata.subankdatas.Where(b => b.mdate == mdate).Select(e => new { e.SubName, e.nums }).OrderByDescending(b => b.nums).ToList().TakeLast(5),
                    _ => _mdata.subankdatas.Where(b => b.mdate == mdate).Select(e => new { e.SubName, e.nums }).OrderByDescending(b => b.nums).ToList(),

                };
                return Ok(Kdata);
            }
            else
            {
                var Kdata = _mdata.subankdatas.Where(b => b.SubName == SubName && b.mdate == mdate).ToList();
                return Ok(Kdata);
            }



        }

        [HttpGet]

        public FileResult DownKpi()
        {
            string tempfile = "wwwroot/kpi.xlsx";
            var stream = System.IO.File.OpenRead(tempfile);

            string fileExt = Path.GetExtension(tempfile);

            //获取文件的ContentType

            var provider = new FileExtensionContentTypeProvider();

            var memi = provider.Mappings[fileExt];

            return File(stream, memi, Path.GetFileName(tempfile));


        }

        [HttpPost]
        public async Task<ActionResult> tmepkpiAsync(IFormFile file, string mdate)
        {
            using var fs = file.OpenReadStream();

            var mapper = new Mapper(fs);

            var Kpidata = mapper.Take<Subankdata>(0).Select(e => e.Value).Where(e => e.nums != 0).OrderByDescending(e => e.nums);
            await _mdata.DeleteRangeAsync<Subankdata>(e => e.SubName != null && e.mdate == mdate);
            await _mdata.BulkInsertAsync(Kpidata);
            await _mdata.BatchUpdate<Subankdata>().Set(b => b.mdate, b => mdate).Where(b => b.mdate == "2021-01" || b.mdate == mdate).ExecuteAsync();
            var Kpifirst = _mdata.subankdatas.Where(a => a.mdate == mdate).OrderByDescending(e => e.nums).ToList().Take(5);
            var Kpilast = _mdata.subankdatas.Where(a => a.mdate == mdate).OrderByDescending(e => e.nums).ToList().TakeLast(5);
            var kpi = Kpifirst.Union(Kpilast);
            var JsonKpi = JsonConvert.SerializeObject(kpi);

            mapper.Save("wwwroot/kpi.xlsx");
            return Ok(JsonKpi);
        }


        [HttpPost]

        public async Task<ActionResult> Postkpi(IFormFile file, string mdate)
        {
            using var fs = file.OpenReadStream();

            var mapper = new Mapper(fs);

            var Kpidata = mapper.Take<Subankdata>("sheet1").Select(e => e.Value).Where(e => e.nums != 0).OrderByDescending(e => e.nums);


            await _mdata.DeleteRangeAsync<Subankdata>(e => e.SubName != null && e.mdate == mdate);
            await _mdata.BulkInsertAsync(Kpidata);
            await _mdata.BatchUpdate<Subankdata>().Set(b => b.mdate, b => mdate).Where(b => b.mdate == "2021-01" || b.mdate == mdate).ExecuteAsync();
            var Kpifirst = _mdata.subankdatas.Where(a => a.mdate == mdate).OrderByDescending(e => e.nums).ToList().Take(5);
            var Kpilast = _mdata.subankdatas.Where(a => a.mdate == mdate).OrderByDescending(e => e.nums).ToList().TakeLast(5);
            var kpi = Kpifirst.Union(Kpilast);
            var JsonKpi = JsonConvert.SerializeObject(kpi);

            mapper.Save("wwwroot/kpi.xlsx");
            return Ok(JsonKpi);


        }

        private string Rfilename(bool flag)
        {
            if (flag)
            {
                var mfile = _mdata.credittokens.Where(e => e.fileId == 1).Single();
                mfile.token = new Random().Next(100, 10000).ToString();
                //_mdata.Add(mfile);
                _mdata.SaveChanges();
                return mfile.token.ToString();
            }
            else
            {
                var nmfile = _mdata.credittokens.Where(e => e.fileId == 1).Single();
                return nmfile.token.ToString();
            }

        }


        [HttpPost]

        //贷款审批流程监控 消息模板
        public async Task<ActionResult> WxsendmsgAsync(string mopenid)
        {
            var myminiProgram = new TemplateModel_MiniProgram()
            {
                appid = WxOpenAppId,
                pagepath = "pages/index/index"
            };
            var mdata = new TempBkmsg("未处理事项提醒", "贷款审批平台", "贷款审批", "中台审批超期", "客户张三 客户号:1235566", "信贷中台:王二", "请在时限内完成审批流程");

            var result = await TemplateApi.SendTemplateMessageAsync(Config.SenparcWeixinSetting.Items["second"].WeixinAppId, mopenid, mdata, myminiProgram);//使用异步方法



            return Ok(result.errmsg);
        }


        [HttpPost]
        [Authorize]
        //KPI消息模板
        public async Task<ActionResult> Wxsendkpi(Sendmsg msg)
        {
            var usn = this.User.FindFirst(ClaimTypes.Name)!.Value;
            var usr = await userManager.FindByNameAsync(usn);
            Boolean Rolname = await userManager.IsInRoleAsync(usr, "kpimain");


            if (!Rolname)
            {
                return BadRequest("无权限");
            }

            var myminiProgram = new TemplateModel_MiniProgram()
            {
                appid = WxOpenAppId,
                pagepath = "pages/index/index"
            };

            var res = await userManager.GetUsersInRoleAsync("kpi");
            var Kpifirst = _mdata.subankdatas.Where(a => a.mdate == msg.mdate).OrderByDescending(e => e.nums).ToList().Take(5);
            var Kpilast = _mdata.subankdatas.Where(a => a.mdate == msg.mdate).OrderByDescending(e => e.nums).ToList().TakeLast(5);
            string tempbank = "前五名:";
            foreach (var kpi in Kpifirst)
            {
                tempbank += kpi.SubName + kpi.nums.ToString("0.00");
            }
            tempbank += Environment.NewLine + "后五名:";
            foreach (var kpi in Kpilast)
            {
                tempbank += kpi.SubName + kpi.nums.ToString("0.00");
            }
            foreach (var user in res)
            {
                var mdata = new TempKpi("Kpi红黄牌通知", msg.mdate, "各支行营业部", "月度绩效", "各业务条线Kpi指标", tempbank);
                var mres = await TemplateApi.SendTemplateMessageAsync(Config.SenparcWeixinSetting.Items["second"].WeixinAppId, user.Wxopenid, mdata, myminiProgram);//使用异步方法
                Console.WriteLine(mres);
            }
            return Ok($"{Rolname}发送{res.Count}条消息");
        }

        [HttpGet]

        public ActionResult WxOpen(string Signature, string Timestamp, string Nonce, string echostr)

        {



            if (CheckSignature.Check(Signature, Timestamp, Nonce, mToken))
            {
                return Content(echostr); //返回随机字符串则表示验证通过
            }
            else
            {
                return Content("failed:" + Signature + "," + CheckSignature.GetSignature(Timestamp, Nonce, mToken) + "。" +
                    "如果你在浏览器中看到这句话，说明此地址可以被作为微信小程序后台的Url，请注意保持Token一致。");
            }

        }




        [HttpGet]
        public async Task<string> GetuserbystraffAsync(string straffid)
        {
            var userX = await userManager.FindByNameAsync(straffid);
            if (userX == null)
            {
                return "405";
            }
            if (userX.Cname == null)
            {
                return "405";
            }
            return userX.Cname;
        }

        [HttpGet]
        public ActionResult Wx2CustomId(string code)
        {

            var res = SnsApi.JsCode2Json(WxOpenAppId2, WxOpenAppSecret2, code);
            return Ok(res);

        }

        /// <summary>
        /// 根据电话号码注册小程序用户统一平台信息 
        /// </summary>
        /// <param name="code"></param>
        /// <param name="encryptedData"></param>
        /// <param name="IV"></param>
        /// <param name="straffid"></param>
        /// <param name="jwtOptions"></param>
        /// <returns></returns>
        [HttpGet]

        public async Task<ActionResult> GetuserPhone(string code, string encryptedData, string IV, string straffid, [FromServices] IOptions<JWTOptions> jwtOptions)
        {


            //测试号功能添加
            if (straffid == "642005")
            {
                return Ok("eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjYiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiNjQyMDA1IiwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93cy8yMDA4LzA2L2lkZW50aXR5L2NsYWltcy9yb2xlIjpbImtwaSIsImtwaW1haW4iXSwiZXhwIjoxNjUwMzAwNTcxfQ.s3t8OlJ6ezYDSeGrJoPthdlH-bgxq9raz-AJRV7_Wyo");
            }


            var res = _mdata.wxsessions.Where(e => e.code == code).FirstOrDefault();
            //var sessionBag = SessionContainer.UpdateSession(null, res.openid, res.session_key, res.unionid);

            // var phoneNum = WechatDecrypt(encryptedData, res.sessionkey, IV);
            var phoneNum = EncryptHelper.DecryptPhoneNumberBySessionKey(res.sessionkey, encryptedData, IV);

            var muser = _mdata.employees.Where(e => e.EmployeeID == straffid).FirstOrDefault();
            if (muser == null)
            {
                return BadRequest("straffid is not exited");
            }


            User userN = new User();

            userN.UserName = muser.EmployeeID;   //这是用户表employee里面的数据
            userN.PhoneNumber = phoneNum.phoneNumber; //这是解密出来的数据
            userN.Cname = muser.Name;  //这是用户表employee里面的数据
            userN.WxUnionid = res.unionid;  //统一ID        这是解密出来的数据
                                            // userN.Appopenid = res.openid;   //小程序用户id  这是解密出来的数据


            var tempdata = _mdata.wxunionids.Where(e => e.WxUnionid == res.unionid).FirstOrDefault();
            if (tempdata == null)
            {
                return BadRequest("用户未关注公众号1");
            }
            if (tempdata.Wxopenid == null)
            {
                return BadRequest("用户未关注公众号2");
            }

            if (tempdata.Wxopenid == "")
            {
                return BadRequest("用户未关注公众号3");
            }
            userN.Wxopenid = tempdata.Wxopenid;


            bool roleExists = await roleManager.RoleExistsAsync("kpi");
            if (!roleExists)
            {
                myRole myrole = new myRole { Name = "kpi" };

                var r = await roleManager.CreateAsync(myrole);
                if (!r.Succeeded)
                {
                    return BadRequest(r.Errors);
                }
            }





            var userX = await userManager.FindByNameAsync(userN.UserName);
            if (userX == null)
            {
                var result = await userManager.CreateAsync(userN, "123456");


                if (!result.Succeeded)
                {
                    return BadRequest(result.Errors);
                }
                result = await userManager.AddToRoleAsync(userN, "kpi");
                if (!result.Succeeded)
                {
                    return BadRequest(result.Errors);
                }
            }


            //获取token
            var claims = new List<Claim>();
            var user = await userManager.FindByNameAsync(userN.UserName);

            if (user == null)
            {
                return BadRequest("Failed");
            }

            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
            claims.Add(new Claim(ClaimTypes.Name, user.UserName));
            var roles = await userManager.GetRolesAsync(user);
            foreach (string role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            string jwtToken = BuildToken(claims, jwtOptions.Value);
            return Ok(jwtToken);



            //return phoneNum.phoneNumber + res.unionid + "==" + res.openid + res.errcode + res.errmsg + phoneNum.countryCode;



        }



        [HttpPost]
        public ActionResult Kpiinfo(Kpiinfo Infos)
        {

            var tmpdata = _mdata.subankdatas.Where(e => e.SubName == Infos.Subname && e.mdate == Infos.Mdate)
                .Select(b => new Resdata(b.Dept + b.PubDept + b.LowDept + b.NewDept + b.Risk + b.Gold + b.Count01 + b.Cunnt02,
                b.Ebank0 + b.Ebank1 + b.Ebank2 + b.Ebank3, b.CR01 + b.CR02 + b.CR03, b.XD01 + b.XD02, b.RiskK01 + b.Risk02, b.nums, b.mdate));



            return Ok(tmpdata);
        }

        /* [HttpGet]

         public ActionResult Kpidetails(string Subname,string Mdate,string ActName) {
             var Kdata = ActName switch
             {
                 "dept" => _mdata.subankdatas.Where(b => b.mdate == Mdate && b.SubName==Subname).Select(e => new { e.Dept,e.PubDept,e.LowDept }).ToList(),
                 _=>null


             };


             return Ok(Kdata);
         } */

        [HttpPost]
        public async Task<ActionResult> SetunionidAsync()
        {

            var userlist = await UserApi.GetAsync(Senparc.Weixin.Config.SenparcWeixinSetting.Items["second"].WeixinAppId, "");
            foreach (var user in userlist.data.openid)
            {
                var wxid = await UserApi.InfoAsync(Senparc.Weixin.Config.SenparcWeixinSetting.Items["second"].WeixinAppId, user);
                var muser = _mdata.customers.Where(e => e.openid == user).FirstOrDefault();
                if (muser != null)
                {
                    muser.unionId = wxid.unionid;
                    _ = await _mdata.SaveChangesAsync();
                }
            }




            return Ok("over");

        }
        [HttpPost]

        public string Getbranch(postJsonBank branch)
        {
            // ControllerContext.HttpContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");


            string bankN = wxusers.banks.Where(o => o.bankName == branch.branchName).Single().bankID;
            var mbank = wxusers.banks.Where(b => b.parentID == bankN).Select(e => new { e.bankID, e.bankName, e.parentID }).ToList();



            return mbank.ToJson();


        }

        // PUT api/<WxController>/5


        [HttpGet]
        public IActionResult getwxurl(int flag)
        {
            var state = "blc-" + SystemTime.Now.Millisecond;//随机数，用于识别请求可靠性

            // HttpContext.Session.SetString("State", state);//储存随机数到Session

            string mreturnurl = "";

            if (flag == 1)
            {

                mreturnurl = OAuthApi.GetAuthorizeUrl(Senparc.Weixin.Config.SenparcWeixinSetting.Items["second"].WeixinAppId,
                         "https://rcbcybank.com",
                        state, OAuthScope.snsapi_userinfo);

            }
            else if (flag == 0)
            {
                mreturnurl = OAuthApi.GetAuthorizeUrl(Senparc.Weixin.Config.SenparcWeixinSetting.Items["second"].WeixinAppId,
                             "https://rcbcybank.com",
                            state, OAuthScope.snsapi_base);
            }

            return Ok(Convert.ToString(mreturnurl));





        }



        /// <summary>
        /// OAuthScope.snsapi_userinfo方式回调
        /// </summary>
        /// <param name="code"></param>
        /// <param name="state"></param>
        /// <param name="returnUrl">用户最初尝试进入的页面</param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult? Getuser(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return null;
            }



            var result = OAuthApi.GetAccessToken(Senparc.Weixin.Config.SenparcWeixinSetting.Items["second"].WeixinAppId, Senparc.Weixin.Config.SenparcWeixinSetting.Items["second"].WeixinAppSecret, code);



            //下面2个数据也可以自己封装成一个类，储存在数据库中（建议结合缓存）
            //如果可以确保安全，可以将access_token存入用户的cookie中，每一个人的access_token是不一样的


            //因为第一步选择的是OAuthScope.snsapi_userinfo，这里可以进一步获取用户详细信息

            var userInfo = OAuthApi.GetUserInfo(result.access_token, result.openid);
            // return Redirect(returnUrl);
            return Ok(userInfo.ToJson());



        }
    }
}
