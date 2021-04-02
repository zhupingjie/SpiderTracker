using SpiderTracker.Imp.MWeiboJson;
using System;
using System.Collections.Generic;
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
            return DBHelper.GetEntityCount("sina_status", $"`uid`={uid} and `retweet`=0 and `ignore`=0");
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
            return DBHelper.GetEntityCount("sina_status", $"`uid`={uid} and `retweet`>0");
        }
        public SinaUser GetUser(string uid)
        {
            return DBHelper.GetEntity<SinaUser>("sina_user", $"`uid`='{uid}'");
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

        public List<SinaStatus> GetUserStatuses(string uid)
        {
            return DBHelper.GetEntitys<SinaStatus>("sina_status", $"`uid`='{uid}' and `retweet`=0 and  `ignore`=0");
        }

        public List<SinaStatus> GetUserStatuseByIds(string uid, string keyword)
        {
            return DBHelper.GetEntitys<SinaStatus>("sina_status", $"`uid`='{uid}' and `bid` like '%{keyword}%' and `retweet`=0 and  `ignore`=0");
        }

        public List<SinaStatus> GetUserStatuseOfNoArchives(string uid)
        {
            return DBHelper.GetEntitys<SinaStatus>("sina_status", $"`uid`='{uid}' and `retweet`=0 and `ignore`=0 and `archive`=0");
        }

        public List<SinaSource> GetUserPictures(string uid)
        {
            return DBHelper.GetEntitys<SinaSource>("sina_source", $"`uid`='{uid}'");
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

        public SinaUser StoreSinaUser(SpiderRunningConfig runningConfig, MWeiboUser user)
        {
            var sinaUser = new SinaUser();
            sinaUser.category = runningConfig.Category;
            sinaUser.uid = user.id;
            sinaUser.name = user.screen_name;
            sinaUser.avatar = user.avatar_hd;
            sinaUser.desc = user.description;
            sinaUser.follows = user.follow_count;
            sinaUser.followers = user.followers_count;
            sinaUser.profile = SinaUrlUtil.GetSinaUserUrl(user.id);
            sinaUser.statuses = user.statuses_count.HasValue?user.statuses_count.Value : 0;

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
                var suc = UpdateSinaUser(sinaUser, new string[] { "follows", "followers", "statuses", "name", "avatar", "desc", "profile" });
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
            sinaStatus.retweet = 1;
            sinaStatus.retuid = retweet.user != null ? retweet.user.id : "Unauthorization";
            sinaStatus.retbid = retweet.bid;
            sinaStatus.site = runningConfig.Site;
            var extSinaStatus = GetUserStatus(status.bid);
            if (extSinaStatus == null)
            {
                CreateSinaStatus(sinaStatus);
            }
        }

        public void StoreSinaStatus(SpiderRunningConfig runningConfig, MWeiboUser user, MWeiboStatus status, int mtype, int readSourceCount, int readStatusImageCount, bool ignore)
        {
            var sinaStatus = new SinaStatus();
            sinaStatus.uid = user.id;
            sinaStatus.bid = status.bid;
            sinaStatus.mid = status.id;
            sinaStatus.mtype = mtype;
            sinaStatus.text = status.status_title;
            sinaStatus.url = SinaUrlUtil.GetSinaUserStatusUrl(status.bid);
            sinaStatus.qty = readSourceCount;
            sinaStatus.getqty = readStatusImageCount;
            sinaStatus.site = runningConfig.Site;
            sinaStatus.ignore = ignore ? 1 : 0;
            var extSinaStatus = GetUserStatus(status.bid);
            if (extSinaStatus == null)
            {
                CreateSinaStatus(sinaStatus);
            }
            else
            {
                extSinaStatus.ignore = ignore ? 1 : 0;
                extSinaStatus.qty = readSourceCount;
                extSinaStatus.getqty = readStatusImageCount;
                extSinaStatus.site = runningConfig.Site;
                UpdateSinaStatus(extSinaStatus, new string[] { "ignore", "qty", "getqty", "site" });
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


        public bool IgnoreSinaUser(string user)
        {
            var sinaUser = GetUser(user);
            if (sinaUser == null) return true;

            sinaUser.ignore = 2;
            var suc = UpdateSinaUser(sinaUser, new string[] { "ignore" });
            if (suc)
            {
                var sinaStatuses = GetUserStatuses(user);
                foreach (var sinaStatus in sinaStatuses)
                {
                    sinaStatus.ignore = 2;
                    UpdateSinaStatus(sinaStatus, new string[] { "ignore" });
                }
            }
            return suc;
        }

        public bool FocusSinaUser(string user)
        {
            var sinaUser = GetUser(user);
            if (sinaUser == null) return true;

            sinaUser.focus = sinaUser.focus == 0 ? 1 : 0;
            UpdateSinaUser(sinaUser, new string[] { "focus" });
            return sinaUser.focus > 0 ? true : false;
        }

        public bool UpdateSinaUserQty(string uid)
        {
            var sinaUser = GetUser(uid);
            UpdateSinaUserQty(sinaUser);
            return true;
        }

        public SinaUser UpdateSinaUserQty(SinaUser sinaUser)
        {
            sinaUser.finds = GetUserStatusFindCount(sinaUser.uid);
            sinaUser.gets = GetUserStatusGetCount(sinaUser.uid);
            sinaUser.ignores = GetUserStatusIgnoreCount(sinaUser.uid);
            sinaUser.retweets = GetUserStatusRetweetCount(sinaUser.uid);
            sinaUser.originals = sinaUser.finds - sinaUser.retweets;
            UpdateSinaUser(sinaUser, new string[] { "finds", "gets", "ignores", "retweets", "originals" });
            return sinaUser;
        }

        public bool IgnoreSinaStatus(string status)
        {
            var sinaStatus = GetUserStatus(status);
            if (sinaStatus == null) return true;

            sinaStatus.ignore = 1;
            UpdateSinaStatus(sinaStatus, new string[] { "ignore" });
            return UpdateSinaUserQty(sinaStatus.uid);
        }
        
        public bool ArchiveSinaStatus(string status)
        {
            var sinaStatus = GetUserStatus(status);
            if (sinaStatus == null) return true;

            sinaStatus.archive = 1;
            UpdateSinaStatus(sinaStatus, new string[] { "archive" });
            return UpdateSinaUserQty(sinaStatus.uid);
        }

        public bool ArchiveSinaUser(string user)
        {
            var sinaStatuss= GetUserStatuses(user);
            foreach (var sinaStatus in sinaStatuss)
            {
                sinaStatus.archive = 1;
                UpdateSinaStatus(sinaStatus, new string[] { "archive" });
            }
            return UpdateSinaUserQty(user);
        }
    }
}
