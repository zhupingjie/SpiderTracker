using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiderCore.Model.MWeiboJson
{
    public class MWeiboFocusCard
    {
        public int card_style { get; set; }
        public int card_type { get; set; }
        public string desc1 { get; set; }
        public string desc2 { get; set; }

        public string itemid { get; set; }

        public MWeiboUser user { get; set; }
        public MWeiboFocusCard[] card_group { get; set; }
    }
}
