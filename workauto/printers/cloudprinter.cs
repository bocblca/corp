using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace workapi.printers
{

    public record Clouddatas(Cloudprinter Cpr,List<Printers> Printers);
    public record PMsg(string msg);
    /// <summary>
    /// code:云打印识别码 前台生成：开源nanoid 生成uuid
    /// name:机构名称
    /// prlist:打印机列表（不同机构，列表不同）
    /// Fname:针式打印机
    /// Sname:A4打印机
    /// Gps:机构（打印机）地理位置坐标
    /// gpsflag:坐标初始化
    /// PrTop:打印上边距
    /// PrLeft:打印左边距
    /// regcode 地区代码
    /// bankcode 开户行代码
    /// bankname 开户行名称
    /// 数据库表名、字段定义详见同名config类
    /// </summary>

    public class Cloudprinter
    {  
        public string Code { get; set; }
        public string Name { get; set; } 
        public string straff { get; set; }
        public string Fprname { get; set; }
        public string Sprname { get; set; }
        public double Lon { get; set; }
        public double Lat { get; set; }
        public bool Gpsflag { get; set; }=false;
        public int PrTop { get; set; }
        public int PrLeft { get; set; }
        public string Regcode { get; set; }
        public string Bankcode { get; set; }
        public string BankName { get; set; }

    }
    /// <summary>
    /// 打印机列表类
    /// </summary>
    public class Printers {

     [JsonIgnore]
        public long Id { get; set; }
     [MaxLength(100)]
      public string Code { get; set; }  
      [MaxLength(100)]
      public string name { get; set; }
  
    }

    /// <summary>
    /// 坐标类 Gps
    /// </summary>
 

}
