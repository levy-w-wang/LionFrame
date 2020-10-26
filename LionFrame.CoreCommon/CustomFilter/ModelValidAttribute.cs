using LionFrame.Basic.Extensions;
using LionFrame.CoreCommon.Controllers;
using LionFrame.Model;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;

namespace LionFrame.CoreCommon.CustomFilter
{
    /// <inheritdoc />
    /// <summary>
    /// 模型数据验证
    /// </summary>
    public class ModelValidAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.ModelState.ErrorCount == 0) //filterContext.ModelState.IsValid
                return;
            var errMsg = new List<string>();
            foreach (var modelStateKey in filterContext.ModelState.Keys)
            {
                //decide if you want to show it or not...
                var value = filterContext.ModelState[modelStateKey];
                foreach (var error in value.Errors)
                {
                    if (!error.ErrorMessage.IsNullOrEmpty())
                    {
                        errMsg.Add(error.ErrorMessage);
                    }
                }
            }
            if (filterContext.Controller is BaseController controller)
                filterContext.Result = controller.Fail(ResponseCode.RequestDataVerifyFail, $"{string.Join(",", errMsg)}");
            else
                throw new Exception("默认所有Controller都需要继承BaseController，以实现模型的验证，错误的提醒，若有特殊情况，再讨论！");
        }
    }
}
