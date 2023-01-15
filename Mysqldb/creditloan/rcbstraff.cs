
namespace Mysqldb
{   
    public record EditCust(string unionid,DateTime cdate,int stage,string information);
    public record Straffinfo(IQueryable<Booking> AllBookings,IQueryable<Booking> Bookings,Rcbstraff Rcbstraff);
    public record Bookinfo(Booking booking,string msg);
    public record Smsinfo(string custname, string custphone, string straffphone);
    public record CustomerInfo(Booking customer,Rcbstraff straff);
    public record Lonainfo(string name,string nums,string code,string encryptedData,string iv);

    //{"phoneNumber":"18642116581","purePhoneNumber":"18642116581","countryCode":"86","watermark":{"timestamp":1648093671,"appid":"wx0b5685d2415d4b1b"}}
    public record Watermark(string timestamp, string appid);
    public record Phoneinfo(string phoneNumber,string purePhoneNumber,string countryCode,Watermark watermark);

   
    public class Booking
    {
        public string Unionid { get; set; }
        public string? Wxopenid { get; set; }
        public string? CustomerName { get; set; }
        public DateTime? FirstDate { get; set; }
        public DateTime? Seconddatae { get; set; }
        public DateTime? Thirddatae { get; set; }
        public DateTime? Forthdatae { get; set; }
        public int? Stage { get; set; }
        public string? phone { get; set; }
        public Boolean IsProccess { get; set; }=false;
        public string? Straff { get; set; }
        public string? Information { get; set; }
        public String? Nums { get; set; }
    }
    public class Rcbstraff
    { 
       public string Straff { get; set; }
       public string BankId { get; set; }
       public string Name { get; set; }
       public string Ace { get; set; }
       public string Unionid { get; set; }
       public string phone { get; set; }

    }
}
