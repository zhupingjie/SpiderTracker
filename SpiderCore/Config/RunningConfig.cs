using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SpiderCore.Config
{
    public class RunningConfig
    {
        public RunningConfig()
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
        /// 每页数量
        /// </summary>
        [OptionAttribute]
        public int ReadPageSize { get; set; } = 10;

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
        /// 已读微博采集
        /// </summary>
        [OptionAttribute]
        public bool GatherStatusWithNoSource { get; set; } = false;

        /// <summary>
        /// 更新同步资源
        /// </summary>
        [OptionAttribute]
        public bool GatherStatusUpdateLocalSource { get; set; } = false;

        /// <summary>
        /// 采集起始页码
        /// </summary>
        [OptionAttribute]
        public bool GatherContinueLastPage { get; set; } = false;

        /// <summary>
        /// 采集完成关机
        /// </summary>
        [OptionAttribute]
        public bool GatherUserNewPublishTime { get; set; } = false;

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
        /// 空闲读取下一条微博等待时间(毫秒)
        /// </summary>
        public int ReadFreeWaitMilSecond { get; set; } = 500;

        /// <summary>
        /// 下载下一个资源等待时间(毫秒)
        /// </summary>
        public int DownloadSourceWaitMilSecond { get; set; } = 200;

        /// <summary>
        /// 空闲上传下一条微博等待时间(毫秒)
        /// </summary>
        public int UploadFreeWaitSecond { get; set; } = 5;

        /// <summary>
        /// 下载下一个资源等待时间(毫秒)
        /// </summary>
        public int UploadSourceWaitMilSecond { get; set; } = 200;

        /// <summary>
        /// 读取图片最小尺寸
        /// </summary>
        public int MinReadImageSize { get; set; } = 480;

        /// <summary>
        /// 读取图片最大尺寸
        /// </summary>
        public int MaxReadImageSize { get; set; } = 99999;
        
        /// <summary>
        /// 是否预览图片
        /// </summary>
        public int PreviewImageNow { get; set; } = 1;

        /// <summary>
        /// 采集用户最少微博数量
        /// </summary>
        public int GatherUserMinStatuses { get; set; } = 10;

        public string DefaultUploadServerIP { get; set; } = "49.232.192.220:8088";//"localhost:8080";

        /// <summary>
        /// 上传图片api
        /// </summary>
        public string DefaultUploadImageAPI { get; set; } = "api/Spider/UploadSinaSources";

        /// <summary>
        /// 删除图片api
        /// </summary>
        public string DefaultDeleteImageAPI { get; set; } = "api/Spider/DeleteSinaSources";

        /// <summary>
        /// 获取图片api
        /// </summary>
        public string DefaultGetImageAPI { get; set; } = "api/Spider/GetSinaSources";

        /// <summary>
        /// 默认存档临时文件夹
        /// </summary>
        public string DefaultUploadPath { get; set; } = "@upload";

        /// <summary>
        /// 默认window背景桌面图片目录
        /// </summary>
        public string DefaultWallpaperPath { get; set; } = "@wallpaper";

        /// <summary>
        /// 缩略图宽度
        /// </summary>
        public int ThumbnailImageWidth { get; set; } = 180;

        /// <summary>
        /// 缩略图高度
        /// </summary>
        public int ThumbnailImageHeight { get; set; } = 220;

        /// <summary>
        /// 采集用户名称
        /// </summary>
        public string ReadUserNameLike { get; set; } = "jk,ol,cos,leg,stock,腿,丝,袜,萌,酱,萝莉,制服,私房,写真,约拍";


        /// <summary>
        /// 桌面背景显示时间间隔
        /// </summary>
        public int ShowWinBackgoundIntervalSencond { get; set; } = 300;

        /// <summary>
        /// 查看在线资源图片宽度
        /// </summary>
        public int DefaultDisplayWebImageWidth { get; set; } = 180;

        /// <summary>
        /// 查看在线资源图片高度
        /// </summary>
        public int DefaultDisplayWebImageHeight { get; set; } = 260;


        /// <summary>
        /// 查看在线资源图片一行多少张
        /// </summary>
        public int DefaultDisplayWebImageRowCount { get; set; } = 5;

        #endregion

        #region 内置函数

        public RunningConfig Clone()
        {
            RunningConfig runningConfig = new RunningConfig();
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

        #region 服务配置


        public int GatherUserDataServiceInterval { get; set; } = 3 * 60;

        #endregion
    }

    public class OptionAttribute:Attribute
    {

    }
}
