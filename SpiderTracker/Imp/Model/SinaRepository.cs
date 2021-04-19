using SpiderTracker.Imp.MWeiboJson;
using SpiderTracker.Imp.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiderTracker.Imp.Model
{
    public class SinaRepository
    {
        protected SQLiteDBHelper DBHelper = SQLiteDBHelper.Instance;

        public SinaRepository()
        {

        }

        public bool CreateSinaSource(SinaSource picture)
        {
            return DBHelper.CreateEntity(picture, "sina_source");
        }

        public bool CreateSinaUser(SinaUser user)
        {
            return DBHelper.CreateEntity(user, "sina_user");
        }

        public bool CreateSinaTopic(SinaTopic super)
        {
            return DBHelper.CreateEntity(super, "sina_topic");
        }
        public bool UpdateSinaTopic(SinaTopic super, string[] columns)
        {
            return DBHelper.UpdateEntity(super, "sina_topic", "containerid", super.containerid, columns);
        }

        public bool DeleteSinaUser(SinaUser user)
        {
            return DBHelper.DeleteEntity("sina_user", "uid", user.uid);
        }

        public bool UpdateSinaUser(SinaUser user, string[] columns)
        {
            return DBHelper.UpdateEntity(user, "sina_user", "uid", user.uid, columns);
        }

        public bool CreateSinaStatus(SinaStatus status)
        {
            return DBHelper.CreateEntity(status, "sina_status");
        }

        public bool UpdateSinaStatus(SinaStatus status, string[] columns)
        {
            return DBHelper.UpdateEntity(status, "sina_status", "bid", status.bid, columns);
        }

        public bool UpdateSinaStatuses(string uid, string col, object value)
        {
            return DBHelper.UpdateEntitys("sina_status", $"`uid`='{uid}'", col, value);
        }

        public bool UpdateSinaSource(SinaSource source, string[] columns)
        {
            return DBHelper.UpdateEntity(source, "sina_source", "id", $"{source.id}", columns);
        }

        public bool DeleteSinaStatus(SinaStatus status)
        {
            return DBHelper.DeleteEntity("sina_status", "bid", status.bid);
        }

        public bool DeleteSinaStatus(string bid)
        {
            return DBHelper.DeleteEntity("sina_status", "bid", bid);
        }

        public bool DeleteSinaStatus(SinaUser user)
        {
            return DBHelper.DeleteEntity("sina_status", "uid", user.uid);
        }
        public bool ExistsSinaUser(string uid)
        {
            return DBHelper.ExistsEntity("sina_user", "uid", uid);
        }

        public bool ExistsSinaStatus(string bid)
        {
            return DBHelper.ExistsEntity("sina_status", "bid", bid);
        }

        public bool ExistsSinaSource(string uid, string bid, string imgUrl)
        {
            return DBHelper.ExistsEntity("sina_source", $"`uid`='{uid}' and `bid`='{bid}' and `url`='{imgUrl}'");
        }

        public int GetUserStatusGetCount(string uid)
        {
            return DBHelper.GetEntityCount("sina_status", $"`uid`={uid} and `retweeted`=0 and `ignore`=0 and `gets`>0");
        }
        public int GetUserStatusFindCount(string uid)
        {
            return DBHelper.GetEntityCount("sina_status", $"`uid`={uid}");
        }
        public int GetUserStatusIgnoreCount(string uid)
        {
            return DBHelper.GetEntityCount("sina_status", $"`uid`={uid} and `ignore`>0");
        }
        public int GetUserStatusRetweetCount(string uid)
        {
            return DBHelper.GetEntityCount("sina_status", $"`uid`={uid} and `retweeted`>0");
        }
        public int GetUserStatusUploadCount(string uid)
        {
            return DBHelper.GetEntityCount("sina_status", $"`uid`={uid} and `upload`>0");
        }
        public SinaUser GetUser(string uid)
        {
            return DBHelper.GetEntity<SinaUser>("sina_user", $"`uid`='{uid}'");
        }
        public SinaTopic GetSinaTopic(string containerid)
        {
            return DBHelper.GetEntity<SinaTopic>("sina_topic", $"`containerid`='{containerid}'");
        }

        public List<SinaTopic> GetSinaTopics(string category)
        {
            return DBHelper.GetEntitys<SinaTopic>("sina_topic", $"`type`=0 and `category`='{category}'");
        }

        public List<SinaTopic> GetSinaSupers(string category)
        {
            return DBHelper.GetEntitys<SinaTopic>("sina_topic", $"`type`=1 and `category`='{category}'");
        }

        public List<SinaAction> GetSinaWaitActions()
        {
            return DBHelper.GetEntitys<SinaAction>("sina_action", $"`action`=0");
        }

        public string[] GetGroupNames()
        {
            return DBHelper.GetGroupStrings("sina_user", "category", $"1=1").ToArray();
        }

        public List<SinaUser> GetUsers(string category)
        {
            return DBHelper.GetEntitys<SinaUser>("sina_user", $"`category`='{category}' and `ignore`=0");
        }

        public List<SinaUser> GetUsers(string category, string keyword)
        {
            return DBHelper.GetEntitys<SinaUser>("sina_user", $"`category`='{category}' and `ignore`=0 and (`uid` like '%{keyword}%' or `name` like '%{keyword}%')");
        }
        public List<SinaUser> GetFocusUsers(string category)
        {
            return DBHelper.GetEntitys<SinaUser>("sina_user", $"`category`='{category}' and `ignore`=0 and `focus`=1");
        }

        public List<SinaUser> GetUsers(string[] uids)
        {
            return DBHelper.GetEntitys<SinaUser>("sina_user", $" `uid` in ({string.Join(",", uids.Select(c => $"'{c}'"))})");
        }

        public SinaStatus GetUserStatus(string bid)
        {
            return DBHelper.GetEntity<SinaStatus>("sina_status", $"`bid`='{bid}'");
        }

        public SinaSource GetUserSource(string bid, string name)
        {
            return DBHelper.GetEntity<SinaSource>("sina_source", $"`bid`='{bid}' and `name`='{name}'");
        }

        public bool DeleteSinaSource(SinaSource sinaSource)
        {
            return DBHelper.DeleteEntity("sina_source", "id", $"{sinaSource.id}");
        }

        public bool DeleteSinaSources(string bid)
        {
            return DBHelper.DeleteEntity("sina_source", "bid", bid);
        }

        public List<SinaSource> GetUserSources(string uid, string bid)
        {
            return DBHelper.GetEntitys<SinaSource>("sina_source", $"`uid`='{uid}' and `bid`='{bid}'");
        }

        public List<SinaStatus> GetUserStatuses(string uid)
        {
            return DBHelper.GetEntitys<SinaStatus>("sina_status", $"`uid`='{uid}' and `retweeted`=0 and  `ignore`=0");
        }

        public List<SinaStatus> GetUserStatuseByIds(string uid, string keyword)
        {
            return DBHelper.GetEntitys<SinaStatus>("sina_status", $"`uid`='{uid}' and `bid` like '%{keyword}%' and `retweeted`=0 and  `ignore`=0");
        }

        public List<SinaStatus> GetUserStatuseOfNoUpload(string uid)
        {
            return DBHelper.GetEntitys<SinaStatus>("sina_status", $"`uid`='{uid}' and `retweeted`=0 and `ignore`=0 and `action`=0");
        }

        public List<SinaSource> GetUserPictures(string uid)
        {
            return DBHelper.GetEntitys<SinaSource>("sina_source", $"`uid`='{uid}'");
        }

        public bool ChangeUserCategory(string uid, string category)
        {
            var user = GetUser(uid);
            if (user == null) return false;

            if (user.category == category) return false;
            user.category = category;
            return UpdateSinaUser(user, new string[] { "category" });
        }

        public bool CheckUserFocus(string uid)
        {
            var user = GetUser(uid);
            if (user != null && user.focus > 0) return true;
            return false;
        }

        public bool CheckUserIgnore(string uid)
        {
            var user = GetUser(uid);
            if (user != null && user.ignore > 0) return true;
            return false;
        }

        public bool CheckStatusIgnore(string bid)
        {
            var status = GetUserStatus(bid);
            if (status != null && status.ignore > 0) return true;
            return false;
        }
        public bool UpdateTopicLastpage(string containerid)
        {
            var topic = GetSinaTopic(containerid);
            if (topic == null) return false;

            topic.lastpage = 1;
            return UpdateSinaTopic(topic, new string[] { "lastpage" });
        }

        public bool UpdateUserLastpage(string uid)
        {
            var user = GetUser(uid);
            if (user == null) return false;

            user.lastpage = 1;
            return UpdateSinaUser(user, new string[] { "lastpage" });
        }

        public SinaUser StoreSinaUser(SpiderRunningConfig runningConfig, SpiderRunningCache runningCache, MWeiboUser user)
        {
            var sinaUser = new SinaUser();
            sinaUser.category = runningCache.Category;
            sinaUser.site = runningCache.Site;
            sinaUser.uid = user.id;
            sinaUser.name = user.screen_name;
            sinaUser.avatar = user.avatar_hd;
            sinaUser.desc = user.description;
            sinaUser.follows = user.follow_count;
            sinaUser.followers = user.followers_count;
            sinaUser.profile = SinaUrlUtil.GetSinaUserUrl(user.id);
            sinaUser.statuses = user.statuses_count.HasValue?user.statuses_count.Value : 0;
            if (sinaUser.statuses < runningConfig.GatherUserMinStatuses) sinaUser.ignore = 1;

            var existsUser = GetUser(user.id);
            if (existsUser == null)
            {
                var suc = CreateSinaUser(sinaUser);
                if (!suc)
                {
                    return null;
                }
            }
            else
            {
                sinaUser.id = existsUser.id;
                var suc = UpdateSinaUser(sinaUser, new string[] { "follows", "followers", "statuses", "name", "avatar", "desc", "profile", "ignore", "site" });
                if (!suc)
                {
                    return null;
                }
            }
            return sinaUser;
        }

        public void StoreSinaRetweetStatus(SpiderRunningConfig runningConfig, MWeiboUser user, MWeiboStatus status, MWeiboStatus retweet, int mtype)
        {
            var sinaStatus = new SinaStatus();
            sinaStatus.uid = user.id;
            sinaStatus.bid = status.bid;
            sinaStatus.mid = status.id;
            sinaStatus.mtype = mtype;
            sinaStatus.text = status.status_title;
            sinaStatus.url = SinaUrlUtil.GetSinaUserStatusUrl(status.bid);
            sinaStatus.retweeted = 1;
            sinaStatus.retuid = retweet.user != null ? retweet.user.id : "Unauthorization";
            sinaStatus.retbid = retweet.bid;
            sinaStatus.site = runningConfig.Site;
            sinaStatus.createtime = ObjectUtil.GetCreateTime(status.created_at);
            var extSinaStatus = GetUserStatus(status.bid);
            if (extSinaStatus == null)
            {
                CreateSinaStatus(sinaStatus);
            }
            else
            {
                sinaStatus.retweeted = 1;
                sinaStatus.retuid = retweet.user != null ? retweet.user.id : "Unauthorization";
                sinaStatus.retbid = retweet.bid;
                extSinaStatus.createtime = ObjectUtil.GetCreateTime(status.created_at);
                UpdateSinaStatus(extSinaStatus, new string[] { "retweeted", "retuid", "retbid", "createtime" });
            }
        }

        public void StoreSinaStatus(SpiderRunningConfig runningConfig, MWeiboUser user, MWeiboStatus status, int mtype, int readSourceCount, int getSourceCount, bool ignore)
        {
            var sinaStatus = new SinaStatus();
            sinaStatus.uid = user.id;
            sinaStatus.bid = status.bid;
            sinaStatus.mid = status.id;
            sinaStatus.mtype = mtype;
            sinaStatus.text = status.status_title;
            sinaStatus.url = SinaUrlUtil.GetSinaUserStatusUrl(status.bid);
            sinaStatus.qty = readSourceCount;
            sinaStatus.gets = getSourceCount;
            sinaStatus.site = runningConfig.Site;
            sinaStatus.ignore = ignore ? 1 : 0;
            sinaStatus.createtime = ObjectUtil.GetCreateTime(status.created_at);
            var extSinaStatus = GetUserStatus(status.bid);
            if (extSinaStatus == null)
            {
                CreateSinaStatus(sinaStatus);
            }
            else
            {
                if (!runningConfig.IgnoreDownloadSource) extSinaStatus.gets = getSourceCount;
                else if (runningConfig.IgnoreDownloadSource && getSourceCount > 0) extSinaStatus.gets = getSourceCount;
                extSinaStatus.ignore = ignore ? 1 : 0;
                extSinaStatus.qty = readSourceCount;
                //extSinaStatus.site = runningConfig.Site;
                extSinaStatus.createtime = ObjectUtil.GetCreateTime(status.created_at);
                UpdateSinaStatus(extSinaStatus, new string[] { "ignore", "qty", "gets", "createtime" });
            }
        }

        public bool DeleteSinaUser(string user)
        {
            var sinaUser = GetUser(user);
            if (sinaUser == null) return true;

            sinaUser.ignore = 1;
            var suc = DeleteSinaUser(sinaUser);
            if (suc)
            {
                DeleteSinaStatus(sinaUser);
            }
            return suc;
        }


        public bool FocusSinaUser(string user)
        {
            var sinaUser = GetUser(user);
            if (sinaUser == null) return false;

            sinaUser.focus = sinaUser.focus == 0 ? 1 : 0;
            UpdateSinaUser(sinaUser, new string[] { "focus" });
            return sinaUser.focus > 0 ? true : false;
        }

        public bool UpdateSinaUserQty(string uid)
        {
            var sinaUser = GetUser(uid);
            if (sinaUser == null) return false;

            UpdateSinaUserInfo(sinaUser);
            return true;
        }

        public bool UpdateSinaUserPage(string uid, int readPageIndex)
        {
            var sinaUser = GetUser(uid);
            if (sinaUser == null) return false;

            if (readPageIndex > 0) sinaUser.readpage = readPageIndex;
            return UpdateSinaUser(sinaUser, new string[] { "readpage" });
        }

        public bool UpdateSinaTopicPage(string containerid, int readPageIndex)
        {
            var topic = GetSinaTopic(containerid);
            if (topic == null) return false;

            if (readPageIndex > 0 && readPageIndex > topic.readpage) topic.readpage = readPageIndex;
            return UpdateSinaTopic(topic, new string[] { "readpage" });
        }

        public SinaUser UpdateSinaUserInfo(SinaUser sinaUser)
        {
            sinaUser.finds = GetUserStatusFindCount(sinaUser.uid);
            sinaUser.gets = GetUserStatusGetCount(sinaUser.uid);
            sinaUser.ignores = GetUserStatusIgnoreCount(sinaUser.uid);
            sinaUser.retweets = GetUserStatusRetweetCount(sinaUser.uid);
            sinaUser.uploads = GetUserStatusUploadCount(sinaUser.uid);
            sinaUser.originals = sinaUser.finds - sinaUser.retweets;
            UpdateSinaUser(sinaUser, new string[] { "finds", "gets", "ignores", "retweets", "originals", "uploads" });
            return sinaUser;
        }

        public void MakeUploadAction(string category, string status, FileInfo[] files, bool cancel)
        {
            var sinaStatus = GetUserStatus(status);
            if (sinaStatus == null) return;

            var updQty = UpdateSinaSourceStatus(sinaStatus, files, cancel);
            sinaStatus.upload = updQty;
            UpdateSinaStatus(sinaStatus, new string[] { "upload" });
            UpdateSinaUserQty(sinaStatus.uid);
            CreateSinaAction(sinaStatus, files, category, cancel ? 1: 0);
        }

        public void MakeIgnoreUserAction(string category, string user)
        {
            var sinaUser = GetUser(user);
            if (sinaUser == null) return;

            sinaUser.ignore = 2;
            UpdateSinaUser(sinaUser, new string[] { "ignore" });
            CreateSinaAction(sinaUser, category, 2);
        }


        public void MakeIgnoreStatusAction(string category, string status)
        {
            var sinaStatus = GetUserStatus(status);
            if (sinaStatus == null) return;
            sinaStatus.ignore = 2;
            UpdateSinaStatus(sinaStatus, new string[] { "ignore" });
            CreateSinaAction(sinaStatus, category, 2);
        }

        public void MakeIgnoreStatusSourceAction(string category, string status, string filename)
        {
            var sinaStatus = GetUserStatus(status);
            if (sinaStatus == null) return;

            var sinaSource = GetUserSource(status, filename);
            if (sinaSource != null)
            {
                DeleteSinaSource(sinaSource);
                CreateSinaAction(sinaStatus, filename, category, 2);
            }
        }

        public void ExecuteIgnoreStatus(string category, string uid, string status, string filename)
        {
            var sinaStatus = GetUserStatus(status);
            if (sinaStatus == null) return;

            if (string.IsNullOrEmpty(filename))
            {
                DeleteSinaSources(sinaStatus.bid);
                UpdateSinaUserQty(sinaStatus.uid);
            }
            else
            {
                var sinaSources = GetUserSources(uid, status);
                sinaStatus.upload = sinaSources.Where(c => c.upload > 0).Count();
                if (sinaSources.Count == 0)
                {
                    sinaStatus.ignore = 2;
                }
                UpdateSinaStatus(sinaStatus, new string[] { "ignore", "upload" });
                UpdateSinaUserQty(sinaStatus.uid);
            }
        }

        int UpdateSinaSourceStatus(SinaStatus sinaStatus, FileInfo[] files, bool cancel)
        {
            var sources = GetUserSources(sinaStatus.uid, sinaStatus.bid);
            foreach(var source in sources)
            {
                if(files.Any(c=>c.Name == source.name))
                {
                    source.upload = (!cancel ? 1 : 0);
                    UpdateSinaSource(source, new string[] { "upload" });
                }
            }            
            return sources.Where(c => c.upload > 0).Count();
        }

        public void CreateSinaAction(SinaStatus status, FileInfo[] files, string category, int actType)
        {
            foreach(var file in files)
            {
                var upload = GetSinaAction(status.uid, status.bid, file.Name, actType);
                if (upload == null)
                {
                    MakeSinaAction(category, status.uid, status.bid, file.Name, actType);
                }                
            }
        }

        public void CreateSinaAction(SinaStatus status, string filename, string category, int actType)
        {
            var upload = GetSinaAction(status.uid, status.bid, filename, actType);
            if (upload == null)
            {
                MakeSinaAction(category, status.uid, status.bid, filename, actType);
            }
        }

        void CreateSinaAction(SinaUser user, string category, int actType)
        {
            var upload = GetSinaAction(user.uid, string.Empty, string.Empty, actType);
            if (upload == null)
            {
                MakeSinaAction(category, user.uid, string.Empty, string.Empty, actType);
            }
        }

        void CreateSinaAction(SinaStatus status, string category, int actType)
        {
            var upload = GetSinaAction(status.uid, status.bid, string.Empty, actType);
            if (upload == null)
            {
                MakeSinaAction(category, status.uid, status.bid, string.Empty, actType);
            }
        }

        void MakeSinaAction(string category, string uid, string bid, string file, int actType)
        {
            var upload = new SinaAction()
            {
                actid = DateTime.Now.ToString("MMddHHmmsss"),
                acttype = actType,
                category = category,
                uid = uid,
                bid = bid,
                file = file,
                createtime = DateTime.Now.ToString("yyyy/MM/dd HH:mm"),
            };
            DBHelper.CreateEntity(upload, "sina_action");
        }

        public SinaAction GetSinaAction(string uid, string bid, string file, int actType)
        {
            return DBHelper.GetEntity<SinaAction>("sina_action", $"`uid`='{uid}' and `bid`='{bid}' and `file`='{file}' and `acttype`={actType} and `action`=0");
        }

        public bool UpdateSinaAction(SinaAction upload, string[] columns)
        {
            return DBHelper.UpdateEntity(upload, "sina_action", "id", $"{upload.id}", columns);
        }
    }
}
