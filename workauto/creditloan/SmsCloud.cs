using Mysqldb;
using TencentCloud.Common;
using TencentCloud.Common.Profile;
using TencentCloud.Sms.V20210111;
using TencentCloud.Sms.V20210111.Models;

namespace workapi.creditloan
{
    public static class SmsCloud
    {
        public static string Sms(Smsinfo smsinfo)
        {
            try
            {
                Credential cred = new Credential
                {
                    SecretId = "AKIDYv3uVpseAPN4TcV7SYMuBCwPaUEhsmQn",
                    SecretKey = "6vUt1sTnqc7MJzOWNxFH2qOefOr3ycz5"
                };
                var clientProfile = new ClientProfile();
                clientProfile.SignMethod = ClientProfile.SIGN_TC3SHA256;
                HttpProfile httpProfile = new HttpProfile();
                httpProfile.ReqMethod = "GET";
                httpProfile.Timeout = 20;
                httpProfile.Endpoint = "sms.tencentcloudapi.com";
                clientProfile.HttpProfile = httpProfile;
                SmsClient client = new SmsClient(cred, "ap-guangzhou", clientProfile);
                SendSmsRequest req = new SendSmsRequest();
                req.SmsSdkAppId = "1400650721";
                req.SignName = "葫芦岛农商银行";
                req.TemplateId = "1342721";
                req.TemplateParamSet = new String[] { smsinfo.custname, smsinfo.custphone };
                req.PhoneNumberSet = new String[] { smsinfo.straffphone };
                req.SessionContext = "";
                req.ExtendCode = "";

                /* 国际/港澳台短信 senderid（无需要可忽略）: 国内短信填空，默认未开通，如需开通请联系 [腾讯云短信小助手] */
                req.SenderId = "";

                SendSmsResponse resp = client.SendSmsSync(req);

                // 输出json格式的字符串回包
                return AbstractModel.ToJsonString(resp);
            }
            catch (Exception e)
            {
                return e.ToString();
            }

        }







    }
}
