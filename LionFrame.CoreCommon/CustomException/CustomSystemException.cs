using LionFrame.Model;
using System;

namespace LionFrame.CoreCommon.CustomException
{
    /// <inheritdoc />
    /// <summary>
    /// 自定义系统错误异常
    /// </summary>
    public class CustomSystemException : Exception
    {

        /// <summary>
        /// 响应状态码
        /// </summary>
        public ResponseCode Code { get; set; }

        /// <summary>
        /// 错误消息
        /// </summary>
        public object[] Args { get; set; }


        public CustomSystemException(string message, ResponseCode code, params object[] args) : base(message)
        {
            Code = code;
            Args = args;
        }

    }
}
