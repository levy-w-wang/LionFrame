using System.Diagnostics;
using System.Text;
using LionFrame.Basic;
using LionFrame.Basic.Extensions;
using LionFrame.CoreCommon.CustomFilter;
using LionFrame.CoreCommon.CustomResult;
using LionFrame.Model;
using LionFrame.Model.ResponseDto.ResultModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LionFrame.CoreCommon.Controllers
{
    /// <summary>
    /// 基控制器
    /// </summary>
    [ModelValid, ApiController]
    public abstract class BaseController : Controller
    {
        private Stopwatch _stopWatch;

        /// <summary>
        /// 从 Request.Body 中获取数据并JSON序列化成对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected T GetJsonParams<T>()
        {
            if (Request.ContentLength != null)
            {
                var bytes = new byte[(int)Request.ContentLength];
                Request.Body.Read(bytes, 0, bytes.Length);
                var json = Encoding.UTF8.GetString(bytes);
                return json.ToObject<T>();
            }

            return default(T);
        }

        /// <summary>
        /// 返回Json数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected ActionResult MyJson(BaseResponseModel data)
        {
            return new CustomJsonResult
            {
                Data = data,
                DateTimeFormat = "yyyy-MM-dd HH:mm:ss"
            };
        }

        /// <summary>
        /// 返回成功
        /// Json格式
        /// </summary>
        /// <returns></returns>
        protected ActionResult Succeed()
        {
            return Succeed(true);
        }

        /// <summary>
        /// 返回成功
        /// Json格式
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected ActionResult Succeed<T>(T data)
        {
            return MyJson(new ResponseModel().Succeed(data));
        }
        /// <summary>
        /// 无错误返回数据
        /// </summary>
        /// <param name="code"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        internal ActionResult Fail(string message, ResponseCode code = ResponseCode.Fail)
        {
            return MyJson(new ResponseModel().Fail(code, message, null));
        }

        /// <summary>
        /// 带错误返回数据
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        protected ActionResult Fail(string message)
        {
            return MyJson(new ResponseModel<string>().Fail(ResponseCode.Fail, message, message));
        }
        /// <summary>
        /// 带错误返回数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="code"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        protected ActionResult Fail<T>(T data, string message, ResponseCode code = ResponseCode.Fail)
        {
            return MyJson(new ResponseModel().Fail(code, message, data));
        }
        /// <summary>
        /// 请求开始前的处理
        /// </summary>
        /// <param name="context"></param>
        [ApiExplorerSettings(IgnoreApi = true)]
        public override void OnActionExecuting(ActionExecutingContext context)
        {
#if DEBUG
            LogHelper.Logger.Debug(
                "[Ip]:{0}\n [Arguments]:{1}\n [Headers]:{2}\n ",
                LionWeb.GetClientIp(),
                context.ActionArguments.ToJson(true),
               context.HttpContext.Request.Headers.ToJson(true)
            );

            _stopWatch = new Stopwatch();
            _stopWatch.Start();
#endif
            base.OnActionExecuting(context);
        }

        /// <summary>
        /// 请求完成后的处理
        /// </summary>
        /// <param name="context"></param>
        [ApiExplorerSettings(IgnoreApi = true)]
        public override void OnActionExecuted(ActionExecutedContext context)
        {
#if DEBUG
            _stopWatch.Stop();
            LogHelper.Logger.Trace("[Id]:{0}\n [Value]:{1}", context.ActionDescriptor.DisplayName, _stopWatch.ElapsedMilliseconds);
#endif
        }
    }
}
