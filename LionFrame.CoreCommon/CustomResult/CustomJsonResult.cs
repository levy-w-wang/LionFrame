using System;
using System.Text;
using LionFrame.Basic.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace LionFrame.CoreCommon.CustomResult
{
    /// <summary>
    /// 自定义返回Json数据
    /// </summary>
    public class CustomJsonResult : ActionResult
    {
        public object Data { get; set; }

        public string DateTimeFormat { get; set; } = "yyyy-MM-dd HH:mm:ss";


        public override void ExecuteResult(ActionContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            var response = context.HttpContext.Response;
            response.ContentType = "application/json;charset=utf-8";
            if (Data == null) return;
            if (string.IsNullOrEmpty(DateTimeFormat))
            {
                DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
            }
            string json;
#if DEBUG
            json = Data.ToJson(true, true, true, DateTimeFormat);//方便调式
#else
            json = Data.ToJson(true, false, true, DateTimeFormat);
#endif
            var data = Encoding.UTF8.GetBytes(json);
            response.Body.Write(data, 0, data.Length);
        }
    }
}
