using Jurassic.Library;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpiderTracker.Imp.MWeiboJson;
using System.Drawing;
using System.Threading;
using SpiderTracker.Imp.Model;
using System.Collections.Specialized;
using SpiderTracker.UI;

namespace SpiderTracker.Imp
{
    public class MWeiboSpiderService : ISpiderService
    {
        #region Spider Event

        public delegate void ShowStatusEventHander(string msg, bool bLog = false, Exception ex = null);

        public event ShowStatusEventHander OnShowStatus;

        public void ShowStatus(string msg, bool bLog = true, Exception ex = null)
        {
            OnShowStatus?.Invoke($"[{Thread.CurrentThread.ManagedThreadId}]{msg}", bLog, ex);
        }

        public delegate void RefreshConfigEventHander(SpiderRunningConfig spiderRunninConfig);

        public event RefreshConfigEventHander OnRefreshConfig;

        public void RefreshConfig(SpiderRunningConfig spiderRunninConfig)
        {
            OnRefreshConfig?.Invoke(spiderRunninConfig);
        }


        public delegate void SpiderStartedEventHander(SpiderRunningConfig spiderRunninConfig);

        public event SpiderStartedEventHander OnSpiderStarted;

        public void SpiderStarted(IList<SinaUser> users)
        {
            IsSpiderStarted = true;
            StopSpiderWork = false;

            var readUsers = new List<SinaUser>();
            if(users != null && users.Count> 0)
            {
                foreach(var user in users)
                {
                    RunningConfig.AddUser(user);
                }
            }
            if (RunningConfig.ReadAllOfUser == 1)
            {
                readUsers = Repository.GetUsers(RunningConfig.Category);
                //if (!string.IsNullOrEmpty(RunningConfig.ReadUserNameLike))
                //{
                //    readUsers = readUsers.Where(c => (c.name.Contains(RunningConfig.ReadUserNameLike) || c.desc.Contains(RunningConfig.ReadUserNameLike))).ToList();
                //}
                foreach (var user in readUsers.OrderBy(c=>c.lastdate).ToArray())
                {
                    RunningConfig.AddUser(user);
                }
            }
            if(RunningConfig.ReadUserOfMyFocus == 1)
            {
                readUsers = Repository.GetFocusUsers(RunningConfig.Category);
                //if (!string.IsNullOrEmpty(RunningConfig.ReadUserNameLike))
                //{
                //    readUsers = FilterReadUser(readUsers, RunningConfig.ReadUserNameLike);
                //}
                foreach (var user in readUsers)
                {
                    RunningConfig.AddUser(user);
                }
            }
            if (RunningConfig.ReadUserOfHeFocus == 1)
            {
                var focusUser = GatherHeFocusUsers(RunningConfig);
                if (!string.IsNullOrEmpty(RunningConfig.ReadUserNameLike))
                {
                    focusUser = FilterReadUser(focusUser, RunningConfig.ReadUserNameLike);
                }
                foreach (var user in focusUser.OrderBy(c => c.lastdate).ToArray())
                {
                    RunningConfig.AddUser(user);
                }
            }
            if (OnSpiderStarted != null)
            {
                OnSpiderStarted?.Invoke(RunningConfig);
            }
        }


        List<SinaUser> FilterReadUser(IList<SinaUser> users, string filter)
        {
            var lst = new List<SinaUser>();
            var keys = filter.Split(new string[] { "," }, StringSplitOptions.None);
            foreach(var key in keys)
            {
                var us = users.Where(c => c.name.Contains(key) || c.desc.Contains(key)).ToArray();
                foreach(var u in us)
                {
                    if (!lst.Any(c => c.uid == u.uid))
                        lst.Add(u);
                }
            }
            return lst;
        }

        bool CheckReadUser(MWeiboUser user, string filter)
        {
            if (string.IsNullOrEmpty(filter)) return true;

            var keys = filter.Split(new string[] { "," }, StringSplitOptions.None);
            foreach (var key in keys)
            {
                if (user.screen_name.Contains(key) || user.screen_name.Contains(key)) return true;
            }
            return false;
        }


        public delegate void SpiderCompleteEventHander();

        public event SpiderCompleteEventHander OnSpiderComplete;

        public void SpiderComplete()
        {
            IsSpiderStarted = false;
            OnSpiderComplete?.Invoke();
        }

        public delegate void SpiderStopingEventHander();

        public event SpiderStopingEventHander OnSpiderStoping;

        public void SpiderStoping()
        {
            OnSpiderStoping?.Invoke();
        }

        public delegate void SpiderGatherNewUserEventHander(SinaUser user);

        public event SpiderGatherNewUserEventHander OnGatherNewUser;

        public void GatherNewUser(SinaUser user)
        {
            OnGatherNewUser?.Invoke(user);
        }

        public delegate void SpiderGatherNewStatusEventHander(string uid, int readImageQty);

        public event SpiderGatherNewStatusEventHander OnGatherStatusComplete;

