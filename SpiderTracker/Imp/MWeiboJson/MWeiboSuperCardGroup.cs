using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiderTracker.Imp.MWeiboJson
{
    public class MWeiboSuperCardGroup
    {
        public int card_type { get; set; }
        public string itemid { get; set; }
        public string card_type_name { get; set; }
        public string desc { get; set; }
        public int show_type { get; set; }
        public MWeiboStatus mblog { get; set; }
        public string scheme { get; set; }
    }
}
