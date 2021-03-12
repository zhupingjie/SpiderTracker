using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiderTracker.Imp.Model
{
    public class SinaUser : BaseEntity
    {
        public SinaUser()
        {
            lastdate = DateTime.Now;
        }
        public string uid { get; set; }
        public string name { get; set; }
        public string avatar { get; set; }
        public string desc { get; set; }
        public string profile { get; set; }
        public int follows { get; set; }
        public int followers { get; set; }
        public int statuses { get; set; }
        public string verified { get; set; }
        public int focus { get; set; }
        public int ignore { get; set; }
        public string groupname { get; set; }

        public int getstatuses { get; set; }
        public int originals { get; set; }
        public int retweets { get; set; }
        public int piccount { get; set; }

        public int newcount { get; set; }
    }
}
