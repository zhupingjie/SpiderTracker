using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiderCore.Model.MWeiboJson
{
    public class MWeiboStatusMedia
    {
        public string title { get; set; }
        public string content1 { get; set; }
        public string content2 { get; set; }
        public string type { get; set; }

        public MWeiboStatusMediaUrl urls { get; set; }
    }

    public class MWeiboStatusMediaUrl
    {
        public string mp4_hd_mp4 { get; set; }
        public string mp4_ld_mp4 { get; set; }
    }
}
