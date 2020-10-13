using System.ComponentModel.DataAnnotations;

namespace LionFrame.Model.RequestParam.UserParams
{
    /// <summary>
    /// 修改密码参数
    /// </summary>
    public class ModifyPwdParam
    {
        /// <summary>
        /// 旧密码
        /// </summary>
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d$@!%*#?&~]{6,20}$", ErrorMessage = "旧密码格式错误"), Required(ErrorMessage = "请输入旧密码")]
        public string OldPassWord { get; set; }

        /// <summary>
        /// 新密码
        /// </summary>
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d$@!%*#?&~]{6,20}$", ErrorMessage = "新密码格式错误"), Required(ErrorMessage = "请输入新密码")]
        public string NewPassWord { get; set; }

        /// <summary>
        /// 重复密码
        /// </summary>
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d$@!%*#?&~]{6,20}$", ErrorMessage = "重复密码格式错误"), Required(ErrorMessage = "请输入重复密码")]
        public string NewPassWordRepet { get; set; }
    }
}
