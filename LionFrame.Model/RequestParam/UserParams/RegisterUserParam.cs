using System.ComponentModel.DataAnnotations;

namespace LionFrame.Model.RequestParam.UserParams
{
    /// <summary>
    /// 账号注册
    /// </summary>
    public class RegisterUserParam
    {
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d$@!%*#?&~]{6,20}$", ErrorMessage = "密码格式错误"),
         Required(ErrorMessage = "请输入密码")]
        public string PassWord { get; set; }

        [RegularExpression(@"^[A-Za-z0-9\u4e00-\u9fa5_-]+@[a-zA-Z0-9_-]+(\.[a-zA-Z0-9_-]+)+$", ErrorMessage = "邮箱格式错误"),
         Required(ErrorMessage = "请输入邮箱"),MaxLength(100)]
        public string Email { get; set; }

        /// <summary>
        /// 租户名称
        /// </summary>
        [Required(ErrorMessage = "请输入租户名称"), MaxLength(50,ErrorMessage = "租户名称为1-50字符")]
        public string TenantName { get; set; }

        [Required(ErrorMessage = "请输入昵称"), MaxLength(30,ErrorMessage = "昵称为1-30字符")]
        public string NickName { get; set; }

        [RegularExpression(@"^[0-9]{6}$", ErrorMessage = "验证码为6位数字"), Required(ErrorMessage = "请输入验证码")]
        public string Captcha { get; set; }

        ///// <summary>
        ///// 男女 1男 0女
        ///// </summary>
        //[Range(0, 1, ErrorMessage = "性别设置错误")]
        //public int Sex { get; set; } = 1;
    }
}
