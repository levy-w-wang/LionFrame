using LionFrame.Domain.BaseDomain;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LionFrame.Domain.SystemDomain
{
    [Table("Sys_Role")]
    public class SysRole : BaseCommonModel
    {
        [Key]
        public long RoleId { get; set; }

        [MaxLength(128), Required]
        public string RoleName { get; set; }

        [MaxLength(128)]
        public string RoleDesc { get; set; }

        /// <summary>
        /// 当前角色能否操作对应权限和删除该角色等
        /// </summary>
        [DefaultValue(true)]
        public bool Operable { get; set; }

        public List<SysRoleMenuRelation> SysRoleMenuRelations { get; set; }
        public List<SysUserRoleRelation> SysUserRoleRelations { get; set; }
    }
}
