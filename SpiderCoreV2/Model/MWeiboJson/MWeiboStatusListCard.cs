using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiderCore.Model.MWeiboJson
{
    public class MWeiboStatusListCard
    {
        public int card_type { get; set; }
        public string scheme { get; set; }
        public string itemid { get; set; }
        public MWeiboStatus mblog { get; set; }
    }
}
