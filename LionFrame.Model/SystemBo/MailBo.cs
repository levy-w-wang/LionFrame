namespace LionFrame.Model.SystemBo
{
    public class MailBo
    {
        public MailBo()
        {
            MailFromName = "测试";
            MailFrom = "levy_wang@qq.com";
            MailPwd = "xxxxxxxxxxx";
            MailHost = "smtp.qq.com";
            MailPort = 25; // 465
        }

        /// <summary>
        /// 发件邮箱人名
        /// </summary>
        public string MailFromName { get; set; }

        /// <summary>
        /// 发件邮箱
        /// </summary>
        public string MailFrom { get; set; }

        /// <summary>
        /// 邮箱密码
        /// </summary>
        public string MailPwd { get; set; }

        /// <summary>
        /// 发件服务器
        /// </summary>
        public string MailHost { get; set; }

        /// <summary>
        /// 发件服务器端口
        /// </summary>
        public int MailPort { get; set; }

        /// <summary>
        /// 收件邮箱人名
        /// </summary>
        public string MailToName { get; set; }

        /// <summary>
        /// 收件邮箱
        /// </summary>
        public string MailTo { get; set; }
    }
}
