using LionFrame.Basic;
using LionFrame.Basic.Extensions;
using LionFrame.CoreCommon.CustomException;
using LionFrame.Model;
using LionFrame.Model.ResponseDto.ResultModel;
using Microsoft.AspNetCore.Http;
using System;
using System.Text;
using System.Threading.Tasks;

namespace LionFrame.CoreCommon.CustomMiddler
{
    /// <summary>
    /// 自定义异常处理中间件
    /// </summary>
    public class CustomExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        public CustomExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                LogHelper.Logger.Fatal(ex,
            $"【异常信息】：{ex.Message} 【请求路径】：{httpContext.Request.Method}:{httpContext.Request.Path}\n " +
                $"【UserHostAddress】:{ LionWeb.GetClientIp()} " +
                $"【UserAgent】:{ httpContext.Request.Headers["User-Agent"]}");

                if (ex is CustomSystemException se)
                {
                    await ExceptionResult(httpContext, new ResponseModel().Fail(se.Code, se.Message, "").ToJson(true, isLowCase: true));
                }
                else if (ex is DataValidException de)
                {
                    await ExceptionResult(httpContext, new ResponseModel().Fail(de.Code, de.Message, "").ToJson(true, isLowCase: true));
                }
                else
                {
#if DEBUG
                    Console.WriteLine(ex);
                    var content = ex.ToJson();
#else
                    var content = "系统错误，请稍后再试或联系管理人员。";
#endif
                    await ExceptionResult(httpContext, new ResponseModel().Fail(ResponseCode.UnknownEx, content, "").ToJson(true, isLowCase: true));
                }
            }
        }

        public async Task ExceptionResult(HttpContext httpContext, string data)
        {
            httpContext.Response.StatusCode = 200;
            if (string.IsNullOrEmpty(data))
                return;
            httpContext.Response.ContentType = "application/json;charset=utf-8";
            var bytes = Encoding.UTF8.GetBytes(data);

            await httpContext.Response.Body.WriteAsync(bytes, 0, bytes.Length);
        }

    }
}
