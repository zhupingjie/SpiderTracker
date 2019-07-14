using log4net;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SpiderTracker.Imp
{
    public class WeiboSpiderService : ISpiderService
    {
        /// <summary>
        /// 标记是否停止工作
        /// </summary>
        bool StopSpiderWork { get; set; } = false;

        SpiderRunninConfig SpiderRunninConfig { get; set; }

        public WeiboSpiderService()
        {
        }

        public void StartSpider(SpiderRunninConfig runninConfig)
        {
            Task.Factory.StartNew(() =>
            {
                StopSpiderWork = false;
                SpiderRunninConfig = runninConfig;

                var weiboUrlEnum = GetWeiboUrlEnum(runninConfig.StartUrl);
                switch (weiboUrlEnum) {
                    case WeiboUrlEnum.UserArcListUrl:
                        GatherWeiboUserAllArcListUrls(runninConfig);
                        break;
                    case WeiboUrlEnum.UserArcImgListUrl:
                        GatherWeiboUserArcListUrls(runninConfig);
                        break;
                }
            });
        }

        public void StopSpider()
        {
            StopSpiderWork = true;

            SpiderStoping();
        }

        /// <summary>
        /// 通过用户微博列表入口采集
        /// </summary>
        /// <param name="runninConfig"></param>
        void GatherWeiboUserAllArcListUrls(SpiderRunninConfig runninConfig)
        {
            using (var stream = GetResponseStream(runninConfig.StartUrl, GetCookie()))
            {
                if (stream == null)
                {
                    ShowStatus($"远程服务器返回错误-(Get Page Count)!!!");
                    SaveGatherWeiboUserUrl(runninConfig.StartUrl);
                    SpiderComplete();
                    return;
                }
                runninConfig.MaxCountPageCount = GetWeiboUserArcListMaxPageCount(stream);
                RefreshConfig(runninConfig);
            }
            if (runninConfig.StartPageIndex > runninConfig.MaxCountPageCount) runninConfig.StartPageIndex = runninConfig.MaxCountPageCount;

            int haveReadPageCount = 0, pageIndex = 0;
            int readPageCount = runninConfig.StartPageIndex + runninConfig.ReadPageCount;
            for (pageIndex = runninConfig.StartPageIndex; pageIndex < readPageCount; pageIndex++)
            {
                runninConfig.CurrentPageIndex = pageIndex;
                RefreshConfig(runninConfig);

                ShowStatus($"开始读取用户{runninConfig.StartUrl}的第{pageIndex}页微博数据...", true);

                var cookie = GetCookie();
                using (var stream = GetResponseStream($"{runninConfig.StartUrl}?page={pageIndex}", cookie))
                {
                    if (stream == null)
                    {
                        ShowStatus($"远程服务器返回错误-(Get Page Content)!!!");
                        return;
                    }

                    var lst = GetAllArcListUrlsByAnalysHtml(runninConfig.StartUrl, stream);
                    foreach (var item in lst)
                    {
                        if (CheckArcIdExists(item.UserId, item.ArcId))
                        {
                            ShowStatus($"忽略组图{item.ArcId}!");
                            continue;
                        }

                        ShowStatus($"开始读取页面{item.ListUrl}的组图合集...", true);
                        var sourceImgUrls = GetWeiboUserAllSourceImgUrls(runninConfig.StartUrl, item.ListUrl, cookie);
                        if (sourceImgUrls == null)
                        {
                            ShowStatus($"远程服务器返回错误-(Get Page Image Urls)!!!");
                            break;
                        }

                        var imgs = new List<string>();
                        foreach (var sourceImg in sourceImgUrls)
                        {
                            ShowStatus($"开始解析图片{sourceImg.SourceImgUrl}的原始路径...");
                            var realImgUrl = GetWeiboUserRealImgUrls(runninConfig.StartUrl, sourceImg, cookie);
                            if (string.IsNullOrEmpty(realImgUrl))
                            {
                                ShowStatus($"远程服务器返回错误-(Get Image Real Url)!!!");
                                break;
                            }
                            sourceImg.RealImgUrl = realImgUrl;

                            ShowStatus($"开始下载图片{sourceImg.RealImgUrl}...");
                            var imgUrl = GetWeiboUserImgStream(sourceImg, cookie);
                            if (string.IsNullOrEmpty(imgUrl))
                            {
                                ShowStatus($"远程服务器返回错误-(Get Image Data)!!!");
                                break;
                            }
                            haveReadPageCount++;
                            ShowStatus($"图片下载完成!");
                            Thread.Sleep(1000);
                        }
                        ShowStatus($"等待{runninConfig.ReadNextArcWaitSecond}秒读取下一条微博数据...");
                        Thread.Sleep(runninConfig.ReadNextArcWaitSecond * 1000);
                    }

                    if (StopSpiderWork) break;

                    if (pageIndex + 1 < readPageCount)
                    {
                        ShowStatus($"等待{runninConfig.ReadNextPageWaitSecond}秒读取下一页微博数据...");
                        Thread.Sleep(runninConfig.ReadNextPageWaitSecond * 1000);
                    }
                }
            }
            ShowStatus($"采集完成,采集图集第{pageIndex}页,共图片{haveReadPageCount}张");
            SaveGatherWeiboUserUrl(runninConfig.StartUrl);
            SpiderComplete();
        }

        void GatherWeiboUserArcListUrls(SpiderRunninConfig runninConfig)
        {
            var arcListUrl = runninConfig.StartUrl;

            ShowStatus($"开始读取页面用户信息...");
            var userId = GetWeiboEmptyUserId();

            var arcId = GetWeiboUserArcIdByArcListUrl(arcListUrl);
            if (arcId == null)
            {
                ShowStatus($"远程服务器返回错误-(Get Page Act ID)!!!");
                return;
            }
            GatherWeiboUserArcListUrls(userId, arcId);
        }

        string GetWeiboEmptyUserId()
        {
            return "0".PadLeft(10, '0');
        }

        /// <summary>
        /// 通过用户组图合集入口采集
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="arcId"></param>
        void GatherWeiboUserArcListUrls(string userId, string arcId)
        {
            var startUrl = GetWeiboUserUrl(userId);
            var arcUrl = GetWeiboUserArcImgUrl(userId, arcId);

            var cookie = GetCookie();
            ShowStatus($"开始读取页面{arcUrl}的组图合集...", true);
            var sourceImgUrls = GetWeiboUserAllSourceImgUrls(startUrl, arcUrl, cookie);
            if (sourceImgUrls == null)
            {
                ShowStatus($"远程服务器返回错误-(Get Page Image Urls)!!!");
                return;
            }

            //重试需要先删除已经存在的文章目录
            var path = GetLocalArcImgPath(userId, arcId);
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
            int imgCount = 0;
            foreach (var sourceImg in sourceImgUrls)
            {
                ShowStatus($"开始解析图片{sourceImg.SourceImgUrl}的原始路径...");
                var realImgUrl = GetWeiboUserRealImgUrls(startUrl, sourceImg, cookie);
                if (string.IsNullOrEmpty(realImgUrl))
                {
                    ShowStatus($"远程服务器返回错误-(Get Image Real Url)!!!");
                    break;
                }
                sourceImg.RealImgUrl = realImgUrl;

                ShowStatus($"开始下载图片{sourceImg.RealImgUrl}...");
                var imgUrl = GetWeiboUserImgStream(sourceImg, cookie);
                if (string.IsNullOrEmpty(imgUrl))
                {
                    ShowStatus($"远程服务器返回错误-(Get Image Data)!!!");
                    break;
                }
                imgCount++;
                ShowStatus($"图片下载完成!");
                Thread.Sleep(1000);
            }
            ShowStatus($"采集完成,共采集图片{imgCount}张");
        }

        /// <summary>
        /// 获取微博用户文章列表最大分页数
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        int GetWeiboUserArcListMaxPageCount(Stream stream)
        {
            int pageSize = 1;
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.Load(stream, Encoding.UTF8);

            var node = doc.DocumentNode.SelectSingleNode(".//div[@id=\"pagelist\"]/form/div");
            if (node != null)
            {
                //下页&nbsp;&nbsp;1/830页
                if (node.InnerText.StartsWith("下页") && node.InnerText.EndsWith("页"))
                {
                    var str = node.InnerText.Replace("下页&nbsp;&nbsp;", "").Replace("页", "");
                    var arr = str.Split(new string[] { "/" }, StringSplitOptions.None);
                    if (arr.Length == 2)
                    {
                        int.TryParse(arr[1], out pageSize);
                    }
                }
            }
            return pageSize;
        }


        /// <summary>
        /// 读取微博用户文章列表包含的图片集合路径(组图共X张)
        /// </summary>
        /// <param name="url"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        List<WeiboImgDto> GetAllArcListUrlsByAnalysHtml(string startUrl, Stream stream)
        {
            var lst = new List<WeiboImgDto>();

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.Load(stream, Encoding.UTF8);
            var nodes = doc.DocumentNode.SelectNodes(".//div[@class=\"c\"]/div/a");
            if (nodes == null) return lst;

            foreach (var node in nodes)
            {
                if (node.InnerText.Contains("组图共"))
                {
                    if (node.Attributes.Contains("href"))
                    {
                        var href = node.Attributes["href"].Value;
                        if (lst.Any(c => c.ListUrl == href)) continue;
                        lst.Add(new WeiboImgDto(startUrl)
                        {
                            ListUrl = href
                        });
                    }
                }
                else if (node.FirstChild != null && node.FirstChild.Name == "img")
                {
                    if (node.Attributes.Contains("href"))
                    {
                        var href = node.Attributes["href"].Value;
                        if (lst.Any(c => c.ListUrl == href)) continue;
                        lst.Add(new WeiboImgDto(startUrl)
                        {
                            ListUrl = href
                        });
                    }
                }
            }
            return lst;
        }
        
        
        /// <summary>
        /// 读取微博用户图片集合包含的图片原始路径(原图)
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="cookie"></param>
        /// <returns></returns>
        List<WeiboImgDto> GetWeiboUserAllSourceImgUrls(string startUrl, string arcListUrl, string cookie)
        {
            using (var stream = GetResponseStream(arcListUrl, cookie))
            {
                if (stream == null) return null;

                var lst = GetAllSourceImgUrlsByAnalysHtml(startUrl, arcListUrl, stream);
                return lst;
            }
        }

        /// <summary>
        /// 解析微博用户图片集合包含的图片原始路径(原图)
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        List<WeiboImgDto> GetAllSourceImgUrlsByAnalysHtml(string startUrl, string arcListUrl, Stream stream)
        {
            var lst = new List<WeiboImgDto>();

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.Load(stream, Encoding.UTF8);
            var nodes = doc.DocumentNode.SelectNodes(".//div[@class=\"c\"]/a");
            if (nodes == null) return lst;

            foreach (var node in nodes)
            {
                if (node.InnerText == "原图")
                {
                    if (node.Attributes.Contains("href"))
                    {
                        var href = node.Attributes["href"].Value.Replace("&amp;", "&");
                        var url = $"{GetWeiboHost()}{href}";

                        if (lst.Any(c => c.SourceImgUrl == url)) continue;
                        lst.Add(new WeiboImgDto(startUrl)
                        {
                            ListUrl = arcListUrl,
                            SourceImgUrl = url
                        });
                    }
                }
            }
            return lst;
        }

        /// <summary>
        /// 解析微博用户图片原始路径
        /// </summary>
        /// <param name="startUrl"></param>
        /// <param name="dto"></param>
        /// <param name="cookie"></param>
        /// <returns></returns>
        string GetWeiboUserRealImgUrls(string startUrl, WeiboImgDto dto, string cookie)
        {
            using (var stream = GetResponseStream(dto.SourceImgUrl, cookie))
            {
                if (stream == null) return null;

                var url = GetRealImgUrlsByAnalysHtml(startUrl, dto, stream);
                return url;
            }
        }

        /// <summary>
        /// 解析微博用户图片集合包含的图片原始路径(原图)
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        string GetRealImgUrlsByAnalysHtml(string startUrl, WeiboImgDto dto, Stream stream)
        {
            string url = string.Empty;

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.Load(stream, Encoding.UTF8);
            var nodes = doc.DocumentNode.SelectNodes(".//div[@class=\"nm\"]/a");
            if (nodes == null)
            {
                //当前图片尺寸较小,可直接下载
                return dto.SourceImgUrl;
            }

            foreach (var node in nodes)
            {
                if (node.InnerText == "确定")
                {
                    if (node.Attributes.Contains("href"))
                    {
                        url = node.Attributes["href"].Value.Replace("&amp;", "&");
                        break;
                    }
                }
            }
            return url;
        }

        /// <summary>
        /// 读取微博用户图片原始路径
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="cookie"></param>
        /// <returns></returns>
        string GetWeiboUserImgStream(WeiboImgDto dto, string cookie)
        {
            using (var stream = GetResponseStream(dto.RealImgUrl, cookie, true))
            {
                if (stream == null) return null;

                var lst = DownloadRealImgByUrl(dto, stream);
                return lst;
            }
        }

        /// <summary>
        /// 解析图片地址并下载图片到用户文章目录下
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        string DownloadRealImgByUrl(WeiboImgDto dto, Stream stream)
        {
            try
            {
                Image downImage = System.Drawing.Image.FromStream(stream);
                string path = GetLocalArcImgPath(dto.UserId, dto.ArcId);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string fileName = $"{DateTime.Now.ToString("HHmmssffff")}.jpg";
                var img = Path.Combine(path, fileName);
                downImage.Save(img);
                downImage.Dispose();
                ShowStatus($"获取图片资源{dto.RealImgUrl}成功!", true);
                return img;
            }
            catch (Exception ex)
            {
                var msg = $"获取图片资源{dto.RealImgUrl}异常:{ex.Message}";
                ShowStatus(msg, true, ex);
                return null;
            }
        }


        /// <summary>
        /// 读取远程请求返回数据
        /// </summary>
        /// <param name="url"></param>
        /// <param name="cookie"></param>
        /// <param name="referer"></param>
        /// <returns></returns>
        Stream GetResponseStream(string url, string cookie, bool readImgResource = false)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            if (readImgResource)
            {
                request.ContentType = "text/html";
                request.Headers["Content-Encoding"] = "gzip, deflate, sdch, br";
            }
            else
            {
                request.ContentType = "application/x-www-form-urlencoded; charset=gb2312";
            }
            request.Method = "GET";
            request.KeepAlive = true;
            request.Headers.Add("Cookie", cookie);
            try
            {
                var response = request.GetResponse();
                return response.GetResponseStream();
            }
            catch (Exception ex)
            {
                ShowStatus($"获取路径资源{url}异常:{ex.Message}",true, ex);
                return null;
            }
        }

        /// <summary>
        /// 获取新浪微博的主域名
        /// </summary>
        /// <returns></returns>
        string GetWeiboHost()
        {
            return "https://weibo.cn";
        }

        /// <summary>
        /// 获取起始路径类型
        /// </summary>
        /// <param name="startUrl"></param>
        /// <returns></returns>
        WeiboUrlEnum GetWeiboUrlEnum(string startUrl)
        {
            //0 = https://weibo.cn/u/1738733270?page=18
            //1 = https://weibo.cn/mblog/picAll/Gdg1au045?rl=1
            //2 = https://weibo.cn/mblog/oripic?id=Gdg1au045&u=ede81841gy1fqldgdzxlpj20qo14011g&rl=1
            //3 = http://wx2.sinaimg.cn/large/ede81841gy1fqldf2bcuij22kw3vc1l5.jpg

            WeiboUrlEnum weiboUrlEnum = WeiboUrlEnum.UserArcListUrl;
            if (startUrl.StartsWith($"{GetWeiboHost()}/u"))
            {
                weiboUrlEnum = WeiboUrlEnum.UserArcListUrl;
            }
            else if (startUrl.StartsWith($"{GetWeiboHost()}/mblog/pic"))
            {
                weiboUrlEnum = WeiboUrlEnum.UserArcImgListUrl;
            }
            return weiboUrlEnum;
        }
                
        string GetCookie()
        {
            string root = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cache", "StoreCookie");
            if (!Directory.Exists(root)) Directory.CreateDirectory(root);
            var path = Path.Combine(root, "cache.txt");
            if (!File.Exists(path)) throw new Exception($"未找到登陆认证Cookie,请先配置Cookie!!!");
            var cookies = File.ReadAllLines(path);
            if (cookies.Length > 0)
            {
                var random = new Random();
                var select = random.Next(0, cookies.Length);
                return cookies[select];
            }
            else
            {
                throw new Exception($"未找到登陆认证Cookie,请先配置Cookie!!!");
            }
        }

        string GetWeiboUserUrl(string userId)
        {
            return $"{GetWeiboHost()}/u/{userId}";
        }

        string GetWeiboUserArcImgUrl(string userId, string arcId)
        {
            return $"{GetWeiboHost()}/mblog/picAll/{arcId}?rl=1";
        }

        string GetWeiboUserArcIdByArcListUrl(string arcListUrl)
        {
            //https://weibo.cn/mblog/picAll/Gdg1au045?rl=1
            var tmp = arcListUrl.Split(new string[] { "?" }, StringSplitOptions.None)[0];
            if (tmp.Contains("/"))
            {
                var arr = tmp.Split(new string[] { "/" }, StringSplitOptions.None);
                return arr[arr.Length - 1];
            }
            else
            {
                return null;
            }
        }

        public string GetLocalArcImgPathBySelect(string select)
        {
            var arr = select.Split(new string[] { "/" }, StringSplitOptions.None);
            var userId = arr[0];
            var arcId = arr[1];
            return GetLocalArcImgPath(userId, arcId);
        }
        public string GetLocalArcImgPath(string userId = null, string arcId = null)
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cache", "StoreImgs");
            if (userId != null)
            {
                path = Path.Combine(path, userId);
            }
            if (arcId != null)
            {
                path = Path.Combine(path, arcId);
            }
            if (userId == null && arcId != null)
            {
                throw new Exception($"非法路径!");
            }
            return path;
        }
        public string GetUserIdByPath(string path)
        {
            //D:\Project\SpiderTracker\SpiderTracker\bin\Debug\cache\StoreImgs\1738733270
            var arr = path.Split(new string[] { "\\" }, StringSplitOptions.None);
            if (arr.Length > 0) return arr[arr.Length - 1];
            return null;
        }
        public string GetArcIdByPath(string path)
        {
            //D:\Project\SpiderTracker\SpiderTracker\bin\Debug\cache\StoreImgs\1738733270\GdUJXDvU5
            var arr = path.Split(new string[] { "\\" }, StringSplitOptions.None);
            if (arr.Length > 0) return arr[arr.Length - 1];
            return null;
        }
        bool CheckArcIdExists(string userId, string arcId)
        {
            string path = GetLocalArcImgPath(userId, arcId);
            return Directory.Exists(path);
        }

        void SaveGatherWeiboUserUrl(string startUrl)
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cache", "StoreConfig");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            var file = Path.Combine(path, "config.txt");
            if (File.Exists(file))
            {
                var lines = File.ReadAllLines(file);
                if (!lines.Contains(startUrl))
                {
                    File.AppendAllLines(file, new string[] { startUrl });
                }
            }
            else
            {
                File.AppendAllLines(file, new string[] { startUrl });
            }
        }

    }
}
