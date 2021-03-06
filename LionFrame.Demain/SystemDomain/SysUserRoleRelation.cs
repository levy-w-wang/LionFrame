﻿using LionFrame.Domain.BaseDomain;
using System.ComponentModel.DataAnnotations.Schema;

namespace LionFrame.Domain.SystemDomain
{
    [Table("Sys_User_Role_Relation")]
    public class SysUserRoleRelation : BaseCommonModel
    {
        public long UserId { get; set; }
        public SysUser SysUser { get; set; }

        public long RoleId { get; set; }
        public SysRole SysRole { get; set; }

        public long TenantId { get; set; }

        /// <summary>
        /// 可用状态1 其它待定
        /// </summary>
        public int State { get; set; }
    }
}