        public void GatherStatusComplete(string uid, int readImageQty)
        {
            OnGatherStatusComplete?.Invoke(uid, readImageQty);
        }

        public delegate void SpiderGatherPageCompleteEventHander(string uid, int pageIndex, int readImageQty);

        public event SpiderGatherPageCompleteEventHander OnGatherPageComplete;

        public void GatherPageComplete(string uid, int pageIndex, int readImageQty)
        {
            OnGatherPageComplete?.Invoke(uid, pageIndex, readImageQty);
        }

        public delegate void SpiderGatherUserCompleteEventHander(SinaUser user, int readImageQty);

        public event SpiderGatherUserCompleteEventHander OnGatherUserComplete;

        public void GatherUserComplete(SinaUser user, int readImageQty)
        {
            OnGatherUserComplete?.Invoke(user, readImageQty);
        }

        public delegate void SpiderGatherUserStartedEventHander(SinaUser user);

        public event SpiderGatherUserStartedEventHander OnGatherUserStarted;
        public void GatherUserStarted(SinaUser user)
        {
            OnGatherUserStarted?.Invoke(user);
        }

        public delegate void SpiderGatherAppendUserEventHander(SinaUser user);

        public event SpiderGatherAppendUserEventHander OnGatherAppendUser;

        #endregion

        #region 内部属性
        /// <summary>
        /// 标记是否已经启动
        /// </summary>
        public bool IsSpiderStarted { get; set; }

        /// <summary>
        /// 标记是否停止工作
        /// </summary>
        protected bool StopSpiderWork { get; set; } = false;

        /// <summary>
        /// 停止采集用户
        /// </summary>
        protected List<string> CancelUsers { get; set; }

        /// <summary>
        /// 负责存储数据
        /// </summary>
        public SinaRepository Repository { get; set; }

        public SpiderRunningConfig RunningConfig { get; set; }

        #endregion

        #region 构造函数
        public MWeiboSpiderService()
        {
            Repository = new SinaRepository();

            CancelUsers = new List<string>();
        }

        #endregion

        #region 开始&结束&追加&取消采集

        public void StartSpider(SpiderRunningConfig runningConfig, IList<SinaUser> users, IList<string> statusIds)
        {
            this.CancelUsers.Clear();
            this.RunningConfig = runningConfig;
            this.RunningConfig.Reset();

            if (string.IsNullOrEmpty(RunningConfig.Category))
            {
                ShowStatus("采集类目为空!");
                return;
            }

            Task.Factory.StartNew(() =>
            {
                SpiderStarted(users);

                if(RunningConfig.GatherType == GatherTypeEnum.GahterStatus)
                {
                    StartAutoGatherByStatus(statusIds);
                }
                else
                {
                    StartAutoGatherByUser();
                }
                SpiderComplete();

                if (RunningConfig.GatherCompleteShutdown > 0 && !StopSpiderWork)
                {
                    PathUtil.Shutdown();
                }
            });
        }
        public void StopSpider()
        {
            StopSpiderWork = true;

            SpiderStoping();
        }

        public void CancelUser(string uid)
        {
            if (!CancelUsers.Contains(uid))
            {
                CancelUsers.Add(uid);
            }
        }

        protected bool CheckUserCanceled(string uid)
        {
            return this.CancelUsers.Contains(uid);
        }

        public void AppendUser(SinaUser user)
        {
            if (this.RunningConfig.AddUser(user))
            {
                if (OnGatherAppendUser != null)
                {
                    OnGatherAppendUser?.Invoke(user);
                }
            }
        }

        protected SinaUser PeekUser()
        {
            SinaUser user = null;
            RunningConfig.DoUsers.TryDequeue(out user);
            return user;
        }

        void StartAutoGatherByStatus(IList<string> statusIds)
        {
            var threads = new List<Task>();
            var task = Task.Factory.StartNew(() =>
            {
                ShowStatus($"准备读取用户的微博数据...");
                int readStatusImageCount = 0;
                foreach (var status in statusIds)
                {
                    var tempRuningConfig = RunningConfig.Clone();
                    tempRuningConfig.StartUrl = $"https://m.weibo.cn/status/{status}";
                    readStatusImageCount += GatherSinaStatusByStatusUrl(tempRuningConfig);
                }
                ShowStatus($"采集完成,共采集资源【{readStatusImageCount}】.");
            });
            threads.Add(task);
            Task.WaitAll(threads.ToArray());
        }

        void StartAutoGatherByUser()
        {
            ShowStatus($"准备读取用户的微博数据...");

            var threads = new List<Task>();
            var maxReadUserCount = RunningConfig.MaxReadUserThreadCount;
            for (var i = 0; i < maxReadUserCount; i++)
            {
                var task = Task.Factory.StartNew(() =>
                {
                    RunningConfig.DoTasks.TryAdd(Thread.CurrentThread.ManagedThreadId, ThreadState.Running);
                    StartSpiderGatherTaskThread(Thread.CurrentThread.ManagedThreadId);
                });
                threads.Add(task);
            }
            Task.WaitAll(threads.ToArray());
        }

