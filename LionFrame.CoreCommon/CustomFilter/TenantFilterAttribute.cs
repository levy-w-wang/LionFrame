using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LionFrame.CoreCommon.CustomException;
using LionFrame.CoreCommon.CustomResult;
using LionFrame.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LionFrame.CoreCommon.CustomFilter
{
    /// <summary>
    /// 权限验证 -- 判断当前操作用户 租户是否有操作权限
    /// </summary>
    public class TenantFilterAttribute : Attribute, IActionFilter
    {
        public TenantFilterAttribute()
        {
        }

        public List<long> TenantIds { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="tenant">权限组  以英文字符分割</param>
        public TenantFilterAttribute(params long[] tenant)
        {
            TenantIds = tenant.ToList();
        }
        /// <summary>
        /// Called after the action executes, before the action result.
        /// </summary>
        /// <param name="context">The <see cref="T:Microsoft.AspNetCore.Mvc.Filters.ActionExecutedContext" />.</param>
        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        /// <summary>
        /// Called before the action executes, after model binding is complete.
        /// </summary>
        /// <param name="context">The <see cref="T:Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext" />.</param>
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!(context.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor))
            {
                return;
            }

            var isDefined = controllerActionDescriptor.MethodInfo.IsDefined(typeof(AllowAnonymousAttribute), true);
            if (isDefined)
            {
                return;
            }

            // ActionFilter在AuthorizationFilter之后执行，只要是有登录验证的，都能取到user
            var user = LionUser.CurrentUser;
            if (user == null)
            {
                throw new CustomSystemException("请先对需权限验证的方法加上登录验证", ResponseCode.Unauthorized);
            }

            if (TenantIds == null || TenantIds.Count == 0)
                return;

            if (TenantIds.Contains(user.TenantId))
            {
                return;
            }

            context.Result = new CustomHttpStatusCodeResult(200, ResponseCode.Unauthorized1, "未授权");
            return;
        }
    }
}
