namespace workauto.corp
{
    //如果好用，请收藏地址，帮忙分享。
    public class Applyer
    {
        /// <summary>
        /// 
        /// </summary>
        public string userid { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string partyid { get; set; }
    }

    public class Approver
    {
        /// <summary>
        /// 
        /// </summary>
        public string userid { get; set; }
    }

    public class DetailsItem
    {
        /// <summary>
        /// 
        /// </summary>
        public Approver approver { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string speech { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int sp_status { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int sptime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> media_id { get; set; }
    }

    public class Sp_recordItem
    {
        /// <summary>
        /// 
        /// </summary>
        public int sp_status { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int approverattr { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<DetailsItem> details { get; set; }
    }

    public class TitleItem
    {
        /// <summary>
        /// 请假类型
        /// </summary>
        public string text { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string lang { get; set; }
    }

    public class FilesItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string file_id { get; set; }
    }

    public class ValueItem
    {
        /// <summary>
        /// 病假
        /// </summary>
        public string text { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string lang { get; set; }
    }

    public class OptionsItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string key { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<ValueItem> value { get; set; }
    }

    public class Selector
    {
        /// <summary>
        /// 
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<OptionsItem> options { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> op_relations { get; set; }
    }

    public class Date_range
    {
        /// <summary>
        /// 
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int new_begin { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int new_end { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int new_duration { get; set; }
    }

    public class Day_itemsItem
    {
        /// <summary>
        /// 
        /// </summary>
        public int daytime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> time_sections { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int duration { get; set; }
    }

    public class Slice_info
    {
        /// <summary>
        /// 
        /// </summary>
        public List<Day_itemsItem> day_items { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int state { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int duration { get; set; }
    }

    public class Attendance
    {
        /// <summary>
        /// 
        /// </summary>
        public Date_range date_range { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int type { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Slice_info slice_info { get; set; }
    }

    public class Vacation
    {
        /// <summary>
        /// 
        /// </summary>
        public Selector selector { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Attendance attendance { get; set; }
    }

    public class Value
    {
        /// <summary>
        /// 
        /// </summary>
        public List<string> tips { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> members { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> departments { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<FilesItem> files { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> children { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> stat_field { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Vacation vacation { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> sum_field { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> related_approval { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> students { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> classes { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> docs { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> wedrive_files { get; set; }
    }

    public class ContentsItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string control { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<TitleItem> title { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Value value { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int display { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int require { get; set; }
    }

    public class Apply_data
    {
        /// <summary>
        /// 
        /// </summary>
        public List<ContentsItem> contents { get; set; }
    }

    public class Info
    {
        /// <summary>
        /// 
        /// </summary>
        public string sp_no { get; set; }
        /// <summary>
        /// 请假
        /// </summary>
        public string sp_name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int sp_status { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string template_id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int apply_time { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Applyer applyer { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<Sp_recordItem> sp_record { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<NotifyerItem> notifyer { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Apply_data apply_data { get; set; }
        /// <summary>
        /// 
        /// </summary>
        List<CommentsItem> comments { get; set; }
    }
    public class CommentUserInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public string userid { get; set; }
    }

    public class CommentsItem
    {
        /// <summary>
        /// 
        /// </summary>
        public CommentUserInfo commentUserInfo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int commenttime { get; set; }
        /// <summary>
        /// 这是备注信息
        /// </summary>
        public string commentcontent { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string commentid { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> media_id { get; set; }
    }
    public class NotifyerItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string userid { get; set; }
    }
    public class Spdetail
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
        public Info info { get; set; }
    }



    public class FiltersItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string key { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string value { get; set; }
    }

    public class Corp_Approval
    {
        /// <summary>
        /// 
        /// </summary>
        public string starttime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string endtime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int cursor { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int size { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<FiltersItem> filters { get; set; }
    }
    public class Splistresult
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
        public int next_cursor { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> sp_no_list { get; set; }
    }

}
