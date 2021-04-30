using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiderCore.Model.BilibiliJson
{
    public class BilibiliUserStateResult
    {
        public string aid { get; set; }
        public string bvid { get; set; }
        public BilibiliUserStateData videoData { get; set; }

        public BilibiliUser upData { get; set; }
    }
}
