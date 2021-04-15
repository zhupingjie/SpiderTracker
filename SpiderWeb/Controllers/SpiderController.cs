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
        public APIResult GetSinaSources(string category, string status, string filename, int thumb)
        {
            var res = new APIResult();
            res.Result = GetSinaSoureImageFiles(category, status, filename, thumb);
            return res;
        }

        [HttpPost]
        public APIResult DeleteSinaSources(string category, string status, string fileName)
        {
            var res = new APIResult();
            res.Result = DeleteSinaSoureImageFiles(category, status, fileName);
            return res;
        }

        [HttpPost]
        public APIResult UploadSinaSources([FromForm] IFormCollection formData)
        {
            var res = new APIResult();
            try
            {
                var formValues = formData.ToDictionary(c => c.Key, c => c.Value);
                if (formValues.ContainsKey("category"))
                {
                    var category = formValues["category"];
                    int thumbWidth = 100, thumbHeight = 100;
                    if (formValues.ContainsKey("width"))
                    {
                        thumbWidth = (int)decimal.Parse(formValues["width"]);
                    }
                    if (formValues.ContainsKey("height"))
                    {
                        thumbHeight = (int)decimal.Parse(formValues["height"]);
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
            catch (Exception ex)
            {
                LogUtil.Error(ex);
                res.Code = 504;
                res.Message = ex.Message;
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
