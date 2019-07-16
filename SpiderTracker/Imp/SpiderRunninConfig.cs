using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiderTracker.Imp
{
    public class SpiderRunningConfig
    {
        public SpiderRunningConfig()
        {
        }

        public GatherTypeEnum GatherType { get; set; } = GatherTypeEnum.SingleGather;

        public string Name { get; set; } = "default";

        /// <summary>
        /// 我的用户
        /// </summary>
        public string LoginUid { get; set; }

        /// <summary>
        /// 登陆Token
        /// </summary>
        public string LoginCookie { get; set; }

        /// <summary>
        /// 起始地址
        /// </summary>
        public string StartUrl { get; set; }


        /// <summary>
        /// 用户ID集合
        /// </summary>
        public string[] UserIds { get; set; }


        /// <summary>
        /// 微博路径集合
        /// </summary>
        public string[] StatusUrls { get; set; }

        /// <summary>
        /// 起始页面
        /// </summary>
        public int StartPageIndex { get; set; } = 1;

        /// <summary>
        /// 读取页数
        /// </summary>
        public int ReadPageCount { get; set; } = 1;

        /// <summary>
        /// 读取下一页等待时间(秒)
        /// </summary>
        public int ReadNextPageWaitSecond { get; set; } = 5;

        /// <summary>
        /// 读取下一条等待时间(秒)
        /// </summary>
        public int ReadNextStatusWaitSecond { get; set; } = 5;

        /// <summary>
        /// 读取最小图数
        /// </summary>
        public int ReadMinOfImgCount { get; set; } = 5;

        /// <summary>
        /// 读取图片最小尺寸
        /// </summary>
        public int ReadMinOfImgSize { get; set; } = 1000;

        /// <summary>
        /// 读取图片最大尺寸
        /// </summary>
        public int ReadMaxOfImgSize { get; set; } = 5000;

        /// <summary>
        /// 分析强制更新
        /// </summary>
        public int AnalyseAllowUpdate { get; set; } = 0;

        /// <summary>
        /// 分析拉黑图集
        /// </summary>
        public int AnalyseIgnoreStatusCount { get; set; }

        /// <summary>
        /// 分析关注图集
        /// </summary>
        public int AnalyseFocusStatusCount { get; set; } = 0;

        /// <summary>
        /// 分析删除图集
        /// </summary>
        public int AnalyseDeleteUserOrStatus { get; set; } = 0;


        /// <summary>
        /// 分析自动采集
        /// </summary>
        public int AnalyseAutoGather { get; set; } = 0;

        /// <summary>
        /// 当前读取页数
        /// </summary>
        public int CurrentPageIndex { get; set; } = 1;

        /// <summary>
        /// 只采集本用户
        /// </summary>
        public int OnlyReadOwnerUser { get; set; } = 0;

        /// <summary>
        /// 只采集用户数
        /// </summary>
        public int OnlyReadUserInfo { get; set; } = 0;
        /// <summary>
        /// 只采集微博数(不下载图片)
        /// </summary>
        public int OnlyReadUserStatus { get; set; } = 0;

        /// <summary>
        /// 只读关注用户
        /// </summary>
        public int OnlyReadFocusUser { get; set; } = 0;

        public string SinaUserName { get; set; } = "zhupj@foxmail.com";
        public string SinaUserPassword { get; set; } = "wdmm643030";
    }
}
