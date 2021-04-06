using SpiderTracker.Imp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiderTracker.Imp
{
    public class MWeiboSpiderStartOption
    {
        public GatherTypeEnum GatherType { get; set; }

        public SinaUser[] SelectUsers { get; set; }

        public string[] StatusIds { get; set; }

        public string StatusId { get; set; }

        public string UserId { get; set; }
        public string Topic { get; set; }
        public string ContainserId { get; set; }
    }
}