        public void StartSpiderGatherTaskThread(int taskIndex)
        {
            while (!StopSpiderWork)
            {
                var user = PeekUser();
                if (user == null)
                {
                    if(!RunningConfig.DoTasks.Any(c=>c.Value == ThreadState.Running))
                    {
                        break;
                    }
                    else
                    {
                        RunningConfig.DoTasks.TryUpdate(taskIndex, ThreadState.Suspended, ThreadState.Running);
                        Thread.Sleep(5 * 1000);
                        continue;
                    }
                }
                else
                {
                    RunningConfig.DoTasks.TryUpdate(taskIndex, ThreadState.Running, ThreadState.Suspended);
                }

                GatherUserStarted(user);
                
                if (!CheckUserCanceled(user.uid))
                {
                    var tempRuningConfig = RunningConfig.Clone();
                    tempRuningConfig.StartUrl = SinaUrlUtil.GetSinaUserUrl(user.uid);
                    var readStatusImageCount = GatherSinaStatusByUserUrl(tempRuningConfig);

                    //更新用户微博信息
                    user = Repository.UpdateSinaUserQty(user);

                    GatherUserComplete(user, readStatusImageCount);

                    ShowStatus($"用户[{user.uid}]采集完成,共采集资源【{readStatusImageCount}】.");
                }
                else
                {
                    GatherUserComplete(user, 0);
                }
            }
        }

        #endregion

        #region 采集并解析微博

        /// <summary>
        /// 直接读取用户数据
        /// </summary>
        /// <param name="runningConfig"></param>
        int GatherSinaStatusByUserUrl(SpiderRunningConfig runningConfig)
        {
            var userId = SinaUrlUtil.GetSinaUserByStartUrl(runningConfig.StartUrl);
            var user = GatherSinaUserByUserUrl(runningConfig, userId);
            if (user == null) return 0;

            var sinaUser = Repository.GetUser(user.id);
            if (sinaUser != null && sinaUser.ignore > 0)
            {
                ShowStatus($"用户【{user.id}】已忽略采集.");
                return 0;
            }

            bool hasReadLastPage = false;
            if (sinaUser != null && sinaUser.lastpage > 0) hasReadLastPage = true;

            int readUserImageCount = 0, readPageIndex = 0, emptyPageCount = 0;
            int readPageCount = (runningConfig.ReadPageCount == 0 ? int.MaxValue : runningConfig.StartPageIndex + runningConfig.ReadPageCount);
            for (readPageIndex = runningConfig.StartPageIndex; readPageIndex < readPageCount; readPageIndex++)
            {
                if (StopSpiderWork)
                {
                    ShowStatus($"中止采集用户微博数据...");
                    break;
                }
                if (CheckUserCanceled(userId))
                {
                    ShowStatus($"取消采集用户微博数据...");
                    break;
                }

                bool readPageEmpty = false, stopReadNextPage = false;
                int readPageImageCount = GatherSinaStatusByStatusPageUrl(runningConfig, user, readPageIndex, readPageCount, hasReadLastPage, out readPageEmpty, out stopReadNextPage);
                readUserImageCount += readPageImageCount;

                GatherPageComplete(user.id, readPageIndex, readUserImageCount);

                if (stopReadNextPage)
                {
                    ShowStatus($"结束采集用户微博数据(下页已采集)...");
                    break;
                }

                if (readPageEmpty) emptyPageCount++;
                else emptyPageCount = 0;

                if (emptyPageCount > 3)
                {
                    if (runningConfig.ReadPageCount == 0)
                    {
                        Repository.UpdateUserLastpage(userId);
                    }
                    ShowStatus($"中止采集用户微博数据(连续超过3页无数据)...");
                    break;
                }

                if (StopSpiderWork)
                {
                    ShowStatus($"中止采集用户微博数据...");
                    break;
                }
                if (CheckUserCanceled(userId))
                {
                    ShowStatus($"取消采集用户微博数据...");
                    break;
                }
                if (readPageIndex + 1 < readPageCount && readPageImageCount > 0)
                {
                    Repository.UpdateSinaUserQty(userId);

                    ShowStatus($"等待【{runningConfig.ReadNextPageWaitSecond}】秒读取用户【{userId}】下一页微博数据...");
                    Thread.Sleep(runningConfig.ReadNextPageWaitSecond * 1000);
                }
                else
                {
                    Thread.Sleep(500);
                }
            }
            return readUserImageCount;
        }

