
using Microsoft.AspNetCore.Http;


namespace Mysqldb
{
    public class Workdepart: Senparc.Weixin.Work.AdvancedAPIs.MailList.DepartmentList
    {
       public int level { get; set; }

    }
    public class Member : Senparc.Weixin.Work.AdvancedAPIs.MailList.UserList_Simple
    { 
       new public string? department { get; set; }
    }
    public class Asset
    {
        public string Qrcode { get; set; }= "0";  //编号 key
        public string? Pname { get; set; } //物品名称
        public Kinds kind { get; set; } //分类 0 电子设备 1 办公用品 2 低值易耗品
        public string? Userid { get; set; } //员工id
        public Gps? Point { get; set; }   //gps地址
        public string? Img { get; set; }  //照片
        public double? Money { get; set; } = 0; //资产价值
        public States State { get; set; } = States.use; //0 未使用 1 使用中  2 报废 3 清理中 4 清理变现(残值)
     
    }
    /// <summary>
    /// 定义资产状态数据库
    /// 此处的ID都是指银行的员工号
    /// spanid:时间戳  取消时间字段，以时间戳(key)替代
    /// origin_id:资产原始拥有都ID,assetnew入库时应该为空
    /// operator_id:资产登记操作者ID
    /// target_id:接收者也是现在的资产拥有者ID
    /// </summary>
    public class asset_state {
        
        public long spanid{get;set; } = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
        public string qrcode { get; set; } //新增资产编号 
        public string origin_id { get; set; }
        public string  operator_id { get; set; }
        public string operator_content { get; set; }
        public string target_id { get; set; }
        
        //  public DateTime cdate { get; set; }
        public int state { get; set; } = 0;

    }
    public class Selectoption { 
      public int value { get; set; }
      public string? label { get; set; }
    
    }
    public enum Kinds
    {
        electronic,
        office,
        consumable

    }
    public enum States
    {
        news,
        use,
        scrap,
        clean,
        Salvage
    }

    public class Gps
    {
        public int id { get; set; } //key
        public double Lat { get; set; } //纬度
        public double Lon { get; set; } //经度
    }
    public class Masset {
        public IFormFile file { get; set; }
        public string Qrcode { get; set; }
        public string Pname { get; set; }
        public int kind { get; set; }
        public int state { get; set; }
        public double money { get; set; }
        public string Userid { get; set; }
        public double Lon { get; set; }
        public double Lat { get; set; }
        public string? operator_id { get; set; }
      

    }
    public class Kasset
    { 
       public IFormFile? file { get; set; }
       public Nasset? asset { get; set; }
    }
    public class FileAsset
    {
        public IFormFile? file { get; set; }
        public string  qrcode { get; set; }
        public string? operator_id { get; set; }

    }
    public class Nasset
    {
       
        public string Qrcode { get; set; }
        public string Pname { get; set; }
        public int kind { get; set; }
        public int state { get; set; }
        public double money { get; set; }
        public string Userid { get; set; }
        public double Lon { get; set; }
        public double Lat { get; set; }
        public string? operator_id { get; set; } 

    }
    public class Assetinfo
    {
        public string fileurl { get; set; }
        public string Qrcode { get; set; }
        public string Pname { get; set; }
        public int kind { get; set; }
        public int state { get; set; }
        public double money { get; set; }
        public string Userid { get; set; }
        public double Lon { get; set; }
        public double Lat { get; set; }

    }
    public class Customupdateuser : Senparc.Weixin.Work.AdvancedAPIs.MailList.Member.MemberUpdateRequest {

        public string[] direct_leader { get; set; }


    }
   

}
