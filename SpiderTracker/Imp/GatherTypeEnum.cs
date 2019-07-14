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
        SingleGather = 0,

        /// <summary>
        /// 批量采集
        /// </summary>
        MultiGather = 1,

        /// <summary>
        /// 智能采集
        /// </summary>
        SmartGather = 2,

        /// <summary>
        /// 智能分析
        /// </summary>
        SmartAnalyse = 3
    }
}
