using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiderTracker.Imp
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
        public static string[] GetStoreUserThumbnailImageFiles(string name, string uid, string bid)
        {
            if (string.IsNullOrEmpty(name)) return new string[] { };
            var path = GetStoreUserPath(name, uid);
            if (!Directory.Exists(path)) return new string[] { };
            path = Path.Combine(path, "thumb");
            if (!Directory.Exists(path)) return new string[] { };
            return Directory.GetFiles(path, $"{bid}_*").Where(c => c.EndsWith(".jpg")).ToArray();
        }

        public static string[] GetStoreUserImageFiles(string name, string uid, string bid)
        {
            if (string.IsNullOrEmpty(name)) return new string[] { };
            var path = GetStoreUserPath(name, uid);
            if (!Directory.Exists(path)) return new string[] { };
            return Directory.GetFiles(path, $"{bid}_*").Where(c => c.EndsWith(".jpg")).ToArray();
        }

        public static void DeleteStoreUserImageFiles(string name, string user, string status)
        {
            var files = GetStoreUserImageFiles(name, user, status);
            foreach(var file in files)
            {
                File.Delete(file);
            }
        }

        public static void DeleteStoreUserVideoFile(string name, string user, string status)
        {
            var file = GetStoreUserVideoFile(name, user, status);
            if (File.Exists(file)) File.Delete(file);
        }


        public static bool CheckStoreUserImageExists(string name, string user, string status)
        {
            if (string.IsNullOrEmpty(name)) return false;
            string path = PathUtil.GetStoreUserPath(name, user);
            return Directory.Exists(path) && Directory.GetFiles(path, $"{status}_*").Where(c => c.EndsWith("jpg")).Count() > 0;
        }

        public static bool CheckStoreUserVideoExists(string name, string user, string status)
        {
            if (string.IsNullOrEmpty(name)) return false;
            var fileName = GetStoreUserVideoFile(name, user, status);
            return File.Exists(fileName);
        }

    }
}
