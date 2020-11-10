using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LionFrame.Model.RequestParam.RoleParams
{
    /// <summary>
    /// 修改角色能访问的页面权限
    /// </summary>
    public class ModifyRoleMenuParam
    {
        [Required(ErrorMessage = "角色缺失")]
        public long RoleId { get; set; }
        [Required(ErrorMessage = "权限缺失")]
        public List<string> MenuIds { get; set; }
    }
}
