using System.ComponentModel;

namespace LionFrame.Model.QuartzModels
{
    /// <summary>
    /// 发送邮件提醒
    /// </summary>
    public enum MailMessageEnum
    {
        [Description("不发送")]
        None = 0,
        [Description("错误发送")]
        Err = 1,
        [Description("全量发送")]
        All = 2
    }
}
