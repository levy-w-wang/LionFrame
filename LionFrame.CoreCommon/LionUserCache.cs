using Autofac;
using LionFrame.Basic;
using LionFrame.Config;
using LionFrame.CoreCommon.Cache;
using LionFrame.CoreCommon.Cache.Redis;
using LionFrame.Model.SystemBo;
using System;
using LionFrame.Basic.Extensions;
using LionFrame.CoreCommon.CustomException;
using LionFrame.Model;

namespace LionFrame.CoreCommon
{
    /// <summary>
    /// 用户信息缓存类
    /// </summary>
    public class LionUserCache
    {
        private static readonly LionMemoryCache Cache = LionWeb.AutofacContainer.Resolve<LionMemoryCache>();

        private static TimeSpan _timeout = TimeSpan.Zero;
        private static TimeSpan _redisTimeout = TimeSpan.Zero;

        /// <summary>
        /// 内存默认2小时过期
        /// </summary>
        private static TimeSpan Timeout
        {
            get
            {
                if (_timeout != TimeSpan.Zero)
                    return _timeout;
                try
                {
                    // 内存只存储2小时，修改密码重新登录时将上一个替换掉，redis中的给删除掉，重新设置
                    _timeout = TimeSpan.FromHours(2);
                    return _timeout;
                }
                catch (Exception)
                {
                    return TimeSpan.FromMinutes(60);
                }
            }
        }

        /// <summary>
        /// redis 默认3天过期
        /// </summary>
        private static TimeSpan RedisTimeout
        {
            get
            {
                if (_redisTimeout != TimeSpan.Zero)
                    return _redisTimeout;
                try
                {
                    // 通常情况下，登录三天有效 -- 结合前端滑动更新  当使用时间小于8小时时，就更新redis过期时间，或重新颁发
                    // 修改密码或重新登录 将使该用户上一个token过期
                    _redisTimeout = TimeSpan.FromDays(3);
                    return _redisTimeout;
                }
                catch (Exception)
                {
                    return TimeSpan.FromHours(12);
                }
            }
        }

        private static string FormatPrefixKey(string uid)
        {
            return $"{CacheKeys.USER}{uid}";
        }

        /// <summary>
        /// 创建缓存
        /// </summary>
        /// <param name="userCache"></param>
        public static void CreateUserCache(UserCacheBo userCache)
        {
            // 直接以用户id作为缓存key -- 单点登录
            // 若想一个账号多点登录的，可以换一种方式-如guid等
            var sid = userCache.UserId.ToString();
            var key = FormatPrefixKey(sid);

            userCache.LoginIp = LionWeb.GetClientIp();
            userCache.LoginTime = DateTime.Now;

            var redisClient = LionWeb.AutofacContainer.Resolve<RedisClient>();
            redisClient.Set(key, userCache, RedisTimeout);
            AddToMemoryCache(key, userCache);
            LionWeb.HttpContext.Items["user"] = userCache;

            //UserCacheBo oldCache = Get(sid,false);
            //UserCacheBo cache = oldCache;

            ////如果是同一用户,仅更新时间 单点登录
            //if (oldCache?.LoginIp != userCache.LoginIp || oldCache?.UserToken != userCache.UserToken || oldCache?.PassWord != userCache.PassWord)
            //{
            //    DeleteCache(sid);
            //    oldCache = null;
            //}
            //if (oldCache == null)
            //{
            //    cache = userCache;
            //    var redisClient = LionWeb.AutofacContainer.Resolve<RedisClient>();
            //    redisClient.Set(key, cache, RedisTimeout);
            //    AddToMemoryCache(key, cache);
            //}
        }

        /// <summary>
        /// 获取request中的uid
        /// </summary>
        /// <returns></returns>
        public static string GetUidFromClient()
        {
            var http = LionWeb.HttpContext;
            if (http == null)
                return string.Empty;
            // 在是否登录验证时,设置到Items中
            string uid = http.Items["uid"].ToString();
            return uid;
        }

        /// <summary>
        /// 获取当前用户缓存
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="getFromLocal">从item和memorycache 中取出来的是一样的，可能是因为引用类型 指向同一地址</param>
        /// <returns></returns>
        public static UserCacheBo Get(string uid = "",bool getFromLocal = true)
        {
            var http = LionWeb.HttpContext;
            if (http == null)
                throw new CustomSystemException("未授权，请重新登录", ResponseCode.Unauthorized);
            if (http.Items["user"] is UserCacheBo user && getFromLocal)
                return user;
            uid = uid.IsNullOrEmpty() ? GetUidFromClient() : uid;
            if (uid.IsNullOrEmpty())
                return null;
            var key = FormatPrefixKey(uid);

            if (getFromLocal)
            {
                //从本地缓存中读取cache
                var cacheCache = Cache.Get<UserCacheBo>(key);
                if (cacheCache != null)
                {
                    return cacheCache;
                }
            }

            // 本地缓存中不存在，则从redis中获取，然后存回本地缓存
            try
            {
                var redisClient = LionWeb.AutofacContainer.Resolve<RedisClient>();
                var cache = redisClient.Get<UserCacheBo>(key);
                if (cache != null)
                {
                    AddToMemoryCache(key, cache);
                    return cache;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Logger.Fatal(ex, "Get \n key:{0}", key);
                return null;
            }
            return null;
        }

        /// <summary>
        /// 添加到本地缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="cache"></param>
        private static void AddToMemoryCache(string key, UserCacheBo cache)
        {
            Cache.Set(key, cache, Timeout);
        }

        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="sid"></param>
        public static void DeleteCache(string sid = "")
        {
            if (string.IsNullOrEmpty(sid))
                sid = GetUidFromClient();
            if (string.IsNullOrEmpty(sid))
                return;
            var key = FormatPrefixKey(sid);
            var redisClient = LionWeb.AutofacContainer.Resolve<RedisClient>();

            redisClient.Delete(key);
            Cache.Remove(key);
        }
    }
}
