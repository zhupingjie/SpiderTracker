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
using SpiderTracker.Imp.Util;

namespace SpiderTracker.Imp
{
    public class MWeiboSpiderService : ISpiderService
    {
        #region Spider Event

        public delegate void SpiderUploadShowEventHander(SinaUpload[] uploads);

        public event SpiderUploadShowEventHander OnSpiderUploadShow;

        public void SpiderUploadShow(SinaUpload[] uploads)
        {
            if (OnSpiderUploadShow != null)
            {
                OnSpiderUploadShow?.Invoke(uploads);
            }
        }

        public delegate void SpiderUploadRefreshEventHander(SinaUpload upload, string state);

        public event SpiderUploadRefreshEventHander OnSpiderUploadRefresh;

        public void SpiderUploadRefresh(SinaUpload upload, string state)
        {
            if (OnSpiderUploadRefresh != null)
            {
                OnSpiderUploadRefresh?.Invoke(upload, state);
            }
        }


        public delegate void ShowStatusEventHander(string msg, bool bLog = false, Exception ex = null);

        public event ShowStatusEventHander OnShowStatus;

        public void ShowStatus(string msg, bool bLog = false, Exception ex = null)
        {
            OnShowStatus?.Invoke($"[{Thread.CurrentThread.ManagedThreadId}]{msg}", bLog, ex);
        }

        public delegate void RefreshConfigEventHander(SpiderRunningConfig spiderRunninConfig);

        public event RefreshConfigEventHander OnRefreshConfig;

        public void RefreshConfig(SpiderRunningConfig spiderRunninConfig)
        {
            OnRefreshConfig?.Invoke(spiderRunninConfig);
        }


        public delegate void SpiderStartedEventHander(SpiderRunningTask runningTask);

        public event SpiderStartedEventHander OnSpiderStarted;

