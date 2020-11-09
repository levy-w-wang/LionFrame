using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LionFrame.Model.RequestParam.UserParams
{
    /// <summary>
    /// 编辑用户信息
    /// </summary>
    public class ModifyUserParam
    {
        [Required(ErrorMessage = "请重新选择修改用户")]
        public long UserId { get; set; }
        [RegularExpression(@"^[A-Za-z0-9\u4e00-\u9fa5_-]+@[a-zA-Z0-9_-]+(\.[a-zA-Z0-9_-]+)+$", ErrorMessage = "邮箱格式错误"),
         Required(ErrorMessage = "请输入邮箱")]
        public string Email { get; set; }
        [Required(ErrorMessage = "未选择角色")]
        public List<long> RoleIds { get; set; }
    }
}
