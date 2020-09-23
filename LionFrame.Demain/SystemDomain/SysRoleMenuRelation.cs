using LionFrame.Domain.BaseDomain;
using System.ComponentModel.DataAnnotations.Schema;

namespace LionFrame.Domain.SystemDomain
{
    [Table("Sys_Role_Menu_Relation")]
    public class SysRoleMenuRelation : BaseCommonModel
    {
        public long RoleId { get; set; }
        public SysRole SysRole { get; set; }

        public string MenuId { get; set; }
        public SysMenu SysMenu { get; set; }
    }
}
