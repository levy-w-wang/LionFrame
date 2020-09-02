using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;

namespace LionFrame.CoreCommon.Cache
{
    /// <summary>
    /// MemoryCache缓存
    /// </summary>
    public class MemoryCache
    {
        private static readonly HashSet<string> Keys = new HashSet<string>();

        /// <summary>
        /// 缓存前缀
        /// </summary>
        public string Prefix { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="prefix"></param>
        public MemoryCache(string prefix)
        {
            Prefix = prefix + "_";
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T Get<T>(string key)
        {
            return LionWeb.MemoryCache.Get<T>(Prefix + key);
        }

        /// <summary>
        /// 设置 无过期时间
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        public void Set<T>(string key, T data)
        {
            key = Prefix + key;
            LionWeb.MemoryCache.Set(key, data);
            if (!Keys.Contains(key))
            {
                Keys.Add(key);
            }
        }

        /// <summary>
        /// 设置
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <param name="absoluteExpiration"></param>
        public void Set<T>(string key, T data, DateTimeOffset absoluteExpiration)
        {
            key = Prefix + key;
            LionWeb.MemoryCache.Set(key, data, absoluteExpiration);
            if (!Keys.Contains(key))
            {
                Keys.Add(key);
            }
        }

        /// <summary>
        /// 设置
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <param name="absoluteExpirationRelativeToNow"></param>
        public void Set<T>(string key, T data, TimeSpan absoluteExpirationRelativeToNow)
        {
            key = Prefix + key;
            LionWeb.MemoryCache.Set(key, data, absoluteExpirationRelativeToNow);
            if (!Keys.Contains(key))
            {
                Keys.Add(key);
            }
        }

        /// <summary>
        /// 设置
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <param name="expirationToken"></param>
        public void Set<T>(string key, T data, IChangeToken expirationToken)
        {
            key = Prefix + key;
            LionWeb.MemoryCache.Set(key, data, expirationToken);
            if (!Keys.Contains(key))
            {
                Keys.Add(key);
            }
        }

        /// <summary>
        /// 设置
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <param name="options"></param>
        public void Set<T>(string key, T data, MemoryCacheEntryOptions options)
        {
            key = Prefix + key;
            LionWeb.MemoryCache.Set(key, data, options);
            if (!Keys.Contains(key))
            {
                Keys.Add(key);
            }
        }

        /// <summary>
        /// 移除某个
        /// </summary>
        /// <param name="key"></param>
        public void Remove(string key)
        {
            key = Prefix + key;
            LionWeb.MemoryCache.Remove(key);
            if (Keys.Contains(key))
            {
                Keys.Remove(key);
            }
        }

        /// <summary>
        /// 清空所有
        /// </summary>
        public void ClearAll()
        {
            foreach (var key in Keys)
            {
                LionWeb.MemoryCache.Remove(key);
            }
            Keys.Clear();
        }

    }
}
