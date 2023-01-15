namespace  Mysqldb

{

    public record Cloudprintset(string straff,string fprname,string sprname,double lon,double lat);
    public record CardJson(string token,string images);
    public record Cardreq(string file64,string frontback);
    //public record Gps(double lon,double lat);
    public class Aitoken
    {
        public int Id { get; set; }
        public string AiToken { get; set; } = "000";
        public DateTime Expires { get; set; }= DateTime.Now;

    }
}
