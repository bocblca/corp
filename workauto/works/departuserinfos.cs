namespace workauto.works
{
    public class Departuserinfos
    {
        /// <summary>
        /// 
        /// </summary>
        public int errcode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string errmsg { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<Userlist> userlist { get; set; }
    }
    public class Extattr
    {
        /// <summary>
        /// 
        /// </summary>
        public List<string> attrs { get; set; }
    }

    public class External_profile
    {
        /// <summary>
        /// 
        /// </summary>
        public List<string> external_attr { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string external_corp_name { get; set; }
    }

    public class Userlist
    {
        /// <summary>
        /// 安玉国
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<int> department { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string position { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int status { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int enable { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int isleader { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Extattr extattr { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int hide_mobile { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string telephone { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<int> order { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public External_profile external_profile { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int main_department { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string alias { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<int> is_leader_in_dept { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string userid { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> direct_leader { get; set; }
    }
}
