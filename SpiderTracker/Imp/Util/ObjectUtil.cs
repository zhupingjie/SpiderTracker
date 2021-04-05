using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiderTracker.Imp.Util
{
    public class ObjectUtil
    {
        public static DateTime? GetCreateTime(string create)
        {
            //Sun Oct 07 20:10:48 +0800 2018
            var mons = new string[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sept", "Oct", "Nov", "Dec" };
            var weeks = new string[] { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" };

            if (string.IsNullOrEmpty(create)) return null;

            var arrs = create.Split(new string[] { " " }, StringSplitOptions.None);
            if (arrs.Length != 6) return null;

            if (!mons.Contains(arrs[1])) return null;

            var strTime = $"{arrs[5]}-{Array.IndexOf(mons, arrs[1]) + 1}-{arrs[2]} {arrs[3]}";
            DateTime time = DateTime.Now;
            if (!DateTime.TryParse(strTime, out time)) return null;

            return time;
        }
    }
}
