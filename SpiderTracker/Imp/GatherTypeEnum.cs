using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiderTracker.Imp
{
    public enum GatherTypeEnum
    {
        /// <summary>
        /// 单次采集
        /// </summary>
        GatherUser = 0,

        /// <summary>
        /// 批量采集
        /// </summary>
        GahterStatus = 1
    }
}
