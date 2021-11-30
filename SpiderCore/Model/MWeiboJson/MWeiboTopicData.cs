using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiderCore.Model.MWeiboJson
{
    public class MWeiboTopicData
    {
        public MWeiboTopic cardlistInfo { get; set; }
        public MWeiboTopicCard[] cards { get; set; }
        public string scheme { get; set; }
    }
}
