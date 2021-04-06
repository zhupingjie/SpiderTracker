using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiderTracker.Imp.MWeiboJson
{
    public class MWeiboSuperCard
    {
        public int card_type { get; set; }
        public string itemid { get; set; }
        public MWeiboSuperCardGroup[] card_group { get; set; }
    }
}