        MWeiboUser GatherSinaUserByUserUrl(SpiderRunningConfig runningConfig, string userId)
        {
            ShowStatus($"开始读取用户【{userId}】的微博信息...", true);
            var getApi = $"https://m.weibo.cn/api/container/getIndex?type=uid&value={userId}&containerid=100505{userId}";
            var html = HttpUtil.GetHttpRequestHtmlResult(getApi, runningConfig);
            if (html == null)
            {
                ShowStatus($"获取用户信息错误!!!!!!");
                return null;
            }
            var result = GetWeiboUserResult(html);
            if (result == null || result.data == null || result.data.userInfo == null)
            {
                ShowStatus($"解析用户信息错误!!!!!!");
                return null;
            }
            var user = result.data.userInfo;

            var  sinaUser = Repository.StoreSinaUser(runningConfig, user);
            if (sinaUser == null)
            {
                ShowStatus($"存储用户信息错误!!!!!!");
                return null;
            }
            if (sinaUser.id == 0)
            {
                GatherNewUser(sinaUser);
            }
            return user;
        }


        /// <summary>
        /// 采集用户微博列表数据
        /// </summary>
        /// <param name="runningConfig"></param>
        /// <param name="user"></param>
        /// <param name="readPageIndex"></param>
        /// <param name="readPageCount"></param>
        /// <returns></returns>
        int GatherSinaStatusByStatusPageUrl(SpiderRunningConfig runningConfig, MWeiboUser user, int readPageIndex, int readPageCount, bool hasReadLastPage, out bool readPageEmpty, out bool stopReadNextPage)
        {
            readPageEmpty = false;
            stopReadNextPage = false;
            runningConfig.CurrentPageIndex = readPageIndex;
            RefreshConfig(runningConfig);

            ShowStatus($"开始读取用户【{user.id}】的第【{readPageIndex}】页微博数据...", true);
            var getApi = $"https://m.weibo.cn/api/container/getIndex?type=uid&value={user.id}&containerid=107603{user.id}&page={readPageIndex}";
            var html = HttpUtil.GetHttpRequestHtmlResult(getApi, runningConfig);
            if (html == null)
            {
                ShowStatus($"获取用户微博列表错误!!!!!!");
                return 0;
            }
            var result = GetSinaStatusListResult(html);
            if (result == null || result.data == null)
            {
                ShowStatus($"解析用户微博列表错误!!!!!!");
                return 0;
            }
            if (result.data.cards.Length == 0)
            {
                ShowStatus($"解析用户微博列表为空...");
                readPageEmpty = true;
                return 0;
            }
            int readPageImageCount = 0;
            foreach (var card in result.data.cards)
            {
                //非微博数据，跳过
                if (card.card_type != 9) continue;

                if (StopSpiderWork)
                {
                    ShowStatus($"中止采集微博数据...");
                    break;
                }
                if (CheckUserCanceled(user.id))
                {
                    ShowStatus($"取消采集微博数据...");
                    break;
                }
                bool ignoreSourceReaded = false;
                var readStatusImageCount = GatherSinaStatusByStatusOrRetweeted(runningConfig, card.mblog, user, out ignoreSourceReaded);
                readPageImageCount += readStatusImageCount;

                if (StopSpiderWork)
                {
                    ShowStatus($"中止采集微博数据...");
                    break;
                }
                if (CheckUserCanceled(user.id))
                {
                    ShowStatus($"取消采集微博数据...");
                    break;
                }
                if (ignoreSourceReaded && hasReadLastPage)
                {
                    stopReadNextPage = true;
                    ShowStatus($"结束采集微博数据(下页已采集)...");
                    break;
                }
                if (readStatusImageCount > 0)
                {
                    ShowStatus($"等待【{runningConfig.ReadNextStatusWaitSecond}】秒读取用户【{user.id}】下一条微博数据...");
                    Thread.Sleep(runningConfig.ReadNextStatusWaitSecond * 1000);
                }
                else
                {
                    Thread.Sleep(500);
                }
            }
            return readPageImageCount;
        }

        /// <summary>
        /// 采集微博详细数据（通过URL）
        /// </summary>
        /// <param name="runningConfig"></param>
        int GatherSinaStatusByStatusUrl(SpiderRunningConfig runningConfig)
        {
            bool ignoreSourceReaded = false;
            var html = HttpUtil.GetHttpRequestHtmlResult(runningConfig.StartUrl, runningConfig);
            if (html == null)
            {
                ShowStatus($"获取用户微博列表错误!!!!!!");
                return 0;
            }
            var result = GetWeiboStatusResult(html);
            if (result == null || result.status == null)
            {
                ShowStatus($"解析用户微博列表错误!!!!!!");
                return 0;
            }
            var user = result.status.user;
            if (user == null)
            {
                ShowStatus($"微博数据已删除.");
                return 0;
            }
            if (Repository.CheckUserIgnore(user.id))
            {
                ShowStatus($"用户【{user.id}】已忽略采集.");
                return 0;
            }
            return GatherSinaStatusByStatusOrRetweeted(runningConfig, result.status, result.status.user, out ignoreSourceReaded);
        }

