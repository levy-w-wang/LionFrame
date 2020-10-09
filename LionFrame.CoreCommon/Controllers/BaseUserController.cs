using LionFrame.Basic.Extensions;
using LionFrame.Config;
using LionFrame.CoreCommon.CustomResult;
using LionFrame.Model;
using LionFrame.Model.SystemBo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LionFrame.CoreCommon.Controllers
{
    /// <summary>
    /// 登录验证控制器
    /// </summary>
    public abstract class BaseUserController : BaseController
    {
        private UserCacheBo _user;

        /// <summary>
        /// 当前用户
        /// </summary>
        protected UserCacheBo CurrentUser
        {
            get => _user ?? (_user = LionUser.CurrentUser);
            set
            {
                LionUser.CurrentUser = value;
                _user = value;
            }
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            if (filterContext.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
            {
                var isDefined = controllerActionDescriptor.MethodInfo.GetCustomAttributes(true)
                    .Any(a => a.GetType() == typeof(AllowAnonymousAttribute));
                if (isDefined)
                {
                    return;
                }
            }

            var token = Request.Headers["token"];
            if (string.IsNullOrEmpty(token))
            {
                filterContext.Result = new CustomHttpStatusCodeResult(200, ResponseCode.Unauthorized, ResponseCode.Unauthorized.GetDescription());
                return;
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
                switch (validResult)
                {
                    case SysConstants.TokenValidType.LogonInvalid:
                        filterContext.Result = new CustomHttpStatusCodeResult(200, ResponseCode.Unauthorized, validResult.GetDescription());
                        return;
                    case SysConstants.TokenValidType.LoggedInOtherPlaces:
                        filterContext.Result = new CustomHttpStatusCodeResult(200, ResponseCode.Unauthorized, validResult.GetDescription());
                        return;
                }
                //当token过期时间小于8小时，更新token并重新返回新的token
                if (date.AddHours(-8) > DateTime.Now) return;
                var nToken = TokenManager.GenerateToken(str);
                Response.Headers["token"] = nToken;
                Response.Headers["Access-Control-Expose-Headers"] = "token";
                return;
            }

            filterContext.Result = new CustomHttpStatusCodeResult(200, ResponseCode.Unauthorized, ResponseCode.Unauthorized.GetDescription());
        }
    }
}
