using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiderTracker.Imp.MWeiboJson
{
    public class MWeiboSuperData
    {
        public MWeiboSuper pageInfo { get; set; }
        public MWeiboSuperCard[] cards { get; set; }
        public string scheme { get; set; }
    }
}
