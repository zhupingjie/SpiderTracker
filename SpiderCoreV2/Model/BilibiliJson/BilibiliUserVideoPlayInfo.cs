using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiderCore.Model.BilibiliJson
{
    public class BilibiliUserVideoPlayInfo
    {
        public string from { get; set; }
        public string result { get; set; }
        public int quality { get; set; }
        public string format { get; set; }
        public int timelength { get; set; }
        public string accept_format { get; set; }
        public string[] accept_description { get; set; }
        public int[] accept_quality { get; set; }
        public int video_codecid { get; set; }
        public BilibiliUserViedoDash dash { get; set; }
    }
}
