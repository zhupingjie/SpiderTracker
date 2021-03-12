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
            OnShowStatus?.Invoke(msg, bLog, ex);
        }

        public delegate void RefreshConfigEventHander(SpiderRunningConfig spiderRunninConfig);

        public event RefreshConfigEventHander OnRefreshConfig;

        public void RefreshConfig(SpiderRunningConfig spiderRunninConfig)
        {
            OnRefreshConfig?.Invoke(spiderRunninConfig);
        }


        public delegate void SpiderStartedEventHander();

        public event SpiderStartedEventHander OnSpiderStarted;

        public void SpiderStarted()
        {
            IsSpiderStarted = true;

            OnSpiderStarted?.Invoke();
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

        public delegate void ChangeUserStatusEventHander(string uid, bool ignore);

        public event ChangeUserStatusEventHander OnChangeUserStatus;

        public void ChangeUserStatus(string uid, bool ignore)
        {
            //OnChangeUserStatus?.Invoke(uid, ignore);
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

        public MWeiboSpiderService()
        {
            Repository = new SinaRepository();
        }

        public void StartSpider(SpiderRunningConfig runningConfig)
        {
            Task.Factory.StartNew(() =>
            {
                if (string.IsNullOrEmpty(runningConfig.Name))
                {
                    ShowStatus("采集类目为空!");
                    return;
                }

                StopSpiderWork = false;
                SpiderStarted();

                switch (runningConfig.GatherType)
                {
                    case GatherTypeEnum.SingleGather:
                        {
                            var readStatusImageCount = StartSpiderGatherTask(runningConfig);
                            ShowStatus($"采集完成,共采集图片【{readStatusImageCount}】张.");
                            break;
                        }
                    case GatherTypeEnum.MultiGather:
                        {
                            var readStatusImageCount = StartStartMultiGatherTask(runningConfig);
                            ShowStatus($"采集完成,共采集图片【{readStatusImageCount}】张.");
                            break;
                        }
                    case GatherTypeEnum.MyFocusGather:
                        {
                            var readStatusImageCount = StartMyFocusGatherTask(runningConfig);
                            ShowStatus($"采集完成,共采集图片【{readStatusImageCount}】张.");
                            break;
                        }
                    case GatherTypeEnum.HeFocusGather:
                        {
                            var readStatusImageCount = StartHeFocusGatherTask(runningConfig);
                            ShowStatus($"采集完成,共采集图片【{readStatusImageCount}】张.");
                            break;
                        }
                    default:
                        {
                            var readStatusImageCount = StartAutoGatherTask(runningConfig);
                            ShowStatus($"采集完成,共采集图片【{readStatusImageCount}】张.");
                            break;
                        }
                }
                SpiderComplete();
            });
        }
        public void StopSpider()
        {
            StopSpiderWork = true;

            SpiderStoping();
        }

        
        void SetWeiboLoginCookieResult(string cookie)
        {
            //SUB=_2A25wKNB1DeRhGedG7lIS8S3PzTiIHXVT0vA9rDV6PUJbkdAKLULFkW1NUViBpBbgQBJA907ftPxat7B2us9K069Y; Path=/; Domain=.weibo.cn; Expires=Tue, 14 Jul 2020 15:47:49 GMT; HttpOnly,
            //SUHB =0103XdpGvcqUF0; expires=Tuesday, 14-Jul-2020 15:47:49 GMT; path=/; domain=.weibo.cn,
            //SSOLoginState =1563205669; path=/; domain=weibo.cn,ALF=1565797669; expires=Wednesday, 14-Aug-2019 15:47:49 GMT; path=/; domain=.sina.cn,
            //login =b956ab2a781575709c4b14df65b7a16b; Path=/

            var sub = cookie.Substring(cookie.IndexOf("SUB=") + 4, cookie.IndexOf(";"));
            cookie = cookie.Substring(cookie.IndexOf(";"));
            var suhb = cookie.Substring(cookie.IndexOf("SUHB =") + 6, cookie.IndexOf(";"));
            cookie = cookie.Substring(cookie.IndexOf(";"));
            var ssologinstate = cookie.Substring(cookie.IndexOf("SSOLoginState =") + 15, cookie.IndexOf(";"));
            cookie = cookie.Substring(cookie.IndexOf(";"));
            var login = cookie.Substring(cookie.IndexOf("login =") + 7, cookie.IndexOf(";"));



        }
        
        int StartAutoGatherTask(SpiderRunningConfig runningConfig)
        {
            var readUsers = new List<SinaUser>();
            if(runningConfig.ReadAllOfUser == 1)
            {
                readUsers = Repository.GetUsers(runningConfig.Name);
                if (!string.IsNullOrEmpty(runningConfig.ReadUserNameLike))
                {
                    readUsers = readUsers.Where(c => c.name.Contains(runningConfig.ReadUserNameLike)).ToList();
                }
                runningConfig.UserIds = readUsers.Select(c => c.uid).ToArray();
            }
            if(runningConfig.ReadUserOfFocus == 1)
            {
                var focusUser = GatherHeFocusUsers(runningConfig);
                if (!string.IsNullOrEmpty(runningConfig.ReadUserNameLike))
                {
                    focusUser = focusUser.Where(c => c.screen_name.Contains(runningConfig.ReadUserNameLike)).ToArray();
                }
                runningConfig.UserIds = runningConfig.UserIds.Concat(focusUser.Select(c => c.id)).Distinct().ToArray();
            }
            if(runningConfig.UserIds.Length == 0)
            {
                ShowStatus($"无采集用户数据.");
                return 0;
            }
            ShowStatus($"准备读取【{runningConfig.UserIds.Length}】个用户的微博数据...");

            return StartStartMultiGatherTask(runningConfig);
        }

        int StartSpiderGatherTask(SpiderRunningConfig runninConfig)
        {
            var sinaUrlEnum = SinaUrlUtil.GetSinaUrlEnum(runninConfig.StartUrl);
            switch (sinaUrlEnum)
            {
                case SinaUrlEnum.UserUrl:
                    return GatherSinaStatusByUserUrl(runninConfig);
                case SinaUrlEnum.StatusUrl:
                    return GatherSinaStatusByStatusUrl(runninConfig);
                default:
                    return 0;
            }
        }



        int StartStartMultiGatherTask(SpiderRunningConfig runningConfig)
        {
            int readStatusImageCount = 0;
            foreach (var user in runningConfig.UserIds)
            {
                runningConfig.StartUrl = SinaUrlUtil.GetSinaUserUrl(user);
                readStatusImageCount += StartSpiderGatherTask(runningConfig);

                if (StopSpiderWork) break;
            }
            return readStatusImageCount;
        }

        int StartMyFocusGatherTask(SpiderRunningConfig runningConfig)
        {
            var user= GatherMyFocusUsers(runningConfig);
            ShowStatus($"总共读取到【{user.Length}】个关注用户的微博数据...");

            runningConfig.UserIds = user.Select(c => c.id).ToArray();
            return StartStartMultiGatherTask(runningConfig);
        }

        MWeiboUser[] GatherMyFocusUsers(SpiderRunningConfig runningConfig)
        {
            int page = 0;
            var focusUsers = new List<MWeiboUser>();
            while (++page > 0)
            {
                ShowStatus($"开始读取我关注的第{page}页用户信息...", true);
                var getApi = $"https://m.weibo.cn/api/container/getIndex?containerid=231093_-_selffollowed&page={page}";
                var html = HttpUtil.GetHttpRequestHtmlResult(getApi, runningConfig);
                if (html == null)
                {
                    ShowStatus($"读取我关注的第{page}页用户信息错误!");
                    return null;
                }
                var result = GetWeiboFocusResult(html);
                if (result == null || result.data == null)
                {
                    ShowStatus($"解析我关注的第{page}页用户错误!");
                    return null;
                }
                var focusUserCard = result.data.cards.FirstOrDefault(c => c.card_type == 11 && !string.IsNullOrEmpty(c.itemid));
                if(focusUserCard == null)
                {
                    break;
                }
                var users = focusUserCard.card_group.Select(c => c.user).ToArray();
                focusUsers.AddRange(users);
            }
            return focusUsers.Distinct().ToArray();
        }

        MWeiboFocusResult GetWeiboFocusResult(string html)
        {
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);

            var jsonResult = Newtonsoft.Json.JsonConvert.DeserializeObject<MWeiboFocusResult>(doc.DocumentNode.InnerText) as MWeiboFocusResult;
            return jsonResult;
        }


        int StartHeFocusGatherTask(SpiderRunningConfig runningConfig)
        {
            var user = GatherHeFocusUsers(runningConfig);
            ShowStatus($"总共读取到【{user.Length}】个关注用户的微博数据...");

            if(!string.IsNullOrEmpty(runningConfig.ReadUserNameLike))
            {
                user = user.Where(c => c.screen_name.Contains(runningConfig.ReadUserNameLike)).ToArray();
            }

            runningConfig.UserIds = user.Select(c => c.id).ToArray();
            return StartStartMultiGatherTask(runningConfig);
        }

        MWeiboUser[] GatherHeFocusUsers(SpiderRunningConfig runningConfig)
        {
            var focusUsers = new List<MWeiboUser>();

            foreach(var userId in runningConfig.UserIds)
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
                    var users = focusUserCard.card_group.Select(c => c.user).ToArray();
                    focusUsers.AddRange(users);
                }
            }
            return focusUsers.Distinct().ToArray();
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

            //if (runningConfig.OnlyReadUserInfo == 1)
            //{
            //    ShowStatus($"只采集用户数据忽略内容【{user.id}】.");
            //    return 0;
            //}

            int readUserImageCount = 0, readPageIndex = 0, emptyPageCount = 0;
            int readPageCount = (runningConfig.ReadPageCount == 0 ? int.MaxValue : runningConfig.StartPageIndex + runningConfig.ReadPageCount);
            for (readPageIndex = runningConfig.StartPageIndex; readPageIndex < readPageCount; readPageIndex++)
            {
                bool stopReadNextPage = false;
                int readPageImageCount = GatherSinaStatusByStatusPageUrl(runningConfig, user, readPageIndex, readPageCount, out stopReadNextPage);
                readUserImageCount += readPageImageCount;

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
                    ShowStatus($"等待【{runningConfig.ReadNextPageWaitSecond}】秒读取下一页微博数据...");
                    Thread.Sleep(runningConfig.ReadNextPageWaitSecond * 1000);
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
                ShowStatus($"获取用户信息错误!");
                return null;
            }
            var result = GetWeiboUserResult(html);
            if (result == null || result.data == null || result.data.userInfo == null)
            {
                ShowStatus($"解析用户信息错误!");
                return null;
            }
            var user = result.data.userInfo;
            var rst = Repository.StoreSinaUser(runningConfig, user, true);
            if (!string.IsNullOrEmpty(rst))
            {
                ShowStatus(rst);
                return null;
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
                ShowStatus($"关注用户信息错误!");
                return;
            }
            var result = GetWeiboFocusUserResult(html);
            if (result == null || result.ok != 1)
            {
                ShowStatus($"关注用户信息错误!");
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
                ShowStatus($"获取用户微博列表错误!");
                return 0;
            }
            var result = GetSinaStatusListResult(html);
            if (result == null || result.data == null)
            {
                ShowStatus($"解析用户微博列表错误!");
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
                    ShowStatus($"中止采集组图数据...");
                    break;
                }
                if (readStatusImageCount > 0)
                {
                    ShowStatus($"等待【{runningConfig.ReadNextStatusWaitSecond}】秒读取下一条微博数据...");
                    Thread.Sleep(runningConfig.ReadNextStatusWaitSecond * 1000);
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
                ShowStatus($"获取用户微博列表错误!");
                return 0;
            }
            var result = GetWeiboStatusResult(html);
            if (result == null || result.status == null)
            {
                ShowStatus($"解析用户微博列表错误!");
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
            if (status.pics != null)
            {
                readStatusImageCount = GatherSinaStatusByStatusResult(runningConfig, status);
                Repository.StoreSinaStatus(user, status, null, readStatusImageCount);
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
                if (Repository.CheckUserIgnore(status.retweeted_status.user.id))
                {
                    ShowStatus($"用户【{status.retweeted_status.user.id}】已忽略采集.");
                    return 0;
                }
                //存储转发用户信息
                var rst = Repository.StoreSinaUser(runningConfig, status.retweeted_status.user, false);
                if (!string.IsNullOrEmpty(rst))
                {
                    ShowStatus(rst);
                    return 0;
                }
                readStatusImageCount = GatherSinaStatusByStatusResult(runningConfig, status.retweeted_status);
                Repository.StoreSinaStatus(user, status, status.retweeted_status, readStatusImageCount);
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
        int GatherSinaStatusByStatusResult(SpiderRunningConfig runninConfig, MWeiboStatus status)
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
            if (PathUtil.CheckUserStatusExists(runninConfig.Name, user.id, status.bid))
            {
                ShowStatus($"跳过已缓存组图【{status.bid}】.");
                return -1;
            }
            var sinaStatus = Repository.GetUserStatus(status.bid);
            if(sinaStatus != null)
            {
                if(sinaStatus.ignore == 1)
                {
                    ShowStatus($"跳过已忽略组图【{status.bid}】.");
                    return -1;
                }
                if(runninConfig.IgnoreReadArchiveStatus == 1 && sinaStatus.archive == 1)
                {
                    ShowStatus($"跳过已存档组图【{status.bid}】.");
                    return -1;
                }
            }
            if (status.pics == null || status.pics.Length < runninConfig.ReadMinOfImgCount)
            {
                ShowStatus($"跳过不符合最小图数组图【{status.bid}】.");
                return 0;
            }
            ShowStatus($"开始采集用户【{user.id}】第【{runninConfig.CurrentPageIndex}】页组图【{status.bid}】...");
            int haveReadImageCount = 0, readImageIndex = 0;
            foreach (var pic in status.pics)
            {
                var succ = DownloadUserStatusImage(runninConfig, user.id, status.bid, pic.large.url, readImageIndex);
                if (succ)
                {
                    haveReadImageCount++;
                }
                readImageIndex++;
                Thread.Sleep(500);
            }
            if (haveReadImageCount == 0)
            {
                ShowStatus($"组图【{status.bid}】已无符合尺寸的图片.");
                var path = PathUtil.GetStoreImageUserStatusPath(runninConfig.Name, user.id, status.bid);
                if(Directory.Exists(path)) Directory.Delete(path, true);
            }
            else if (haveReadImageCount < runninConfig.ReadMinOfImgCount)
            {
                ShowStatus($"组图【{status.bid}】已下载图数不符合删除.");
                var path = PathUtil.GetStoreImageUserStatusPath(runninConfig.Name, user.id, status.bid);
                if (Directory.Exists(path)) Directory.Delete(path, true);

                haveReadImageCount = 0;
            }
            return haveReadImageCount;
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
        bool DownloadUserStatusImage(SpiderRunningConfig runningConfig, string userId, string arcId, string imgUrl, int haveReadPageCount)
        {
            if (!Repository.ExistsSinaPicture(userId, arcId, imgUrl))
            {
                var sinaPicture = new SinaPicture()
                {
                    uid = userId,
                    bid = arcId,
                    picurl = imgUrl
                };
                var suc = Repository.CreateSinaPicture(sinaPicture);
                if (!suc)
                {
                    ShowStatus($"创建本地微博图片错误.");
                    return false;
                }
            }
            //if(runningConfig.OnlyReadUserPicture == 1)
            //{
            //    ShowStatus($"只采集微博图片忽略下载.");
            //    return false;
            //}
            var image = HttpUtil.GetHttpRequestImageResult(imgUrl, runningConfig);
            if (image == null)
            {
                ShowStatus($"下载第【{(haveReadPageCount + 1)}】张图片错误!");
                return false;
            }
            if (!CheckImageSize(runningConfig, image, haveReadPageCount)) return false;

            var path = PathUtil.GetStoreImageUserStatusPath(runningConfig.Name, userId, arcId);
            PathUtil.CheckCreateDirectory(path);
            var img = Path.Combine(path, $"{DateTime.Now.ToString("HHmmssffff")}.jpg");
            try
            {
                image.Save(img);
                image.Dispose();
                ShowStatus($"下载第【{(haveReadPageCount + 1)}】张图片(OK).");
                return true;
            }
            catch(Exception ex)
            {
                ShowStatus($"下载第【{(haveReadPageCount + 1)}】张图片错误!");
                return false;
            }
        }

        bool CheckImageSize(SpiderRunningConfig runningConfig,Image image, int haveReadPageCount)
        {
            if(image.Width <= runningConfig.ReadMinOfImgSize || image.Height <= runningConfig.ReadMinOfImgSize)
            {
                ShowStatus($"第【{(haveReadPageCount + 1)}】张图片尺寸太小忽略");
                return false;
            }
            if (image.Width > runningConfig.ReadMaxOfImgSize || image.Height > runningConfig.ReadMaxOfImgSize)
            {
                ShowStatus($"第【{(haveReadPageCount + 1)}】张图片尺寸太大忽略");
                return false;
            }
            return true;
        }
        #endregion
    }
}
