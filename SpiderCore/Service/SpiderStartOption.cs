using SpiderCore.Config;
using SpiderCore.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiderCore.Service
{
    public class SpiderStartOption
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
                    case "video":
                        return GatherTypeEnum.GatherVideo;
                    default:
                        return GatherTypeEnum.GatherUser;
                }
            }
        }

        public string StartUrl { get; set; }

        public SinaUser[] SelectUsers { get; set; }

        public string[] StatusIds { get; set; }
    }
}
