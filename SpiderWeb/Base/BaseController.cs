using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SpiderWeb;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace StockSimulateWeb.Base
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class BaseController : ControllerBase
    {
        private string uploadImagePath = "images";

        private string webRootPath;
        protected string WebRootPath
        {
            get
            {
                return webRootPath;
            }
        }
        public BaseController(IHostingEnvironment hostingEnvironment)
        {
            webRootPath = hostingEnvironment.WebRootPath;
        }

        public virtual FileInfoModel[] SaveUploadFiles(IFormCollection formCollection, string moduleFolder = "images", int thumbWidth = 100, int thumbHeight = 100)
        {
            var fileInfos = new List<FileInfoModel>();
            try
            {
                foreach (var file in formCollection.Files)
                {
                    var uploadPath = Path.Combine(WebRootPath, uploadImagePath, moduleFolder);
                    if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);

                    var thumbPath = Path.Combine(WebRootPath, uploadImagePath, moduleFolder, "thumb");
                    if (!Directory.Exists(thumbPath)) Directory.CreateDirectory(thumbPath);

                    var fileName = Path.Combine(uploadPath, file.FileName);
                    var thumbName = Path.Combine(thumbPath, file.FileName);
                    using (FileStream fs = System.IO.File.Create(fileName))
                    {
                        file.CopyTo(fs);
                        fs.Flush();

                        var image = Image.FromStream(fs);
                        var thumbSize = GetThumbImageSize(image, thumbWidth, thumbHeight);
                        var thumbImg = image.GetThumbnailImage(thumbSize.Width, thumbSize.Height, null, IntPtr.Zero);
                        thumbImg.Save(thumbName, ImageFormat.Jpeg);
                    }
                    fileInfos.Add(new FileInfoModel()
                    {
                        Name = file.FileName,
                        FileName = file.FileName,
                        FileUrl = GetUrlByPhysicalPath(fileName),
                        PhyFileUrl = fileName
                    });
                }
            }
            catch(Exception ex)
            {
                LogUtil.Error(ex);
            }
            return fileInfos.ToArray();
        }

        public string[] GetSinaSoureImageFiles(string category, string status, int thumb)
        {
            var files = new List<string>();
            var path = Path.Combine(WebRootPath, uploadImagePath, category);
            if (thumb > 0) path = Path.Combine(path, "thumb");
            if (!Directory.Exists(path)) return files.ToArray();

            return Directory.GetFiles(path, $"{status}*.jpg").Select(c=> GetUrlByPhysicalPath(c)).ToArray();
        }

        public string GetUrlByPhysicalPath(string fileName)
        {
            if (fileName.Contains("/")) fileName = fileName.Replace("/", "\\");

            var rootPath = WebRootPath;
            if (rootPath.Contains("/")) rootPath = rootPath.Replace("/", "\\");
            return fileName.Replace(rootPath, "", StringComparison.CurrentCultureIgnoreCase).Replace("\\", "/");
        }

        Size GetThumbImageSize(Image image, int thumbWidth, int thumbHeight)
        {
            var width = thumbWidth;
            var height = thumbHeight;
            var rate = image.Width * 1.0m / image.Height * 1.0m;

            if (image.Width > image.Height)
            {
                height = (int)(width / rate);
            }
            else
            {
                width = (int)(height * rate);
            }
            return new Size(width, height);
        }
    }

    public class FileInfoModel
    {
        public string Name { get; set; }
        public string FileName { get; set; }
        public string FileUrl { get; set; }

        public string PhyFileUrl { get; set; }

        public string Extension { get; set; }
        public long Length { get; set; }
    }
}
