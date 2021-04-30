using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiderCore.Model.BilibiliJson
{
    public class BilibiliUserViedoDash
    {
        public int duration { get; set; }
        public float minBufferTime { get; set; }
        public float min_buffer_time { get; set; }
        public BilibiliUserViedoInfo[] video { get; set; }
        public BilibiliUserViedoAudio[] audio { get; set; }
    }
}
