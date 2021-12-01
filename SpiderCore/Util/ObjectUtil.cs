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
        public static PropertyInfo[] GetPropertyInfos(object obj)
        {
            if (obj == null) return null;
            return obj.GetType().GetProperties();
        }

        public static PropertyInfo GetPropertyInfo(object obj, string propertyName)
        {
            if (obj == null) return null;
            return obj.GetType().GetProperties().FirstOrDefault(c => c.Name.ToUpper() == propertyName.ToUpper());
        }
        public static void SetPropertyValue(object obj, string propertyName, object propertyValue)
        {
            var propertyInfo = GetPropertyInfo(obj, propertyName);
            if (propertyInfo != null && propertyInfo.CanWrite)
            {
                propertyInfo.SetValue(obj, propertyValue);
            }
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
        public static object ToValue(object value, Type type)
        {
            try
            {
                if (value == null) return null;
                var val = $"{value}".Trim();

                if (value == null && type.IsGenericType) return Activator.CreateInstance(type);
                if (value == null) return null;
                if (type == value.GetType()) return value;
                if (type.IsEnum)
                {
                    if (value is string)
                        return Enum.Parse(type, value as string);
                    else
                        return Enum.ToObject(type, value);
                }
                if (!type.IsInterface && type.IsGenericType)
                {
                    Type innerType = type.GetGenericArguments()[0];
                    object innerValue = ToValue(value, innerType);
                    return Activator.CreateInstance(type, new object[] { innerValue });
                }
                if (value is string && type == typeof(Guid)) return new Guid(value as string);
                if (value is string && type == typeof(Version)) return new Version(value as string);
                if (!(value is IConvertible)) return value;
                return Convert.ChangeType(value, type);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static Dictionary<string, object> GetPropertyValues(object obj, bool setKeyWithDesc = false)
        {
            var result = new Dictionary<string, object>();
            var propertyInfos = obj.GetType().GetProperties();
            foreach (var propertyInfo in propertyInfos)
            {
                if (propertyInfo.GetMethod.IsStatic) continue;

                var key = propertyInfo.Name;
                if (setKeyWithDesc)
                {
                    var descAttrs = propertyInfo.GetCustomAttributes(typeof(DescriptionAttribute), true);
                    if (descAttrs.Count() > 0)
                    {
                        key = (descAttrs.FirstOrDefault() as DescriptionAttribute).Description;
                    }
                }

                if (!result.ContainsKey(key))
                {
                    var value = propertyInfo.GetValue(obj);
                    if (propertyInfo.PropertyType.IsEnum || (IsNullableType(propertyInfo.PropertyType) && GetNullableType(propertyInfo.PropertyType).IsEnum))
                    {
                        result.Add(key, (int)value);
                    }
                    else
                    {
                        result.Add(key, value);
                    }
                }
            }
            return result;
        }

        public static bool IsNullableType(Type theType)
        {
            return Nullable.GetUnderlyingType(theType) != null;
        }

        public static Type GetNullableType(Type theType)
        {
            return Nullable.GetUnderlyingType(theType);
        }

    }
}
