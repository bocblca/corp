/* using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Transform;
using Aliyun.Acs.Core.Utils;

namespace weixin.Models
{
    public class SendSmsRequest : RpcAcsRequest<SendSmsResponse>
    {
        public SendSmsRequest() : base("Dysmsapi", "2017-05-25", "SendSms")
        {
        }

        private string templateCode;

        private string phoneNumbers;

        private string accessKeyId;

        private string signName;

        private string resourceOwnerAccount;

        private string templateParam;

        private string action;

        private long? resourceOwnerId;

        private long? ownerId;

        private string smsUpExtendCode;

        private string outId;

        public string TemplateCode
        {
            get
            {
                return templateCode;
            }
            set
            {
                templateCode = value;
                DictionaryUtil.Add(QueryParameters, "TemplateCode", value);
            }
        }

        public string PhoneNumbers
        {
            get
            {
                return phoneNumbers;
            }
            set
            {
                phoneNumbers = value;
                DictionaryUtil.Add(QueryParameters, "PhoneNumbers", value);
            }
        }

        public string AccessKeyId
        {
            get
            {
                return accessKeyId;
            }
            set
            {
                accessKeyId = value;
                DictionaryUtil.Add(QueryParameters, "AccessKeyId", value);
            }
        }

        public string SignName
        {
            get
            {
                return signName;
            }
            set
            {
                signName = value;
                DictionaryUtil.Add(QueryParameters, "SignName", value);
            }
        }

        public string ResourceOwnerAccount
        {
            get
            {
                return resourceOwnerAccount;
            }
            set
            {
                resourceOwnerAccount = value;
                DictionaryUtil.Add(QueryParameters, "ResourceOwnerAccount", value);
            }
        }

        public string TemplateParam
        {
            get
            {
                return templateParam;
            }
            set
            {
                templateParam = value;
                DictionaryUtil.Add(QueryParameters, "TemplateParam", value);
            }
        }

        public string Action
        {
            get
            {
                return action;
            }
            set
            {
                action = value;
                DictionaryUtil.Add(QueryParameters, "Action", value);
            }
        }

        public long? ResourceOwnerId
        {
            get
            {
                return resourceOwnerId;
            }
            set
            {
                resourceOwnerId = value;
                DictionaryUtil.Add(QueryParameters, "ResourceOwnerId", value.ToString());
            }
        }

        public long? OwnerId
        {
            get
            {
                return ownerId;
            }
            set
            {
                ownerId = value;
                DictionaryUtil.Add(QueryParameters, "OwnerId", value.ToString());
            }
        }

        public string SmsUpExtendCode
        {
            get
            {
                return smsUpExtendCode;
            }
            set
            {
                smsUpExtendCode = value;
                DictionaryUtil.Add(QueryParameters, "SmsUpExtendCode", value);
            }
        }

        public string OutId
        {
            get
            {
                return outId;
            }
            set
            {
                outId = value;
                DictionaryUtil.Add(QueryParameters, "OutId", value);
            }
        }

        public override SendSmsResponse GetResponse(UnmarshallerContext unmarshallerContext)
        {
            SendSmsResponse sendSmsResponse = new SendSmsResponse
            {
                HttpResponse = unmarshallerContext.HttpResponse,
                RequestId = unmarshallerContext.StringValue("SendSms.RequestId"),
                BizId = unmarshallerContext.StringValue("SendSms.BizId"),
                Code = unmarshallerContext.StringValue("SendSms.Code"),
                Message = unmarshallerContext.StringValue("SendSms.Message")
            };

            return sendSmsResponse;
        }

      
    }
}
*/