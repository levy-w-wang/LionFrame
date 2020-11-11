using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LionFrame.Model.RequestParam.RoleParams
{
    /// <summary>
    /// 修改角色能访问的页面权限
    /// </summary>
    public class ModifyUserRoleParam
    {
        [Required(ErrorMessage = "角色缺失")]
        public long RoleId { get; set; }

        [Required(ErrorMessage = "用户缺失")]
        public List<long> UserIds { get; set; }
    }
}
