using System.ComponentModel.DataAnnotations;

namespace LionFrame.Model.RequestParam.RoleParams
{
    /// <summary>
    /// 角色新增修改
    /// </summary>
    public class IncrementRoleParam
    {
        public long RoleId { get; set; }
        [Required(ErrorMessage = "角色名不能为空"),StringLength(maximumLength:20,MinimumLength = 2,ErrorMessage = "角色名长度为2-20个字符")]
        public string RoleName { get; set; }
        [MaxLength(120,ErrorMessage = "长度为0-120个字符")]
        public string RoleDesc { get; set; }
        
    }
}
