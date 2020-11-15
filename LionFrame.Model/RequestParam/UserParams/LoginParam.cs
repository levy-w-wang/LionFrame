using System.ComponentModel.DataAnnotations;

namespace LionFrame.Model.RequestParam.UserParams
{
    /// <summary>
    /// 用户登录参数
    /// </summary>
    public class LoginParam
    {
        [RegularExpression(@"^[A-Za-z0-9\u4e00-\u9fa5_-]+@[a-zA-Z0-9_-]+(\.[a-zA-Z0-9_-]+)+$", ErrorMessage = "邮箱格式错误"),
         Required(ErrorMessage = "请输入邮箱")]
        public string Email { get; set; }
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
