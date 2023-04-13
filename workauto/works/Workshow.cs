namespace workauto.works
{
    public class Workshow
    {
        public string type { get; set; }
        public string  agentid { get ; set; }   
        
        public keydata keydata { get; set; }

        public Boolean replace_user_data { get; set; }
    }

    public class keydata {
      
       public List<items> items { get; set; }

    }

    public class items { 
      
        public string key { get; set; }
        public string data { get; set; }

        public string jump_url { get; set; }

        public string pagepath { get; set; }=null;


    }
    public class Workshow_result {
       public string errcode { get; set; }
       public string errmsg { get; set; }


    }
}