        public void SpiderStarted()
        {
            IsSpiderStarted = true;
            StopSpiderWork = false;
            if (OnSpiderStarted != null)
            {
                OnSpiderStarted?.Invoke(RunningTask);
            }
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
                readUsers = Repository.GetUsers(RunningConfig.Category);
                foreach (var user in readUsers.OrderBy(c => c.lastdate).ToArray())
                {
                    RunningTask.AddUser(user);
                }
            }
            else if (RunningConfig.ReadUserOfMyFocus)
            {
                readUsers = Repository.GetFocusUsers(RunningConfig.Category);
                foreach (var user in readUsers.OrderByDescending(c => c.focus).ThenByDescending(c => c.lastdate).ToArray())
                {
                    RunningTask.AddUser(user);
                }
            }
            if(RunningTask.DoUsers.Count == 0)
            {
                if (!string.IsNullOrEmpty(startUser) && startUser.Length == 10)
                {
                    var user = Repository.GetUser(startUser);
                    if (user == null)
                    {
                        user = new SinaUser()
                        {
                            uid = startUser,
                            category = RunningConfig.Category
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
            //更新用户微博信息
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

        /// <summary>
        /// 负责存储数据
        /// </summary>
        public SinaRepository Repository { get; set; }

        public SpiderRunningConfig RunningConfig { get; set; }

        public SpiderRunningTask RunningTask { get; set; } 

        #endregion

        #region 构造函数
        public MWeiboSpiderService()
        {
            Repository = new SinaRepository();

            RunningConfig = new SpiderRunningConfig();
            RunningTask = new SpiderRunningTask();
        }

        #endregion

        #region 开始&结束&追加&取消采集

        public void StartSpider(SpiderRunningConfig runningConfig, MWeiboSpiderStartOption option)
        {
            this.RunningConfig = runningConfig;

            this.RunningTask.GatherType = option.GatherType;
            this.RunningTask.Reset();

            if (string.IsNullOrEmpty(RunningConfig.Category))
            {
                ShowStatus("采集类目为空!");
                return;
            }

            Task.Factory.StartNew(() =>
            {
                if (option.GatherType == GatherTypeEnum.GatherUser)
                {
                    MakeGatherUsers(option.SelectUsers, option.StartUrl);
                    SpiderStarted();
                    StartAutoGatherByUser();
                }
                else if (option.GatherType == GatherTypeEnum.GahterStatus)
                {
                    SpiderStarted();
                    StartAutoGatherByStatus(option.StatusIds, option.StartUrl);
                }
                else if (option.GatherType == GatherTypeEnum.GatherTopic)
                {
                    SpiderStarted();
                    StartAutoGatherByTopic(option.StartUrl);
                }
                else
                {
                    SpiderStarted();
                    StartAutoGatherBySuper(option.StartUrl);
                }

                SpiderComplete();
            });
        }
        public void StopSpider()
        {
            StopSpiderWork = true;

            SpiderStoping();
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

        void StartAutoGatherByStatus(IList<string> statusIds, string userId)
        {
            var threads = new List<Task>();
            var task = Task.Factory.StartNew(() =>
            {
                ShowStatus($"准备读取用户【{userId}】的微博数据...");

                var tempRuningCache = new SpiderRunningCache(RunningConfig.Category, RunningConfig.Site, userId);
                int readStatusImageCount = 0;
                foreach (var status in statusIds)
                {
                    readStatusImageCount += GatherSinaStatusByStatusUrl(tempRuningCache, status);
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
                    if(!RunningTask.CheckTaskRunning())
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
                    var runningCache = new SpiderRunningCache(RunningConfig.Category, RunningConfig.Site, user.uid);
                    var readStatusImageCount = 0;
                    if (RunningConfig.GatherStatusWithNoSource)
                    {
                        readStatusImageCount = GatherSinaStatusByUserStatus(runningCache, user.uid);
                    }
                    else if(RunningConfig.GatherStatusUpdateLocalSource)
                    {
                        readStatusImageCount = GatherSinaSourceByUserStatus(runningCache, user.uid);
                    }
                    else
                    {
                        readStatusImageCount = GatherSinaStatusByUserUrl(runningCache, user.uid);
                    }

                    GatherUserComplete(user, readStatusImageCount);

                    ShowStatus($"用户[{user.uid}]采集完成,共采集资源【{readStatusImageCount}】.");
                }
                else
                {
                    GatherUserComplete(user, 0);
                }
            }
        }

        void StartAutoGatherByTopic(string topic)
        {
            ShowStatus($"准备读取话题的微博数据...");

            var threads = new List<Task>();
            var task = Task.Factory.StartNew(() =>
            {
                ShowStatus($"准备读取#{topic}#的微博数据...");
                int readStatusImageCount = 0;
                var tempRuningCache = new SpiderRunningCache(RunningConfig.Category, RunningConfig.Site, null);
                readStatusImageCount += GatherSinaStatusByTopicUrl(tempRuningCache, topic);
                ShowStatus($"采集完成,共采集资源【{readStatusImageCount}】.");
            });
            threads.Add(task);
            Task.WaitAll(threads.ToArray());
        }

        void StartAutoGatherBySuper(string containerid)
        {
            ShowStatus($"准备读取超话的微博数据...");

            var threads = new List<Task>();
            var task = Task.Factory.StartNew(() =>
            {
                ShowStatus($"准备读取#{containerid}#的微博数据...");
                int readStatusImageCount = 0;
                var tempRuningCache = new SpiderRunningCache(RunningConfig.Category, RunningConfig.Site, null);
                readStatusImageCount += GatherSinaStatusBySuperUrl(tempRuningCache, containerid);
                ShowStatus($"采集完成,共采集资源【{readStatusImageCount}】.");
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
                ShowStatus($"用户【{userId}】未采集.");
                return 0;
            }
            if (sinaUser.ignore > 0)
            {
                ShowStatus($"用户【{userId}】已忽略采集.");
                return 0;
            }
            int readStatusImageCount = 0, readUserImageCount = 0;
            var sinaStatuses = Repository.GetUserStatuses(userId);
            foreach(var status in sinaStatuses)
            {
                var suc = CheckUserStatusGather(runningCache, sinaUser, status);
                if (suc)
                {
                    ShowStatus($"开始采集用户【{userId}】微博【{status.bid}】...");
                    readStatusImageCount = GatherSinaStatusByStatusUrl(runningCache, status.bid);
                    readUserImageCount += readStatusImageCount;

                    if (readStatusImageCount > 0)
                    {
                        Repository.UpdateSinaUserQty(userId);
                    }
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
                Thread.Sleep(RunningConfig.ReadFreeWaitMilSecond);
            }
            return readStatusImageCount;
        }

        bool CheckUserStatusGather(SpiderRunningCache runningCache, SinaUser user, SinaStatus status)
        {
            if (RunningConfig.IgnoreDownloadSource)
            {
                ShowStatus($"跳过下载微博资源【{status.bid}】.");
                return false;
            }
            if (status.ignore > 0)
            {
                ShowStatus($"跳过已忽略微博【{status.bid}】.");
                return false;
            }
            if (RunningConfig.IgnoreReadArchiveStatus && status.upload > 0)
            {
                ShowStatus($"跳过已存档微博【{status.bid}】.");
                return false;
            }
            if (RunningConfig.IgnoreReadArchiveStatus && status.upload > 0)
            {
                ShowStatus($"跳过已存档微博【{status.bid}】.");
                return false;
            }
            if (RunningConfig.IgnoreReadGetStatus && status.gets > 0)
            {
                ShowStatus($"跳过已采集微博【{status.bid}】.");
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
                    ShowStatus($"跳过已缓存微博【{status.bid}】.");
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
                ShowStatus($"用户【{userId}】未采集.");
                return 0;
            }
            if (sinaUser.ignore > 0)
            {
                ShowStatus($"用户【{userId}】已忽略采集.");
                return 0;
            }
            int readStatusImageCount = 0;
            var sinaStatuses = Repository.GetUserStatuses(userId);
            var imageFiles = PathUtil.GetStoreUserThumbnailImageFiles(runningCache.Category, userId);
            if(imageFiles.Length > 0)
            {
                var hasBids = sinaStatuses.Where(c => c.mtype == 0).Select(c => c.bid).ToArray();
                var localStatuses = imageFiles.Select(c => new FileInfo(c).Name.Split('_')[0]).GroupBy(c => c).Select(c => new { bid = c.Key, qty = c.Count() }).ToArray();
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
                        ShowStatus($"用户【{userId}】微博【{status.bid}】已更新采集数量.");

                        GatherStatusComplete(userId, localStatus.qty);
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

                    Thread.Sleep(RunningConfig.ReadFreeWaitMilSecond);
                }
                var notExtBids = bids.Where(c => !hasBids.Contains(c)).ToArray();
                var notExtLoaclStatuses = localStatuses.Where(c => notExtBids.Contains(c.bid)).ToArray();
                foreach (var localStatus in notExtLoaclStatuses)
                {
                    ShowStatus($"开始采集用户【{userId}】微博【{localStatus.bid}】...");
                    readStatusImageCount += GatherSinaStatusByStatusUrl(runningCache, localStatus.bid);

                    var extStatus = Repository.GetUserStatus(localStatus.bid);
                    if(extStatus != null && extStatus.ignore > 0)
                    {
                        PathUtil.DeleteStoreUserImageFiles(runningCache.Category, userId, extStatus.bid);
                        ShowStatus($"用户【{userId}】微博【{extStatus.bid}】已忽略删除.");
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
                    Thread.Sleep(RunningConfig.ReadFreeWaitMilSecond);
                }

                Repository.UpdateSinaUserQty(userId);
            }
            var videoFiles = PathUtil.GetStoreUserVideoFiles(runningCache.Category, userId);
            if(videoFiles.Length > 0)
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
                        ShowStatus($"用户【{userId}】微博【{status.bid}】已更新采集数量.");

                        GatherStatusComplete(userId, localStatus.qty);
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
                    Thread.Sleep(RunningConfig.ReadFreeWaitMilSecond);
                }
                var notExtBids = bids.Where(c => !hasBids.Contains(c)).ToArray();
                var notExtLoaclStatuses = localStatuses.Where(c => notExtBids.Contains(c.bid)).ToArray();
                foreach (var localStatus in notExtLoaclStatuses)
                {
                    ShowStatus($"开始采集用户【{userId}】微博【{localStatus.bid}】...");
                    readStatusImageCount += GatherSinaStatusByStatusUrl(runningCache, localStatus.bid);

                    var extStatus = Repository.GetUserStatus(localStatus.bid);
                    if (extStatus != null && extStatus.ignore > 0)
                    {
                        PathUtil.DeleteStoreUserVideoFile(runningCache.Category, userId, extStatus.bid);
                        ShowStatus($"用户【{userId}】微博【{extStatus.bid}】已忽略删除.");
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

            var sinaUser = Repository.GetUser(user.id);
            if (sinaUser != null && sinaUser.ignore > 0)
            {
                ShowStatus($"用户【{user.id}】已忽略采集.");
                return 0;
            }

            int lastReadPageIndex = 0;
            if (sinaUser != null)
            {
                if (sinaUser.readpage > 0) lastReadPageIndex = sinaUser.readpage;
            }

            int readUserImageCount = 0, readPageIndex = 0, emptyPageCount = 0;
            int startPageIndex = RunningConfig.StartPageIndex;
            if (RunningConfig.GatherContinueLastPage && lastReadPageIndex > 0) startPageIndex = lastReadPageIndex;
            int readPageCount = (RunningConfig.MaxReadPageCount == 0 ? int.MaxValue : startPageIndex + RunningConfig.MaxReadPageCount);
            for (readPageIndex = startPageIndex; readPageIndex < readPageCount; readPageIndex++)
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
                int readPageImageCount = GatherSinaStatusByStatusPageUrl(runningCache, user, readPageIndex, readPageCount, out readPageEmpty, out stopReadNextPage);
                readUserImageCount += readPageImageCount;
                if (!readPageEmpty)
                {
                    GatherPageComplete(user.id, readPageIndex, readUserImageCount);
                }
                if (stopReadNextPage)
                {
                    ShowStatus($"结束采集用户微博数据(下页已采集)...");
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

                    ShowStatus($"等待【{RunningConfig.ReadNextPageWaitSecond}】秒读取用户【{userId}】下一页微博数据...");
                    Thread.Sleep(RunningConfig.ReadNextPageWaitSecond * 1000);
                }
                else
                {
                    Thread.Sleep(RunningConfig.ReadFreeWaitMilSecond);
                }
            }
            return readUserImageCount;
        }

        MWeiboUser GatherSinaUserByUserUrl(SpiderRunningCache runningCache, string userId)
        {
            ShowStatus($"开始读取用户【{userId}】的微博信息...", true);
            var getApi = $"https://m.weibo.cn/api/container/getIndex?type=uid&value={userId}&containerid=100505{userId}";
            var html = HttpUtil.GetHttpRequestHtmlResult(getApi, RunningConfig);
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

            var  sinaUser = Repository.StoreSinaUser(RunningConfig, runningCache, user);
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
        int GatherSinaStatusByStatusPageUrl(SpiderRunningCache runningCache, MWeiboUser user, int readPageIndex, int readPageCount, out bool readPageEmpty, out bool stopReadNextPage)
        {
            readPageEmpty = false;
            stopReadNextPage = false;
            runningCache.CurrentPageIndex = readPageIndex;

            ShowStatus($"开始读取用户【{user.id}】的第【{readPageIndex}】页微博数据...", true);
            var getApi = $"https://m.weibo.cn/api/container/getIndex?type=uid&value={user.id}&containerid=107603{user.id}&page={readPageIndex}";
            var html = HttpUtil.GetHttpRequestHtmlResult(getApi, RunningConfig);
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
                var readStatusImageCount = GatherSinaStatusByStatusOrRetweeted(runningCache, card.mblog, user, out ignoreSourceReaded);
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
                if (readStatusImageCount > 0)
                {
                    ShowStatus($"等待【{RunningConfig.ReadNextStatusWaitSecond}】秒读取用户【{user.id}】下一条微博数据...");
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
        /// 采集微博详细数据（通过URL）
        /// </summary>
        /// <param name="runningConfig"></param>
        int GatherSinaStatusByStatusUrl(SpiderRunningCache runningCache, string status)
        {
            var statusUrl = SinaUrlUtil.GetSinaUserStatusUrl(status);
            var html = HttpUtil.GetHttpRequestHtmlResult(statusUrl, RunningConfig);
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
            bool ignoreSourceReaded = false;
            return GatherSinaStatusByStatusOrRetweeted(runningCache, result.status, result.status.user, out ignoreSourceReaded);
        }

        /// <summary>
        /// 采用微博详细数据（通过Result）
        /// </summary>
        /// <param name="runningConfig"></param>
        /// <param name="status"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        int GatherSinaStatusByStatusOrRetweeted(SpiderRunningCache runningCache, MWeiboStatus status, MWeiboUser user, out bool ignoreSourceReaded)
        {
            ignoreSourceReaded = false;
            int readStatusImageCount = 0;
            //非转发微博
            if (status.retweeted_status == null)
            {
                if (status.pics != null)
                {
                    readStatusImageCount = GatherSinaStatusPicsByStatusResult(runningCache, status, out ignoreSourceReaded);
                    if (readStatusImageCount > 0)
                    {
                        GatherStatusComplete(user.id, readStatusImageCount);
                    }
                }
                else if (status.page_info != null && status.page_info.urls != null)
                {
                    readStatusImageCount = GatherSinaStatusViedoByStatusResult(runningCache, status, out ignoreSourceReaded);
                    if (readStatusImageCount > 0)
                    {
                        GatherStatusComplete(user.id, readStatusImageCount);
                    }
                }
                else
                {
                    Repository.StoreSinaStatus(RunningConfig, user, status, 0, 0, 0, true);
                }
            }
            else
            {
                //存储当前转发微博数据
                Repository.StoreSinaRetweetStatus(RunningConfig, user, status, status.retweeted_status, 0);

                if (status.retweeted_status.user == null)
                {
                    ShowStatus($"微博数据已删除.");
                    return 0;
                }
                if (RunningConfig.ReadOwnerUserStatus && status.retweeted_status.user.id != user.id)
                {
                    ShowStatus($"跳过非本用户【{status.retweeted_status.user.id}】转发数据.");
                    return 0;
                }
                if (!CheckReadUser(status.retweeted_status.user, RunningConfig.ReadUserNameLike))
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
                var sinaUser = Repository.StoreSinaUser(RunningConfig, runningCache, status.retweeted_status.user);
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
                    readStatusImageCount = GatherSinaStatusPicsByStatusResult(runningCache, status.retweeted_status, out ignoreSourceReaded);
                    if (readStatusImageCount > 0)
                    {
                        GatherStatusComplete(user.id, readStatusImageCount);
                    }
                }
                else if (status.retweeted_status.page_info != null && status.retweeted_status.page_info.urls != null)
                {
                    readStatusImageCount = GatherSinaStatusViedoByStatusResult(runningCache, status.retweeted_status, out ignoreSourceReaded);
                    if (readStatusImageCount > 0)
                    {
                        GatherStatusComplete(user.id, readStatusImageCount);
                    }
                }
                else
                {
                    Repository.StoreSinaStatus(RunningConfig, user, status, 1, 0, 0, true);
                }
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
                ShowStatus($"微博数据已删除.");
                return 0;
            }
            if (Repository.CheckUserIgnore(user.id))
            {
                ShowStatus($"用户【{user.id}】已忽略采集.");
                return 0;
            }
            if (status.pics == null || status.pics.Length < RunningConfig.MinReadImageCount)
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
                if (RunningConfig.IgnoreReadArchiveStatus && sinaStatus.upload > 0)
                {
                    ShowStatus($"跳过已存档微博【{status.bid}】.");
                    return 0;
                }
                if (RunningConfig.IgnoreReadGetStatus && sinaStatus.gets > 0)
                {
                    ignoreSourceReaded = true;
                    ShowStatus($"跳过已采集微博【{status.bid}】.");
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
                ShowStatus($"跳过下载微博资源【{status.bid}】.");
                //存储微博
                Repository.StoreSinaStatus(RunningConfig, user, status, 0, status.pics.Length, readCount, false);
                return status.pics.Length;
            }
            else
            {
                if (readCount > 0)
                {
                    //ignoreSourceReaded = true;
                    ShowStatus($"跳过已缓存微博【{status.bid}】.");
                    Repository.StoreSinaStatus(RunningConfig, user, status, 0, status.pics.Length, readCount, false);
                    return 0;
                }
                else
                {
                    ShowStatus($"开始采集用户【{user.id}】第【{runningCache.CurrentPageIndex}】页微博【{status.bid}】...");
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
                        PathUtil.DeleteStoreUserImageFiles(runningCache.Category, user.id, status.bid);
                    }
                    else if (haveReadImageCount > 0 && haveReadImageCount < RunningConfig.MinReadImageCount)
                    {
                        ShowStatus($"微博【{status.bid}】已下载图数不符合删除.");
                        PathUtil.DeleteStoreUserImageFiles(runningCache.Category, user.id, status.bid);

                        haveReadImageCount = 0;
                    }
                    //存储微博
                    Repository.StoreSinaStatus(RunningConfig, user, status, 0, status.pics.Length, haveReadImageCount, haveReadImageCount == 0);
                    return haveReadImageCount;
                }
            }
        }

        int GatherSinaStatusViedoByStatusResult(SpiderRunningCache runningCache, MWeiboStatus status, out bool ignoreSourceReaded)
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
                if (RunningConfig.IgnoreReadArchiveStatus && sinaStatus.upload > 0)
                {
                    ShowStatus($"跳过已存档视频【{status.bid}】.");
                    return 0;
                }
                if (RunningConfig.IgnoreReadGetStatus && sinaStatus.gets > 0)
                {
                    ignoreSourceReaded = true;
                    ShowStatus($"跳过已采集视频【{status.bid}】.");
                    return 0;
                }
            }
            int readCount = 0;
            if (!RunningConfig.IgnoreReadDownStatus)
            {
                if (runningCache.EnabledUserCahce)
                {
                    readCount = runningCache.ExistsVideoLocalFiles.Where(c => c.Contains($"{status.bid}")).Count();
                }
                else
                {
                    readCount = PathUtil.GetStoreUserVideoCount(runningCache.Category, user.id, status.bid);
                }
            }
            if (RunningConfig.IgnoreDownloadSource)
            {
                ShowStatus($"跳过下载微博资源【{status.bid}】.");
                //存储微博
                Repository.StoreSinaStatus(RunningConfig, user, status, 1, 1, readCount, false);
                return 1;
            }
            else
            {
                if (readCount > 0)
                {
                    ShowStatus($"跳过已缓存视频【{status.bid}】.");
                    Repository.StoreSinaStatus(RunningConfig, user, status, 1, 1, readCount, false);
                    return 0;
                }
                else
                {
                    ShowStatus($"开始采集用户【{user.id}】第【{runningCache.CurrentPageIndex}】页视频【{status.bid}】...");
                    int haveReadVedioCount = 0, errorReadVedioCount = 0;
                    var vedioUrl = status.page_info.urls.mp4_hd_mp4;
                    if (string.IsNullOrEmpty(vedioUrl)) vedioUrl = status.page_info.urls.mp4_ld_mp4;

                    var succ = DownloadUserStatusVedio(runningCache, user.id, status.bid, vedioUrl);
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
                        Repository.StoreSinaStatus(RunningConfig, user, status, 1, 1, haveReadVedioCount, haveReadVedioCount == 0);
                    }
                    return haveReadVedioCount;
                }
            }
        }

        /// <summary>
        /// 采集微博详细数据（通过URL）
        /// </summary>
        /// <param name="runningConfig"></param>
        int GatherSinaStatusBySuperUrl(SpiderRunningCache runningCache, string containerid)
        {
            var superUrl = SinaUrlUtil.GetSinaUserSuperUrl(containerid);
            var sinaTopic = GatherSinaTopicBySuperUrl(runningCache, superUrl);
            if (sinaTopic == null) return 0;

            bool hasReadLastPage = false;
            int lastReadPageIndex = 0;
            if (sinaTopic != null)
            {
                if (sinaTopic.lastpage > 0) hasReadLastPage = true;
                if (sinaTopic.readpage > 0) lastReadPageIndex = sinaTopic.readpage;
            }

            int readUserImageCount = 0, readPageIndex = 0, emptyPageCount = 0;
            int startPageIndex = RunningConfig.StartPageIndex;
            if (RunningConfig.GatherContinueLastPage && lastReadPageIndex > 0) startPageIndex = lastReadPageIndex;
            int readPageCount = (RunningConfig.MaxReadPageCount == 0 ? int.MaxValue : startPageIndex + RunningConfig.MaxReadPageCount);
            for (readPageIndex = startPageIndex; readPageIndex < readPageCount; readPageIndex++)
            {
                if (StopSpiderWork)
                {
                    ShowStatus($"中止采集超话微博数据...");
                    break;
                }
                bool readPageEmpty = false, stopReadNextPage = false;
                int readPageImageCount = GatherSinaStatusBySuperPageUrl(runningCache, sinaTopic, readPageIndex, readPageCount, hasReadLastPage, out readPageEmpty, out stopReadNextPage);
                readUserImageCount += readPageImageCount;

                Repository.UpdateSinaTopicPage(sinaTopic.containerid, readPageIndex);

                if (stopReadNextPage)
                {
                    ShowStatus($"结束采集超话微博数据(下页已采集)...");
                    break;
                }

                if (readPageEmpty) emptyPageCount++;
                else emptyPageCount = 0;

                if (emptyPageCount > 3)
                {
                    if (RunningConfig.MaxReadPageCount == 0)
                    {
                        Repository.UpdateTopicLastpage(sinaTopic.containerid);
                    }
                    ShowStatus($"中止采集超话微博数据(连续超过3页无数据)...");
                    break;
                }

                if (StopSpiderWork)
                {
                    ShowStatus($"中止采集超话微博数据...");
                    break;
                }
                if (readPageIndex + 1 < readPageCount && readPageImageCount > 0)
                {
                    ShowStatus($"等待【{RunningConfig.ReadNextPageWaitSecond}】秒读取超话【{sinaTopic.name}】下一页微博数据...");
                    Thread.Sleep(RunningConfig.ReadNextPageWaitSecond * 1000);
                }
                else
                {
                    Thread.Sleep(RunningConfig.ReadFreeWaitMilSecond);
                }
            }
            return readUserImageCount;
        }

        SinaTopic GatherSinaTopicBySuperUrl(SpiderRunningCache runningCache, string startUrl)
        {
            var html = HttpUtil.GetHttpRequestHtmlResult(startUrl, RunningConfig);
            if (html == null)
            {
                ShowStatus($"获取超话基本信息错误!!!!!!");
                return null;
            }
            var result = GetSinaSuperResult(html);
            if (result == null || result.data == null || result.data.pageInfo == null)
            {
                ShowStatus($"解析超话基本信息错误!!!!!!");
                return null;
            }
            var pageInfo = result.data.pageInfo;
            var containerid = pageInfo.containerid.Replace("_-_sort_time", "");
            var sinaTopic = Repository.GetSinaTopic(containerid);
            if(sinaTopic == null)
            {
                sinaTopic = new SinaTopic()
                {
                    type = 1,
                    containerid = containerid,
                    name = pageInfo.page_type_name,
                    desc = pageInfo.desc,
                    category = runningCache.Category,
                    profile = SinaUrlUtil.GetSinaUserSuperWebUrl(containerid),
                };
                Repository.CreateSinaTopic(sinaTopic);
            }
            else
            {
                sinaTopic.category = runningCache.Category;
                sinaTopic.name = pageInfo.page_type_name;
                sinaTopic.desc = pageInfo.desc;
                sinaTopic.profile = SinaUrlUtil.GetSinaUserSuperWebUrl(containerid);
                Repository.UpdateSinaTopic(sinaTopic, new string[] { "name", "desc", "category", "profile" });
            }
            return sinaTopic;
        }

        int GatherSinaStatusBySuperPageUrl(SpiderRunningCache runningCache, SinaTopic sinaTopic, int readPageIndex, int readPageCount, bool hasReadLastPage, out bool readPageEmpty, out bool stopReadNextPage)
        {
            readPageEmpty = false;
            stopReadNextPage = false;
            runningCache.CurrentPageIndex = readPageIndex;

            ShowStatus($"开始读取超话【{sinaTopic.name}】的第【{readPageIndex}】页微博数据...", true);
            //var getApi = $"https://m.weibo.cn/api/container/getIndex?containerid={sinaTopic.containerid}_-_sort_time&page={readPageIndex}";
            var getApi = SinaUrlUtil.GetSinaUserSuperUrl(sinaTopic.containerid, readPageIndex);
            var html = HttpUtil.GetHttpRequestHtmlResult(getApi,RunningConfig);
            if (html == null)
            {
                ShowStatus($"获取超话微博列表错误!!!!!!");
                return 0;
            }
            var result = GetSinaSuperResult(html);
            if (result == null || result.data == null)
            {
                ShowStatus($"解析超话微博列表错误!!!!!!");
                return 0;
            }
            if (result.data.cards.Length == 0)
            {
                ShowStatus($"解析超话微博列表为空...");
                readPageEmpty = true;
                return 0;
            }
            int readPageImageCount = 0;
            foreach (var card in result.data.cards)
            {
                //非微博数据，跳过
                if (card.card_type != 11) continue;
                if (card.card_group == null) continue;

                foreach (var cardGroup in card.card_group)
                {
                    if (cardGroup.card_type != 9) continue;
                    if (cardGroup.card_type_name != "微博") continue;
                    if (cardGroup.mblog == null) continue;
                    if (cardGroup.mblog.user == null) continue;

                    if (StopSpiderWork)
                    {
                        ShowStatus($"中止采集微博数据...");
                        break;
                    }

                    var user = cardGroup.mblog.user;
                    
                    bool ignoreSourceReaded = false;
                    var readStatusImageCount = GatherSinaStatusByStatusOrRetweeted(runningCache, cardGroup.mblog, user, out ignoreSourceReaded);
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
                    if(readStatusImageCount > 0)
                    {
                        var sinaUser = Repository.StoreSinaUser(RunningConfig, runningCache, user);
                        if (sinaUser == null)
                        {
                            ShowStatus($"存储用户信息错误!!!!!!");
                            continue;
                        }
                        if (sinaUser.ignore == 0)
                        {
                            if (sinaUser.id == 0)
                            {
                                GatherNewUser(sinaUser);
                            }
                            Repository.UpdateSinaUserQty(user.id);
                        }
                    }
                    if (ignoreSourceReaded && hasReadLastPage)
                    {
                        stopReadNextPage = true;
                        ShowStatus($"结束采集微博数据(下页已采集)...");
                        break;
                    }
                    if (readStatusImageCount > 0)
                    {
                        ShowStatus($"等待【{RunningConfig.ReadNextStatusWaitSecond}】秒读取超话【{sinaTopic.name}】下一条微博数据...");
                        Thread.Sleep(RunningConfig.ReadNextStatusWaitSecond * 1000);
                    }
                    else
                    {
                        Thread.Sleep(RunningConfig.ReadFreeWaitMilSecond);
                    }
                }
            }
            return readPageImageCount;
        }

        int GatherSinaStatusByTopicUrl(SpiderRunningCache runningCache, string topic)
        {
            var topicUrl = SinaUrlUtil.GetSinaUserTopicUrl(topic);
            var sinaTopic = GatherSinaTopicByTopicUrl(runningCache, topicUrl);
            if (sinaTopic == null) return 0;

            bool hasReadLastPage = false;
            int lastReadPageIndex = 0;
            if (sinaTopic != null)
            {
                if (sinaTopic.lastpage > 0) hasReadLastPage = true;
                if (sinaTopic.readpage > 0) lastReadPageIndex = sinaTopic.readpage;
            }

            int readUserImageCount = 0, readPageIndex = 0, emptyPageCount = 0;
            int startPageIndex = RunningConfig.StartPageIndex;
            if (RunningConfig.GatherContinueLastPage && lastReadPageIndex > 0) startPageIndex = lastReadPageIndex;
            int readPageCount = (RunningConfig.MaxReadPageCount == 0 ? int.MaxValue : startPageIndex + RunningConfig.MaxReadPageCount);
            for (readPageIndex = startPageIndex; readPageIndex < readPageCount; readPageIndex++)
            {
                if (StopSpiderWork)
                {
                    ShowStatus($"中止采集话题微博数据...");
                    break;
                }
                bool readPageEmpty = false, stopReadNextPage = false;
                int readPageImageCount = GatherSinaStatusByTopicPageUrl(runningCache, sinaTopic, readPageIndex, readPageCount, hasReadLastPage, out readPageEmpty, out stopReadNextPage);
                readUserImageCount += readPageImageCount;

                Repository.UpdateSinaTopicPage(sinaTopic.containerid, readPageIndex);

                if (stopReadNextPage)
                {
                    ShowStatus($"结束采集话题微博数据(下页已采集)...");
                    break;
                }

                if (readPageEmpty) emptyPageCount++;
                else emptyPageCount = 0;

                if (emptyPageCount > 3)
                {
                    if (RunningConfig.MaxReadPageCount == 0)
                    {
                        Repository.UpdateTopicLastpage(sinaTopic.containerid);
                    }
                    ShowStatus($"中止采集话题微博数据(连续超过3页无数据)...");
                    break;
                }

                if (StopSpiderWork)
                {
                    ShowStatus($"中止采集话题微博数据...");
                    break;
                }
                if (readPageIndex + 1 < readPageCount && readPageImageCount > 0)
                {
                    ShowStatus($"等待【{RunningConfig.ReadNextPageWaitSecond}】秒读取超话【{sinaTopic.name}】下一页微博数据...");
                    Thread.Sleep(RunningConfig.ReadNextPageWaitSecond * 1000);
                }
                else
                {
                    Thread.Sleep(RunningConfig.ReadFreeWaitMilSecond);
                }
            }
            return readUserImageCount;
        }

        SinaTopic GatherSinaTopicByTopicUrl(SpiderRunningCache runningCache, string startUrl)
        {
            var html = HttpUtil.GetHttpRequestHtmlResult(startUrl, RunningConfig);
            if (html == null)
            {
                ShowStatus($"获取话题基本信息错误!!!!!!");
                return null;
            }
            var result = GetSinaTopicResult(html);
            if (result == null || result.data == null || result.data.cardlistInfo == null)
            {
                ShowStatus($"解析话题基本信息错误!!!!!!");
                return null;
            }
            var pageInfo = result.data.cardlistInfo;
            var name = pageInfo.cardlist_title.Replace("#", "");
            var sinaTopic = Repository.GetSinaTopic(pageInfo.containerid);
            if (sinaTopic == null)
            {
                sinaTopic = new SinaTopic()
                {
                    type = 0,
                    containerid = pageInfo.containerid,
                    name = name,
                    desc = pageInfo.desc,
                    category = runningCache.Category,
                    profile = SinaUrlUtil.GetSinaUserTopicWebUrl(name),
                };
                Repository.CreateSinaTopic(sinaTopic);
            }
            else
            {
                sinaTopic.category = runningCache.Category;
                sinaTopic.name = name;
                sinaTopic.desc = pageInfo.desc;
                sinaTopic.profile = SinaUrlUtil.GetSinaUserTopicWebUrl(name);
                Repository.UpdateSinaTopic(sinaTopic, new string[] { "name", "desc", "category", "profile" });
            }
            return sinaTopic;
        }

        int GatherSinaStatusByTopicPageUrl(SpiderRunningCache runningCache, SinaTopic sinaTopic, int readPageIndex, int readPageCount, bool hasReadLastPage, out bool readPageEmpty, out bool stopReadNextPage)
        {
            readPageEmpty = false;
            stopReadNextPage = false;
            runningCache.CurrentPageIndex = readPageIndex;

            ShowStatus($"开始读取话题【{sinaTopic.name}】的第【{readPageIndex}】页微博数据...", true);
            //var getApi = $"https://m.weibo.cn/api/container/getIndex?containerid=containerid=231522type=61%26t=20%26q=%23{sinaTopic.containerid}%23#&page={readPageIndex}";
            var getApi = SinaUrlUtil.GetSinaUserTopicUrl(sinaTopic.name, readPageIndex);
            var html = HttpUtil.GetHttpRequestHtmlResult(getApi, RunningConfig);
            if (html == null)
            {
                ShowStatus($"获取话题微博列表错误!!!!!!");
                return 0;
            }
            var result = GetSinaTopicResult(html);
            if (result == null || result.data == null)
            {
                ShowStatus($"解析话题微博列表错误!!!!!!");
                return 0;
            }
            if (result.data.cards.Length == 0)
            {
                ShowStatus($"解析话题微博列表为空...");
                readPageEmpty = true;
                return 0;
            }
            int readPageImageCount = 0;
            foreach (var card in result.data.cards)
            {
                //非微博数据，跳过
                if (card.card_type != 9) continue;
                if (card.mblog == null) continue;
                if (card.mblog.user == null) continue;

                if (StopSpiderWork)
                {
                    ShowStatus($"中止采集微博数据...");
                    break;
                }

                var user = card.mblog.user;

                bool ignoreSourceReaded = false;
                var readStatusImageCount = GatherSinaStatusByStatusOrRetweeted(runningCache, card.mblog, user, out ignoreSourceReaded);
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
                if (readStatusImageCount > 0)
                {
                    var sinaUser = Repository.StoreSinaUser(RunningConfig, runningCache, user);
                    if (sinaUser == null)
                    {
                        ShowStatus($"存储用户信息错误!!!!!!");
                        continue;
                    }
                    if (sinaUser.ignore == 0)
                    {
                        if (sinaUser.id == 0)
                        {
                            GatherNewUser(sinaUser);
                        }
                        Repository.UpdateSinaUserQty(user.id);
                    }
                }
                if (ignoreSourceReaded && hasReadLastPage)
                {
                    stopReadNextPage = true;
                    ShowStatus($"结束采集微博数据(下页已采集)...");
                    break;
                }
                if (readStatusImageCount > 0)
                {
                    ShowStatus($"等待【{RunningConfig.ReadNextStatusWaitSecond}】秒读取话题【{sinaTopic.name}】下一条微博数据...");
                    Thread.Sleep(RunningConfig.ReadNextStatusWaitSecond * 1000);
                }
                else
                {
                    Thread.Sleep(RunningConfig.ReadFreeWaitMilSecond);
                }
            }
            return readPageImageCount;
        }

        List<SinaUser> GatherHeFocusUsers()
        {
            var focusUsers = new List<SinaUser>();

            foreach (var user in RunningTask.DoUsers)
            {
                int page = 0;
                while (++page > 0)
                {
                    ShowStatus($"开始读取{user.uid}关注的第{page}页用户信息...", true);
                    var getApi = $"https://m.weibo.cn/api/container/getIndex?containerid=231051_-_followers_-_{user.uid}_-_1042015%253AtagCategory_039&luicode=10000011&lfid=1076033810669779&page={page}";
                    var html = HttpUtil.GetHttpRequestHtmlResult(getApi, RunningConfig);
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

        MWeiboUserResult GetWeiboUserResult(string html)
        {
            try
            {
                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(html);

                var jsonResult = Newtonsoft.Json.JsonConvert.DeserializeObject<MWeiboUserResult>(doc.DocumentNode.InnerText) as MWeiboUserResult;
                return jsonResult;
            }
            catch (Exception ex)
            {
                LogUtil.Error(ex);
                return null;
            }
        }

        MWeiboStatusListResult GetSinaStatusListResult(string html)
        {
            try
            {
                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(html);

                var jsonResult = Newtonsoft.Json.JsonConvert.DeserializeObject<MWeiboStatusListResult>(doc.DocumentNode.InnerText) as MWeiboStatusListResult;
                return jsonResult;
            }
            catch(Exception ex)
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
                ShowStatus($"下载微博【{arcId}】第【{readImageIndex}】张图片错误!");
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
            if(image.Width <= RunningConfig.MinReadImageSize || image.Height <= RunningConfig.MinReadImageSize)
            {
                ShowStatus($"微博【{arcId}】第【{readImageIndex}】张图片尺寸太小忽略");
                return false;
            }
            if (image.Width > RunningConfig.MaxReadImageSize || image.Height > RunningConfig.MaxReadImageSize)
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
        bool DownloadUserStatusVedio(SpiderRunningCache runningCache, string userId, string arcId, string vedioUrl)
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
            var filePath = PathUtil.GetStoreUserVideoFile(runningCache.Category, userId, arcId);
            var down = HttpUtil.GetHttpRequestVedioResult(vedioUrl, filePath, RunningConfig);
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

        #region 上传微博

        public void StartUploadTask()
        {
            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(5 * 1000);

                while(true)
                {
                    var uploads = Repository.GetSinaUploads();
                    if(uploads.Count == 0)
                    {
                        Thread.Sleep(RunningConfig.UploadFreeWaitSecond * 1000);
                        continue;
                    }
                    else
                    {
                        SpiderUploadShow(uploads.ToArray());

                        foreach(var upload in uploads)
                        {
                            var localfile = PathUtil.GetStoreUserImageFile(upload.category, upload.uid, upload.file);
                            if (string.IsNullOrEmpty(localfile))
                            {
                                SpiderUploadRefresh(upload, "文件不存在");
                                continue;
                            }
                            var imgFile = new FileInfo(localfile);

                            if (!imgFile.Exists)
                            {
                                SpiderUploadRefresh(upload, "文件不存在");
                                continue;
                            }

                            //上传开始
                            SpiderUploadRefresh(upload, "Uploading...");

                            var rst = HttpUtil.UploadRemoteImage(RunningConfig, upload, imgFile);
                            if (rst == null)
                            {
                                //上传失败
                                SpiderUploadRefresh(upload, "失败");
                            }
                            else if (!rst.Success)
                            {
                                //上传失败
                                SpiderUploadRefresh(upload, rst.Message);
                            }
                            else
                            {
                                upload.upload = 1;
                                upload.uploadtime = DateTime.Now.ToString("yyyy/MM/dd HH:mm");
                                Repository.UpdateSinaUpload(upload, new string[] { "upload", "uploadtime" });

                                //上传完成
                                SpiderUploadRefresh(upload, "✔");
                            }
                            Thread.Sleep(RunningConfig.UploadSourceWaitMilSecond);
                        }

                        Thread.Sleep(RunningConfig.UploadFreeWaitSecond * 1000);
                    }
                }
            });
        }

        #endregion
    }
}
