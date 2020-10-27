using System.ComponentModel.DataAnnotations;

namespace LionFrame.Model.RequestParam.UserParams
{
    /// <summary>
    /// 找回密码模型
    /// </summary>
    public class RetrievePwdParam
    {
        [Required(ErrorMessage = "邮箱未输入"),RegularExpression(@"^[A-Za-z0-9\u4e00-\u9fa5_-]+@[a-zA-Z0-9_-]+(\.[a-zA-Z0-9_-]+)+$",ErrorMessage = "邮箱格式不正确")]
        public string Email { get; set; }

        [RegularExpression(@"^[0-9]{6}$", ErrorMessage = "验证码为6位数字"), Required(ErrorMessage = "请输入验证码")]
        public string Captcha { get; set; }

        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d$@!%*#?&~]{6,20}$", ErrorMessage = "密码格式错误"), Required(ErrorMessage = "请输入密码")]
        public string Pwd { get; set; }
    }
}
