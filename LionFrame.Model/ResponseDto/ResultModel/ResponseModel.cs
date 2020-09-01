namespace LionFrame.Model.ResponseDto.ResultModel
{
    /// <summary>
    /// 响应返回体
    /// </summary>
    public class ResponseModel : ResponseModel<object>
    {
    }
    /// <summary>
    /// 响应返回体
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ResponseModel<T> : BaseResponseModel
    {
        //public FailModel Error { get; set; }
        public T Data { get; set; }

        public ResponseModel<T> Fail(ResponseCode code, string content, T data)
        {
            Code = code;
            Message = content;
            Data = data;
            return this;
        }

        public ResponseModel<T> Succeed(T data, ResponseCode code = ResponseCode.Success, string msg = "successful")
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
