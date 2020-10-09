using System.ComponentModel.DataAnnotations;

namespace LionFrame.Model.RequestParam
{
    /// <summary>
    /// 用户登录参数
    /// </summary>
    public class LoginParam
    {
        /// <summary>
        /// 账号
        /// </summary>
        [Required(ErrorMessage = "账号未输入")]
        public string UserName { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        [Required(ErrorMessage = "密码未输入")]
        public string Password { get; set; }

        /// <summary>
        /// 签名
        /// </summary>
        [Required(ErrorMessage = "参数错误")]
        public string Uuid { get; set; }

        /// <summary>
        /// 验证码
        /// </summary>
        [Required(ErrorMessage = "验证码未输入")]
        public string Captcha { get; set; }

    }
}
