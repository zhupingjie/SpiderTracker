using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiderTracker.Imp
{
    public class UserUtil
    {
        

        public static Dictionary<string, List<string>> IgnoreUsers { get; set; } = new Dictionary<string, List<string>>();

        public static Dictionary<string, List<string>> FocusUsers { get; set; } = new Dictionary<string, List<string>>();

        public static void UpdateIgnoreUsers(string name, List<string> users)
        {
            var file = PathUtil.GetStoreConfigFile(name, StoreConfigEnum.StoreIgnore);
            var result = PathUtil.GetStoreConfigResult(file);

            var needWriteUserIds = users.Where(c => !result.Contains(c)).ToArray();
            if (needWriteUserIds.Length > 0)
            {
                File.AppendAllLines(file, needWriteUserIds);
            }

            foreach (var user in users)
            {
                var userPath = PathUtil.GetStoreImageUserPath(name, user);
                if (!Directory.Exists(userPath)) continue;
                Directory.Delete(userPath, true);
            }

            var lines = File.ReadAllLines(file).ToList();
            IgnoreUsers.Remove(name);
            IgnoreUsers.Add(name, lines);
        }

        public static void UpdateFocusUsers(string name, List<string> users)
        {
            var file = PathUtil.GetStoreConfigFile(name, StoreConfigEnum.StoreFoucs);
            var result = PathUtil.GetStoreConfigResult(file);

            var needWriteUserIds = users.Where(c => !result.Contains(c)).ToArray();
            if (needWriteUserIds.Length > 0)
            {
                File.AppendAllLines(file, needWriteUserIds);
            }

            var lines = File.ReadAllLines(file).ToList();
            FocusUsers.Remove(name);
            FocusUsers.Add(name, lines);
        }

        public static void ClearAnalyseUsers(string name)
        {
            var file = PathUtil.GetStoreConfigFile(name, StoreConfigEnum.StoreAnalyse);
            if (File.Exists(file)) File.Delete(file);
        }

        public static void UpdateAnalyseUsers(string name, List<string> users)
        {
            var file = PathUtil.GetStoreConfigFile(name, StoreConfigEnum.StoreAnalyse);
            var result = PathUtil.GetStoreConfigResult(file);

            var needWriteUserIds = users.Where(c => !result.Contains(c)).ToArray();
            if (needWriteUserIds.Length > 0)
            {
                File.AppendAllLines(file, needWriteUserIds);
            }
        }

        public static void RemoveAnalyseUsers(string name, List<string> users)
        {
            var file = PathUtil.GetStoreConfigFile(name, StoreConfigEnum.StoreAnalyse);
            var result = PathUtil.GetStoreConfigResult(file);

            var needWriteUserIds = result.Where(c => !users.Contains(c)).ToArray();
            File.WriteAllLines(file, needWriteUserIds);
        }

        public static bool CheckUserIgnore(string name, string user)
        {
            if (!IgnoreUsers.ContainsKey(name))
            {
                var file = PathUtil.GetStoreConfigFile(name, StoreConfigEnum.StoreIgnore);
                if (File.Exists(file))
                {
                    var lines = File.ReadAllLines(file).ToList();
                    IgnoreUsers.Add(name, lines);
                }
            }
            return IgnoreUsers.ContainsKey(name) && IgnoreUsers[name].Contains(user);
        }

        public static bool CheckUserFocus(string name, string user)
        {
            if (!FocusUsers.ContainsKey(name))
            {
                var file = PathUtil.GetStoreConfigFile(name, StoreConfigEnum.StoreFoucs);
                if (File.Exists(file))
                {
                    var lines = File.ReadAllLines(file).ToList();
                    FocusUsers.Add(name, lines);
                }
            }
            return FocusUsers.ContainsKey(name) && FocusUsers[name].Contains(user);
        }
    }
}
