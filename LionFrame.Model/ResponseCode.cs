namespace LionFrame.Model
{
    public enum ResponseCode
    {
        Success = 200,
        LoginFail = 201,//  登录失败
        Unauthorized = 401,
        Unauthorized1 = 1000, //未授权
        UnknownEx = 500, //未知异常标识
        DbEx = 999, //数据库操作异常
        DataIsNull = 1002, //数据为空
        DataFormatError = 1003, //数据格式错误
        DataTypeError = 1004, //数据类型错误
        RequestDataVerifyFail = 1005, //请求数据验证失败
        UnityDataError = 1006, //统一数据处理错误码
    }
}
