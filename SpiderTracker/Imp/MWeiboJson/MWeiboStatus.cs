using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiderTracker.Imp.MWeiboJson
{
    public class MWeiboStatus
    {
        public string bid { get; set; }
        public string id { get; set; }
        public string text { get; set; }
        public string status_title { get; set; }

        public string created_at { get; set; }

        public MWeiboStatusMedia page_info { get; set; }
        public MWeiboPic[] pics { get; set; }

        public MWeiboStatus retweeted_status { get; set; }

        public MWeiboUser user { get; set; }
    }
}
