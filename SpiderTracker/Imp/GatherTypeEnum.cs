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
        /// 我的关注采集
        /// </summary>
        MyFocusGather = 2,

        /// <summary>
        /// 他的关注采集
        /// </summary>
        HeFocusGather = 3,

        /// <summary>
        /// 按配置采集
        /// </summary>
        AutoGather = 4
    }
}
