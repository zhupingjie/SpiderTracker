using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiderCore.Model.MWeiboJson
{
    public class MWeiboUser
    {
        public string description { get; set; }
        public string avatar_hd { get; set; }
        public int follow_count { get; set; }
        //public int followers_count { get; set; }
        public string verified_reason { get; set; }
        public string id { get; set; }
        public string profile_url { get; set; }
        public string screen_name { get; set; }

        public int? statuses_count { get; set; }
    }
}
