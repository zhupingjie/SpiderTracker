using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiderCore.Model.MWeiboJson
{
    public class MWeiboPic
    {
        public MWeiboLarge large { get; set; }
        public string pid { get; set; }
        public string size { get; set; }
        public string url { get; set; }
    }
}
