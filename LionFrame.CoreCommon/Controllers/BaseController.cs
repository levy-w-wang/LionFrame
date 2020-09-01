using LionFrame.Basic.Extensions;
using LionFrame.CoreCommon.CustomResult;
using LionFrame.Model;
using LionFrame.Model.ResponseDto.ResultModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text;

namespace LionFrame.CoreCommon.Controllers
{
    [ApiController]
    public abstract class BaseController : Controller
    {
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
        /// <param name="content"></param>
        /// <returns></returns>
        protected ActionResult Fail(ResponseCode code, string content = "")
        {
            return MyJson(new ResponseModel().Fail(code, content, null));
        }

        /// <summary>
        /// 带错误返回数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="code"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        protected ActionResult Fail<T>(T data, ResponseCode code, string content = "")
        {
            return MyJson(new ResponseModel().Fail(code, content, data));
        }

        /// <summary>
        /// 请求开始前的处理
        /// </summary>
        /// <param name="context"></param>
        [ApiExplorerSettings(IgnoreApi = true)]
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
        }

        /// <summary>
        /// 请求完成后的处理
        /// </summary>
        /// <param name="context"></param>
        [ApiExplorerSettings(IgnoreApi = true)]
        public override void OnActionExecuted(ActionExecutedContext context)
        {
        }
    }
}
