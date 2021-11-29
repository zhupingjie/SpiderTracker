using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiderCore.Util
{
    public class PathUtil
    {
        public static string BaseDirectory
        {
            get
            {
                return Path.Combine(new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName, "cache");
            }
        }

        public static void CheckCreateDirectory(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        public static string GetStoreCustomPath(string path)
        {
            var usePath = Path.Combine(PathUtil.BaseDirectory, path);
            PathUtil.CheckCreateDirectory(usePath);
            return usePath;
        }

        public static string GetStoreImagePath(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new Exception("采集类目未选择");
            var path = Path.Combine(BaseDirectory, name, "StoreImage");
            CheckCreateDirectory(path);
            return path;
        }

        public static string GetStoreUserPath(string name, string user)
        {
            var path = GetStoreImagePath(name);
            path = Path.Combine(path, user);
            return path;
        }

        public static string GetStoreUserVideoFile(string name, string user, string bid)
        {
            string path = GetStoreUserPath(name, user);
            path = Path.Combine(path, "video");
            PathUtil.CheckCreateDirectory(path);
            return Path.Combine(path, $"{bid}.mp4");
        }

        public static string[] GetStoreUserVideoFiles(string name, string uid)
        {
            if (string.IsNullOrEmpty(name)) return new string[] { };
            var path = GetStoreUserPath(name, uid);
            path = Path.Combine(path, "video");
            if (!Directory.Exists(path)) return new string[] { };
            return Directory.GetFiles(path, "*.mp4").ToArray();
        }

        public static string[] GetStoreUserThumbnailImageFiles(string name, string uid)
        {
            if (string.IsNullOrEmpty(name)) return new string[] { };
            var path = GetStoreUserPath(name, uid);
            if (!Directory.Exists(path)) return new string[] { };
            path = Path.Combine(path, "thumb");
            if (!Directory.Exists(path)) return new string[] { };
            return Directory.GetFiles(path, "*.jpg").ToArray();
        }

        public static FileInfo[] GetStoreUserThumbnailImageFiles(string name, string uid, string bid)
        {
            if (string.IsNullOrEmpty(name)) return new FileInfo[] { };
            var path = GetStoreUserPath(name, uid);
            if (!Directory.Exists(path)) return new FileInfo[] { };
            path = Path.Combine(path, "thumb");
            if (!Directory.Exists(path)) return new FileInfo[] { };
            return Directory.GetFiles(path, $"{bid}_*").Where(c => c.EndsWith(".jpg")).Select(c => new FileInfo(c)).ToArray();
        }

        public static FileInfo GetStoreUserImageFile(string name, string uid, string file)
        {
            var path = GetStoreUserPath(name, uid);
            var filename = Path.Combine(path, file);
            return new FileInfo(filename);
        }

        public static string[] GetStoreUserImageFiles(string name, string uid)
        {
            if (string.IsNullOrEmpty(name)) return new string[] { };
            var path = GetStoreUserPath(name, uid);
            if (!Directory.Exists(path)) return new string[] { };
            return Directory.GetFiles(path, $"*.jpg").ToArray();
        }

        public static FileInfo[] GetStoreUserImageFiles(string name, string uid, string bid)
        {
            if (string.IsNullOrEmpty(name)) return new FileInfo[] { };
            var path = GetStoreUserPath(name, uid);
            if (!Directory.Exists(path)) return new FileInfo[] { };
            return Directory.GetFiles(path, $"{bid}_*").Where(c => c.EndsWith(".jpg")).Select(c=> new FileInfo(c)).ToArray();
        }

        public static void DeleteStoreUserSource(string name, string user)
        {
            var userPath = PathUtil.GetStoreUserPath(name, user);
            if (Directory.Exists(userPath)) Directory.Delete(userPath, true);
        }

        public static void DeleteStoreUserImageFiles(string name, string user, string status, string filename)
        {
            var files = GetStoreUserImageFiles(name, user, status);
            foreach(var file in files)
            {
                if (!string.IsNullOrEmpty(filename) && filename != file.Name) continue;

                try
                {
                    if (file.Exists) file.Delete();
                }
                catch (Exception) { }
            }
            files = GetStoreUserThumbnailImageFiles(name, user, status);
            foreach (var file in files)
            {
                if (!string.IsNullOrEmpty(filename) && filename != file.Name) continue;

                try
                {
                    if (file.Exists) file.Delete();
                }
                catch (Exception) { }
            }
        }

        public static void DeleteStoreUserVideoFile(string name, string user, string status)
        {
            var file = GetStoreUserVideoFile(name, user, status);
            try
            {
                if (File.Exists(file)) File.Delete(file);
            }
            catch (Exception) { }
        }


        public static int GetStoreUserImageFileCount(string name, string user, string status)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(user) || string.IsNullOrEmpty(status)) return 0;
            string path = PathUtil.GetStoreUserPath(name, user);
            path = Path.Combine(path, "thumb");
            if (!Directory.Exists(path)) return 0;
            return Directory.GetFiles(path, $"{status}_*").Where(c => c.EndsWith("jpg")).Count();
        }

        public static int GetStoreUserVideoCount(string name, string user, string status)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(user) || string.IsNullOrEmpty(status)) return 0;
            var fileName = GetStoreUserVideoFile(name, user, status);
            if (!File.Exists(fileName)) return 0;
            return 1;
        }

        public static void MoveStoreUserSource(string fromName, string toName, string user)
        {
            var userPath = GetStoreUserPath(fromName, user);
            if (!Directory.Exists(userPath)) return;

            var destPath = GetStoreUserPath(toName, user);
            if (Directory.Exists(destPath))
            {
                var backPath = GetStoreUserPath(toName, $"{user}_bak");
                Directory.Move(destPath, backPath);
            }
            Directory.Move(userPath, destPath);
        }

        public static void Shutdown(int time = 30)
        {
            try
            {
                //启动本地程序并执行命令
                Process.Start("Shutdown.exe", $" -s -t {time}");
            }
            catch (Exception)
            {
                
            }
        }
        public static FileInfo GetImageByThumbImage(string thumbFile)
        {
            return GetImageByThumbImage(new FileInfo(thumbFile));
        }

        public static FileInfo GetImageByThumbImage(FileInfo thumb)
        {
            var img = Path.Combine(thumb.Directory.Parent.FullName, thumb.Name);
            return new FileInfo(img);
        }

        public static void CopyUploadImageFiles(string imgFile, string defaultUploadPath)
        {
            var archivePath = Path.Combine(PathUtil.BaseDirectory, defaultUploadPath);
            if (!Directory.Exists(archivePath)) Directory.CreateDirectory(archivePath);

            var file = new FileInfo(imgFile);
            if(file.Exists)
            {
                try
                {
                    var destFile = Path.Combine(archivePath, file.Name);
                    file.CopyTo(destFile, true);
                }
                catch (Exception) { }
            }
        }
    }
}
