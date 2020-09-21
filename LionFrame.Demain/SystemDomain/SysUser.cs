using LionFrame.Domain.BaseDomain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LionFrame.Domain.SystemDomain
{
    [Table("Sys_User")]
    public class SysUser : BaseModel
    {
        public SysUser()
        {
            SysUserRoleRelations = new List<SysUserRoleRelation>();
        }
        [Key]
        public long UserId { get; set; }

        [Column(TypeName = "varchar(128)"), Required]
        public string UserName { get; set; }

        [Column(TypeName = "varchar(512)"), Required]
        public string PassWord { get; set; }

        public bool Status { get; set; }

        [Column(TypeName = "datetime2(7)"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedTime { get; set; }

        [Column(TypeName = "datetime2(7)"), DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime UpdatedTime { get; set; }

        public List<SysUserRoleRelation> SysUserRoleRelations { get; set; }
    }
}
