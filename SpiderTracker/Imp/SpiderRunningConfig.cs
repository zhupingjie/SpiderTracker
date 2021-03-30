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

        public GatherTypeEnum GatherType { get; set; } = GatherTypeEnum.SingleGather;

        public long Id { get; set; }

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


        ///// <summary>
        ///// 用户ID集合
        ///// </summary>
        public string[] UserIds { get; set; }

        public List<string> RunUserIds { get; set; } = new List<string>();

        /// <summary>
        /// 待处理用户集合
        /// </summary>
        public ConcurrentQueue<string> DoUserIds { get; set; } = new ConcurrentQueue<string>();

        public ConcurrentDictionary<int, ThreadState> DoTasks { get; set; } = new ConcurrentDictionary<int, ThreadState>();

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
        public int OnlyReadOwnerUser { get; set; } = 1;

        /// <summary>
        /// 采集原创视频
        /// </summary>
        public int ReadUserPics { get; set; } = 1;

        /// <summary>
        /// 采集视频
        /// </summary>
        public int ReadUserVideo { get; set; } = 1;

        /// <summary>
        /// 采集所有用户
        /// </summary>
        public int ReadAllOfUser { get; set; } = 0;
        /// <summary>
        /// 采集用户关注
        /// </summary>
        public int ReadUserOfHeFocus { get; set; } = 0;
        /// <summary>
        /// 采集用户关注
        /// </summary>
        public int ReadUserOfMyFocus { get; set; } = 0;

        /// <summary>
        /// 忽略存档图集
        /// </summary>
        public int IgnoreReadArchiveStatus { get; set; } = 0;

        /// <summary>
        /// 忽略已删图集
        /// </summary>
        public int IgnoreDeleteSourceStatus { get; set; } = 0;


        /// <summary>
        /// 预览图片数量
        /// </summary>
        public int PreviewImageCount { get; set; } = 9;

        /// <summary>
        /// 是否预览图片
        /// </summary>
        public int PreviewImageNow { get; set; } = 1;


        /// <summary>
        /// 并发用户数量
        /// </summary>
        public int MaxReadUserThreadCount { get; set; } = 1;

        /// <summary>
        /// 关注用户名称
        /// </summary>
        public string ReadUserNameLike { get; set; }

        public string DefaultArchivePath { get; set; }

        public int LoadUserCount { get; set; } = 100;

        public void Reset()
        {
            this.DoUserIds = new ConcurrentQueue<string>();
            this.RunUserIds.Clear();
            this.DoTasks.Clear();
        }

        public bool AddUser(string uid, bool append = true)
        {
            if (!string.IsNullOrEmpty(uid) && !this.DoUserIds.Contains(uid))
            {
                if (!this.RunUserIds.Contains(uid))
                {
                    this.DoUserIds.Enqueue(uid);
                    this.RunUserIds.Add(uid);
                    return true;
                }
            }
            return false;
        }

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
    }
}
