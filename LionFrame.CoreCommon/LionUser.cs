using LionFrame.Basic;
using LionFrame.Config;
using LionFrame.CoreCommon.CustomException;
using LionFrame.Model;
using LionFrame.Model.SystemBo;
using System;
using System.Collections.Generic;
using LionFrame.Basic.Extensions;
using LionFrame.CoreCommon.CustomResult;
using Microsoft.AspNetCore.Mvc.Filters;

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
                var cache = LionUserCache.Get(); //先从内存获取，再从redis获取
                if (cache != null)
                {
                    http.Items["user"] = cache;
#if DEBUG
                    LogHelper.Logger.Debug("[LoginName]:{0}\n \n [UserId]:{1}\n [UserToken]:{2}", cache.NickName, cache.UserId, cache.UserToken);
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
        internal static SysConstants.TokenValidType ValidSessionId(string uid, string sessionId)
        {
            var cache = LionUserCache.Get(uid); //先从内存获取，再从redis获取
            if (cache == null)
            {
                return SysConstants.TokenValidType.LogonInvalid;
            }
            if (cache.SessionId == sessionId)
            {
#if DEBUG
                LogHelper.Logger.Debug("[LoginName]:{0}\n \n [UserId]:{1}\n [UserToken]:{2}", cache.NickName, cache.UserId, cache.UserToken);
#endif
                return SysConstants.TokenValidType.Success;
            }
            return SysConstants.TokenValidType.LoggedInOtherPlaces;
        }

        /// <summary>
        /// 验证token
        /// </summary>
        /// <returns></returns>
        public static SysConstants.TokenValidType TokenLogin()
        {
            var token = LionWeb.HttpContext.Request.Headers["token"];
            if (string.IsNullOrEmpty(token))
            {
                return SysConstants.TokenValidType.LogonInvalid;
            }

            // 验证登录
            var str = TokenManager.ValidateToken(token, out DateTime date);
            if (!string.IsNullOrEmpty(str) || date > DateTime.Now)
            {
                var userDic = str.ToObject<Dictionary<string, string>>();
                LionWeb.HttpContext.Items["uid"] = userDic["uid"];
                LionWeb.HttpContext.Items["sessionId"] = userDic["sessionId"];
                // 单点登录验证
                var validResult = LionUser.ValidSessionId(userDic["uid"], userDic["sessionId"]);

                if (validResult != SysConstants.TokenValidType.Success)
                {
                    return validResult;
                }
                //当token过期时间小于8小时，更新token并重新返回新的token
                if (date.AddHours(-8) > DateTime.Now) return validResult;
                #region 滑动刷新Token

                var newSessionId = Guid.NewGuid().ToString("N");
                userDic["sessionId"] = newSessionId;
                var nToken = TokenManager.GenerateToken(userDic.ToJson());
                CurrentUser.SessionId = newSessionId;
                CurrentUser.UserToken = nToken;
                LionUser.CurrentUser = CurrentUser;
                LionWeb.HttpContext.Response.Headers["token"] = nToken;
                LionWeb.HttpContext.Response.Headers["Access-Control-Expose-Headers"] = "token";
                return validResult;

                #endregion
            }

            return SysConstants.TokenValidType.LogonInvalid;
        }
    }
}
