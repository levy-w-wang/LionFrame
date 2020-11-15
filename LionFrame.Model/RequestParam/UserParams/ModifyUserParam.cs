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

        [Required(ErrorMessage = "请输入昵称"), MaxLength(30, ErrorMessage = "昵称为1-30字符")]
        public string NickName { get; set; }

        [Required(ErrorMessage = "未选择角色")]
        public List<long> RoleIds { get; set; }
    }
}