        /// <summary>
        /// 采用微博详细数据（通过Result）
        /// </summary>
        /// <param name="runningConfig"></param>
        /// <param name="status"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        int GatherSinaStatusByStatusOrRetweeted(SpiderRunningConfig runningConfig, MWeiboStatus status, MWeiboUser user, out bool ignoreSourceReaded)
        {
            ignoreSourceReaded = false;
            int readStatusImageCount = 0;
            //非转发微博
            if (status.retweeted_status == null)
            {
                if (status.pics != null)
                {
                    readStatusImageCount = GatherSinaStatusPicsByStatusResult(runningConfig, status, out ignoreSourceReaded);
                    if (readStatusImageCount > 0)
                    {
                        GatherStatusComplete(user.id, readStatusImageCount);
                    }
                }
                else if (status.page_info != null && status.page_info.urls != null)
                {
                    readStatusImageCount = GatherSinaStatusViedoByStatusResult(runningConfig, status, out ignoreSourceReaded);
                    if (readStatusImageCount > 0)
                    {
                        GatherStatusComplete(user.id, readStatusImageCount);
                    }
                }
            }
            else
            {
                //存储当前转发微博数据
                Repository.StoreSinaRetweetStatus(runningConfig, user, status, status.retweeted_status, 0);

                if (status.retweeted_status.user == null)
                {
                    ShowStatus($"微博数据已删除.");
                    return 0;
                }
                if (runningConfig.OnlyReadOwnerUser == 1 && status.retweeted_status.user.id != user.id)
                {
                    ShowStatus($"跳过非本用户【{status.retweeted_status.user.id}】转发数据.");
                    return 0;
                }
                if (!CheckReadUser(status.retweeted_status.user, runningConfig.ReadUserNameLike))
                {
                    ShowStatus($"跳过未包含关键字用户【{status.retweeted_status.user.id}】转发数据.");
                    return 0;
                }
                if (Repository.CheckUserIgnore(status.retweeted_status.user.id))
                {
                    ShowStatus($"用户【{status.retweeted_status.user.id}】已忽略采集.");
                    return 0;
                }
                //存储转发用户信息
                var sinaUser = Repository.StoreSinaUser(runningConfig, status.retweeted_status.user);
                if (sinaUser == null)
                {
                    ShowStatus($"存储用户信息错误!!!!!!");
                    return 0;
                }
                if (sinaUser.id == 0)
                {
                    GatherNewUser(sinaUser);
                }

                if (status.retweeted_status.pics != null)
                {
                    readStatusImageCount = GatherSinaStatusPicsByStatusResult(runningConfig, status.retweeted_status, out ignoreSourceReaded);
                    if (readStatusImageCount > 0)
                    {
                        GatherStatusComplete(user.id, readStatusImageCount);
                    }
                }
                else if (status.retweeted_status.page_info != null && status.retweeted_status.page_info.urls != null)
                {
                    readStatusImageCount = GatherSinaStatusViedoByStatusResult(runningConfig, status.retweeted_status, out ignoreSourceReaded);
                    if (readStatusImageCount > 0)
                    {
                        GatherStatusComplete(user.id, readStatusImageCount);
                    }
                }
            }
            return readStatusImageCount > 0 ? readStatusImageCount : 0;
        }

