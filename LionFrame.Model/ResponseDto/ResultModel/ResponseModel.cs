namespace LionFrame.Model.ResponseDto.ResultModel
{
    /// <summary>
    /// 响应返回体
    /// </summary>
    public class ResponseModel : ResponseModel<object>
    {
        public new ResponseModel Fail(ResponseCode code, string message)
        {
            Code = code;
            Message = message;
            return this;
        }
    }
    /// <summary>
    /// 响应返回体
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ResponseModel<T> : BaseResponseModel
    {
        //public FailModel Error { get; set; }
        public T Data { get; set; }

        /// <summary>
        /// 失败返回
        /// </summary>
        /// <param name="code"></param>
        /// <param name="message"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public ResponseModel<T> Fail(ResponseCode code, string message, T data)
        {
            Code = code;
            Message = message;
            Data = data;
            return this;
        }
        /// <summary>
        /// 通用错误返回
        /// </summary>
        /// <param name="message"></param>
        /// <param name="data"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public ResponseModel<T> Fail(string message, T data, ResponseCode code = ResponseCode.Fail)
        {
            Code = code;
            Message = message;
            Data = data;
            return this;
        }

        /// <summary>
        /// 错误返回 - 自定义错误码
        /// </summary>
        /// <param name="message"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public ResponseModel<T> Fail(ResponseCode code, string message)
        {
            Code = code;
            Message = message;
            Data = default(T);
            return this;
        }

        /// <summary>
        /// 通用错误返回
        /// </summary>
        /// <param name="message"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public ResponseModel<T> Fail(string message, ResponseCode code = ResponseCode.Fail)
        {
            Code = code;
            Message = message;
            Data = default(T);
            return this;
        }
        /// <summary>
        /// 成功返回
        /// </summary>
        /// <param name="data"></param>
        /// <param name="code"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public ResponseModel<T> Succeed(T data, ResponseCode code = ResponseCode.Success, string msg = "success")
        {
            Code = code;
            Message = msg;
            Data = data;
            return this;
        }
        /// <summary>
        /// 成功默认返回
        /// </summary>
        /// <returns></returns>
        public ResponseModel<T> Succeed()
        {
            Code = ResponseCode.Success;
            Message = "success";
            Data = default(T);
            return this;
        }
    }

    public class BaseResponseModel
    {
        public ResponseCode Code { get; set; }

        public string Message { get; set; }

        public bool Success => Code == ResponseCode.Success;
    }
}
