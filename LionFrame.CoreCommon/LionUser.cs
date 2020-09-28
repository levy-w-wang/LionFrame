using LionFrame.Basic;
using LionFrame.CoreCommon.CustomException;
using LionFrame.Model;
using LionFrame.Model.SystemBo;
using System;
using LionFrame.Config;

namespace LionFrame.CoreCommon
{
    /// <summary>
    /// 用户处理类
    /// </summary>
    public class LionUser
    {
        public static UserCacheBo CurrentUser
        {
            get => GetCurrentUser(true);
            set
            {
                var http = LionWeb.HttpContext;
                if (http != null)
                {
                    http.Items["user"] = value;
                }
                LionUserCache.CreateUserCache(value);
            }
        }

        /// <summary>
        /// 获取当前用户
        /// </summary>
        /// <param name="throwException"></param>
        /// <returns></returns>
        private static UserCacheBo GetCurrentUser(bool throwException = false)
        {
            try
            {
                var http = LionWeb.HttpContext;
                if (http == null)
                    throw new CustomSystemException("未授权，请重新登录", ResponseCode.Unauthorized);
                if (http.Items["user"] is UserCacheBo user)
                    return user;
                var cache = LionUserCache.Get(); //先从内存获取，再从redis获取
                if (cache != null)
                {
                    http.Items["user"] = cache;
#if DEBUG
                    LogHelper.Logger.Debug("[LoginName]:{0}\n \n [UserId]:{1}\n [UserToken]:{2}", cache.UserName, cache.UserId, cache.UserToken);
#endif
                    return cache;
                }
            }
            catch (Exception)
            {
                if (throwException)
                    throw new CustomSystemException("未授权，请重新登录", ResponseCode.Unauthorized);
            }

            return null;
        }

        /// <summary>
        /// token 验证正确性 单点登录
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        internal static SysConstants.TokenValidType ValidSessionId(string uid,string sessionId)
        {
            var cache = LionUserCache.Get(uid); //先从内存获取，再从redis获取
            if (cache == null)
            {
                return SysConstants.TokenValidType.LogonInvalid;
            }
            if (cache.SessionId == sessionId)
            {
                LionWeb.HttpContext.Items["user"] = cache;
#if DEBUG
                LogHelper.Logger.Debug("[LoginName]:{0}\n \n [UserId]:{1}\n [UserToken]:{2}", cache.UserName, cache.UserId, cache.UserToken);
#endif
                return SysConstants.TokenValidType.Success;
            }
            return SysConstants.TokenValidType.LoggedInOtherPlaces;
        }
    }
}
