using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiderCore.Model.MWeiboJson
{
    public class BilibiliUserViedoInfo
    {
        public int id { get; set; }
        public string baseUrl { get; set; }
        public int bandwidth { get; set; }
        public string mimeType { get; set; }
        public string codecs { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public string frameRate { get; set; }
        public string frame_rate { get; set; }
        public string sar { get; set; }
    }
}