        /// <summary>
        /// 采集微博详细信息
        /// </summary>
        /// <param name="runninConfig"></param>
        /// <param name="status"></param>
        /// <returns>
        /// -1:忽略
        /// 0:未采集到有效图片,需忽略
        /// </returns>
        int GatherSinaStatusPicsByStatusResult(SpiderRunningConfig runninConfig, MWeiboStatus status, out bool ignoreSourceReaded)
        {
            ignoreSourceReaded = false;

            var user = status.user;
            if (user == null)
            {
                ShowStatus($"微博数据已删除.");
                return 0;
            }
            if (Repository.CheckUserIgnore(user.id))
            {
                ShowStatus($"用户【{user.id}】已忽略采集.");
                return 0;
            }
            if (status.pics == null || status.pics.Length < runninConfig.ReadMinOfImgCount)
            {
                ShowStatus($"跳过不符合最小图数微博【{status.bid}】.");
                return 0;
            }
            var sinaStatus = Repository.GetUserStatus(status.bid);
            if (sinaStatus != null)
            {
                if (sinaStatus.ignore > 0)
                {
                    ShowStatus($"跳过已忽略微博【{status.bid}】.");
                    return 0;
                }
                if (runninConfig.IgnoreReadArchiveStatus == 1 && sinaStatus.archive == 1)
                {
                    ShowStatus($"跳过已存档微博【{status.bid}】.");
                    return 0;
                }
                if (runninConfig.IgnoreReadSourceStatus == 1 && sinaStatus.getqty > 0)
                {
                    ignoreSourceReaded = true;
                    ShowStatus($"跳过已采集微博【{status.bid}】.");
                    return 0;
                }
            }
            if (runninConfig.IgnoreDownloadSource == 1)
            {
                ShowStatus($"跳过下载微博资源【{status.bid}】.");
                //存储微博
                Repository.StoreSinaStatus(runninConfig, user, status, 0, status.pics.Length, 0, false);
                return 0;
            }
            else
            {
                var readCount = PathUtil.GetStoreUserImageFileCount(runninConfig.Category, user.id, status.bid);
                if (readCount > 0)
                {
                    ignoreSourceReaded = true;
                    ShowStatus($"跳过已缓存微博【{status.bid}】.");
                    Repository.StoreSinaStatus(runninConfig, user, status, 0, status.pics.Length, readCount, false);
                    return 0;
                }
                else
                {
                    ShowStatus($"开始采集用户【{user.id}】第【{runninConfig.CurrentPageIndex}】页微博【{status.bid}】...");
                    int haveReadImageCount = 0, readImageIndex = 0, errorReadImageCount = 0, errorSaveImageCount = 0;
                    foreach (var pic in status.pics)
                    {
                        //1:下载图片错误,2:保存数据错误,3:保存图片错误,
                        int errType = 0;
                        var succ = DownloadUserStatusImage(runninConfig, user.id, status.bid, pic.large.url, ++readImageIndex, out errType);
                        if (succ)
                        {
                            haveReadImageCount++;
                        }
                        if (errType > 0)
                        {
                            errorReadImageCount++;

                            if (errType == 3)
                            {
                                errorSaveImageCount++;
                            }
                        }
                        Thread.Sleep(200);
                    }
                    if (errorSaveImageCount > 0)
                    {
                        StopSpiderWork = true;
                        ShowStatus($"微博【{status.bid}】保存图片错误,停止采集!!!!!!");
                        return 0;
                    }
                    if (errorReadImageCount == status.pics.Length && errorReadImageCount > 0)
                    {
                        haveReadImageCount = -1;
                        ShowStatus($"微博【{status.bid}】已下载图片全部错误.");
                    }
                    if (haveReadImageCount == 0)
                    {
                        ShowStatus($"微博【{status.bid}】已无符合尺寸的图片.");
                        PathUtil.DeleteStoreUserImageFiles(runninConfig.Category, user.id, status.bid);
                    }
                    else if (haveReadImageCount > 0 && haveReadImageCount < runninConfig.ReadMinOfImgCount)
                    {
                        ShowStatus($"微博【{status.bid}】已下载图数不符合删除.");
                        PathUtil.DeleteStoreUserImageFiles(runninConfig.Category, user.id, status.bid);

                        haveReadImageCount = 0;
                    }
                    //存储微博
                    Repository.StoreSinaStatus(runninConfig, user, status, 0, status.pics.Length, haveReadImageCount, haveReadImageCount == 0);
                    return haveReadImageCount;
                }
            }
        }

        int GatherSinaStatusViedoByStatusResult(SpiderRunningConfig runninConfig, MWeiboStatus status, out bool ignoreSourceReaded)
        {
            ignoreSourceReaded = false;
            var user = status.user;
            if (user == null)
            {
                ShowStatus($"微博数据已删除.");
                return 0;
            }
            if (Repository.CheckUserIgnore(user.id))
            {
                ShowStatus($"用户【{user.id}】已忽略采集.");
                return 0;
            }
            if (status.page_info.urls == null || string.IsNullOrEmpty(status.page_info.urls.mp4_hd_mp4) || string.IsNullOrEmpty(status.page_info.urls.mp4_ld_mp4))
            {
                ShowStatus($"跳过无链接视频【{status.bid}】.");
                return 0;
            }
            var sinaStatus = Repository.GetUserStatus(status.bid);
            if (sinaStatus != null)
            {
                if (sinaStatus.ignore > 0)
                {
                    ShowStatus($"跳过已忽略视频【{status.bid}】.");
                    return 0;
                }
                if (runninConfig.IgnoreReadArchiveStatus == 1 && sinaStatus.archive == 1)
                {
                    ShowStatus($"跳过已存档视频【{status.bid}】.");
                    return 0;
                }
                if (runninConfig.IgnoreReadSourceStatus == 1 && sinaStatus.getqty > 0)
                {
                    ignoreSourceReaded = true;
                    ShowStatus($"跳过已采集视频【{status.bid}】.");
                    return 0;
                }
            }
            if (runninConfig.IgnoreDownloadSource == 1)
            {
                ShowStatus($"跳过下载微博资源【{status.bid}】.");
                //存储微博
                Repository.StoreSinaStatus(runninConfig, user, status, 1, 1, 0, false);
                return 0;
            }
            else
            {
                var readCount = PathUtil.GetStoreUserVideoCount(runninConfig.Category, user.id, status.bid);
                if (readCount > 0)
                {
                    ignoreSourceReaded = true;
                    ShowStatus($"跳过已缓存视频【{status.bid}】.");
                    Repository.StoreSinaStatus(runninConfig, user, status, 1, 1, readCount, false);
                    return 0;
                }
                else
                {
                    ShowStatus($"开始采集用户【{user.id}】第【{runninConfig.CurrentPageIndex}】页视频【{status.bid}】...");
                    int haveReadVedioCount = 0, errorReadVedioCount = 0;
                    var vedioUrl = status.page_info.urls.mp4_hd_mp4;
                    if (string.IsNullOrEmpty(vedioUrl)) vedioUrl = status.page_info.urls.mp4_ld_mp4;

                    var succ = DownloadUserStatusVedio(runninConfig, user.id, status.bid, vedioUrl);
                    if (succ)
                    {
                        haveReadVedioCount++;
                    }
                    else
                    {
                        errorReadVedioCount++;
                    }
                    Thread.Sleep(500);
                    if (errorReadVedioCount == 0)
                    {
                        //存储微博
                        Repository.StoreSinaStatus(runninConfig, user, status, 1, 1, haveReadVedioCount, haveReadVedioCount == 0);
                    }
                    return haveReadVedioCount;
                }
            }
        }

