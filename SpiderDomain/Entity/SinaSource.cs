using SpiderDomain.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiderDomain.Entity
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
        public int width { get; set; }
        public int height { get; set; }
        public int size { get; set; }
        public int upload { get; set; }
        public string downdate { get; set; }
    }
}
