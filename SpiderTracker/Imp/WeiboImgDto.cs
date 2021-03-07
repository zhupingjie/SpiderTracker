using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiderTracker.Imp
{
    public class WeiboImgDto
    {
        public WeiboImgDto(string startUrl)
        {
            Host = "https://weibo.cn";
            StartUrl = startUrl;
        }
        public string Host { get; set; }
        public string StartUrl { get; set; }

        public string ListUrl { get; set; }
        public string SourceImgUrl { get; set; }

        public string RealImgUrl { get; set; }

        //https://weibo.cn/mblog/picAll/GfewBjJK8?rl=1
        //https://weibo.cn/mblog/pic/zjSAD066v?rl=0
        public string ArcId
        {
            get
            {
                if (!string.IsNullOrEmpty(ListUrl))
                {
                    return ListUrl.Replace("https://weibo.cn/mblog/picAll/", "").Replace("https://weibo.cn/mblog/pic/", "").Split(new string[] { "?" }, StringSplitOptions.None)[0];
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        //https://weibo.cn/u/1738733270
        public string UserId
        {
            get
            {
                if (!string.IsNullOrEmpty(StartUrl))
                {
                    return StartUrl.Replace("https://weibo.cn/u/", "");
                }
                else
                {
                    return string.Empty;
                }
            }
        }
    }
}
