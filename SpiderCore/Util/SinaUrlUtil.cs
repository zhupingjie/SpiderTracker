using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiderCore.Util
{
    public class SinaUrlUtil
    {
        public static string GetSinaHost()
        {
            return "https://m.weibo.cn";
        }

        public static string GetBilibiliHost()
        {
            return "https://www.bilibili.com";
        }

        public static string GetSinaUserUrl(string userId)
        {
            return $"{GetSinaHost()}/u/{userId}";
        }

        public static string GetSinaUserStatusUrl(string status)
        {
            return $"{SinaUrlUtil.GetSinaHost()}/status/{status}";
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
            return $"{SinaUrlUtil.GetSinaHost()}/api/container/getIndex?containerid=231522type=61%26t=20%26q=%23{topic}%23&page={page}";
        }

        public static string GetSinaUserSuperUrl(string containerid, int page = 0)
        {
            return $"{SinaUrlUtil.GetSinaHost()}/api/container/getIndex?containerid={containerid}_-_sort_time&page={page}";
        }

        public static string GetSinaUserTopicWebUrl(string topic)
        {
            return $"{SinaUrlUtil.GetSinaHost()}/search?containerid=231522type=61%26t=20%26q=%23{topic}%23";
        }

        public static string GetSinaUserSuperWebUrl(string containerid)
        {
            return $"{SinaUrlUtil.GetSinaHost()}/p/index?containerid={containerid}";
        }

        public static string GetSinaUserFollowerUrl(string uid)
        {
            return $"{SinaUrlUtil.GetSinaHost()}/p/index?containerid=231051_-_followers_-_{uid}_-_1042015%253AtagCategory_039&luicode=10000011&lfid=107603{uid}";
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
            if (url.StartsWith($"{SinaUrlUtil.GetSinaHost()}/u"))
            {
                return SinaUrlEnum.UserUrl;
            }
            else if (url.StartsWith($"{SinaUrlUtil.GetSinaHost()}/status"))
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
    }
}
