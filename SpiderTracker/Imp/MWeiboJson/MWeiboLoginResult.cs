using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiderTracker.Imp.MWeiboJson
{
    public class MWeiboLoginResult
    {
        public int retcode { get; set; }
        public bool success
        {
            get
            {
                return retcode == 20000000;
            }
        }

        public string msg { get; set; }

        public MWeiboLoginData data { get; set; }
    }
}
