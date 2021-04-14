using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using StockSimulateWeb.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpiderWeb.Controllers
{
    public class SpiderController : BaseController
    {
        public SpiderController(IHostingEnvironment hostingEnvironment) : base(hostingEnvironment)
        {
        }

        [HttpGet]
        public APIResult Index()
        {
            var res = new APIResult();
            res.Result = "欢迎访问Spider WEB API站点";
            return res;
        }

        [HttpGet]
        public APIResult GetSinaSources(string category, string status, int thumb)
        {
            var res = new APIResult();
            res.Result = GetSinaSoureImageFiles(category, status, thumb);
            return res;
        }

        [HttpPost]
        public APIResult UploadSinaSources([FromForm] IFormCollection formData)
        {
            var res = new APIResult();

            //var files = SaveUploadFiles(formData, "cosplay", 100, 100);
            //if (files.Length == 0)
            //{
            //    res.Code = 501;
            //    res.Message = "无文件可存储";
            //    return res;
            //}
            //return res;

            var formValues = formData.ToDictionary(c => c.Key, c => c.Value);
            if (formValues.ContainsKey("category"))
            {
                var category = formValues["category"];
                int thumbWidth = 100, thumbHeight = 100;
                if (formValues.ContainsKey("width"))
                {
                    thumbWidth = int.Parse(formValues["width"]);
                }
                if (formValues.ContainsKey("height"))
                {
                    thumbHeight = int.Parse(formValues["height"]);
                }
                var files = SaveUploadFiles(formData, category, thumbWidth, thumbHeight);
                if (files.Length == 0)
                {
                    res.Code = 501;
                    res.Message = "无文件可存储";
                    return res;
                }
                return res;
            }
            else
            {
                res.Code = 501;
                res.Message = "未传参[category]";
                return res;
            }
        }
    }

    public class SpiderUploadModel
    { 
        public string category { get; set; }
        public string uid { get; set; }
        public string bid { get; set; }
    }
}
