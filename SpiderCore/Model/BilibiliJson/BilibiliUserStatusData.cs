using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiderCore.Model.MWeiboJson
{
    public class BilibiliUserStatusData
    {
        public int code { get; set; }
        public BilibiliUserStatusList list { get; set; }
        public BilibiliUserStatusPage page { get; set; }
    }
}
