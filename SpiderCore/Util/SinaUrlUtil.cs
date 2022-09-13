using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiderCore.Util
{
    public class SinaUrlUtil
    {
        public static string GetSinaMWebHost()
        {
            return "https://m.weibo.cn";
        }
        public static string GetSinaWebHost()
        {
            return "https://weibo.com";
        }
        public static string GetBilibiliHost()
        {
            return "https://www.bilibili.com";
        }

        public static string GetBilibiliUserHost()
        {
            return "https://space.bilibili.com";
        }

        public static string GetSinaUserUrl(string userId)
        {
            return $"{GetSinaWebHost()}/u/{userId}";
        }

        public static string GetSinaUserStatusUrl(string status)
        {
            return $"{SinaUrlUtil.GetSinaMWebHost()}/status/{status}";
        }
        public static string GetSinaUserStatusUrl(string user, string status)
        {
            return $"{SinaUrlUtil.GetSinaWebHost()}/{user}/{status}";
        }

        public static string GetBilibiliUserUrl(string userId)
        {
            return $"{GetBilibiliUserHost()}/{userId}";
        }

        public static string GetBilibiliStatusByUrl(string url)
        {
            if (url.Contains("?")) url = url.Split('?')[0];
            if (url.Contains("/video/"))
            {
                var status = url.Replace(GetBilibiliHost(), "").Replace("/video/", "").Replace("/","");
                return status;
            }
            else
            {
                return null;
            }
        }

        public static string GetBilibiliUserStatusUrl(string status)
        {
            return $"{SinaUrlUtil.GetBilibiliHost()}/video/{status}";
        }

        public static string GetSinaUserStatusWebUrl(string user, string status)
        {
            return $"https://weibo.com/{user}/{status}";
        }

        public static string GetSinaUserTopicUrl(string topic, int page = 0)
        {
            return $"{SinaUrlUtil.GetSinaMWebHost()}/api/container/getIndex?containerid=231522type=61%26t=20%26q=%23{topic}%23&page={page}";
        }

        public static string GetSinaUserSuperUrl(string containerid, int page = 0)
        {
            return $"{SinaUrlUtil.GetSinaMWebHost()}/api/container/getIndex?containerid={containerid}_-_sort_time&page={page}";
        }

        public static string GetSinaUserTopicWebUrl(string topic)
        {
            return $"{SinaUrlUtil.GetSinaMWebHost()}/search?containerid=231522type=61%26t=20%26q=%23{topic}%23";
        }

        public static string GetSinaUserSuperWebUrl(string containerid)
        {
            return $"{SinaUrlUtil.GetSinaMWebHost()}/p/index?containerid={containerid}";
        }

        public static string GetSinaUserFollowerUrl(string uid)
        {
            return $"{SinaUrlUtil.GetSinaMWebHost()}/p/index?containerid=231051_-_followers_-_{uid}_-_1042015%253AtagCategory_039&luicode=10000011&lfid=107603{uid}";
        }


        /// <summary>
        /// 获取起始路径类型
        /// </summary>
        /// <param name="startUrl"></param>
        /// <returns></returns>
        public static SinaUrlEnum GetSinaUrlEnum(string url)
        {
            //0 = https://m.weibo.cn/u/5436956791
            //1 = https://m.weibo.cn/status/GdgGz4zda
            if (url.StartsWith($"{SinaUrlUtil.GetSinaMWebHost()}/u"))
            {
                return SinaUrlEnum.UserUrl;
            }
            else if (url.StartsWith($"{SinaUrlUtil.GetSinaMWebHost()}/status"))
            {
                return SinaUrlEnum.StatusUrl;
            }
            else
            {
                return SinaUrlEnum.NotEffectUrl;
            }
        }


        public static string GetSinaUserByStartUrl(string startUrl)
        {
            if (!startUrl.Contains("/u/")) return null;
            var arr = startUrl.Split(new string[] { "/u/" }, StringSplitOptions.None);
            return arr[arr.Length - 1];
        }

        public static (string, string) GetSinaUserStatusByStartUrl(string startUrl)
        {
            var url = startUrl.Split("?", StringSplitOptions.RemoveEmptyEntries)[0];
            if (url.Contains("weibo.com/")) url = url.Split("weibo.com/", StringSplitOptions.RemoveEmptyEntries)[1];
            var strs = url.Split("/", StringSplitOptions.RemoveEmptyEntries);
            if (strs.Length == 2) return (strs[0], strs[1]);
            return (string.Empty, string.Empty);
        }
    }
}
