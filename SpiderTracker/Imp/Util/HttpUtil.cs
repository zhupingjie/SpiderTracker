using System;
using System.Collections.Generic;
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

        public static string GetHttpRequestHtmlResult(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.Timeout = 5 * 1000;
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

        public static Image GetHttpRequestImageResult(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.Timeout = 5 * 1000;
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
    }
}
