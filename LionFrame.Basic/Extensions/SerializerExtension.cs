using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;

namespace LionFrame.Basic.Extensions
{
    /// <summary>
    /// 序列化静态扩展
    /// </summary>
    public static class SerializerExtension
    {
        /// <summary>
        /// 返回Json字符串
        /// </summary>
        /// <param name="obj">需要序列化的对象  例:T.ToJson()</param>
        /// <param name="isNullValueHandling">是否忽略Null字段，最终字符串中是否包含Null字段</param>
        /// <param name="indented">是否展示为具有Json格式的字符串</param>
        /// <param name="isLowCase">是否忽略大小写</param>
        /// <param name="dateTimeFormat">时间转换格式</param>
        /// <returns>Json字符串</returns>
        public static string ToJson(this object obj, bool isNullValueHandling = false, bool indented = false, bool isLowCase = false, string dateTimeFormat = "yyyy-MM-dd HH:mm:ss")
        {
            var options = new JsonSerializerSettings();

            if (indented)
                options.Formatting = Formatting.Indented;
            if (isLowCase)
            {
                options.ContractResolver = new CamelCasePropertyNamesContractResolver();
            }
            if (isNullValueHandling)
                options.NullValueHandling = NullValueHandling.Ignore;
            options.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            options.Converters = new List<JsonConverter> { new IsoDateTimeConverter { DateTimeFormat = dateTimeFormat } };
            return obj.ToJson(options);
        }

        /// <summary>
        /// Json字符串
        /// </summary>
        /// <param name="obj">需要序列化的对象  例:T.ToJson(settings)</param>
        /// <param name="settings">Json序列化设置</param>
        /// <returns>Json字符串</returns>
        public static string ToJson(this object obj, JsonSerializerSettings settings)
        {
            return JsonConvert.SerializeObject(obj, settings);
        }

        /// <summary>
        /// 转换成对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str">序列字符串</param>
        /// <param name="isIgnoreNull">是否忽略空对象</param>
        /// <param name="isIgnoreEx">是否忽略序列异常</param>
        /// <returns></returns>
        public static T ToObject<T>(this string str, bool isIgnoreNull = true, bool isIgnoreEx = false)
        {
            var setting = new JsonSerializerSettings
            {
                NullValueHandling = isIgnoreNull ? NullValueHandling.Ignore : NullValueHandling.Include
            };
            try
            {
                if (string.IsNullOrEmpty(str))
                {
                    return default(T);
                }
                else if ("\"\"" == str)
                {
                    return default(T);
                }
                else
                {
                    return JsonConvert.DeserializeObject<T>(str, setting);
                }
            }
            catch (Exception)
            {
                if (!isIgnoreEx)
                    throw;
                return default(T);
            }
        }

        /// <summary>
        /// 转换成对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static T ToObject<T>(this string str, JsonSerializerSettings settings)
        {
            try
            {
                if (string.IsNullOrEmpty(str))
                {
                    return default(T);
                }
                else if ("\"\"" == str)
                {
                    return default(T);
                }
                else
                {
                    return JsonConvert.DeserializeObject<T>(str, settings);
                }
            }
            catch (Exception)
            {

                return default(T);
            }
        }
    }
}
