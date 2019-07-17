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

        public bool CreateSinaPicture(SinaPicture picture)
        {
            return DBHelper.CreateEntity(picture, "sina_picture");
        }

        public bool CreateSinaUser(SinaUser user)
        {
            return DBHelper.CreateEntity(user, "sina_user");
        }

        public bool DeleteSinaUser(SinaUser user)
        {
            return DBHelper.DeleteEntity("sina_user", "uid", user.uid);
        }

        public bool DeleteSinaUserPicture(SinaUser user)
        {
            return DBHelper.DeleteEntity("sina_picture", "uid", user.uid);
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

        public bool ExistsSinaPicture(string uid, string bid, string imgurl)
        {
            return DBHelper.ExistsEntity("sina_picture", $"uid='{uid}' and bid='{bid}' and picurl='{imgurl}'");
        }

        public SinaUser GetUser(string uid)
        {
            return DBHelper.GetEntity<SinaUser>("sina_user", $"uid='{uid}'");
        }

        public string[] GetGroupNames()
        {
            return DBHelper.GetGroupStrings("sina_user", "groupname", $"1=1").ToArray();
        }

        public List<SinaUser> GetUsers(string groupname)
        {
            return DBHelper.GetEntitys<SinaUser>("sina_user", $"groupname='{groupname}'");
        }

        public List<SinaUser> GetUsers(string groupname, string keyword)
        {
            return DBHelper.GetEntitys<SinaUser>("sina_user", $"groupname='{groupname}' and ignore=0 and uid like '%{keyword}%'");
        }

        public List<SinaUser> GetUsers(string[] uids)
        {
            return DBHelper.GetEntitys<SinaUser>("sina_user", $" uid in ({string.Join(",", uids.Select(c => $"'{c}'"))})");
        }

        public SinaStatus GetUserStatus(string bid)
        {
            return DBHelper.GetEntity<SinaStatus>("sina_status", $"bid='{bid}'");
        }

        public List<SinaStatus> GetUserStatuses(string uid)
        {
            return DBHelper.GetEntitys<SinaStatus>("sina_status", $"uid='{uid}' and retweeted=0 and ignore=0");
        }

        public List<SinaPicture> GetUserPictures(string uid)
        {
            return DBHelper.GetEntitys<SinaPicture>("sinna_picture", $"uid='{uid}'");
        }

        public List<SinaUser> GetFocusUsers(string groupname)
        {
            return DBHelper.GetEntitys<SinaUser>("sina_user", $"groupname='{groupname}' and focus=1");
        }
        public List<SinaUser> GetAnalyseFocusUsers(string groupname)
        {
            return DBHelper.GetEntitys<SinaUser>("sina_user", $"groupname='{groupname}' and mayfocus=1");
        }
        public List<SinaUser> GetAnalyseIgnoreUsers(string groupname)
        {
            return DBHelper.GetEntitys<SinaUser>("sina_user", $"groupname='{groupname}' and mayignore=1");
        }

        public bool CheckUserIgnore(string uid)
        {
            var user = GetUser(uid);
            if (user != null && user.ignore == 1) return true;
            return false;
        }

        public bool CheckStatusIgnore(string bid)
        {
            var status = GetUserStatus(bid);
            if (status != null && status.ignore == 1) return true;
            return false;
        }

        public string StoreSinaUser(SpiderRunningConfig runningConfig, MWeiboUser user, bool bUpdate = false)
        {
            var sinaUser = new SinaUser();
            sinaUser.groupname = runningConfig.Name;
            sinaUser.uid = user.id;
            sinaUser.name = user.screen_name;
            sinaUser.avatar = user.avatar_hd;
            sinaUser.desc = user.description;
            sinaUser.follows = user.follow_count;
            sinaUser.followers = user.followers_count;
            sinaUser.profile = SinaUrlUtil.GetSinaUserUrl(user.id);
            sinaUser.statuses = user.statuses_count.HasValue?user.statuses_count.Value : 0;

            if (!ExistsSinaUser(user.id))
            {
                var suc = CreateSinaUser(sinaUser);
                if (!suc)
                {
                    return $"创建本地用户错误!";
                }
            }
            else if (bUpdate)
            {
                var suc = UpdateSinaUser(sinaUser, new string[] { "follows", "followers", "statuses" });
                if (!suc)
                {
                    return $"更新本地用户错误!";
                }
            }
            return null;
        }
        public string StoreSinaStatus(MWeiboUser user, MWeiboStatus status, MWeiboStatus retweeted, int readStatusImageCount)
        {
            var sinaStatus = new SinaStatus();
            sinaStatus.uid = user.id;
            sinaStatus.bid = status.bid;
            sinaStatus.mid = status.id;
            sinaStatus.url = SinaUrlUtil.GetSinaUserStatusUrl(status.bid);
            if (readStatusImageCount != -1)
            {
                //sinaStatus.ignore = (readStatusImageCount == 0 ? 1 : 0);
            }
            if (retweeted == null)
            {
                sinaStatus.pics = status.pics != null ? status.pics.Length : 0;
            }
            else
            {
                sinaStatus.retweeted = 1;
                sinaStatus.retuid = retweeted.user.id;
                sinaStatus.retbid = retweeted.bid;
            }
            if (!ExistsSinaStatus(status.bid))
            {
                var suc = CreateSinaStatus(sinaStatus);
                if (!suc)
                {
                    return $"创建本地微博错误!";
                }
            }
            if (retweeted != null)
            {
                if (!ExistsSinaStatus(retweeted.bid))
                {
                    sinaStatus = new SinaStatus();
                    sinaStatus.uid = retweeted.user.id;
                    sinaStatus.bid = retweeted.bid;
                    sinaStatus.mid = retweeted.id;
                    sinaStatus.url = SinaUrlUtil.GetSinaUserStatusUrl(retweeted.bid);
                    sinaStatus.pics = retweeted.pics != null ? retweeted.pics.Length : 0;
                    if (readStatusImageCount != -1)
                    {
                        //sinaStatus.ignore = (readStatusImageCount == 0 ? 1 : 0);
                    }
                    var suc = CreateSinaStatus(sinaStatus);
                    if (!suc)
                    {
                        return $"创建本地转发微博错误!";
                    }
                }
            }
            return null;
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

            sinaUser.ignore = 1;
            var suc = UpdateSinaUser(sinaUser, new string[] { "ignore" });
            if (suc)
            {
                var sinaStatuses = GetUserStatuses(user);
                foreach (var sinaStatus in sinaStatuses)
                {
                    sinaStatus.ignore = 1;
                    UpdateSinaStatus(sinaStatus, new string[] { "ignore" });
                }
            }
            return suc;
        }

        public bool FocusSinaUser(string user)
        {
            var sinaUser = GetUser(user);
            if (sinaUser == null) return true;

            sinaUser.focus = 1;
            return UpdateSinaUser(sinaUser, new string[] { "focus" });
        }

        public bool IgnoreSinaStatus(string status)
        {
            var sinaStatus = GetUserStatus(status);
            if (sinaStatus == null) return true;

            sinaStatus.ignore = 1;
            return UpdateSinaStatus(sinaStatus, new string[] { "ignore" });
        }
        public bool FocusSinaStatus(string status)
        {
            var sinaStatus = GetUserStatus(status);
            if (sinaStatus == null) return true;

            sinaStatus.focus = 1;
            return UpdateSinaStatus(sinaStatus, new string[] { "focus" });
        }
    }
}
