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
    /// 权限验证 -- 判断当前操作用户是否包含设定的角色id
    /// </summary>
    public class PopedomFilterAttribute : Attribute, IActionFilter
    {
        public PopedomFilterAttribute()
        {
        }

        public List<long> Popedoms { get; set; } 

        /// <summary>
        /// 包含其中一个权限就通过
        /// </summary>
        public bool IsAny { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="popedom">权限组  以英文字符分割</param>
        /// <param name="isAny">是否包含一个就通过</param>
        public PopedomFilterAttribute(bool isAny, params long[] popedom)
        {
            Popedoms = popedom.ToList();
            IsAny = isAny;
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

            if (Popedoms == null || Popedoms.Count == 0)
                return;

            if (IsAny)
            {
                var popedomList = Popedoms.Intersect(user.RoleIdList).ToList();
                if (popedomList.Any())
                    return;
                context.Result = new CustomHttpStatusCodeResult(200, ResponseCode.Unauthorized1, "未授权");
            }
            else
            {
                foreach (var item in Popedoms.Where(item => !user.RoleIdList.Contains(item)))
                {
                    context.Result = new CustomHttpStatusCodeResult(200, ResponseCode.Unauthorized1, "未授权");
                }

                return;
            }

            context.Result = new CustomHttpStatusCodeResult(200, ResponseCode.Unauthorized1, "未授权");
        }
    }
}
