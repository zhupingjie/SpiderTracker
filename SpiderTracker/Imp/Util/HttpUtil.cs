using SpiderTracker.Imp.MWeiboJson;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SpiderTracker.Imp
{
    public class HttpUtil
    {
        public static string GetHttpRequestHtmlResult(string url, SpiderRunningConfig runningConfig)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.Timeout = 5 * 1000;
            if (!string.IsNullOrEmpty(runningConfig.LoginCookie))
            {
                request.Headers["Cookie"] = runningConfig.LoginCookie;
            }
            try
            {
                var response = request.GetResponse();
                var stream = response.GetResponseStream();
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static Image GetHttpRequestImageResult(string url, SpiderRunningConfig runningConfig)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.Timeout = 5 * 1000;
            if (!string.IsNullOrEmpty(runningConfig.LoginCookie))
            {
                request.Headers["Cookie"] = runningConfig.LoginCookie;
            }
            try
            {
                var response = request.GetResponse();
                var stream = response.GetResponseStream();

                return Image.FromStream(stream);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        
        public static bool SinaLogin(SpiderRunningConfig runningConfig, string userName, string password)
        {
            var postApi = $"https://passport.weibo.cn/sso/login";
            var paramData = $"username={userName}&password={password}&savestate=1&r=https://m.weibo.cn/&ec=0&mainpageflag=1&entry=mweibo";

            var cookie = GetHttpRequestCookie(postApi, paramData);
            if (!string.IsNullOrEmpty(cookie))
            {
                runningConfig.LoginCookie = cookie;
                runningConfig.LoginUser = userName;
                runningConfig.LoginPassword = password;
                return true;
            }
            return false;
        }

        public static bool GetSinaLoginToken(SpiderRunningConfig runningConfig)
        {
            var postApi = $"https://m.weibo.cn/api/config";

            var html = PostHttpRequest(postApi, string.Empty, runningConfig);

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);

            var jsonResult = Newtonsoft.Json.JsonConvert.DeserializeObject<MWeiboLoginResult>(doc.DocumentNode.InnerText) as MWeiboLoginResult;
            if(jsonResult == null || !jsonResult.success || !jsonResult.data.login)
            {
                return false;
            }
            runningConfig.LoginUid = jsonResult.data.uid;
            runningConfig.LoginToken = jsonResult.data.st;
            return true;
        }



        static string GetHttpRequestCookie(string url, string paramData)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.Timeout = 5 * 1000;
            request.Referer = "https://passport.weibo.cn/signin/login";
            request.ContentType = "application/x-www-form-urlencoded";

            try
            {
                byte[] byteArray = Encoding.Default.GetBytes(paramData);
                request.ContentLength = byteArray.Length;

                using (Stream writeStream = request.GetRequestStream())
                {
                    writeStream.Write(byteArray, 0, byteArray.Length);
                    writeStream.Close();
                }

                var response = request.GetResponse();
                var stream = response.GetResponseStream();
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                {
                    var result = reader.ReadToEnd();
                    return response.Headers["Set-Cookie"];
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static string PostHttpRequest(string url, string paramData, SpiderRunningConfig runningConfig)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.Timeout = 5 * 1000;
            //request.Referer = "https://passport.weibo.cn/signin/login";
            request.ContentType = "application/x-www-form-urlencoded";

            if (!string.IsNullOrEmpty(runningConfig.LoginCookie))
            {
                request.Headers["Cookie"] = runningConfig.LoginCookie;
            }
            try
            {
                byte[] byteArray = Encoding.Default.GetBytes(paramData);
                request.ContentLength = byteArray.Length;

                using (Stream writeStream = request.GetRequestStream())
                {
                    writeStream.Write(byteArray, 0, byteArray.Length);
                    writeStream.Close();
                }

                var response = request.GetResponse();
                var stream = response.GetResponseStream();
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
