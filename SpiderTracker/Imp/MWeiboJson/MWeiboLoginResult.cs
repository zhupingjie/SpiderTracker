using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiderTracker.Imp.MWeiboJson
{
    public class MWeiboLoginResult
    {
        public int ok { get; set; }
        public bool success
        {
            get
            {
                return ok == 1;
            }
        }

        public string msg { get; set; }

        public MWeiboLoginData data { get; set; }
    }
}
