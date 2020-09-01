using System;
using System.Collections.Generic;
using System.Linq;

namespace LionFrame.Basic.Extensions
{
    /// <summary>
    /// 集合静态扩展
    /// </summary>
    public static class CollectionExtensions
    {

        public static bool IsNullOrEmpty<T>(this ICollection<T> source)
        {
            return source == null || source.Count <= 0;
        }

        /// <summary>
        /// 不存在就添加
        /// </summary>
        /// <param name="source"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public static bool AddIfNotContains<T>(this ICollection<T> source, T item)
        {
            if (source == null)
                return true;

            if (source.Contains(item))
                return false;

            source.Add(item);
            return true;
        }

        /// <summary>
        /// 如果集合不为null且不存在集合中，则将添加该项
        /// </summary>
        public static bool AddSteady<T>(this ICollection<T> source, T item)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (source.Contains(item))
                return false;

            source.Add(item);
            return true;
        }

        /// <summary>
        /// 如果集合不为null且不存在集合中，则将添加该项
        /// </summary>
        public static bool AddSteady(this ICollection<string> source, string item)
        {
            if (source == null)
                return true;

            if (source.Contains(item))
                return false;

            source.Add(item);
            return true;
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> source)
        {
            return source == null || !source.Any();
        }

        public static bool TryRemove<T>(this ICollection<T> source, Func<T, bool> predicate)
        {
            if (source == null || predicate == null)
                return false;
            var data = default(T);
            foreach (var item in source)
            {
                if (predicate(item))
                {
                    data = item;
                    break;
                }
            }
            if (data != null)
            {
                source.Remove(data);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 去除重复项 选择对应列
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }
    }
}
