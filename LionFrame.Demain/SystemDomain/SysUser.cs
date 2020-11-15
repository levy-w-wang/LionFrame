using LionFrame.Domain.BaseDomain;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LionFrame.Domain.SystemDomain
{
    [Table("Sys_User")]
    public class SysUser : BaseModel
    {
        [Key]
        public long UserId { get; set; }

        [MaxLength(30), Required]
        public string NickName { get; set; }

        [MaxLength(512), Required]
        public string PassWord { get; set; }

        [MaxLength(128), Required]
        [DefaultValue("")]
        public string Email { get; set; }

        /// <summary>
        /// 男女 1男 0女
        /// </summary>
        [DefaultValue(1)]
        public int Sex { get; set; }
        /// <summary>
        /// 1为用户有效状态 -1为无效 其它尚未定义
        /// </summary>
        public int State { get; set; }

        public DateTime CreatedTime { get; set; }
        public long CreatedBy { get; set; }

        public DateTime UpdatedTime { get; set; }
        public long UpdatedBy { get; set; }

        /// <summary>
        /// 租户ID
        /// </summary>
        public long TenantId { get; set; }
        public TenantInfo TenantInfo { get; set; }
        public List<SysUserRoleRelation> SysUserRoleRelations { get; set; }
    }
}
