﻿using System;
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

        public static int GetUserStatusCount(string name, string user)
        {
            if (string.IsNullOrEmpty(name)) throw new Exception("采集类目未选择");
            var path = PathUtil.GetStoreImageUserPath(name, user);
            if (!Directory.Exists(path)) return 0;

            return Directory.GetDirectories(path).Length;
        }

        public static int GetUserStatusImageCount(string userStatusPath)
        {
            if (!Directory.Exists(userStatusPath)) return 0;

            return Directory.GetFiles(userStatusPath).Where(c => c.EndsWith("jpg")).Count();
        }

        public static string[] GetStoreNames()
        {
            var lst = new List<string>();
            var paths = Directory.GetDirectories(PathUtil.BaseDirectory);
            foreach(var path in paths)
            {
                var arr = path.Split(new string[] { "\\" }, StringSplitOptions.None);
                if (arr.Length > 0) lst.Add(arr[arr.Length - 1]);
            }
            return lst.ToArray();
        }

        public static string GetStoreImagePath(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new Exception("采集类目未选择");
            var path = Path.Combine(BaseDirectory, name, "StoreImage");
            CheckCreateDirectory(path);
            return path;
        }

        public static string GetStoreVedioUserPath(string name, string user, string bid)
        {
            string path = GetStoreImageUserPath(name, user);
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            return Path.Combine(path, $"{bid}.mp4");
        }

        public static string GetStoreImageUserPath(string name, string user)
        {
            if (string.IsNullOrEmpty(name)) throw new Exception("采集类目未选择");
            var path = GetStoreImagePath(name);
            path = Path.Combine(path, user);
            return path;
        }
        public static string[] GetStoreImageUserPaths(string name, string user)
        {
            if (string.IsNullOrEmpty(name)) throw new Exception("采集类目未选择");
            var path = GetStoreImagePath(name);
            path = Path.Combine(path, user);
            if(Directory.Exists(path)) return Directory.GetDirectories(path);
            return null;
        }

        public static string GetStoreImageUserStatusPath(string name, string user, string status)
        {
            if (string.IsNullOrEmpty(name)) throw new Exception("采集类目未选择");
            var path = GetStoreImageUserPath(name, user);
            path = Path.Combine(path, status);
            return path;
        }

        public static string GetStoreConfigPath(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new Exception("采集类目未选择");
            var path = Path.Combine(BaseDirectory, name, "StoreConfig");
            CheckCreateDirectory(path);
            return path;
        }

        public static string GetStoreConfigFile(string name, string config)
        {
            if (string.IsNullOrEmpty(name)) throw new Exception("采集类目未选择");
            return Path.Combine(GetStoreConfigPath(name), $"{config}.txt");
        }

        public static List<string> GetStoreConfigResult(string file)
        {
            if (File.Exists(file))
            {
                return File.ReadAllLines(file).ToList();
            }
            return new List<string>();
        }

        public static List<string> GetStoreConfigResult(string name, string config)
        {
            if (string.IsNullOrEmpty(name)) throw new Exception("采集类目未选择");
            var file = GetStoreConfigFile(name, config);
            return GetStoreConfigResult(file);
        }

        public static void UpdateStoreConfigResult(string name, string config, List<string> removed)
        {
            if (string.IsNullOrEmpty(name)) throw new Exception("采集类目未选择");
            var file = GetStoreConfigFile(name, config);
            var result = GetStoreConfigResult(file);

            var news = result.Where(c => !removed.Contains(c)).ToArray();
            File.WriteAllLines(file, news);
        }

        public static bool CheckUserStatusPicsExists(string name, string user, string status)
        {
            if (string.IsNullOrEmpty(name)) throw new Exception("采集类目未选择");
            string path = PathUtil.GetStoreImageUserStatusPath(name, user, status);
            return Directory.Exists(path) && Directory.GetFiles(path).Where(c => c.EndsWith("jpg")).Count() > 0;
        }

        public static bool CheckUserStatusViedoExists(string name, string user, string status)
        {
            if (string.IsNullOrEmpty(name)) throw new Exception("采集类目未选择");
            var fileName = GetStoreVedioUserPath(name, user, status);
            return File.Exists(fileName);
        }

        public static string GetUserByPath(string path)
        {
            var lst = path.Split(new string[] { "\\" }, StringSplitOptions.None).ToList();
            var index = lst.IndexOf("StoreImage");
            if(index < 0) return null;
            if (index + 1 > lst.Count) return null;

            return lst[index + 1];
        }

        public static string GetStatusByPath(string path)
        {
            var lst = path.Split(new string[] { "\\" }, StringSplitOptions.None).ToList();
            var index = lst.IndexOf("StoreImage");
            if (index < 0) return null;
            if (index + 2 > lst.Count) return null;

            return lst[index + 2];
        }

        public static string GetStoreImageUserStatusPathBySelect(string name, string uid, string bid)
        {
            if (string.IsNullOrEmpty(name)) throw new Exception("采集类目未选择");
            var path = $"{GetStoreImagePath(name)}\\{uid}\\{bid}";
            return path;
        }
        public static string[] GetStoreImageFiles(string name, string uid, string bid)
        {
            if (string.IsNullOrEmpty(name)) throw new Exception("采集类目未选择");
            var path = GetStoreImageUserStatusPathBySelect(name, uid, bid);
            if (!Directory.Exists(path)) return new string[] { };
            return Directory.GetFiles(path).Where(c => c.EndsWith(".jpg")).ToArray();
        }

        public static string[] GetStoreImageFiles(string path)
        {
            return Directory.GetFiles(path).Where(c => c.EndsWith(".jpg")).ToArray();
        }
    }
}
