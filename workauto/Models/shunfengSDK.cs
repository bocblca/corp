namespace shunfengSDK
{

    public class msgbkData
    {
        /// <summary>
        /// 
        /// </summary>
        public string? success { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? errorCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? errorMsg { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? msgData { get; set; }
    }
    public class mailstatus
    {
        /// <summary>
        /// 
        /// </summary>
        public string? apiErrorMsg { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? apiResponseID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? apiResultCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? apiResultData { get; set; }
    }
    public class mailmsg
    {
        /// <summary>
        /// 
        /// </summary>
        public string? apiErrorMsg { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? apiResponseID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? apiResultCode { get; set; }
        /// <summary>
        ///
        /// </summary>
        public string? apiResultData { get; set; }
    }
    public class Routes
    {
        /// <summary>
        /// 朝阳市
        /// </summary>
        public string? acceptAddress { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime acceptTime { get; set; }
        /// <summary>
        /// 顺丰速运 已收取快件
        /// </summary>
        public string? remark { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? opCode { get; set; }
    }

    public class RouteResps
    {
        /// <summary>
        /// 
        /// </summary>
        public string? mailNo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<Routes>? routes { get; set; }
    }

    public class MsgData
    {
        /// <summary>
        /// 
        /// </summary>
        public List<RouteResps>? routeResps { get; set; }
    }

    public class maildata
    {
        /// <summary>
        /// 
        /// </summary>
        public bool success { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? errorCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? errorMsg { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public MsgData? msgData { get; set; }
    }
}