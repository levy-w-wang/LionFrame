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
        [Required]
        public string UserName { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        [Required]
        public string PassWord { get; set; }
    }
}
