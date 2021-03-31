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
                readUsers = Repository.GetUsers(RunningConfig.Name);
                if (!string.IsNullOrEmpty(RunningConfig.ReadUserNameLike))
                {
                    readUsers = readUsers.Where(c => (c.name.Contains(RunningConfig.ReadUserNameLike) || c.desc.Contains(RunningConfig.ReadUserNameLike))).ToList();
                }
                foreach (var user in readUsers)
                {
                    RunningConfig.AddUser(user);
                }
            }
            if(RunningConfig.ReadUserOfMyFocus == 1)
            {
                readUsers = Repository.GetFocusUsers(RunningConfig.Name);
                if (!string.IsNullOrEmpty(RunningConfig.ReadUserNameLike))
                {
                    readUsers = FilterReadUser(readUsers, RunningConfig.ReadUserNameLike);
                }
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
                foreach (var user in focusUser)
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

        public SinaUser PeekUser()
        {
            SinaUser user = null;
            RunningConfig.DoUsers.TryDequeue(out user);
            return user;
        }

        public int GetUserCount()
        {
            return RunningConfig.DoUsers.Count;
        }

        #endregion

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

        public MWeiboSpiderService()
        {
            Repository = new SinaRepository();
        }

        public void StartSpider(SpiderRunningConfig runningConfig, IList<SinaUser> users, IList<string> statusIds)
        {
            this.RunningConfig = runningConfig;
            this.RunningConfig.Reset();

            if (string.IsNullOrEmpty(RunningConfig.Name))
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
            });
        }
        public void StopSpider()
        {
            StopSpiderWork = true;

            SpiderStoping();
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

                var tempRuningConfig = RunningConfig.Clone();
                tempRuningConfig.StartUrl = SinaUrlUtil.GetSinaUserUrl(user.uid);
                var readStatusImageCount = GatherSinaStatusByUserUrl(tempRuningConfig);

                GatherUserComplete(user, readStatusImageCount);

                ShowStatus($"用户[{user.uid}]采集完成,共采集资源【{readStatusImageCount}】.");
            }
        }

        MWeiboFocusResult GetWeiboFocusResult(string html)
        {
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);

            var jsonResult = Newtonsoft.Json.JsonConvert.DeserializeObject<MWeiboFocusResult>(doc.DocumentNode.InnerText) as MWeiboFocusResult;
            return jsonResult;
        }

        List<SinaUser> GatherHeFocusUsers(SpiderRunningConfig runningConfig)
        {
            var focusUsers = new List<SinaUser>();

            foreach(var userId in runningConfig.DoUsers)
            {
                int page = 0;
                while (++page > 0)
                {
                    ShowStatus($"开始读取{userId}关注的第{page}页用户信息...", true);
                    var getApi = $"https://m.weibo.cn/api/container/getIndex?containerid=231051_-_followers_-_{userId}_-_1042015%253AtagCategory_039&luicode=10000011&lfid=1076033810669779&page={page}";
                    var html = HttpUtil.GetHttpRequestHtmlResult(getApi, runningConfig);
                    if (html == null)
                    {
                        ShowStatus($"读取{userId}关注的第{page}页用户信息错误!");
                        return null;
                    }
                    var result = GetWeiboFocusResult(html);
                    if (result == null || result.data == null)
                    {
                        ShowStatus($"解析{userId}关注的第{page}页用户错误!");
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
                        groupname = runningConfig.Name
                    }).ToArray();
                    focusUsers.AddRange(users);
                }
            }
            return focusUsers.ToList();
        }


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

            if (Repository.CheckUserIgnore(user.id))
            {
                ShowStatus($"用户【{user.id}】已忽略采集.");
                return 0;
            }

            int readUserImageCount = 0, readPageIndex = 0, emptyPageCount = 0;
            int readPageCount = (runningConfig.ReadPageCount == 0 ? int.MaxValue : runningConfig.StartPageIndex + runningConfig.ReadPageCount);
            for (readPageIndex = runningConfig.StartPageIndex; readPageIndex < readPageCount; readPageIndex++)
            {
                bool stopReadNextPage = false;
                int readPageImageCount = GatherSinaStatusByStatusPageUrl(runningConfig, user, readPageIndex, readPageCount, out stopReadNextPage);
                readUserImageCount += readPageImageCount;

                GatherPageComplete(user.id, readPageIndex, readUserImageCount);

                if (stopReadNextPage) emptyPageCount++;
                else emptyPageCount = 0;

                if (emptyPageCount > 10)
                {
                    ShowStatus($"连续超过10页无数据中止采集...");
                    break;
                }

                if (StopSpiderWork)
                {
                    ShowStatus($"中止采集用户微博数据...");
                    break;
                }
                if (readPageIndex + 1 < readPageCount && readPageImageCount > 0)
                {
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

            SinaUser newUser = null;
            var rst = Repository.StoreSinaUser(runningConfig, user, true, out newUser);
            if (!string.IsNullOrEmpty(rst))
            {
                ShowStatus(rst);
                return null;
            }
            if (newUser != null)
            {
                GatherNewUser(newUser);
            }
            return user;
        }

        void FocusSinaUser(SpiderRunningConfig runningConfig, MWeiboUser user)
        {
            ShowStatus($"开始关注用户【{user.id}】的微博信息...", true);
            var getApi = $"https://m.weibo.cn/api/friendships/create";

            var paramData = $"uid={user.id}&st={runningConfig.LoginToken}";
            var html = HttpUtil.PostHttpRequest(getApi, paramData, runningConfig);
            if (html == null)
            {
                ShowStatus($"关注用户信息错误!!!!!!");
                return;
            }
            var result = GetWeiboFocusUserResult(html);
            if (result == null || result.ok != 1)
            {
                ShowStatus($"关注用户信息错误!!!!!!");
                return;
            }
        }

        MWeiboFoucsUserResult GetWeiboFocusUserResult(string html)
        {
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);

            var jsonResult = Newtonsoft.Json.JsonConvert.DeserializeObject<MWeiboFoucsUserResult>(doc.DocumentNode.InnerText) as MWeiboFoucsUserResult;
            return jsonResult;
        }

        /// <summary>
        /// 采集用户微博列表数据
        /// </summary>
        /// <param name="runningConfig"></param>
        /// <param name="user"></param>
        /// <param name="readPageIndex"></param>
        /// <param name="readPageCount"></param>
        /// <returns></returns>
        int GatherSinaStatusByStatusPageUrl(SpiderRunningConfig runningConfig, MWeiboUser user, int readPageIndex, int readPageCount, out bool stopReadNextPage)
        {
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
            if(result.data.cards.Length == 0)
            {
                stopReadNextPage = true;
                return 0;
            }
            int readPageImageCount = 0;
            foreach (var card in result.data.cards)
            {
                //非微博数据，跳过
                if (card.card_type != 9) continue;

                var readStatusImageCount = GatherSinaStatusByStatusOrRetweeted(runningConfig, card.mblog, user);
                readPageImageCount += readStatusImageCount;

                if (StopSpiderWork)
                {
                    ShowStatus($"中止采集微博数据...");
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
            if(user == null)
            {
                ShowStatus($"微博数据已删除.");
                return 0;
            }
            if (Repository.CheckUserIgnore(user.id))
            {
                ShowStatus($"用户【{user.id}】已忽略采集.");
                return 0;
            }
            return GatherSinaStatusByStatusOrRetweeted(runningConfig, result.status, result.status.user);
        }

        /// <summary>
        /// 采用微博详细数据（通过Result）
        /// </summary>
        /// <param name="runningConfig"></param>
        /// <param name="status"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        int GatherSinaStatusByStatusOrRetweeted(SpiderRunningConfig runningConfig, MWeiboStatus status, MWeiboUser user)
        {
            int readStatusImageCount = 0;
            if (status.pics != null && runningConfig.ReadUserPics == 1)
            {

                SinaStatus newStatus = null;
                readStatusImageCount = GatherSinaStatusPicsByStatusResult(runningConfig, status);
                Repository.StoreSinaStatus(user, status, null, 0, status.pics.Length, readStatusImageCount, out newStatus);
                if (readStatusImageCount > 0)
                {
                    GatherStatusComplete(user.id, readStatusImageCount);
                }
            }
            if(status.page_info != null && status.page_info.urls != null && runningConfig.ReadUserVideo == 1)
            {
                SinaStatus newStatus = null;
                readStatusImageCount = GatherSinaStatusViedoByStatusResult(runningConfig, status);
                Repository.StoreSinaStatus(user, status, null,1, 1, readStatusImageCount, out newStatus);
                if (readStatusImageCount > 0)
                {
                    GatherStatusComplete(user.id, readStatusImageCount);
                }
            }
            if (status.retweeted_status != null)
            {
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
                if(!string.IsNullOrEmpty(runningConfig.ReadUserNameLike) && (!status.retweeted_status.user.description.Contains(runningConfig.ReadUserNameLike) && !status.retweeted_status.user.screen_name.Contains(runningConfig.ReadUserNameLike)))
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
                SinaUser newUser = null;
                var rst = Repository.StoreSinaUser(runningConfig, status.retweeted_status.user, false, out newUser);
                if (!string.IsNullOrEmpty(rst))
                {
                    ShowStatus(rst);
                    return 0;
                }
                if(newUser != null)
                {
                    GatherNewUser(newUser);
                }
                if (status.retweeted_status.pics != null && runningConfig.ReadUserPics == 1)
                {
                    SinaStatus newStatus = null;
                    readStatusImageCount = GatherSinaStatusPicsByStatusResult(runningConfig, status.retweeted_status);
                    Repository.StoreSinaStatus(user, status, status.retweeted_status, 0, status.retweeted_status.pics.Length, readStatusImageCount, out newStatus);
                    if (readStatusImageCount > 0)
                    {
                        GatherStatusComplete(user.id, readStatusImageCount);
                    }
                }
                if(status.retweeted_status.page_info != null && status.retweeted_status.page_info.urls != null  && runningConfig.ReadUserVideo == 1)
                {
                    SinaStatus newStatus = null;
                    readStatusImageCount = GatherSinaStatusViedoByStatusResult(runningConfig, status.retweeted_status);
                    Repository.StoreSinaStatus(user, status, status.retweeted_status, 1, 1, readStatusImageCount, out newStatus);
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
        int GatherSinaStatusPicsByStatusResult(SpiderRunningConfig runninConfig, MWeiboStatus status)
        {
            var user = status.user;
            if (user == null)
            {
                ShowStatus($"微博数据已删除.");
                return -1;
            }
            if (Repository.CheckUserIgnore(user.id))
            {
                ShowStatus($"用户【{user.id}】已忽略采集.");
                return 0;
            }
            if (PathUtil.CheckStoreUserImageExists(runninConfig.Name, user.id, status.bid))
            {
                ShowStatus($"跳过已缓存微博【{status.bid}】.");
                return -1;
            }
            var sinaStatus = Repository.GetUserStatus(status.bid);
            if(sinaStatus != null)
            {
                if(sinaStatus.ignore > 0)
                {
                    ShowStatus($"跳过已忽略微博【{status.bid}】.");
                    return -1;
                }
                if(runninConfig.IgnoreReadArchiveStatus == 1 && sinaStatus.archive == 1)
                {
                    ShowStatus($"跳过已存档微博【{status.bid}】.");
                    return -1;
                }
            }
            if (status.pics == null || status.pics.Length < runninConfig.ReadMinOfImgCount)
            {
                ShowStatus($"跳过不符合最小图数微博【{status.bid}】.");
                return 0;
            }
            ShowStatus($"开始采集用户【{user.id}】第【{runninConfig.CurrentPageIndex}】页微博【{status.bid}】...");
            int haveReadImageCount = 0, readImageIndex = 0, errorReadImageCount = 0;
            foreach (var pic in status.pics)
            {
                bool err = false;
                var succ = DownloadUserStatusImage(runninConfig, user.id, status.bid, pic.large.url, ++readImageIndex, out err);
                if (succ)
                {
                    haveReadImageCount++;
                }
                if(err)
                {
                    errorReadImageCount++;
                }
                Thread.Sleep(500);
            }
            if(errorReadImageCount == status.pics.Length && errorReadImageCount > 0)
            {
                return -1;
            }
            if (haveReadImageCount == 0)
            {
                ShowStatus($"微博【{status.bid}】已无符合尺寸的图片.");
                PathUtil.DeleteStoreUserImageFiles(runninConfig.Name, user.id, status.bid);
            }
            else if (haveReadImageCount < runninConfig.ReadMinOfImgCount)
            {
                ShowStatus($"微博【{status.bid}】已下载图数不符合删除.");
                PathUtil.DeleteStoreUserImageFiles(runninConfig.Name, user.id, status.bid);

                haveReadImageCount = 0;
            }
            return haveReadImageCount;
        }

        int GatherSinaStatusViedoByStatusResult(SpiderRunningConfig runninConfig, MWeiboStatus status)
        {
            var user = status.user;
            if (user == null)
            {
                ShowStatus($"微博数据已删除.");
                return -1;
            }
            if (Repository.CheckUserIgnore(user.id))
            {
                ShowStatus($"用户【{user.id}】已忽略采集.");
                return 0;
            }
            if (PathUtil.CheckStoreUserVideoExists(runninConfig.Name, user.id, status.bid))
            {
                ShowStatus($"跳过已缓存视频【{status.bid}】.");
                return -1;
            }
            var sinaStatus = Repository.GetUserStatus(status.bid);
            if (sinaStatus != null)
            {
                if (sinaStatus.ignore > 0)
                {
                    ShowStatus($"跳过已忽略视频【{status.bid}】.");
                    return -1;
                }
                if (runninConfig.IgnoreReadArchiveStatus == 1 && sinaStatus.archive == 1)
                {
                    ShowStatus($"跳过已存档视频【{status.bid}】.");
                    return -1;
                }
                if (runninConfig.IgnoreDeleteSourceStatus == 1)
                {
                    Repository.IgnoreSinaStatus(status.bid);

                    ShowStatus($"忽略已删除视频【{status.bid}】.");
                    return -1;
                }
            }
            if (status.page_info.urls == null || string.IsNullOrEmpty(status.page_info.urls.mp4_hd_mp4) || string.IsNullOrEmpty(status.page_info.urls.mp4_ld_mp4))
            {
                ShowStatus($"跳过无链接视频【{status.bid}】.");
                return -1;
            }
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
            if (errorReadVedioCount > 0)
            {
                return -1;
            }
            return haveReadVedioCount;
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
        bool DownloadUserStatusImage(SpiderRunningConfig runningConfig, string userId, string arcId, string imgUrl, int readImageIndex, out bool err)
        {
            err = false;

            var path = PathUtil.GetStoreUserPath(runningConfig.Name, userId);
            PathUtil.CheckCreateDirectory(path);
            var imgPath = Path.Combine(path, $"{arcId}_{readImageIndex}.jpg");
            var localPath = $"\\{userId}\\{arcId}_{readImageIndex}.jpg";
            var image = HttpUtil.GetHttpRequestImageResult(imgUrl, runningConfig);
            if (image == null)
            {
                ShowStatus($"下载微博【{arcId}】第【{readImageIndex}】张图片错误!"); 
                err = true;
                return false;
            }
            if (!CheckImageSize(runningConfig, image, arcId, readImageIndex)) return false;

            var (thumbWidth, thumbHeight) = GetThumbImageSize(image, runningConfig.ThumbnailImageWidth, runningConfig.ThumbnailImageHeight);
            var thumbImg = image.GetThumbnailImage(thumbWidth, thumbHeight, null, IntPtr.Zero);
            var thunbPath = Path.Combine(path, "thumb");
            PathUtil.CheckCreateDirectory(thunbPath);
            var thumbnail = $"\\{userId}\\thumb\\{arcId}_{readImageIndex}.jpg";
            var thumbPath = Path.Combine(thunbPath, $"{arcId}_{readImageIndex}.jpg");

            try
            {
                image.Save(imgPath);
                thumbImg.Save(thumbPath);
                ShowStatus($"下载微博【{arcId}】第【{readImageIndex}】张图片(OK).");

                if (!Repository.ExistsSinaPicture(userId, arcId, imgUrl))
                {
                    var sinaPicture = new SinaPicture()
                    {
                        uid = userId,
                        bid = arcId,
                        url = imgUrl,
                        localpath = localPath,
                        thumbnail = thumbnail,
                        width = image.Width,
                        height = image.Height,
                        downdate = DateTime.Now
                    };
                    var suc = Repository.CreateSinaPicture(sinaPicture);
                    if (!suc)
                    {
                        if (File.Exists(imgPath)) File.Delete(imgPath);
                        if (File.Exists(imgPath)) File.Delete(thumbPath);

                        image.Dispose();
                        thumbImg.Dispose();

                        ShowStatus($"创建微博【{arcId}】第【{readImageIndex}】张图片错误!!!!!!");
                        err = true;
                        return false;
                    }
                }
                return true;
            }
            catch(Exception ex)
            {
                if (File.Exists(imgPath)) File.Delete(imgPath);
                if (File.Exists(imgPath)) File.Delete(thumbPath);
                ShowStatus($"下载微博【{arcId}】第【{readImageIndex}】张图片错误!!!!!!");
                err = true;
                return false;
            }
            finally
            {
                image.Dispose();
                thumbImg.Dispose();
            }
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
            if (!Repository.ExistsSinaPicture(userId, arcId, vedioUrl))
            {
                var sinaPicture = new SinaPicture()
                {
                    uid = userId,
                    bid = arcId,
                    url = vedioUrl
                };
                var suc = Repository.CreateSinaPicture(sinaPicture);
                if (!suc)
                {
                    ShowStatus($"创建本地微博图片错误!!!!!!");
                    return false;
                }
            }
            var filePath = PathUtil.GetStoreUserVideoFile(runningConfig.Name, userId, arcId);
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
