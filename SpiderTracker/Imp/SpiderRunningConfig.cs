using SpiderTracker.Imp.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SpiderTracker.Imp
{
    public class SpiderRunningConfig
    {
        public SpiderRunningConfig()
        {
        }

        #region 运行实例(任务运行时无效)

        /// <summary>
        /// 身份标识
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 采集来源
        /// </summary>
        public string Site { get; set; } = "user";

        /// <summary>
        /// 用户分类
        /// </summary>
        public string Category { get; set; } = "cosplay";

        #endregion

        #region 可选配置

        /// <summary>
        /// 并发用户数量
        /// </summary>
        [OptionAttribute]
        public int GatherThreadCount { get; set; } = 5;

        /// <summary>
        /// 起始页面
        /// </summary>
        [OptionAttribute]
        public int StartPageIndex { get; set; } = 1;

        /// <summary>
        /// 读取页数
        /// </summary>
        [OptionAttribute]
        public int MaxReadPageCount { get; set; } = 3;

        /// <summary>
        /// 读取最小图数
        /// </summary>
        [OptionAttribute]
        public int MinReadImageCount { get; set; } = 3;

        /// <summary>
        /// 采集原创图集
        /// </summary>
        [OptionAttribute]
        public bool ReadOwnerUserStatus { get; set; } = true;

        /// <summary>
        /// 采集所有用户
        /// </summary>
        [OptionAttribute]
        public bool ReadAllOfUser { get; set; } = false;
        /// <summary>
        /// 采集用户关注
        /// </summary>
        [OptionAttribute]
        public bool ReadUserOfHeFocus { get; set; } = false;
        /// <summary>
        /// 采集我的关注
        /// </summary>
        [OptionAttribute]
        public bool ReadUserOfMyFocus { get; set; } = false;

        /// <summary>
        /// 忽略存档图集
        /// </summary>
        [OptionAttribute]
        public bool IgnoreReadArchiveStatus { get; set; } = true;

        /// <summary>
        /// 忽略已采图集
        /// </summary>
        [OptionAttribute]
        public bool IgnoreReadGetStatus { get; set; } = true;

        /// <summary>
        /// 忽略本地图集
        /// </summary>
        [OptionAttribute]
        public bool IgnoreReadDownStatus { get; set; } = false;

        /// <summary>
        /// 忽略下载资源
        /// </summary>
        [OptionAttribute]
        public bool IgnoreDownloadSource { get; set; } = false;

        /// <summary>
        /// 断点续传采集
        /// </summary>
        [OptionAttribute]
        public bool GatherContinueLastPage { get; set; } = false;

        /// <summary>
        /// 采集完成关机
        /// </summary>
        [OptionAttribute]
        public bool GatherCompleteShutdown { get; set; } = false;


        #endregion

        #region 默认配置

        /// <summary>
        /// 读取下一页等待时间(秒)
        /// </summary>
        public int ReadNextPageWaitSecond { get; set; } = 3;

        /// <summary>
        /// 读取下一条等待时间(秒)
        /// </summary>
        public int ReadNextStatusWaitSecond { get; set; } = 3;

        /// <summary>
        /// 读取图片最小尺寸
        /// </summary>
        public int MinReadImageSize { get; set; } = 480;

        /// <summary>
        /// 读取图片最大尺寸
        /// </summary>
        public int MaxReadImageSize { get; set; } = 99999;

        /// <summary>
        /// 预览图片数量
        /// </summary>
        public int PreviewImageCount { get; set; } = 9;

        /// <summary>
        /// 是否预览图片
        /// </summary>
        public int PreviewImageNow { get; set; } = 1;

        /// <summary>
        /// 采集用户最少微博数量
        /// </summary>
        public int GatherUserMinStatuses { get; set; } = 50;

        /// <summary>
        /// 默认存档临时文件夹
        /// </summary>
        public string DefaultArchivePath { get; set; } = "archive";

        /// <summary>
        /// 缩略图宽度
        /// </summary>
        public int ThumbnailImageWidth { get; set; } = 138;

        /// <summary>
        /// 缩略图高度
        /// </summary>
        public int ThumbnailImageHeight { get; set; } = 190;

        /// <summary>
        /// 采集用户名称
        /// </summary>
        public string ReadUserNameLike { get; set; } = "jk,ol,cos,leg,stock,腿,丝,袜,萌,酱,萝莉,制服,私房,写真,约拍";


        #endregion

        #region 内置函数

        public SpiderRunningConfig Clone()
        {
            SpiderRunningConfig runningConfig = new SpiderRunningConfig();
            var ps = this.GetType().GetProperties();
            foreach (var p in ps)
            {
                var findP = runningConfig.GetType().GetProperty(p.Name);
                if (findP == null) continue;


                if (findP.GetSetMethod() != null)
                {
                    findP.SetValue(runningConfig, p.GetValue(this, null), null);
                }
            }
            runningConfig.Id = DateTime.Now.Ticks;
            return runningConfig;
        }

        #endregion

        #region 登陆认证

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

        #endregion
    }

    public class OptionAttribute:Attribute
    {

    }
}
