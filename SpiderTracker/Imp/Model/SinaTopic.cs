using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiderTracker.Imp.Model
{
    public class SinaTopic: BaseEntity
    {
        public SinaTopic()
        {
            this.lastdate = DateTime.Now;
        }
        public bool super { get; set; }
        public string category { get; set; }
        public string containerid { get; set; }
        public string name { get; set; }
        public string profile { get; set; }
        public string desc { get; set; }

    }
}
