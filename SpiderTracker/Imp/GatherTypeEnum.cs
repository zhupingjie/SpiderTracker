﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiderTracker.Imp
{
    public enum GatherTypeEnum
    {
        /// <summary>
        /// 用户
        /// </summary>
        GatherUser = 0,

        /// <summary>
        /// 微博
        /// </summary>
        GahterStatus = 1,

        /// <summary>
        /// 话题
        /// </summary>
        GatherTopic = 2,

        /// <summary>
        /// 超话
        /// </summary>
        GatherSuper = 3
    }
}
