namespace Mysqldb
{
    public class hangjob
    {
        public string jobid { get; set; }
        public string procid { get; set; }
        public DateTime date_start { get; set; }
        public DateTime? date_end { get; set; }
        public DateTime? date_warn { get; set; }
        public int Ace { get; set; } = 10;
        public Boolean iswarn { get; set; } = false;
        public Boolean isdel { get; set; } = false;
    }
}
