using LionFrame.Domain.BaseDomain;
using System.ComponentModel.DataAnnotations.Schema;

namespace LionFrame.Domain.SystemDomain
{
    [Table("Sys_User_Role_Relation")]
    public class SysUserRoleRelation : BaseCommonModel
    {
        public SysUserRoleRelation()
        {
            SysRole = new SysRole();
        }
        public long UserId { get; set; }
        public SysUser SysUser { get; set; }

        public long RoleId { get; set; }
        public SysRole SysRole { get; set; }
    }
}
