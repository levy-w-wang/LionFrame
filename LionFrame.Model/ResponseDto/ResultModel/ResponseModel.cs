namespace LionFrame.Model.ResponseDto.ResultModel
{
    /// <summary>
    /// 响应返回体
    /// </summary>
    public class ResponseModel : ResponseModel<object>
    {
        public new ResponseModel Fail(ResponseCode code, string content)
        {
            Code = code;
            Message = content;
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
        /// <param name="content"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public ResponseModel<T> Fail(ResponseCode code, string content, T data)
        {
            Code = code;
            Message = content;
            Data = data;
            return this;
        }
        /// <summary>
        /// 通用错误返回
        /// </summary>
        /// <param name="content"></param>
        /// <param name="data"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public ResponseModel<T> Fail(string content, T data, ResponseCode code = ResponseCode.Fail)
        {
            Code = code;
            Message = content;
            Data = data;
            return this;
        }

        /// <summary>
        /// 错误返回 - 自定义错误码
        /// </summary>
        /// <param name="content"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public ResponseModel<T> Fail(ResponseCode code, string content)
        {
            Code = code;
            Message = content;
            Data = default;
            return this;
        }

        /// <summary>
        /// 通用错误返回
        /// </summary>
        /// <param name="content"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public ResponseModel<T> Fail( string content,ResponseCode code = ResponseCode.Fail)
        {
            Code = code;
            Message = content;
            Data = default;
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
    }

    public class BaseResponseModel
    {
        public ResponseCode Code { get; set; }

        public string Message { get; set; }

        public bool Success => Code == ResponseCode.Success;
    }
}
