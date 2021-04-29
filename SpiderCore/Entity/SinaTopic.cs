using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiderCore.Entity
{
    public class SinaTopic: BaseEntity
    {
        public SinaTopic()
        {
            this.lastdate = DateTime.Now;
        }
        public int type { get; set; }
        public string category { get; set; }
        public string containerid { get; set; }
        public string name { get; set; }
        public string profile { get; set; }
        public string desc { get; set; }
        public int readpage { get; set; }
        public int lastpage { get; set; }

    }
}
