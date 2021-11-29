using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiderCore.Model.BilibiliJson
{
    public class BilibiliUser
    {
        public string mid { get; set; }
        public string sign { get; set; }

        public string face { get; set; }
        public BilibiliUserLiveroom live_room { get; set; }

        public string profile_url
        {
            get
            {
                return $"https://space.bilibili.com/{mid}";
            }
        }
        public string name { get; set; }
        public string liveroom_url
        {
            get
            {
                if (live_room == null) return null;
                return live_room.url;
            }
        }
    }
}
