using LionFrame.Basic.Extensions;
using LionFrame.CoreCommon.CustomResult;
using LionFrame.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using LionFrame.Model.SystemBo;

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
                filterContext.Result = new CustomHttpStatusCodeResult(200, ResponseCode.Unauthorized, ResponseCode.Unauthorized.ToDescription());
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
                if (!LionUser.ValidSessionId(userDic["uid"], userDic["sessionId"]))
                {
                    filterContext.Result = new CustomHttpStatusCodeResult(200, ResponseCode.Unauthorized, "您的账号已在其它地方登录，若非本人操作，请尽快修改密码。");
                    return;
                }
                //当token过期时间小于8小时，更新token并重新返回新的token
                if (date.AddHours(-8) > DateTime.Now) return;
                var nToken = TokenManager.GenerateToken(str);
                Response.Headers["token"] = nToken;
                Response.Headers["Access-Control-Expose-Headers"] = "token";
                return;
            }

            filterContext.Result = new CustomHttpStatusCodeResult(200, ResponseCode.Unauthorized, ResponseCode.Unauthorized.ToDescription());
        }
    }
}
