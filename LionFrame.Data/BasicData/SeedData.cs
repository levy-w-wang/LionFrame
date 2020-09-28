using LionFrame.Basic.Encryptions;
using LionFrame.Config;
using LionFrame.Domain.SystemDomain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace LionFrame.Data.BasicData
{
    public static class SeedData
    {
        /// <summary>
        /// 初始化种子数据
        /// </summary>
        /// <param name="modelBuilder"></param>
        public static void InitData(this ModelBuilder modelBuilder)
        {
            #region 角色

            modelBuilder.Entity<SysUser>().HasData(new SysUser()
            {
                UserId = 1L,
                UserName = "levy",
                PassWord = "qwer1234".Md5Encrypt(),
                Email = "levywang123@gmail.com",
                Sex = 1,
                Status = 1,
                CreatedTime = DateTime.Now,
                UpdatedTime = DateTime.Now
            });

            #endregion

            #region 用户

            modelBuilder.Entity<SysRole>().HasData(new SysRole()
            {
                RoleId = 1L,
                RoleName = "超级管理员",
                RoleDesc = "系统创建",
            });

            #endregion

            #region 角色用户关系

            modelBuilder.Entity<SysUserRoleRelation>().HasData(new SysUserRoleRelation()
            {
                RoleId = 1L,
                UserId = 1L,
            });

            #endregion

            #region 菜单初始数据

            var menus = new List<SysMenu>()
            {
                new SysMenu()
                {
                    MenuId = "M1",
                    MenuName = "系统管理",
                    ParentMenuId = "",
                    Url = "",
                    Type = SysConstants.MenuType.Menu,
                    Level = 1,
                    Icon = "el-icon-setting",
                    OrderIndex = 1,
                },
                new SysMenu()
                {
                    MenuId = "M2",
                    MenuName = "一级菜单",
                    ParentMenuId = "",
                    Url = "menu/singleMenu/index",
                    Level = 1,
                    Type = SysConstants.MenuType.Menu,
                    Icon = "el-icon-wind-power",
                    OrderIndex = 2,
                },
                new SysMenu()
                {
                    MenuId = "M3",
                    MenuName = "二级菜单",
                    ParentMenuId = "",
                    Url = "",
                    Level = 1,
                    Type = SysConstants.MenuType.Menu,
                    Icon = "el-icon-ice-cream-round",
                    OrderIndex = 3,
                },
                new SysMenu()
                {
                    MenuId = "M4",
                    MenuName = "三级多级菜单",
                    ParentMenuId = "",
                    Url = "",
                    Level = 1,
                    Type = SysConstants.MenuType.Menu,
                    Icon = "el-icon-ice-cream-round",
                    OrderIndex = 4,
                },
                new SysMenu()
                {
                    MenuId = "M101",
                    MenuName = "用户管理",
                    ParentMenuId = "M1",
                    Level = 2,
                    Url = "home/systemManage/userManage",
                    Type = SysConstants.MenuType.Menu,
                    Icon = "el-icon-user",
                    OrderIndex = 1,
                },
                new SysMenu()
                {
                    MenuId = "M102",
                    MenuName = "菜单管理",
                    ParentMenuId = "M1",
                    Level = 2,
                    Url = "home/systemManage/menuManage",
                    Type = SysConstants.MenuType.Menu,
                    Icon = "el-icon-menu",
                    OrderIndex = 2,
                },
                new SysMenu()
                {
                    MenuId = "M301",
                    MenuName = "二级1-1",
                    ParentMenuId = "M3",
                    Level = 2,
                    Url = "menu/secondMenu/second1-1",
                    Type = SysConstants.MenuType.Menu,
                    Icon = "el-icon-milk-tea",
                    OrderIndex = 1,
                },
                new SysMenu()
                {
                    MenuId = "M302",
                    MenuName = "二级1-2",
                    ParentMenuId = "M3",
                    Level = 2,
                    Url = "menu/secondMenu/second1-2",
                    Type = SysConstants.MenuType.Menu,
                    Icon = "el-icon-potato-strips",
                    OrderIndex = 2,
                },
                new SysMenu()
                {
                    MenuId = "M303",
                    MenuName = "二级1-3",
                    ParentMenuId = "M3",
                    Level = 2,
                    Url = "menu/secondMenu/second1-3",
                    Type = SysConstants.MenuType.Menu,
                    Icon = "el-icon-lollipop",
                    OrderIndex = 3,
                },
                new SysMenu()
                {
                    MenuId = "M401",
                    MenuName = "三级1-1",
                    ParentMenuId = "M4",
                    Level = 2,
                    Url = "menu/threeMenu/three1-1",
                    Type = SysConstants.MenuType.Menu,
                    Icon = "el-icon-milk-tea",
                    OrderIndex = 1,
                },
                new SysMenu()
                {
                    MenuId = "M402",
                    MenuName = "三级1-2",
                    ParentMenuId = "M4",
                    Level = 2,
                    Url = "",
                    Type = SysConstants.MenuType.Menu,
                    Icon = "el-icon-potato-strips",
                    OrderIndex = 2,
                },
                new SysMenu()
                {
                    MenuId = "M40201",
                    MenuName = "三级1-2-1",
                    ParentMenuId = "M402",
                    Level = 3,
                    Url = "menu/threeMenu/nextMenu/three1-2-1",
                    Type = SysConstants.MenuType.Menu,
                    Icon = "el-icon-milk-tea",
                    OrderIndex = 1,
                },
                new SysMenu()
                {
                    MenuId = "M40202",
                    MenuName = "三级1-2-2",
                    ParentMenuId = "M402",
                    Level = 3,
                    Url = "menu/threeMenu/nextMenu/three1-2-2",
                    Type = SysConstants.MenuType.Menu,
                    Icon = "el-icon-potato-strips",
                    OrderIndex = 2,
                }
            };
            modelBuilder.Entity<SysMenu>().HasData(menus
            );

            #endregion

            #region 菜单用户关系
            var roleMenus = new List<SysRoleMenuRelation>();
            menus.ForEach(m => roleMenus.Add(new SysRoleMenuRelation()
            {
                MenuId = m.MenuId,
                RoleId = 1L
            }));
            modelBuilder.Entity<SysRoleMenuRelation>().HasData(roleMenus);

            #endregion
        }
    }
}
