using System;
using LionFrame.Basic;
using LionFrame.CoreCommon.CustomException;
using LionFrame.CoreCommon.CustomResult;
using LionFrame.Model;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LionFrame.CoreCommon.CustomFilter
{
    /// <inheritdoc />
    /// <summary>
    /// 全局异常处理过滤器
    /// </summary>
    public class ExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.ExceptionHandled)
                return;

            var controllerName = (string)filterContext.RouteData.Values["controller"];
            var actionName = (string)filterContext.RouteData.Values["action"];
            var request = filterContext.HttpContext.Request;

            LogHelper.Logger.Fatal(filterContext.Exception,
                $"【异常信息】：{filterContext.Exception.Message}  【请求路径】：{request.Method}:{request.Path}\n " +
                $"【Controller】:{controllerName} - 【Action】:{actionName}\n " +
                $"【UserHostAddress】:{ LionWeb.GetClientIp()} " +
                $"【UserAgent】:{ request.Headers["User-Agent"]}");

            if (filterContext.Exception is CustomSystemException se)
            {
                filterContext.Result = new CustomHttpStatusCodeResult(200, se.Code, se.Message);
            }
            else if (filterContext.Exception is DataValidException de)
            {
                filterContext.Result = new CustomHttpStatusCodeResult(200, de.Code, de.Message);
            }
            else
            {
                var content = "";
#if DEBUG
                Console.WriteLine(filterContext.Exception);
                content = filterContext.Exception.Message;
#else
                content = "系统错误，请稍后再试或联系管理人员。";
#endif
                filterContext.Result = new CustomHttpStatusCodeResult(200, ResponseCode.UnknownEx, content);
            }
            filterContext.ExceptionHandled = true;
            // 处理完异常之后，请记得将此属性更改为true,表明已经处理过了。将不会
            // 处理范围仅限于MVC中间件，若要捕捉MVC中间件之前的异常，
            // 请使用ExceptionHandlerMiddleware中间件来处理，因为在管道中放置在第一个位置
        }
    }
}
