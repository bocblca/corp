using Flurl;
using Flurl.Http;
using Microsoft.AspNetCore.Mvc;
using Mysqldb;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Senparc.Weixin;
using workapi.baiduapi;

namespace workapi.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class Aibaidu : ControllerBase
    {
        public static readonly string mToken = Config.SenparcWeixinSetting.Items["softrcb"].WxOpenToken;//与微信小程序后台的Token设置保持一致，区分大小写。
        public static readonly string EncodingAESKey = Config.SenparcWeixinSetting.Items["softrcb"].WxOpenEncodingAESKey;//与微信小程序后台的EncodingAESKey设置保持一致，区分大小写。
        public static readonly string WxOpenAppId = Config.SenparcWeixinSetting.Items["softrcb"].WxOpenAppId;//与微信小程序后台的AppId设置保持一致，区分大小写。
        public static readonly string WxOpenAppSecret = Config.SenparcWeixinSetting.Items["softrcb"].WxOpenAppSecret;//与微信小程序账号后台的AppId设置保持一致，区分大小写。




        // GET: api/<WxController>

        private readonly Wxusers _mdata;

        public Aibaidu(Wxusers mdata)
        {
            _mdata = mdata;
        }


        [HttpGet]
        public ActionResult Getmindistance(double lon, double lat)
        {
            var res = _mdata.cloudprinters.Select(e => new { e.Code, e.Name, e.Lat, e.Lon, dis = GetDistance(lat, lon, e.Lat, e.Lon) }).ToList();
            var dismin = res.Min(e => e.dis);
            var result = res.Where(e => e.dis == dismin);
            return Ok(result);

        }

        private static double GetDistance(double lat1, double lng1, double lat2, double lng2)
        {

            var radLat1 = ToRadians(lat1);
            var radLat2 = ToRadians(lat2);
            var deltaLat = radLat1 - radLat2;
            var deltaLng = ToRadians(lng1) - ToRadians(lng2);
            var dis = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(deltaLat / 2), 2) + Math.Cos(radLat1) * Math.Cos(radLat2) * Math.Pow(Math.Sin(deltaLng / 2), 2)));
            return dis * 6378137;
        }
        private static double ToRadians(double x)
        {
            return x * Math.PI / 180;
        }

        [HttpGet]
        public async Task<ActionResult> BaiduGpstranAsync(double lon, double lat)
        {
            string mhost = "https://api.map.baidu.com";
            var res = await mhost.AppendPathSegment("geoconv/v1/")
                .SetQueryParam("coords", lon + "," + lat)
                .SetQueryParam("ak", "NNHBuFqh97QqZAu6wPaRsfCIQqVW9GbN")
                .SetQueryParam("from", 3)
                .SetQueryParam("to", 5)
                .GetJsonAsync();
            return Ok(res);
        }


        [HttpGet]
        public async Task<ActionResult> BaiduaddressAsync(double lon, double lat)
        {
            string mhost = "https://api.map.baidu.com";
            var res = await mhost.AppendPathSegment("reverse_geocoding/v3/")
                .SetQueryParam("location", lat + "," + lon)
                .SetQueryParam("ak", "NNHBuFqh97QqZAu6wPaRsfCIQqVW9GbN")
                .SetQueryParam("coordtype", "bd09ll")
                .SetQueryParam("output", "json")
                .GetJsonAsync();

            return Ok(res);
        }



        [HttpPost]
        public async Task<ActionResult> SetcloudinfoAsync(Cloudprinter Printinfo)
        {
            var res = _mdata.cloudprinters.Where(e => e.straff == Printinfo.straff).FirstOrDefault();
            if (res == null)
            {
                return BadRequest("straff error");
            }
            else
            {

                res.Lon = Printinfo.Lon;
                res.Lat = Printinfo.Lat;
                res.Fprname = Printinfo.Fprname;
                res.Sprname = Printinfo.Sprname;
                res.PrLeft = Printinfo.PrLeft;
                res.PrTop = Printinfo.PrTop;
                res.BankName = Printinfo.BankName;
                res.Bankcode = Printinfo.Bankcode;
                res.Regcode = Printinfo.Regcode;
                res.Gpsflag = true;
                var result = await _mdata.SaveChangesAsync();
                return Ok(result);
            }
        }

        [HttpGet]
        public ActionResult Getsetprinters(string straff)
        {
            var printers = _mdata.cloudprinters.Where(e => e.straff == straff).FirstOrDefault();
            if (printers == null)
            {
                return BadRequest("no such bank");
            }
            return Ok(printers);
        }

        [HttpGet]
        public ActionResult GetPrinters(string straff)
        {
            var printers = _mdata.cloudprinters.Where(e => e.straff == straff).FirstOrDefault();
            if (printers == null)
            {
                return BadRequest("no such bank");
            }
            string code = printers.Code;
            if (code == null)
            {
                return BadRequest("no set printers");
            }
            else
            {
                var res = _mdata.printers.Where(e => e.Code == code).ToArray();
                return Ok(res);
            }
        }

        [HttpPost]

        public async Task<ActionResult> PermitAsync(IFormFile Sfile)
        {
            using var ms = new MemoryStream();
            Sfile.CopyTo(ms);
            var fileBytes = ms.ToArray();
            string s = Convert.ToBase64String(fileBytes);

            try
            {

                string res = await BusinessLicense.CardInfo(bdTokenkeyAsync().Result, s);
                JObject json = JObject.Parse(res);
                return Ok(json);
            }
            catch (Exception ex)
            {
                return Ok(ex);
            }


        }
        [HttpPost]
        public async Task<ActionResult> LoancardAsync(IFormFile Sfile)
        {
            using var ms = new MemoryStream();
            Sfile.CopyTo(ms);
            var fileBytes = ms.ToArray();
            string s = Convert.ToBase64String(fileBytes);

            try
            {

                string res = await BusinessLicense.LoanInfoAsync(bdTokenkeyAsync().Result, s);
                JObject json = JObject.Parse(res);
                return Ok(json);
            }
            catch (Exception ex)
            {
                return Ok(ex);
            }


        }

        [HttpPost]
        public async Task<ActionResult> IdCardtext(IFormFile file, string frontback)
        {



            using var ms = new MemoryStream();
            file.CopyTo(ms);
            var fileBytes = ms.ToArray();
            string s = Convert.ToBase64String(fileBytes);




            try
            {

                string res = await Idcard.CardInfo(bdTokenkeyAsync().Result, s, frontback);
                JObject json = JObject.Parse(res);
                return Ok(json);
            }
            catch (Exception ex)
            {
                return Ok(ex);
            }





        }

        private async Task<string> bdTokenkeyAsync()
        {
            var dtemp = _mdata.aitokens.Where(e => e.Id == 1).FirstOrDefault().Expires;
            string mToken;

            var dcomp = DateTime.Compare(DateTime.Now, dtemp);

            if (dcomp >= 0)
            {

                var datatemp = _mdata.aitokens.Where(e => e.Id == 1).FirstOrDefault();
                var res = AccessToken.GetAccessToken();
                datatemp.AiToken = JsonConvert.DeserializeObject<dynamic>(res).access_token;
                datatemp.Expires = DateTime.Now.AddDays(29);

                mToken = datatemp.AiToken;
                _ = await _mdata.SaveChangesAsync();

            }
            else
            {
                var datatemp = _mdata.aitokens.Where(e => e.Id == 1).FirstOrDefault();
                mToken = datatemp.AiToken;

            }





            return mToken;


        }
    }
}
