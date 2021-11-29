using SpiderCore.Config;
using SpiderDomain.Entity;
using SpiderCore.Model;
using SpiderCore.Model.MWeiboJson;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SpiderCore.Util
{
    public class HttpUtil
    {
        public static string GetHttpRequestHtmlResult(string url,  bool zip, SpiderRunningConfig runningConfig)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.Timeout = 10 * 1000;
            request.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; Trident/4.0; QQWubi 133; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; CIBA; InfoPath.2)";
            if (!string.IsNullOrEmpty(runningConfig.LoginCookie))
            {
                request.Headers["Cookie"] = runningConfig.LoginCookie;
            }
            try
            {
                var response = request.GetResponse();
                var stream = response.GetResponseStream();
                if (zip)
                {
                    var zipstream = new GZipStream(stream, CompressionMode.Decompress);
                    using (var reader = new StreamReader(zipstream, Encoding.UTF8))
                    {
                        return reader.ReadToEnd();
                    }
                }
                else
                {
                    using (var reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static string GetHttpRequestJsonResult(string url, SpiderRunningConfig runningConfig)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.Timeout = 10 * 1000;
            request.ContentType = "application/json; charset=utf-8";
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
            request.Timeout = 300 * 1000;
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

        public static bool GetHttpRequestVedioResult(string url, string filePath, SpiderRunningConfig runningConfig)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.Timeout = 60 * 1000;
            if (!string.IsNullOrEmpty(runningConfig.LoginCookie))
            {
                request.Headers["Cookie"] = runningConfig.LoginCookie;
            }
            try
            {
                var response = request.GetResponse();
                var stream = response.GetResponseStream();
                using (Stream sos = new System.IO.FileStream(filePath, System.IO.FileMode.Create))
                {
                    byte[] img = new byte[1024];
                    int total = stream.Read(img, 0, img.Length);
                    while (total > 0)
                    {
                        //之后再输出内容
                        sos.Write(img, 0, total);
                        total = stream.Read(img, 0, img.Length);
                    }
                    stream.Close();
                    stream.Dispose();
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool GetHttpRequestRangeVedioResult(string url, string referer, string filePath, SpiderRunningConfig runningConfig)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.Timeout = 60 * 1000;
            request.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; Trident/4.0; QQWubi 133; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; CIBA; InfoPath.2)";
            request.Headers["access-control-request-headers"] = "Range";
            request.Headers["access-control-request-method"] = "GET";
            request.Referer = referer;
            if (!string.IsNullOrEmpty(runningConfig.LoginCookie))
            {
                request.Headers["Cookie"] = runningConfig.LoginCookie;
            }
            try
            {
                var response = request.GetResponse();
                var stream = response.GetResponseStream();
                using (Stream sos = new System.IO.FileStream(filePath, System.IO.FileMode.Create))
                {
                    byte[] img = new byte[1024];
                    int total = stream.Read(img, 0, img.Length);
                    while (total > 0)
                    {
                        //之后再输出内容
                        sos.Write(img, 0, total);
                        total = stream.Read(img, 0, img.Length);
                    }
                    stream.Close();
                    stream.Dispose();
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
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
            request.Timeout = 10 * 1000;
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
            request.Timeout = 10 * 1000;
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
                LogUtil.Error(ex);
                return null;
            }
        }

        public static string PostHttpUploadFile(string url, string file, NameValueCollection data, Encoding encoding)
        {
            try
            {
                string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
                byte[] boundarybytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");
                byte[] endbytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");

                //1.HttpWebRequest
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.ContentType = "multipart/form-data; boundary=" + boundary;
                request.Method = "POST";
                request.KeepAlive = true;
                request.Credentials = CredentialCache.DefaultCredentials;

                using (Stream stream = request.GetRequestStream())
                {
                    //1.1 key/value
                    string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
                    if (data != null)
                    {
                        foreach (string key in data.Keys)
                        {
                            stream.Write(boundarybytes, 0, boundarybytes.Length);
                            string formitem = string.Format(formdataTemplate, key, data[key]);
                            byte[] formitembytes = encoding.GetBytes(formitem);
                            stream.Write(formitembytes, 0, formitembytes.Length);
                        }
                    }

                    //1.2 file
                    string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: application/octet-stream\r\n\r\n";
                    byte[] buffer = new byte[4096];
                    int bytesRead = 0;
                    stream.Write(boundarybytes, 0, boundarybytes.Length);
                    string header = string.Format(headerTemplate, "file", Path.GetFileName(file));
                    byte[] headerbytes = encoding.GetBytes(header);
                    stream.Write(headerbytes, 0, headerbytes.Length);
                    using (FileStream fileStream = new FileStream(file, FileMode.Open, FileAccess.Read))
                    {
                        while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                        {
                            stream.Write(buffer, 0, bytesRead);
                        }
                    }

                    //1.3 form end
                    stream.Write(endbytes, 0, endbytes.Length);
                }
                //2.WebResponse
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                using (StreamReader stream = new StreamReader(response.GetResponseStream()))
                {
                    return stream.ReadToEnd();
                }
            }
            catch(Exception ex)
            {
                LogUtil.Error(ex);
                return null;
            }
        }

        static string GetRemoteViewImageApi(string serverIp, string api, string category, string status, string filename, bool thumb)
        {
            return $"http://{serverIp}/{api}?category={category}&status={status}&filename={filename}&thumb={(thumb ? 1 : 0)}";
        }

        /// <summary>
        /// 获取上传的微博图片
        /// </summary>
        /// <param name="category"></param>
        /// <param name="bid"></param>
        /// <param name="thumb"></param>
        /// <returns></returns>
        public static string[] GetRemoteImageFiles(SpiderRunningConfig runningConfig, string bid, string filename, bool thumb)
        {
            var files = new List<string>();
            var api = GetRemoteViewImageApi(runningConfig.DefaultUploadServerIP, runningConfig.DefaultGetImageAPI, runningConfig.Category, bid, filename, thumb);
            var retStr = HttpUtil.GetHttpRequestJsonResult(api, runningConfig);
            if (string.IsNullOrEmpty(retStr)) return files.ToArray();

            var result = Newtonsoft.Json.JsonConvert.DeserializeObject<APIResult>(retStr);
            if (result == null || !result.Success || result.Result == null) return files.ToArray();

            var objArr = Newtonsoft.Json.JsonConvert.DeserializeObject<string[]>(result.Result.ToString());
            return objArr.Select(c => $"http://{runningConfig.DefaultUploadServerIP}{c}").ToArray();
        }


        public static string GetSinaSoureImageUrl(string serverIp, string category, string status, bool thumb)
        {
            return $"http://{serverIp}/getimage.html?category={category}&status={status}&thumb={(thumb ? 1 : 0)}";
        }

        static string GetRemoteActionImageApi(string serverIp, string api)
        {
            return $"http://{serverIp}/{api}";
        }

        /// <summary>
        /// 上传微博图片
        /// </summary>
        /// <param name="runningConfig"></param>
        /// <param name="upload"></param>
        /// <param name="imgFile"></param>
        /// <returns></returns>
        public static APIResult UploadRemoteImage(SpiderRunningConfig runningConfig, SinaAction upload, FileInfo imgFile)
        {
            var nv = new NameValueCollection();
            nv.Add("category", upload.category);
            nv.Add("uid", upload.uid);
            nv.Add("bid", upload.bid);
            nv.Add("width", $"{runningConfig.ThumbnailImageWidth}");
            nv.Add("height", $"{runningConfig.ThumbnailImageHeight}");

            try
            {
                var api = GetRemoteActionImageApi(runningConfig.DefaultUploadServerIP, runningConfig.DefaultUploadImageAPI);
                var result = HttpUtil.PostHttpUploadFile(api, imgFile.FullName, nv, Encoding.Default);
                if (result == null) return null;

                return Newtonsoft.Json.JsonConvert.DeserializeObject<APIResult>(result);
            }
            catch(Exception ex)
            {
                LogUtil.Error(ex);
                return null;
            }
        }

        /// <summary>
        /// 删除已上传的微博图片
        /// </summary>
        /// <param name="runningConfig"></param>
        /// <param name="category"></param>
        /// <param name="bid"></param>
        /// <param name="img"></param>
        /// <returns></returns>
        public static bool DeleteSinaSourceImage(SpiderRunningConfig runningConfig, string bid, string img)
        {
            try
            {
                var api = GetRemoteActionImageApi(runningConfig.DefaultUploadServerIP, runningConfig.DefaultDeleteImageAPI);
                api += $"?category={runningConfig.Category}&status={bid}&filename={img}";
                var result = HttpUtil.PostHttpRequest(api, string.Empty, runningConfig);
                if (string.IsNullOrEmpty(result)) return false;

                var rst = Newtonsoft.Json.JsonConvert.DeserializeObject<APIResult>(result);
                if (rst == null || !rst.Success) return false;
                return true;

            }
            catch (Exception ex)
            {
                LogUtil.Error(ex);
                return false;
            }
        }
    }
}
