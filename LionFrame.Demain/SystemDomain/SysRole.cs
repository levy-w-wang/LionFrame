using LionFrame.Domain.BaseDomain;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LionFrame.Domain.SystemDomain
{
    [Table("Sys_Role")]
    public class SysRole : BaseCommonModel
    {
        [Key]
        public long RoleId { get; set; }

        [Column(TypeName = "nvarchar(128)"), Required]
        public string RoleName { get; set; }

        [Column(TypeName = "nvarchar(128)")]
        public string RoleDesc { get; set; }

        public List<SysRoleMenuRelation> SysRoleMenuRelations { get; set; }
        public List<SysUserRoleRelation> SysUserRoleRelations { get; set; }
    }
}
