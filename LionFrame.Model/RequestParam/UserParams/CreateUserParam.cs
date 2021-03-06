﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LionFrame.Model.RequestParam.UserParams
{
    /// <summary>
    /// 用户管理界面创建用户模型
    /// </summary>
    public class CreateUserParam
    {
        [Required(ErrorMessage = "请输入昵称"),MaxLength(30,ErrorMessage = "昵称为1-30字符")]
        public string NickName { get; set; }
        [RegularExpression(@"^[A-Za-z0-9\u4e00-\u9fa5_-]+@[a-zA-Z0-9_-]+(\.[a-zA-Z0-9_-]+)+$", ErrorMessage = "邮箱格式错误"),
         Required(ErrorMessage = "请输入邮箱")]
        public string Email { get; set; }
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d$@!%*#?&~]{6,20}$", ErrorMessage = "密码格式错误"),
         Required(ErrorMessage = "请输入密码")]
        public string Pwd { get; set; }
        [Required(ErrorMessage = "未选择角色")]
        public List<long> RoleIds { get; set; }
    }
}