        List<SinaUser> GatherHeFocusUsers(SpiderRunningConfig runningConfig)
        {
            var focusUsers = new List<SinaUser>();

            foreach (var user in runningConfig.DoUsers)
            {
                int page = 0;
                while (++page > 0)
                {
                    ShowStatus($"开始读取{user.uid}关注的第{page}页用户信息...", true);
                    var getApi = $"https://m.weibo.cn/api/container/getIndex?containerid=231051_-_followers_-_{user.uid}_-_1042015%253AtagCategory_039&luicode=10000011&lfid=1076033810669779&page={page}";
                    var html = HttpUtil.GetHttpRequestHtmlResult(getApi, runningConfig);
                    if (html == null)
                    {
                        ShowStatus($"读取{user.uid}关注的第{page}页用户信息错误!");
                        return null;
                    }
                    var result = GetWeiboFocusResult(html);
                    if (result == null || result.data == null)
                    {
                        ShowStatus($"解析{user.uid}关注的第{page}页用户错误!");
                        return null;
                    }
                    var focusUserCard = result.data.cards.FirstOrDefault(c => c.card_type == 11 && c.card_style != 1);
                    if (focusUserCard == null)
                    {
                        break;
                    }
                    var users = focusUserCard.card_group.Select(c => new SinaUser()
                    {
                        uid = c.user.id,
                        name = c.user.screen_name,
                        desc = c.user.description,
                        category = runningConfig.Category
                    }).ToArray();
                    focusUsers.AddRange(users);
                }
            }
            return focusUsers.ToList();
        }

        MWeiboFocusResult GetWeiboFocusResult(string html)
        {
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);

            var jsonResult = Newtonsoft.Json.JsonConvert.DeserializeObject<MWeiboFocusResult>(doc.DocumentNode.InnerText) as MWeiboFocusResult;
            return jsonResult;
        }

        MWeiboFoucsUserResult GetWeiboFocusUserResult(string html)
        {
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);

