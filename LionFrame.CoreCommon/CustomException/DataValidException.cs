using LionFrame.Model;
using System;

namespace LionFrame.CoreCommon.CustomException
{
    /// <summary>
    /// 数据验证错误异常
    /// </summary>
    public class DataValidException : Exception
    {
        public ResponseCode Code { get; set; }

        public object[] Args { get; set; }


        public DataValidException(DataValidErrorType type, string message, params object[] args) : base(message)
        {
            switch (type)
            {
                case DataValidErrorType.Null:
                    Code = ResponseCode.DataIsNull;
                    break;
                case DataValidErrorType.Format:
                    Code = ResponseCode.DataFormatError;
                    break;
                case DataValidErrorType.Type:
                    Code = ResponseCode.DataTypeError;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
            Args = args;
        }

        public DataValidException(string message, ResponseCode code, params object[] args) : base(message)
        {
            Code = code;
            Args = args;
        }
    }


    public enum DataValidErrorType
    {
        Null = 1,
        Format = 2,
        Type = 3
    }
}
