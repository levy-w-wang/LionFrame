using System;
using System.Collections.Generic;
using System.Linq;

namespace LionFrame.Basic.Extensions
{
    /// <summary>
    /// 字典静态扩展
    /// </summary>
    public static class DictionaryExtension
    {
        public static KeyValuePair<TKey, TValue> Index<TKey, TValue>(this IDictionary<TKey, TValue> source, int index)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            if (source.Count == 0 || index > source.Count)
                throw new ArgumentOutOfRangeException("index");
            using (var enumerator = source.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (index == 0)
                        return enumerator.Current;
                    --index;
                }
                throw new ArgumentOutOfRangeException("index");
            }
        }

        public static bool TryGetValue<TValue>(this IDictionary<string, object> source, string key, out TValue value)
        {
            object valueObj;
            if (source.TryGetValue(key, out valueObj) && valueObj is TValue)
            {
                value = (TValue)valueObj;
                return true;
            }

            value = default(TValue);
            return false;
        }

        public static string TryGetValueOrDefault(this IDictionary<string, string> source, string key, string defaultValue = "")
        {
            string value;
            if (source.TryGetValue(key, out value))
            {
                return value;
            }
            return defaultValue;
        }
        public static string TryGetValueOrDefault(this IDictionary<int, string> source, int key, string defaultValue = "")
        {
            string value;
            if (source.TryGetValue(key, out value))
            {
                return value;
            }
            return defaultValue;
        }

        public static string TryGetValueOrDefault(this IDictionary<string, string> source, int key, string defaultValue = "")
        {
            string value;
            if (source.TryGetValue(key.ToString(), out value))
            {
                return value;
            }
            return defaultValue;
        }
        public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key)
        {
            TValue obj;
            return source.TryGetValue(key, out obj) ? obj : default(TValue);
        }

        /// <summary>
        /// 获取Dictionary值
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="source"></param>
        /// <param name="key"></param>
        /// <param name="letterSensitive">指定键是否大小写敏感</param>
        /// <returns></returns>
        public static TValue GetValue<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key, bool letterSensitive = true)
        {
            if (letterSensitive)
            {
                TValue obj;
                return source.TryGetValue(key, out obj) ? obj : default(TValue);
            }

            foreach (var item in source)
            {
                if (item.Key.ToString().ToLower() == key.ToString().ToLower())
                {
                    return item.Value;
                }
            }
            return default(TValue);
        }


        public static bool AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key, TValue value)
        {
            if (source == null)
                return false;

            if (source.ContainsKey(key))
            {
                source[key] = value;
            }
            else
            {
                source.Add(key, value);
            }
            return true;
        }

        public static Dictionary<TKey, TValue> Copy<TKey, TValue>(this IDictionary<TKey, TValue> source)
        {
            if (source == null)
                return new Dictionary<TKey, TValue>();

            return source.ToDictionary(value => value.Key, value => value.Value);
        }

        public static bool AddOrNot<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key, TValue value)
        {
            if (source == null)
                return false;

            if (source.ContainsKey(key))
            {
                return true;
            }
            source.Add(key, value);
            return true;
        }

        public static Dictionary<TKey, TValue> ToDictionaryEx<TElement, TKey, TValue>(this IEnumerable<TElement> source, Func<TElement, TKey> keyGetter, Func<TElement, TValue> valueGetter)
        {
            Dictionary<TKey, TValue> dict = new Dictionary<TKey, TValue>();
            foreach (var e in source)
            {
                var key = keyGetter(e);
                if (dict.ContainsKey(key))
                {
                    continue;
                }

                dict.Add(key, valueGetter(e));
            }

            return dict;
        }

    }
}
