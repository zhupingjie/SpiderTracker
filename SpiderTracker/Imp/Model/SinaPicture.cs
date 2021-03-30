using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiderTracker.Imp.Model
{
    public class SinaPicture : BaseEntity
    {
        public SinaPicture()
        {
            lastdate = DateTime.Now;
        }
        public string uid { get; set; }
        public string bid { get; set; }
        public string url { get; set; }

        public string localpath { get; set; }
        public string thumbnail { get; set; }

        public int width { get; set; }

        public int height { get; set; }

        public long size { get; set; }

        public DateTime? downdate { get; set; }
    }
}
