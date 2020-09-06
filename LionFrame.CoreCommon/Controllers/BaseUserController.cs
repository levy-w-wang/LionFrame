using LionFrame.Basic.Extensions;
using LionFrame.CoreCommon.CustomResult;
using LionFrame.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;

namespace LionFrame.CoreCommon.Controllers
{
    /// <summary>
    /// 登录验证控制器
    /// </summary>
    public abstract class BaseUserController : BaseController
    {
        //        private UserModel _user;
        //        /// <summary>
        //        /// 当前用户
        //        /// </summary>
        //        protected new UserModel User
        //        {
        //            get => _user ?? (_user = _userCache.Current);//从缓存中取
        //            set => _user = value;
        //        }

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
            var str = TokenManager.ValidateToken(token, out DateTime date);
            if (!string.IsNullOrEmpty(str) || date > DateTime.Now)
            {
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
