namespace workauto.corp
{
    public class Departinfo
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
        public Department department { get; set; }
    }
    public class Department
    {
        /// <summary>
        /// 
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// 广州研发中心
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string name_en { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<string> department_leader { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int parentid { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int order { get; set; }

    }

}
