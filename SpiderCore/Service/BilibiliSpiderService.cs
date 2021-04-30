using Jurassic.Library;
using SpiderCore.Config;
using SpiderCore.Entity;
using SpiderCore.Model.BilibiliJson;
using SpiderCore.Model.MWeiboJson;
using SpiderCore.Repository;
using SpiderCore.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SpiderCore.Service
{
    public class BilibiliSpiderService : ISpiderService
    {
        #region Spider Event

        public delegate void SpiderNewActionsEventHander(SinaAction[] uploads);

        public event SpiderNewActionsEventHander OnNewActions;

        public void NewActions(SinaAction[] uploads)
        {
            if (OnNewActions != null)
            {
                try
                {
                    OnNewActions?.Invoke(uploads);
                }
                catch (Exception) { }
            }
        }

        public delegate void SpiderNewActionCountEventHander(int needUploads);

        public event SpiderNewActionCountEventHander OnNewActionCount;

        void NewActionCount(List<SinaAction> actions)
        {
            if (OnNewActionCount != null)
            {
                try
                {
                    OnNewActionCount?.Invoke(actions.Where(c => c.action == 0).Count());
                }
                catch (Exception) { }
            }
        }

        public delegate void SpiderShowActionStatusEventHander(SinaAction upload, string state);

        public event SpiderShowActionStatusEventHander OnShowActionStatus;

        void ShowActionStatus(SinaAction upload, string state)
        {
            if (OnShowActionStatus != null)
            {
                try
                {
                    OnShowActionStatus?.Invoke(upload, state);
                }
                catch (Exception) { }
            }
        }


        public delegate void ShowStatusEventHander(string msg, bool bLog = false, Exception ex = null);

        public event ShowStatusEventHander OnShowGatherStatus;

        void ShowGatherStatus(string msg, bool bLog = false, Exception ex = null)
        {
            OnShowGatherStatus?.Invoke($"[{Thread.CurrentThread.ManagedThreadId}]{msg}", bLog, ex);
        }


        public delegate void SpiderStartedEventHander(SpiderRunningTask runningTask);

        public event SpiderStartedEventHander OnSpiderStarted;

        void SpiderStarted()
        {
            IsSpiderStarted = true;
            StopSpiderWork = false;
            if (OnSpiderStarted != null)
            {
                OnSpiderStarted?.Invoke(RunningTask);
            }
        }

        public delegate void SpiderCompleteEventHander();

        public event SpiderCompleteEventHander OnSpiderComplete;

        public void SpiderComplete()
        {
            if (RunningConfig.GatherCompleteShutdown && !StopSpiderWork)
            {
                PathUtil.Shutdown();
            }
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
            Repository.UpdateSinaUserPage(uid, pageIndex);

            OnGatherPageComplete?.Invoke(uid, pageIndex, readImageQty);
        }

        public delegate void SpiderGatherUserCompleteEventHander(SinaUser user, int readImageQty);

        public event SpiderGatherUserCompleteEventHander OnGatherUserComplete;

        public void GatherUserComplete(SinaUser user, int readImageQty)
        {
            //更新用户视频信息
            user = Repository.UpdateSinaUserInfo(user);

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

        protected bool StopShowWinBackgorund { get; set; } = false;

        /// <summary>
        /// 负责存储数据
        /// </summary>
        public SinaRepository Repository { get; set; }

        public SpiderRunningConfig RunningConfig { get; set; }

        public SpiderRunningTask RunningTask { get; set; }

        #endregion

        #region 构造函数
        public BilibiliSpiderService()
        {
            Repository = new SinaRepository();

            RunningConfig = new SpiderRunningConfig();
            RunningTask = new SpiderRunningTask();
        }

        #endregion

        #region 开始&结束&追加&取消采集

        public void StartSpider(SpiderRunningConfig runningConfig, SpiderStartOption option)
        {
            this.RunningConfig = runningConfig;

            this.RunningTask.GatherType = option.GatherType;
            this.RunningTask.Reset();

            if (string.IsNullOrEmpty(RunningConfig.Category))
            {
                ShowGatherStatus("采集类目为空!");
                return;
            }

            var sinaOption = option as SpiderStartOption;

            Task.Factory.StartNew(() =>
            {
                if (option.GatherType == GatherTypeEnum.GatherUser)
                {
                    MakeGatherUsers(sinaOption.SelectUsers, option.StartUrl);
                    SpiderStarted();
                    StartAutoGatherByUser();
                }
                else if(option.GatherType == GatherTypeEnum.GahterStatus)
                {
                    SpiderStarted();
                    StartAutoGatherByStatus(option.StatusIds, option.SelectUserId);
                }
                else if(option.GatherType == GatherTypeEnum.GatherTemp)
                {
                    SpiderStarted();
                    StartAutoGatherByTemp(option.StartUrl);
                }
                SpiderComplete();
            });
        }
        public void StopSpider()
        {
            StopSpiderWork = true;

            SpiderStoping();
        }

        void MakeGatherUsers(IList<SinaUser> users, string startUser)
        {
            var readUsers = new List<SinaUser>();
            if (users != null && users.Count > 0)
            {
                foreach (var user in users)
                {
                    RunningTask.AddUser(user);
                }
            }
            if (RunningConfig.ReadAllOfUser)
            {
                readUsers = Repository.GetUsers(RunningConfig.Category, GatherWebEnum.Bilibili);
                foreach (var user in readUsers.OrderBy(c => c.lastdate).ToArray())
                {
                    RunningTask.AddUser(user);
                }
            }
            else if (RunningConfig.ReadUserOfMyFocus)
            {
                readUsers = Repository.GetFocusUsers(RunningConfig.Category, GatherWebEnum.Bilibili);
                foreach (var user in readUsers.OrderByDescending(c => c.focus).ThenByDescending(c => c.lastdate).ToArray())
                {
                    RunningTask.AddUser(user);
                }
            }
            if (RunningTask.DoUsers.Count == 0)
            {
                if (!string.IsNullOrEmpty(startUser) && startUser.Length == 10)
                {
                    var user = Repository.GetUser(startUser);
                    if (user == null)
                    {
                        user = new SinaUser()
                        {
                            uid = startUser,
                            category = RunningConfig.Category,
                            web = (int)GatherWebEnum.Bilibili
                        };
                        Repository.CreateSinaUser(user);
                    }
                    RunningTask.AddUser(user);
                }
            }
            if (RunningConfig.ReadUserOfHeFocus)
            {
                var focusUser = GatherHeFocusUsers();
                if (!string.IsNullOrEmpty(RunningConfig.ReadUserNameLike))
                {
                    focusUser = FilterReadUser(focusUser, RunningConfig.ReadUserNameLike);
                }
                foreach (var user in focusUser.OrderBy(c => c.lastdate).ToArray())
                {
                    RunningTask.AddUser(user);
                }
            }
        }


        List<SinaUser> FilterReadUser(IList<SinaUser> users, string filter)
        {
            var lst = new List<SinaUser>();
            var keys = filter.Split(new string[] { "," }, StringSplitOptions.None);
            foreach (var key in keys)
            {
                var us = users.Where(c => c.name.Contains(key) || c.desc.Contains(key)).ToArray();
                foreach (var u in us)
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

        public void CancelUser(string uid)
        {
            this.RunningTask.CancelUser(uid);
        }

        protected bool CheckUserCanceled(string uid)
        {
            return this.RunningTask.CheckUserCanceled(uid);
        }

        public void AppendUser(SinaUser user)
        {
            if (this.RunningTask.AddUser(user))
            {
                if (OnGatherAppendUser != null)
                {
                    OnGatherAppendUser?.Invoke(user);
                }
            }
        }

        void StartAutoGatherByUser()
        {
            ShowGatherStatus($"准备读取用户的视频数据...");

            var threads = new List<Task>();
            var maxReadUserCount = RunningConfig.GatherThreadCount;
            for (var i = 0; i < maxReadUserCount; i++)
            {
                var task = Task.Factory.StartNew(() =>
                {
                    RunningTask.NewTask(Thread.CurrentThread.ManagedThreadId);
                    StartSpiderGatherTaskThread(Thread.CurrentThread.ManagedThreadId);
                });
                threads.Add(task);
            }
            Task.WaitAll(threads.ToArray());
        }

        public void StartSpiderGatherTaskThread(int taskId)
        {
            while (!StopSpiderWork)
            {
                var user = RunningTask.PeekUser();
                if (user == null)
                {
                    if (!RunningTask.CheckTaskRunning())
                    {
                        break;
                    }
                    else
                    {
                        RunningTask.UpdateTask(taskId, ThreadState.Suspended);
                        Thread.Sleep(5 * 1000);
                        continue;
                    }
                }
                else
                {
                    RunningTask.UpdateTask(taskId, ThreadState.Running);
                }

                GatherUserStarted(user);

                if (!CheckUserCanceled(user.uid))
                {
                    var runningCache = new SpiderRunningCache(GatherWebEnum.Bilibili, RunningConfig.Category, RunningConfig.Site, user.uid);
                    var readStatusImageCount = 0;
                    if (RunningConfig.GatherStatusWithNoSource)
                    {
                        readStatusImageCount = GatherSinaStatusByUserStatus(runningCache, user.uid);
                    }
                    else if (RunningConfig.GatherStatusUpdateLocalSource)
                    {
                        readStatusImageCount = GatherSinaSourceByUserStatus(runningCache, user.uid);
                    }
                    else
                    {
                        readStatusImageCount = GatherSinaStatusByUserUrl(runningCache, user.uid);
                    }

                    GatherUserComplete(user, readStatusImageCount);

                    ShowGatherStatus($"用户[{user.uid}]采集完成,共采集资源【{readStatusImageCount}】.");
                }
                else
                {
                    GatherUserComplete(user, 0);
                }
            }
        }

        void StartAutoGatherByStatus(IList<string> statusIds, string userId)
        {
            var threads = new List<Task>();
            var task = Task.Factory.StartNew(() =>
            {
                var user = Repository.GetUser(userId);
                if(user == null)
                {
                    ShowGatherStatus($"用户【{userId}】不存在.");
                    return;
                }
                ShowGatherStatus($"准备读取用户【{userId}】的视频数据...");

                var tempRuningCache = new SpiderRunningCache(GatherWebEnum.Sina, RunningConfig.Category, RunningConfig.Site, userId);
                int readStatusImageCount = 0;
                foreach (var status in statusIds)
                {
                    readStatusImageCount += GatherSinaStatusByUserStatus(tempRuningCache, user, status);
                }
                ShowGatherStatus($"采集完成,共采集资源【{readStatusImageCount}】.");
            });
            threads.Add(task);
            Task.WaitAll(threads.ToArray());
        }

        /// <summary>
        /// https://www.bilibili.com/video/BV1QK4y1t7ax?spm_id_from=333.851.b_62696c695f7265706f72745f64616e6365.24
        /// </summary>
        /// <param name="startUrl"></param>
        void StartAutoGatherByTemp(string startUrl)
        {
            var threads = new List<Task>();
            var task = Task.Factory.StartNew(() =>
            {
                var status = SinaUrlUtil.GetBilibiliStatusByUrl(startUrl);
                if (string.IsNullOrEmpty(status))
                {
                    ShowGatherStatus($"采集地址非Bilibili视频网址...");
                    return;
                }

                var tempRuningCache = new SpiderRunningCache(GatherWebEnum.Bilibili, RunningConfig.Category, RunningConfig.Site, null);
                int readStatusImageCount = GatherSinaStatusByStatusUrl(tempRuningCache, status);
                ShowGatherStatus($"采集完成,共采集资源【{readStatusImageCount}】.");
            });
            threads.Add(task);
            Task.WaitAll(threads.ToArray());
        }
        #endregion

        #region 采集并解析微博

        /// <summary>
        /// 采集用户已读微博
        /// </summary>
        /// <param name="runningCache"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        int GatherSinaStatusByUserStatus(SpiderRunningCache runningCache, string userId)
        {
            var sinaUser = Repository.GetUser(userId);
            if (sinaUser == null)
            {
                ShowGatherStatus($"用户【{userId}】未采集.");
                return 0;
            }
            if (sinaUser.ignore > 0)
            {
                ShowGatherStatus($"用户【{userId}】已忽略采集.");
                return 0;
            }
            int readStatusImageCount = 0, readUserImageCount = 0;
            var sinaStatuses = Repository.GetUserStatuses(userId);
            foreach (var status in sinaStatuses)
            {
                var suc = CheckUserStatusGather(runningCache, sinaUser, status);
                if (suc)
                {
                    ShowGatherStatus($"开始采集用户【{userId}】视频【{status.bid}】...");
                    readStatusImageCount = GatherSinaStatusByUserStatus(runningCache, sinaUser, status.bid);
                    readUserImageCount += readStatusImageCount;

                    if (readStatusImageCount > 0)
                    {
                        Repository.UpdateSinaUserQty(userId);
                    }
                }
                if (StopSpiderWork)
                {
                    ShowGatherStatus($"中止采集用户视频数据...");
                    break;
                }
                if (CheckUserCanceled(userId))
                {
                    ShowGatherStatus($"取消采集用户视频数据...");
                    break;
                }
                Thread.Sleep(RunningConfig.ReadFreeWaitMilSecond);
            }
            return readStatusImageCount;
        }

        bool CheckUserStatusGather(SpiderRunningCache runningCache, SinaUser user, SinaStatus status)
        {
            if (RunningConfig.IgnoreDownloadSource)
            {
                ShowGatherStatus($"跳过下载微博资源【{status.bid}】.");
                return false;
            }
            if (status.ignore > 0)
            {
                ShowGatherStatus($"跳过已忽略微博【{status.bid}】.");
                return false;
            }
            if (RunningConfig.IgnoreReadArchiveStatus && status.upload > 0)
            {
                ShowGatherStatus($"跳过已存档微博【{status.bid}】.");
                return false;
            }
            if (RunningConfig.IgnoreReadArchiveStatus && status.upload > 0)
            {
                ShowGatherStatus($"跳过已存档微博【{status.bid}】.");
                return false;
            }
            if (RunningConfig.IgnoreReadGetStatus && status.gets > 0)
            {
                ShowGatherStatus($"跳过已采集微博【{status.bid}】.");
                return false;
            }
            int readCount = 0;
            if (!RunningConfig.IgnoreReadDownStatus)
            {
                if (runningCache.EnabledUserCahce)
                {
                    if (status.mtype == 0)
                    {
                        readCount = runningCache.ExistsImageLocalFiles.Where(c => c.Contains($"{status.bid}")).Count();
                    }
                    else
                    {
                        readCount = runningCache.ExistsVideoLocalFiles.Where(c => c.Contains($"{status.bid}")).Count();
                    }
                }
                else
                {
                    if (status.mtype == 0)
                    {
                        readCount = PathUtil.GetStoreUserImageFileCount(runningCache.Category, user.uid, status.bid);
                    }
                    else
                    {
                        readCount = PathUtil.GetStoreUserVideoCount(runningCache.Category, user.uid, status.bid);
                    }
                }
                if (readCount > 0)
                {
                    ShowGatherStatus($"跳过已缓存微博【{status.bid}】.");
                    return false;
                }
            }
            return true;
        }

        int GatherSinaSourceByUserStatus(SpiderRunningCache runningCache, string userId)
        {
            var sinaUser = Repository.GetUser(userId);
            if (sinaUser == null)
            {
                ShowGatherStatus($"用户【{userId}】未采集.");
                return 0;
            }
            if (sinaUser.ignore > 0)
            {
                ShowGatherStatus($"用户【{userId}】已忽略采集.");
                return 0;
            }
            int readStatusImageCount = 0;
            var sinaStatuses = Repository.GetUserStatuses(userId);
            var videoFiles = PathUtil.GetStoreUserVideoFiles(runningCache.Category, userId);
            if (videoFiles.Length > 0)
            {
                var hasBids = sinaStatuses.Where(c => c.mtype == 1).Select(c => c.bid).ToArray();
                var localStatuses = videoFiles.Select(c => new FileInfo(c).Name.Split('.')[0]).GroupBy(c => c).Select(c => new { bid = c.Key, qty = c.Count() }).ToArray();
                var bids = localStatuses.Select(c => c.bid).ToArray();
                var extStatuses = sinaStatuses.Where(c => bids.Contains(c.bid)).ToArray();
                if (RunningConfig.IgnoreReadGetStatus)
                {
                    extStatuses = extStatuses.Where(c => c.gets > 0).ToArray();
                }
                if (RunningConfig.IgnoreReadArchiveStatus)
                {
                    extStatuses = extStatuses.Where(c => c.upload > 0).ToArray();
                }
                foreach (var status in extStatuses)
                {
                    var suc = CheckUserStatusGather(runningCache, sinaUser, status);
                    if (suc)
                    {
                        var localStatus = localStatuses.FirstOrDefault(c => c.bid == status.bid);
                        status.gets = localStatus.qty;
                        Repository.UpdateSinaStatus(status, new string[] { "gets" });
                        ShowGatherStatus($"用户【{userId}】微博【{status.bid}】已更新采集数量.");

                        GatherStatusComplete(userId, localStatus.qty);
                    }

                    if (StopSpiderWork)
                    {
                        ShowGatherStatus($"中止采集用户视频数据...");
                        break;
                    }
                    if (CheckUserCanceled(userId))
                    {
                        ShowGatherStatus($"取消采集用户视频数据...");
                        break;
                    }
                    Thread.Sleep(RunningConfig.ReadFreeWaitMilSecond);
                }
                var notExtBids = bids.Where(c => !hasBids.Contains(c)).ToArray();
                var notExtLoaclStatuses = localStatuses.Where(c => notExtBids.Contains(c.bid)).ToArray();
                foreach (var localStatus in notExtLoaclStatuses)
                {
                    ShowGatherStatus($"开始采集用户【{userId}】微博【{localStatus.bid}】...");
                    readStatusImageCount += GatherSinaStatusByUserStatus(runningCache, sinaUser, localStatus.bid);

                    var extStatus = Repository.GetUserStatus(localStatus.bid);
                    if (extStatus != null && extStatus.ignore > 0)
                    {
                        PathUtil.DeleteStoreUserVideoFile(runningCache.Category, userId, extStatus.bid);
                        ShowGatherStatus($"用户【{userId}】微博【{extStatus.bid}】已忽略删除.");
                    }
                    if (StopSpiderWork)
                    {
                        ShowGatherStatus($"中止采集用户视频数据...");
                        break;
                    }
                    if (CheckUserCanceled(userId))
                    {
                        ShowGatherStatus($"取消采集用户视频数据...");
                        break;
                    }
                    Thread.Sleep(RunningConfig.ReadFreeWaitMilSecond);
                }
                Repository.UpdateSinaUserQty(userId);
            }
            return readStatusImageCount;
        }

        /// <summary>
        /// 直接读取用户数据
        /// </summary>
        /// <param name="runningConfig"></param>
        int GatherSinaStatusByUserUrl(SpiderRunningCache runningCache, string userId)
        {
            var user = GatherSinaUserByUserUrl(runningCache, userId);
            if (user == null) return 0;

            var sinaUser = Repository.GetUser(user.mid);
            if (sinaUser != null && sinaUser.ignore > 0)
            {
                ShowGatherStatus($"用户【{user.mid}】已忽略采集.");
                return 0;
            }

            int lastReadPageIndex = 0;
            if (sinaUser != null)
            {
                if (sinaUser.readpage > 0) lastReadPageIndex = sinaUser.readpage;
            }

            int readUserImageCount = 0, readPageIndex = 0, emptyPageCount = 0;
            int readPageSize = RunningConfig.ReadPageSize;
            int startPageIndex = RunningConfig.StartPageIndex;
            if (RunningConfig.GatherContinueLastPage && lastReadPageIndex > 0) startPageIndex = lastReadPageIndex;
            int readPageCount = (RunningConfig.MaxReadPageCount == 0 ? int.MaxValue : startPageIndex + RunningConfig.MaxReadPageCount);
            for (readPageIndex = startPageIndex; readPageIndex < readPageCount; readPageIndex++)
            {
                if (StopSpiderWork)
                {
                    ShowGatherStatus($"中止采集用户视频数据...");
                    break;
                }
                if (CheckUserCanceled(userId))
                {
                    ShowGatherStatus($"取消采集用户视频数据...");
                    break;
                }
                bool readPageEmpty = false, stopReadNextPage = false;
                int readPageImageCount = GatherSinaStatusByStatusPageUrl(runningCache, sinaUser, user, readPageIndex, readPageSize, readPageCount, out readPageEmpty, out stopReadNextPage);
                readUserImageCount += readPageImageCount;
                if (!readPageEmpty)
                {
                    GatherPageComplete(user.mid, readPageIndex, readUserImageCount);
                }
                if (stopReadNextPage)
                {
                    ShowGatherStatus($"结束采集用户视频数据(下页已采集)...");
                    break;
                }

                if (readPageEmpty) emptyPageCount++;
                else emptyPageCount = 0;

                if (emptyPageCount > 3)
                {
                    if (RunningConfig.MaxReadPageCount == 0)
                    {
                        Repository.UpdateUserLastpage(userId);
                    }
                    ShowGatherStatus($"中止采集用户视频数据(连续超过3页无数据)...");
                    break;
                }

                if (StopSpiderWork)
                {
                    ShowGatherStatus($"中止采集用户视频数据...");
                    break;
                }
                if (CheckUserCanceled(userId))
                {
                    ShowGatherStatus($"取消采集用户视频数据...");
                    break;
                }
                if (readPageIndex + 1 < readPageCount && readPageImageCount > 0)
                {
                    Repository.UpdateSinaUserQty(userId);

                    ShowGatherStatus($"等待【{RunningConfig.ReadNextPageWaitSecond}】秒读取用户【{userId}】下一页视频数据...");
                    Thread.Sleep(RunningConfig.ReadNextPageWaitSecond * 1000);
                }
                else
                {
                    Thread.Sleep(RunningConfig.ReadFreeWaitMilSecond);
                }
            }
            return readUserImageCount;
        }

        BilibiliUser GatherSinaUserByUserUrl(SpiderRunningCache runningCache, string userId)
        {
            ShowGatherStatus($"开始读取用户【{userId}】的基本信息...", true);
            var getApi = $"https://api.bilibili.com/x/space/acc/info?mid={userId}&jsonp=jsonp";
            var html = HttpUtil.GetHttpRequestJsonResult(getApi, RunningConfig);
            if (html == null)
            {
                ShowGatherStatus($"获取用户信息错误!!!!!!");
                return null;
            }
            var result = GetBilibiliUserResult(html);
            if (result == null || result.data == null)
            {
                ShowGatherStatus($"解析用户信息错误!!!!!!");
                return null;
            }
            var user = result.data;

            var sinaUser = Repository.StoreSinaUser(RunningConfig, runningCache, user);
            if (sinaUser == null)
            {
                ShowGatherStatus($"存储用户信息错误!!!!!!");
                return null;
            }
            if (sinaUser.id == 0)
            {
                GatherNewUser(sinaUser);
            }
            return user;
        }


        /// <summary>
        /// 采集用户视频列表数据
        /// </summary>
        /// <param name="runningConfig"></param> 
        /// <param name="user"></param>
        /// <param name="readPageIndex"></param>
        /// <param name="readPageCount"></param>
        /// <returns></returns>
        int GatherSinaStatusByStatusPageUrl(SpiderRunningCache runningCache, SinaUser sinaUser, BilibiliUser user, int readPageIndex, int readPageSize, int readPageCount, out bool readPageEmpty, out bool stopReadNextPage)
        {
            readPageEmpty = false;
            stopReadNextPage = false;
            runningCache.CurrentPageIndex = readPageIndex;

            ShowGatherStatus($"开始读取用户【{user.mid}】的第【{readPageIndex}】页视频数据...", true);
            var getApi = $"https://api.bilibili.com/x/space/arc/search?mid={user.mid}&ps={readPageSize}&tid=0&pn={readPageIndex}&keyword=&order=pubdate&jsonp=jsonp";
            var html = HttpUtil.GetHttpRequestJsonResult(getApi, RunningConfig);
            if (html == null)
            {
                ShowGatherStatus($"获取用户视频列表错误!!!!!!");
                return 0;
            }
            var result = GetBilibiliStatusListResult(html);
            if (result == null || result.data == null || result.data.list == null)
            {
                ShowGatherStatus($"解析用户视频列表错误!!!!!!");
                return 0;
            }
            if (result.data.list.vlist.Length == 0)
            {
                ShowGatherStatus($"解析用户视频列表为空...");
                readPageEmpty = true;
                return 0;
            }
            if (result.data.page != null && sinaUser.statuses != result.data.page.count)
            {
                sinaUser.statuses = result.data.page.count;
                Repository.UpdateSinaUser(sinaUser, new string[] { "statuses" });
            }
            int readPageImageCount = 0;
            foreach (var card in result.data.list.vlist)
            {
                if (StopSpiderWork)
                {
                    ShowGatherStatus($"中止采集视频数据...");
                    break;
                }
                if (CheckUserCanceled(user.mid))
                {
                    ShowGatherStatus($"取消采集视频数据...");
                    break;
                }
                bool ignoreSourceReaded = false;
                var readStatusImageCount = GatherSinaStatusByStatusResult(runningCache, card, sinaUser, out ignoreSourceReaded);
                readPageImageCount += readStatusImageCount;

                if (StopSpiderWork)
                {
                    ShowGatherStatus($"中止采集视频数据...");
                    break;
                }
                if (CheckUserCanceled(user.mid))
                {
                    ShowGatherStatus($"取消采集视频数据...");
                    break;
                }
                if (readStatusImageCount > 0)
                {
                    //ShowGatherStatus($"等待【{RunningConfig.ReadNextStatusWaitSecond}】秒读取用户【{user.mid}】下一条视频数据...");
                    Thread.Sleep(RunningConfig.ReadNextStatusWaitSecond * 1000);
                }
                else
                {
                    Thread.Sleep(RunningConfig.ReadFreeWaitMilSecond);
                }
            }
            return readPageImageCount;
        }

        /// <summary>
        /// 采集微博详细数据
        /// </summary>
        /// <param name="runningConfig"></param>
        int GatherSinaStatusByStatusUrl(SpiderRunningCache runningCache, string status)
        {
            return GatherSinaStatusByUserStatus(runningCache, null, status);
        }

        /// <summary>
        /// 采集微博详细数据
        /// </summary>
        /// <param name="runningConfig"></param>
        int GatherSinaStatusByUserStatus(SpiderRunningCache runningCache, SinaUser user, string status)
        {
            var statusUrl = SinaUrlUtil.GetBilibiliUserStatusUrl(status);
            var html = HttpUtil.GetHttpRequestHtmlResult(statusUrl, true, RunningConfig);
            if (string.IsNullOrEmpty(html))
            {
                ShowGatherStatus($"获取视频信息错误!!!!!!");
                return 0;
            }
            var result = GetBilibiliStatusStateResult(html);
            if (result == null || result.videoData == null || result.upData == null)
            {
                ShowGatherStatus($"解析用户视频信息错误!!!!!!");
                return 0;
            }

            if (user == null)
            {
                user = Repository.StoreSinaUser(RunningConfig, runningCache, result.upData);
                if (user == null)
                {
                    ShowGatherStatus($"存储用户信息错误!!!!!!");
                    return 0;
                }
            }
            var statusInfo = result.videoData;
            bool ignoreSourceReaded = false;
            return GatherSinaStatusByStatusResult(runningCache, new BilibiliUserStatusVlist()
            {
                bvid = status,
                aid = statusInfo.aid,
                description = statusInfo.desc,
                created = statusInfo.pubdate,
                mid = statusInfo.cid,
                title = statusInfo.title,
            }, user, out ignoreSourceReaded);
        }

        /// <summary>
        /// 采用微博详细数据（通过Result）
        /// </summary>
        /// <param name="runningConfig"></param>
        /// <param name="status"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        int GatherSinaStatusByStatusResult(SpiderRunningCache runningCache, BilibiliUserStatusVlist status, SinaUser user, out bool ignoreSourceReaded)
        {
            ignoreSourceReaded = false;
            int readStatusImageCount = 0;
            readStatusImageCount = GatherSinaStatusViedoByStatusResult(runningCache, status, user, out ignoreSourceReaded);
            if (readStatusImageCount > 0)
            {
                GatherStatusComplete(user.uid, readStatusImageCount);
            }
            return readStatusImageCount > 0 ? readStatusImageCount : 0;
        }

        /// <summary>
        /// 采集微博详细信息
        /// </summary>
        /// <param name="runningConfig"></param>
        /// <param name="status"></param>
        /// <returns>
        /// -1:忽略
        /// 0:未采集到有效图片,需忽略
        /// </returns>
        int GatherSinaStatusPicsByStatusResult(SpiderRunningCache runningCache, MWeiboStatus status, out bool ignoreSourceReaded)
        {
            ignoreSourceReaded = false;

            var user = status.user;
            if (user == null)
            {
                ShowGatherStatus($"视频数据已删除.");
                return 0;
            }
            if (Repository.CheckUserIgnore(user.id))
            {
                ShowGatherStatus($"用户【{user.id}】已忽略采集.");
                return 0;
            }
            if (status.pics == null || status.pics.Length < RunningConfig.MinReadImageCount)
            {
                ShowGatherStatus($"跳过不符合最小图数微博【{status.bid}】.");
                return 0;
            }
            var sinaStatus = Repository.GetUserStatus(status.bid);
            if (sinaStatus != null)
            {
                if (sinaStatus.ignore > 0)
                {
                    ShowGatherStatus($"跳过已忽略微博【{status.bid}】.");
                    return 0;
                }
                if (RunningConfig.IgnoreReadArchiveStatus && sinaStatus.upload > 0)
                {
                    ShowGatherStatus($"跳过已存档微博【{status.bid}】.");
                    return 0;
                }
                if (RunningConfig.IgnoreReadGetStatus && sinaStatus.gets > 0)
                {
                    ignoreSourceReaded = true;
                    ShowGatherStatus($"跳过已采集微博【{status.bid}】.");
                    return 0;
                }
            }
            int readCount = 0;
            if (!RunningConfig.IgnoreReadDownStatus)
            {
                if (runningCache.EnabledUserCahce)
                {
                    readCount = runningCache.ExistsImageLocalFiles.Where(c => c.Contains($"{status.bid}")).Count();
                }
                else
                {
                    readCount = PathUtil.GetStoreUserImageFileCount(runningCache.Category, user.id, status.bid);
                }
            }
            if (RunningConfig.IgnoreDownloadSource)
            {
                ShowGatherStatus($"跳过下载微博资源【{status.bid}】.");
                //存储微博
                Repository.StoreSinaStatus(RunningConfig, user, status, 0, status.pics.Length, readCount, false);
                return status.pics.Length;
            }
            else
            {
                if (readCount > 0)
                {
                    //ignoreSourceReaded = true;
                    ShowGatherStatus($"跳过已缓存微博【{status.bid}】.");
                    Repository.StoreSinaStatus(RunningConfig, user, status, 0, status.pics.Length, readCount, false);
                    return 0;
                }
                else
                {
                    ShowGatherStatus($"开始采集用户【{user.id}】第【{runningCache.CurrentPageIndex}】页微博【{status.bid}】...");
                    int haveReadImageCount = 0, readImageIndex = 0, errorReadImageCount = 0, errorSaveImageCount = 0;
                    foreach (var pic in status.pics)
                    {
                        //1:下载图片错误,2:保存数据错误,3:保存图片错误,
                        int errType = 0;
                        var succ = DownloadUserStatusImage(runningCache, user.id, status.bid, pic.large.url, ++readImageIndex, out errType);
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
                        Thread.Sleep(RunningConfig.DownloadSourceWaitMilSecond);
                    }
                    if (errorSaveImageCount > 0)
                    {
                        StopSpiderWork = true;
                        ShowGatherStatus($"微博【{status.bid}】保存图片错误,停止采集!!!!!!");
                        return 0;
                    }
                    if (errorReadImageCount == status.pics.Length && errorReadImageCount > 0)
                    {
                        haveReadImageCount = -1;
                        ShowGatherStatus($"微博【{status.bid}】已下载图片全部错误.");
                    }
                    if (haveReadImageCount == 0)
                    {
                        ShowGatherStatus($"微博【{status.bid}】已无符合尺寸的图片.");
                        PathUtil.DeleteStoreUserImageFiles(runningCache.Category, user.id, status.bid, null);
                    }
                    else if (haveReadImageCount > 0 && haveReadImageCount < RunningConfig.MinReadImageCount)
                    {
                        ShowGatherStatus($"微博【{status.bid}】已下载图数不符合删除.");
                        PathUtil.DeleteStoreUserImageFiles(runningCache.Category, user.id, status.bid, null);

                        haveReadImageCount = 0;
                    }
                    //存储微博
                    Repository.StoreSinaStatus(RunningConfig, user, status, 0, status.pics.Length, haveReadImageCount, haveReadImageCount == 0);
                    return haveReadImageCount;
                }
            }
        }

        int GatherSinaStatusViedoByStatusResult(SpiderRunningCache runningCache, BilibiliUserStatusVlist status, SinaUser user, out bool ignoreSourceReaded)
        {
            ignoreSourceReaded = false;
            if (Repository.CheckUserIgnore(user.uid))
            {
                ShowGatherStatus($"用户【{user.uid}】已忽略采集.");
                return 0;
            }
            var sinaStatus = Repository.GetUserStatus(status.bvid);
            if (sinaStatus != null)
            {
                if (sinaStatus.ignore > 0)
                {
                    ShowGatherStatus($"跳过已忽略视频【{status.bvid}】.");
                    return 0;
                }
                if (RunningConfig.IgnoreReadArchiveStatus && sinaStatus.upload > 0)
                {
                    ShowGatherStatus($"跳过已存档视频【{status.bvid}】.");
                    return 0;
                }
                if (RunningConfig.IgnoreReadGetStatus && sinaStatus.gets > 0)
                {
                    ignoreSourceReaded = true;
                    ShowGatherStatus($"跳过已采集视频【{status.bvid}】.");
                    return 0;
                }
            }
            int readCount = 0;
            if (!RunningConfig.IgnoreReadDownStatus)
            {
                if (runningCache.EnabledUserCahce)
                {
                    readCount = runningCache.ExistsVideoLocalFiles.Where(c => c.Contains($"{status.bvid}")).Count();
                }
                else
                {
                    readCount = PathUtil.GetStoreUserVideoCount(runningCache.Category, user.uid, status.bvid);
                }
            }
            if (RunningConfig.IgnoreDownloadSource)
            {
                ShowGatherStatus($"跳过下载微博资源【{status.bvid}】.");
                //存储微博
                Repository.StoreSinaStatus(RunningConfig, user, status, readCount, false);
                return 1;
            }
            else
            {
                if (readCount > 0)
                {
                    ShowGatherStatus($"跳过已缓存视频【{status.bvid}】.");
                    Repository.StoreSinaStatus(RunningConfig, user, status, readCount, false);
                    return 0;
                }
                else
                {
                    int haveReadVedioCount = 0, errorReadVedioCount = 0;
                    var avUrl = SinaUrlUtil.GetBilibiliUserStatusUrl(status.bvid);
                    var html = HttpUtil.GetHttpRequestHtmlResult(avUrl, true, RunningConfig);
                    if (html == null)
                    {
                        ShowGatherStatus($"获取用户视频信息错误!!!!!!");
                        return 0;
                    }
                    var result = GetBilibiliStatusViedoResult(html);
                    if (result == null || result.data == null || result.data.dash == null || result.data.dash.video.Length == 0 || result.data.dash.audio.Length == 0)
                    {
                        ShowGatherStatus($"解析用户视频信息错误!!!!!!");
                        return 0;
                    }
                    var video = result.data.dash.video.FirstOrDefault();
                    var audio = result.data.dash.audio.FirstOrDefault();
                    var succ = DownloadUserStatusVedio(runningCache, user.uid, status.bvid, avUrl, video.baseUrl, audio.baseUrl);
                    if (succ)
                    {
                        haveReadVedioCount++;
                    }
                    else
                    {
                        errorReadVedioCount++;
                    }
                    Thread.Sleep(RunningConfig.DownloadSourceWaitMilSecond);

                    if (errorReadVedioCount == 0)
                    {
                        //存储微博
                        Repository.StoreSinaStatus(RunningConfig, user, status, haveReadVedioCount, haveReadVedioCount == 0);
                    }
                    return haveReadVedioCount;
                }
            }
        }

        List<SinaUser> GatherHeFocusUsers()
        {
            var focusUsers = new List<SinaUser>();

            foreach (var user in RunningTask.DoUsers)
            {
                int page = 0;
                while (++page > 0)
                {
                    ShowGatherStatus($"开始读取{user.uid}关注的第{page}页用户信息...", true);
                    var getApi = $"https://m.weibo.cn/api/container/getIndex?containerid=231051_-_followers_-_{user.uid}_-_1042015%253AtagCategory_039&luicode=10000011&lfid=1076033810669779&page={page}";
                    var html = HttpUtil.GetHttpRequestJsonResult(getApi, RunningConfig);
                    if (html == null)
                    {
                        ShowGatherStatus($"读取{user.uid}关注的第{page}页用户信息错误!");
                        return null;
                    }
                    var result = GetWeiboFocusResult(html);
                    if (result == null || result.data == null)
                    {
                        ShowGatherStatus($"解析{user.uid}关注的第{page}页用户错误!");
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
                        category = RunningConfig.Category
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
            try
            {
                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(html);

                var jsonResult = Newtonsoft.Json.JsonConvert.DeserializeObject<MWeiboFoucsUserResult>(doc.DocumentNode.InnerText) as MWeiboFoucsUserResult;
                return jsonResult;
            }
            catch (Exception ex)
            {
                LogUtil.Error(ex);
                return null;
            }
        }

        BilibiliUserResult GetBilibiliUserResult(string html)
        {
            try
            {
                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(html);

                var jsonResult = Newtonsoft.Json.JsonConvert.DeserializeObject<BilibiliUserResult>(doc.DocumentNode.InnerText) as BilibiliUserResult;
                return jsonResult;
            }
            catch (Exception ex)
            {
                LogUtil.Error(ex);
                return null;
            }
        }

        BilibiliUserStatusResult GetBilibiliStatusListResult(string html)
        {
            try
            {
                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(html);

                var jsonResult = Newtonsoft.Json.JsonConvert.DeserializeObject<BilibiliUserStatusResult>(doc.DocumentNode.InnerText) as BilibiliUserStatusResult;
                return jsonResult;
            }
            catch (Exception ex)
            {
                LogUtil.Error(ex);
                return null;
            }
        }

        BilibiliUserStateResult GetBilibiliStatusStateResult(string html)
        {
            try
            {
                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(html);

                var scripts = doc.DocumentNode.Descendants().Where(c => c.Name == "script").Select(c => c.InnerText).ToArray();
                var engine = new Jurassic.ScriptEngine();

                foreach (var script in scripts)
                {
                    if (script.Contains("window.__INITIAL_STATE__="))
                    {
                        var js = script.Replace("window.__INITIAL_STATE__=", "__INITIAL_STATE__=").Substring(script.IndexOf("window.__INITIAL_STATE__="));

                        if (js.Contains("(function()")) js = js.Substring(0, js.IndexOf("(function()"));
                        
                        var result = engine.Evaluate("(function() { " + js + "; return __INITIAL_STATE__; })()");
                        var json = JSONObject.Stringify(engine, result);

                        var jsonResult = Newtonsoft.Json.JsonConvert.DeserializeObject<BilibiliUserStateResult>(json) as BilibiliUserStateResult;
                        return jsonResult;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                LogUtil.Error(ex);
                return null;
            }
        }

        BilibiliUserViedoResult GetBilibiliStatusViedoResult(string html)
        {
            try
            {
                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(html);

                var scripts = doc.DocumentNode.Descendants().Where(c => c.Name == "script").Select(c => c.InnerText).ToArray();
                var engine = new Jurassic.ScriptEngine();

                foreach (var script in scripts)
                {
                    if (script.Contains("window.__playinfo__="))
                    {
                        var js = script.Replace("window.__playinfo__=", "__playinfo__=").Substring(script.IndexOf("window.__playinfo__="));

                        var result = engine.Evaluate("(function() { " + js + "; return __playinfo__; })()") ;
                        var json = JSONObject.Stringify(engine, result);

                        var jsonResult = Newtonsoft.Json.JsonConvert.DeserializeObject<BilibiliUserViedoResult>(json) as BilibiliUserViedoResult;
                        return jsonResult;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                LogUtil.Error(ex);
                return null;
            }
        }

        MWeiboStatusResult GetWeiboStatusResult(string html)
        {
            try
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
            }
            catch (Exception ex)
            {
                LogUtil.Error(ex);
                return null;
            }
            return null;
        }

        MWeiboSuperResult GetSinaSuperResult(string html)
        {
            try
            {
                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(html);

                var jsonResult = Newtonsoft.Json.JsonConvert.DeserializeObject<MWeiboSuperResult>(doc.DocumentNode.InnerText) as MWeiboSuperResult;
                return jsonResult;
            }
            catch (Exception ex)
            {
                LogUtil.Error(ex);
                return null;
            }
        }

        MWeiboTopicResult GetSinaTopicResult(string html)
        {
            try
            {
                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(html);

                var jsonResult = Newtonsoft.Json.JsonConvert.DeserializeObject<MWeiboTopicResult>(doc.DocumentNode.InnerText) as MWeiboTopicResult;
                return jsonResult;
            }
            catch (Exception ex)
            {
                LogUtil.Error(ex);
                return null;
            }
        }

        /// <summary>
        /// 读取微博用户图片原始路径
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="cookie"></param>
        /// <returns></returns>
        bool DownloadUserStatusImage(SpiderRunningCache runningCache, string userId, string arcId, string imgUrl, int readImageIndex, out int errType)
        {
            errType = 0;

            var path = PathUtil.GetStoreUserPath(runningCache.Category, userId);
            PathUtil.CheckCreateDirectory(path);
            var fileName = $"{arcId}_{readImageIndex}.jpg";
            var imgPath = Path.Combine(path, fileName);
            var image = HttpUtil.GetHttpRequestImageResult(imgUrl, RunningConfig);
            if (image == null)
            {
                ShowGatherStatus($"下载微博【{arcId}】第【{readImageIndex}】张图片错误!");
                errType = 1;
                return false;
            }
            if (!CheckImageSize(image, arcId, readImageIndex)) return false;

            var thumbSize = GetThumbImageSize(image, RunningConfig.ThumbnailImageWidth, RunningConfig.ThumbnailImageHeight);
            var thumbImg = image.GetThumbnailImage(thumbSize.Width, thumbSize.Height, null, IntPtr.Zero);
            var thunbPath = Path.Combine(path, "thumb");
            PathUtil.CheckCreateDirectory(thunbPath);
            var thumbPath = Path.Combine(thunbPath, fileName);

            try
            {
                image.Save(imgPath);
                thumbImg.Save(thumbPath);
                ShowGatherStatus($"下载微博【{arcId}】第【{readImageIndex}】张图片(OK).");

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

                        ShowGatherStatus($"创建微博【{arcId}】第【{readImageIndex}】张图片错误!!!!!!");
                        errType = 2;
                        return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                errType = 3;
                if (File.Exists(imgPath)) File.Delete(imgPath);
                if (File.Exists(imgPath)) File.Delete(thumbPath);
                ShowGatherStatus($"保存微博【{arcId}】第【{readImageIndex}】张图片错误!(未知错误)");
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
                downdate = DateTime.Now.ToString("yyyy/MM/dd HH:mm")
            };
        }

        Size GetThumbImageSize(Image image, int thumbWidth, int thumbHeight)
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
            return new Size(width, height);
        }

        bool CheckImageSize(Image image, string arcId, int readImageIndex)
        {
            if (image.Width <= RunningConfig.MinReadImageSize || image.Height <= RunningConfig.MinReadImageSize)
            {
                ShowGatherStatus($"微博【{arcId}】第【{readImageIndex}】张图片尺寸太小忽略");
                return false;
            }
            if (image.Width > RunningConfig.MaxReadImageSize || image.Height > RunningConfig.MaxReadImageSize)
            {
                ShowGatherStatus($"微博【{arcId}】第【{readImageIndex}】张图片尺寸太大忽略");
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
        bool DownloadUserStatusVedio(SpiderRunningCache runningCache, string userId, string arcId, string statusUrl, string videoUrl, string audioUrl)
        {
            if (!Repository.ExistsSinaSource(userId, arcId, statusUrl))
            {
                var sinaPicture = new SinaSource()
                {
                    uid = userId,
                    bid = arcId,
                    url = statusUrl,
                    name = $"{arcId}_1.mp4"
                };
                var suc = Repository.CreateSinaSource(sinaPicture);
                if (!suc)
                {
                    ShowGatherStatus($"创建本地微博视频错误!!!!!!");
                    return false;
                }
            }
            var mp4File = PathUtil.GetStoreUserVideoFile(runningCache.Category, userId, arcId);
            var videoFile = PathUtil.GetStoreUserVideoFile(runningCache.Category, userId, $"{arcId}_vid");
            var video = HttpUtil.GetHttpRequestRangeVedioResult(videoUrl, statusUrl, videoFile, RunningConfig);
            if (video) {
                var audioFile = PathUtil.GetStoreUserVideoFile(runningCache.Category, userId, $"{arcId}_aud");
                var audio = HttpUtil.GetHttpRequestRangeVedioResult(audioUrl, statusUrl, audioFile, RunningConfig);
                if (audio)
                {
                    var mp4 = MegreViedoAndAudioToMP4(videoFile, audioFile, mp4File);
                    if (!mp4)
                    {
                        ShowGatherStatus($"合并音频【{arcId}】文件错误!");
                        return false;
                    }
                    else
                    {
                        File.Delete(videoFile);
                        File.Delete(audioFile);
                    }
                }
            }
            var fileInfo = new FileInfo(mp4File);
            if (!fileInfo.Exists)
            {
                ShowGatherStatus($"下载视频【{arcId}】文件错误!!!");
                return false;
            }
            ShowGatherStatus($"下载视频【{arcId}】文件(OK).");
            return true;
        }

        bool MegreViedoAndAudioToMP4(string video, string audio, string mp4)
        {
            var basePath = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            var ffmpeg = Path.Combine(basePath.Parent.Parent.FullName, "ffmpeg", "ffmpeg.exe");
            try
            {
                CmdUtil.RunCmd(ffmpeg, $"-i {audio} -i {video} -codec copy {mp4}");
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }
        #endregion

        #region 后台任务
        public void StartBackgroundTask()
        {
            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(5 * 1000);

                while (true)
                {
                    var actions = Repository.GetSinaWaitActions();
                    NewActionCount(actions);

                    if (actions.Count == 0)
                    {
                        Thread.Sleep(RunningConfig.UploadFreeWaitSecond * 1000);
                        continue;
                    }

                    NewActions(actions.ToArray());

                    foreach (var action in actions)
                    {
                        switch (action.acttype)
                        {
                            case 0:
                                StartUploadAction(action);
                                break;
                            case 1:
                                StartCancelUploadAction(action);
                                break;
                            case 2:
                                StartIgnoreStatusAction(action);
                                break;
                        }
                        NewActionCount(actions);
                        Thread.Sleep(RunningConfig.UploadSourceWaitMilSecond);
                    }

                    NewActionCount(actions);
                    Thread.Sleep(RunningConfig.UploadFreeWaitSecond * 1000);
                }
            });
        }

        void StartUploadAction(SinaAction action)
        {
            var imgFile = PathUtil.GetStoreUserImageFile(action.category, action.uid, action.file);
            if (!imgFile.Exists)
            {
                ShowActionStatus(action, "文件不存在");
                return;
            }
            //上传开始
            ShowActionStatus(action, "Uploading...");

            var rst = HttpUtil.UploadRemoteImage(RunningConfig, action, imgFile);
            if (rst == null)
            {
                //上传失败
                ShowActionStatus(action, "失败");
            }
            else if (!rst.Success)
            {
                //上传失败
                ShowActionStatus(action, rst.Message);
            }
            else
            {
                action.action = 1;
                action.actiontime = DateTime.Now.ToString("yyyy/MM/dd HH:mm");
                Repository.UpdateSinaAction(action, new string[] { "action", "actointime" });

                PathUtil.CopyUploadImageFiles(imgFile.FullName, RunningConfig.DefaultUploadPath);

                //上传完成
                ShowActionStatus(action, "✔");
            }
        }

        void StartCancelUploadAction(SinaAction action)
        {
            //上传开始
            ShowActionStatus(action, "Canceling...");

            var rst = HttpUtil.DeleteSinaSourceImage(RunningConfig, action.bid, action.file);
            if (!rst)
            {
                //上传失败
                ShowActionStatus(action, "失败");
            }
            else
            {
                action.action = 1;
                action.actiontime = DateTime.Now.ToString("yyyy/MM/dd HH:mm");
                Repository.UpdateSinaAction(action, new string[] { "action", "actiontime" });

                //上传完成
                ShowActionStatus(action, "✔");
            }
        }

        void StartIgnoreStatusAction(SinaAction action)
        {
            if (!string.IsNullOrEmpty(action.bid))
            {
                var sinaStatus = Repository.GetUserStatus(action.bid);
                if (sinaStatus != null)
                {
                    ShowActionStatus(action, "删除微博...");
                    Repository.ExecuteIgnoreStatus(action.category, action.uid, action.bid, action.file);

                    if (sinaStatus.mtype == 0)
                    {
                        ShowActionStatus(action, "删除图片...");

                        HttpUtil.DeleteSinaSourceImage(RunningConfig, action.bid, action.file);

                        PathUtil.DeleteStoreUserImageFiles(RunningConfig.Category, action.uid, action.bid, action.file);
                    }
                    else if (sinaStatus.mtype == 1)
                    {
                        ShowActionStatus(action, "删除视频...");

                        PathUtil.DeleteStoreUserVideoFile(RunningConfig.Category, action.uid, action.bid);
                    }
                }
            }
            else if (!string.IsNullOrEmpty(action.uid))
            {
                var sinaUser = Repository.GetUser(action.uid);
                if (sinaUser != null)
                {
                    ShowActionStatus(action, "删除微博...");
                    Repository.DeleteSinaStatus(sinaUser);

                    ShowActionStatus(action, "删除资源...");
                    PathUtil.DeleteStoreUserSource(RunningConfig.Category, action.uid);
                }
            }
            action.action = 1;
            action.actiontime = DateTime.Now.ToString("yyyy/MM/dd HH:mm");
            Repository.UpdateSinaAction(action, new string[] { "action", "actiontime" });
            ShowActionStatus(action, "✔");
        }

        public List<SinaAction> GetWaitForDoActions()
        {
            return Repository.GetSinaWaitActions();
        }
        #endregion

        #region 开启循环桌面

        public void StartShowWinBackgroundTask()
        {
            Task.Factory.StartNew(() =>
            {
                while (!StopShowWinBackgorund)
                {
                    var bkcImgPath = PathUtil.GetStoreCustomPath(RunningConfig.DefaultWallpaperPath);
                    var bkgImgFiles = Directory.GetFiles(bkcImgPath, "*.bmp").Select(c => new FileInfo(c)).ToArray();
                    if (bkgImgFiles.Length == 0) break;

                    var showTime = 0;
                    while (!StopShowWinBackgorund)
                    {
                        var random = new Random((int)DateTime.Now.Ticks);
                        var index = random.Next(0, bkgImgFiles.Length - 1);

                        var bkgImg = bkgImgFiles[index];

                        if (!bkgImg.Exists) continue;

                        if (StopShowWinBackgorund) break;

                        ImageUtil.SystemParametersInfo(20, 0, bkgImg.FullName, 0x2);

                        Thread.Sleep(RunningConfig.ShowWinBackgoundIntervalSencond * 1000);

                        showTime++;
                        if (showTime >= bkgImgFiles.Length) break;
                    }
                    if (StopShowWinBackgorund) break;
                }
            });
        }

        public void StopShowWinBackgroundTask()
        {
            this.StopShowWinBackgorund = true;
        }

        #endregion
    }
}
