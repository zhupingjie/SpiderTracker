using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SpiderCore.Util
{
    public class ObjectUtil
    {
        public static string StampToDateTime(string timeStamp)
        {
            DateTime dateTimeStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(timeStamp + "0000000");
            TimeSpan toNow = new TimeSpan(lTime);

            return dateTimeStart.Add(toNow).ToString("yyyy/MM/dd HH:mm");
        }

        public static string GetCreateTimeString(string create)
        {
            var time = GetCreateTime(create);
            if (!time.HasValue) return null;
            return time.Value.ToString("yyyy/MM/dd HH:mm");
        }

        public static DateTime? GetCreateTime(string create)
        {
            //Sun Oct 07 20:10:48 +0800 2018
            var mons = new string[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
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


        public static void SetPropertyValue(object obj, PropertyInfo propertyInfo, object propertyValue)
        {
            if (propertyInfo.CanWrite)
            {
                propertyInfo.SetValue(obj, propertyValue);
            }
        }
        public static object GetPropertyValue(object obj, PropertyInfo propertyInfo)
        {
            return propertyInfo.GetValue(obj);
        }
        public static T ToValue<T>(object value, T defaultValue)
        {
            if (value == null) return defaultValue;
            var converter = TypeDescriptor.GetConverter(typeof(T));
            return (T)converter.ConvertTo(value, typeof(T));
        }
    }
}
