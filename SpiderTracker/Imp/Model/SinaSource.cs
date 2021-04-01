using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiderTracker.Imp.Model
{
    public class SinaSource : BaseEntity
    {
        public SinaSource()
        {
            lastdate = DateTime.Now;
        }
        public string uid { get; set; }
        public string bid { get; set; }
        public string url { get; set; }
        public string name { get; set; }
        public string site { get; set; }
        public int width { get; set; }
        public int height { get; set; }

        public long size { get; set; }

        public int archive { get; set; }
        public DateTime? downdate { get; set; }
    }
}
