using System;
using System.Text;
using LionFrame.Basic.Extensions;
using LionFrame.Model;
using LionFrame.Model.ResponseDto.ResultModel;
using Microsoft.AspNetCore.Mvc;

namespace LionFrame.CoreCommon.CustomResult
{
    public class CustomHttpStatusCodeResult : ActionResult
    {
        public int StatusCode { get; }

        public string Data { get; }

        public CustomHttpStatusCodeResult(int httpStatusCode, ResponseCode msgCode, string content = "", object data = null)
        {
            StatusCode = httpStatusCode;
            Data = new ResponseModel().Fail(msgCode, content, data ?? "").ToJson(true, isLowCase: true);
        }

        public override void ExecuteResult(ActionContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            context.HttpContext.Response.StatusCode = StatusCode;
            if (string.IsNullOrEmpty(Data))
                return;
            context.HttpContext.Response.ContentType = "application/json;charset=utf-8";
            var bytes = Encoding.UTF8.GetBytes(Data);

            context.HttpContext.Response.Body.Write(bytes, 0, bytes.Length);
        }
    }
}
