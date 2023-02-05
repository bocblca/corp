namespace Mysqldb
{
    public class wxunionid
    {

        public string WxUnionid { get; set; }  //开放平台标识 unionid 所有平台唯一标识符
        public string? Wxopenid { get; set; }  //微信公众号 openid
        public string? Appopenid { get; set; } //微信小程序openid
        public string? Gappid { get; set; }   //公众号企业或个人的标识 appid
        public string? Xappid{ get; set; }   //小程序 企业或个人的标识 appid
        public Boolean? Subscribe { get; set; } = false; //是否关注

    }
}
