using System;
using System.Collections.Generic;
using LionFrame.Basic.Encryptions;
using LionFrame.Config;
using LionFrame.Domain.SystemDomain;
using Microsoft.EntityFrameworkCore;

namespace LionFrame.Data.BasicData
{
    /// <summary>
    /// The seed data.
    /// </summary>
    public static partial class SeedData
    {
        /// <summary>
        /// 初始化种子数据
        /// </summary>
        /// <param name="modelBuilder"></param>
        public static void InitData(this ModelBuilder modelBuilder)
        {
            #region 租户

            modelBuilder.Entity<SysTenant>().HasData(InitTenants);

            #endregion

            #region 用户

            modelBuilder.Entity<SysUser>().HasData(InitUsers);

            #endregion

            #region 角色

            modelBuilder.Entity<SysRole>().HasData(InitRoles);

            #endregion

            #region 角色用户关系

            modelBuilder.Entity<SysUserRoleRelation>().HasData(InitUserRoleRelations);

            #endregion

            #region 菜单初始数据

            modelBuilder.Entity<SysMenu>().HasData(InitMenus);

            #endregion

            #region 菜单用户关系

            modelBuilder.Entity<SysRoleMenuRelation>().HasData(InitRoleMenuRelations);

            #endregion
        }
    }
}