using System;
using System.Collections.Generic;
using System.Text;

namespace LionFrame.Config
{
    /// <summary>
    /// Quartz JobDataMap存储key
    /// </summary>
    public class QuartzConstant
    {
        /// <summary>
        /// 请求url RequestUrl
        /// </summary>
        public static readonly string REQUESTURL = "RequestUrl";
        /// <summary>
        /// 请求参数 RequestParameters
        /// </summary>
        public static readonly string REQUESTPARAMETERS = "RequestParameters";
        /// <summary>
        /// Headers（可以包含：Authorization授权认证）
        /// </summary>
        public static readonly string HEADERS = "Headers";
        /// <summary>
        /// 请求类型 RequestType
        /// </summary>
        public static readonly string REQUESTTYPE = "RequestType";

        /// <summary>
        /// 是否发送邮件
        /// </summary>
        public static readonly string MAILMESSAGE = "MailMessage";
        /// <summary>
        /// 通知邮箱
        /// </summary>

        public static readonly string NOTIFYEMAIL = "NotifyEmail";

        /// <summary>
        /// 任务类型  http Assembly 两种
        /// </summary>
        public static readonly string JOBTYPE = "JobType";
    }
}
