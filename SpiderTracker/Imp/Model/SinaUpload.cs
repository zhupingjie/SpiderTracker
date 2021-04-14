using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiderTracker.Imp.Model
{
    public class SinaUpload : BaseEntity
    {
        public SinaUpload()
        {
            lastdate = DateTime.Now;
        }
        public string category { get; set; }

        public string uid { get; set; }

        public string bid { get; set; }

        public string file { get; set; }

        public string createtime { get; set; }
        
        public int upload { get; set; }

        public string uploadtime { get; set; }
    }
}
