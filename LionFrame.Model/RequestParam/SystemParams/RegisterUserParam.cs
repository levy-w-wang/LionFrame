using System;
using System.ComponentModel.DataAnnotations;

namespace LionFrame.Model.RequestParam.SystemParams
{
    /// <summary>
    /// 账号注册
    /// </summary>
    public class RegisterUserParam
    {
        [Required(ErrorMessage = "请输入登录账号"),
         RegularExpression("^(?!_)(?!.*?_$)[a-zA-Z0-9_]{4,12}$", ErrorMessage = "登录账号不符合规则")]
        public string UserName { get; set; }

        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d$@!%*#?&~]{6,20}$", ErrorMessage = "密码格式错误"),
         Required(ErrorMessage = "请输入密码")]
        public string PassWord { get; set; }

        [RegularExpression(@"^[A-Za-z0-9\u4e00-\u9fa5_-]+@[a-zA-Z0-9_-]+(\.[a-zA-Z0-9_-]+)+$", ErrorMessage = "邮箱格式错误"),
         Required(ErrorMessage = "请输入邮箱")]
        public string Email { get; set; }

        /// <summary>
        /// 男女 1男 0女
        /// </summary>
        [Range(0, 1, ErrorMessage = "性别设置错误")]
        public int Sex { get; set; } = 1;
    }
}