            var jsonResult = Newtonsoft.Json.JsonConvert.DeserializeObject<MWeiboFoucsUserResult>(doc.DocumentNode.InnerText) as MWeiboFoucsUserResult;
            return jsonResult;
        }

        MWeiboUserResult GetWeiboUserResult(string html)
        {
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);

            var jsonResult = Newtonsoft.Json.JsonConvert.DeserializeObject<MWeiboUserResult>(doc.DocumentNode.InnerText) as MWeiboUserResult;
            return jsonResult;
        }

        MWeiboStatusListResult GetSinaStatusListResult(string html)
        {
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);

            var jsonResult = Newtonsoft.Json.JsonConvert.DeserializeObject<MWeiboStatusListResult>(doc.DocumentNode.InnerText) as MWeiboStatusListResult;
            return jsonResult;
        }

        MWeiboStatusResult GetWeiboStatusResult(string html)
        {
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);

            var scripts = doc.DocumentNode.Descendants().Where(c => c.Name == "script").Select(c => c.InnerText).ToArray();
            var engine = new Jurassic.ScriptEngine();

            foreach (var script in scripts)
            {
                if (script.Contains("config") && script.Contains("$render_data"))
                {
                    var js = script.Substring(script.IndexOf("var $render_data"));

                    var result = engine.Evaluate("(function() { " + js + " return $render_data; })()");
                    var json = JSONObject.Stringify(engine, result);

                    var jsonResult = Newtonsoft.Json.JsonConvert.DeserializeObject<MWeiboStatusResult>(json) as MWeiboStatusResult;
                    return jsonResult;
                }
            }
            return null;
        }

        /// <summary>
        /// 读取微博用户图片原始路径
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="cookie"></param>
        /// <returns></returns>
        bool DownloadUserStatusImage(SpiderRunningConfig runningConfig, string userId, string arcId, string imgUrl, int readImageIndex, out int errType)
        {
            errType = 0;

            var path = PathUtil.GetStoreUserPath(runningConfig.Category, userId);
            PathUtil.CheckCreateDirectory(path);
            var fileName = $"{arcId}_{readImageIndex}.jpg";
            var imgPath = Path.Combine(path, fileName);
            var image = HttpUtil.GetHttpRequestImageResult(imgUrl, runningConfig);
            if (image == null)
            {
                ShowStatus($"下载微博【{arcId}】第【{readImageIndex}】张图片错误!");
                errType = 1;
                return false;
            }
            if (!CheckImageSize(runningConfig, image, arcId, readImageIndex)) return false;

            var (thumbWidth, thumbHeight) = GetThumbImageSize(image, runningConfig.ThumbnailImageWidth, runningConfig.ThumbnailImageHeight);
            var thumbImg = image.GetThumbnailImage(thumbWidth, thumbHeight, null, IntPtr.Zero);
            var thunbPath = Path.Combine(path, "thumb");
            PathUtil.CheckCreateDirectory(thunbPath);
            var thumbPath = Path.Combine(thunbPath, fileName);

            try
            {
                image.Save(imgPath);
                thumbImg.Save(thumbPath);
                ShowStatus($"下载微博【{arcId}】第【{readImageIndex}】张图片(OK).");

                if (!Repository.ExistsSinaSource(userId, arcId, imgUrl))
                {
                    var sinaPicture = MakeSinaSource(userId, arcId, imgUrl, fileName, image);
                    var suc = Repository.CreateSinaSource(sinaPicture);
                    if (!suc)
                    {
                        if (File.Exists(imgPath)) File.Delete(imgPath);
                        if (File.Exists(thumbPath)) File.Delete(thumbPath);

                        image.Dispose();
                        thumbImg.Dispose();

                        ShowStatus($"创建微博【{arcId}】第【{readImageIndex}】张图片错误!!!!!!");
                        errType = 2;
                        return false;
                    }
                }
                return true;
            }
            catch(Exception ex)
            {
                errType = 3;
                if (File.Exists(imgPath)) File.Delete(imgPath);
                if (File.Exists(imgPath)) File.Delete(thumbPath);
                ShowStatus($"保存微博【{arcId}】第【{readImageIndex}】张图片错误!(未知错误)");
                return false;
            }
            finally
            {
                image.Dispose();
                thumbImg.Dispose();
            }
        }

        SinaSource MakeSinaSource(string userId, string arcId, string imgUrl, string fileName, Image image)
        {
            return new SinaSource()
            {
                uid = userId,
                bid = arcId,
                url = imgUrl,
                name = fileName,
                width = image.Width,
                height = image.Height,
                downdate = DateTime.Now
            };
        }

        (int width, int height) GetThumbImageSize(Image image, int thumbWidth, int thumbHeight)
        {
            var width = thumbWidth;
            var height = thumbHeight;
            var rate = image.Width * 1.0m / image.Height * 1.0m;

            if (image.Width > image.Height)
            {
                height = (int)(width / rate);
            }
            else
            {
                width = (int)(height * rate);
            }
            return (width, height);
        }

        bool CheckImageSize(SpiderRunningConfig runningConfig,Image image, string arcId, int readImageIndex)
        {
            if(image.Width <= runningConfig.ReadMinOfImgSize || image.Height <= runningConfig.ReadMinOfImgSize)
            {
                ShowStatus($"微博【{arcId}】第【{readImageIndex}】张图片尺寸太小忽略");
                return false;
            }
            if (image.Width > runningConfig.ReadMaxOfImgSize || image.Height > runningConfig.ReadMaxOfImgSize)
            {
                ShowStatus($"微博【{arcId}】第【{readImageIndex}】张图片尺寸太大忽略");
                return false;
            }
            return true;
        }

        /// <summary>
        /// 读取微博用户视频原始路径
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="cookie"></param>
        /// <returns></returns>
        bool DownloadUserStatusVedio(SpiderRunningConfig runningConfig, string userId, string arcId, string vedioUrl)
        {
            if (!Repository.ExistsSinaSource(userId, arcId, vedioUrl))
            {
                var sinaPicture = new SinaSource()
                {
                    uid = userId,
                    bid = arcId,
                    url = vedioUrl,
                    name = $"{arcId}_1.mp4"
                };
                var suc = Repository.CreateSinaSource(sinaPicture);
                if (!suc)
                {
                    ShowStatus($"创建本地微博视频错误!!!!!!");
                    return false;
                }
            }
            var filePath = PathUtil.GetStoreUserVideoFile(runningConfig.Category, userId, arcId);
            var down = HttpUtil.GetHttpRequestVedioResult(vedioUrl, filePath, runningConfig);
            if (!down)
            {
                ShowStatus($"下载视频【{arcId}】第【1】个文件错误!");
                return false;
            }
            else
            {
                var fileInfo = new FileInfo(filePath);
                if(!fileInfo.Exists)
                {
                    ShowStatus($"视频【{arcId}】第【1】文件不存在忽略");
                    return false;
                }
                if (fileInfo.Length == 0)
                {
                    ShowStatus($"视频【{arcId}】第【1】文件太小忽略");
                    return false;
                }
                ShowStatus($"下载视频【{arcId}】第【1】个文件OK).");
                return true;
            }
        }

        #endregion
    }
}
