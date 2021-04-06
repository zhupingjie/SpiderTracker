using SpiderTracker.Imp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiderTracker.Imp
{
    public class MWeiboSpiderStartOption
    {
        public string GatherName { get; set; }
        public GatherTypeEnum GatherType
        {
            get
            {
                switch (GatherName)
                {
                    case "user":
                        return GatherTypeEnum.GatherUser;
                    case "status":
                        return GatherTypeEnum.GahterStatus;
                    case "topic":
                        return GatherTypeEnum.GatherTopic;
                    case "super":
                        return GatherTypeEnum.GatherSuper;
                    default:
                        return GatherTypeEnum.GatherUser;
                }
            }
        }

        public SinaUser[] SelectUsers { get; set; }

        public string[] StatusIds { get; set; }

        public string StartUrl { get; set; }
    }
}
