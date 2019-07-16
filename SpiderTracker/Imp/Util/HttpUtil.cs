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
            request.Headers["Cookie"] = runningConfig.LoginCookie;
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
            request.Headers["X-Log-Uid"] = runningConfig.LoginUid;
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
        
        public static string GetHttpRequestCookie(string url, string paramData)
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
    }
}
