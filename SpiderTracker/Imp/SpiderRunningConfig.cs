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
        public string LoginUser { get; set; } = "47426568@qq.com";
        /// <summary>
        /// 我的用户
        /// </summary>
        public string LoginPassword { get; set; }

        public string LoginUid { get; set; }

        /// <summary>
        /// 登陆Cookie
        /// </summary>
        public string LoginCookie { get; set; }

        /// <summary>
        /// 登陆Token
        /// </summary>
        public string LoginToken { get; set; }

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
        /// 当前读取页数
        /// </summary>
        public int CurrentPageIndex { get; set; } = 1;

        /// <summary>
        /// 采集原创图集
        /// </summary>
        public int OnlyReadOwnerUser { get; set; } = 0;

        /// <summary>
        /// 采集所有用户
        /// </summary>
        public int ReadAllOfUser { get; set; } = 0;
        /// <summary>
        /// 采集用户关注
        /// </summary>
        public int ReadUserOfFocus { get; set; } = 0;

        /// <summary>
        /// 忽略存档图集
        /// </summary>
        public int IgnoreReadArchiveStatus { get; set; } = 0;

        /// <summary>
        /// 忽略已删图集
        /// </summary>
        public int IgnoreDeleteImageStatus { get; set; } = 0;


        /// <summary>
        /// 预览图片数量
        /// </summary>
        public int PreviewImageCount { get; set; } = 9;

        /// <summary>
        /// 是否预览图片
        /// </summary>
        public int PreviewImageNow { get; set; } = 1;

        /// <summary>
        /// 关注用户名称
        /// </summary>
        public string ReadUserNameLike { get; set; }
    }
}
