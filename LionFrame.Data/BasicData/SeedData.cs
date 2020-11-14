namespace LionFrame.Data.BasicData
{
    using System;
    using System.Collections.Generic;
    using LionFrame.Basic.Encryptions;
    using LionFrame.Config;
    using LionFrame.Domain.SystemDomain;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The seed data.
    /// </summary>
    public static class SeedData
    {
        /// <summary>
        /// 初始化种子数据
        /// </summary>
        /// <param name="modelBuilder"></param>
        public static void InitData(this ModelBuilder modelBuilder)
        {
            #region 用户

            modelBuilder.Entity<SysUser>().HasData(new SysUser()
            {
                UserId = 1L,
                UserName = "levy",
                PassWord = "qwer1234".Md5Encrypt(),
                Email = "levywang123@gmail.com",
                Sex = 1,
                Status = 1,
                CreatedTime = DateTime.Now,
                UpdatedTime = DateTime.Now,
                ParentUid = 0,
                UpdatedBy = 0,
            }, new SysUser()
            {
                UserId = 2L,
                UserName = "levy1",
                PassWord = "qwer1234".Md5Encrypt(),
                Email = "levy_wang@qq.com",
                Sex = 1,
                Status = 1,
                CreatedTime = DateTime.Now,
                UpdatedTime = DateTime.Now,
                ParentUid = 0,
                UpdatedBy = 0,
            });

            #endregion

            #region 角色

            modelBuilder.Entity<SysRole>().HasData(new SysRole()
            {
                RoleId = 1L,
                RoleName = "系统管理员",
                RoleDesc = "系统创建",
                CreatedTime = DateTime.Now,
            }, new SysRole()
            {
                RoleId = 2L,
                RoleName = "超级管理员",
                RoleDesc = "系统创建",
                CreatedTime = DateTime.Now,
            });

            #endregion

            #region 角色用户关系

            modelBuilder.Entity<SysUserRoleRelation>().HasData(new SysUserRoleRelation()
            {
                RoleId = 1L,
                UserId = 1L,
                CreatedTime = DateTime.Now,
                State = 1,
            }, new SysUserRoleRelation()
            {
                RoleId = 2L,
                UserId = 2L,
                CreatedTime = DateTime.Now,
                State = 1,
            }, new SysUserRoleRelation()
            {
                RoleId = 2L,
                UserId = 1L,
                CreatedTime = DateTime.Now,
                State = 1,
            });

            #endregion

            #region 菜单初始数据

            var menus = new List<SysMenu>()
            {
                new SysMenu()
                {
                    MenuId = "M1",
                    MenuName = "系统管理",
                    ParentMenuId = string.Empty,
                    Url = string.Empty,
                    Type = SysConstants.MenuType.Menu,
                    Level = 1,
                    Icon = "el-icon-setting",
                    OrderIndex = 1,
                    CreatedTime = DateTime.Now,
                },
                new SysMenu()
                {
                    MenuId = "M2",
                    MenuName = "一级菜单",
                    ParentMenuId = string.Empty,
                    Url = "menu/singleMenu/index",
                    Level = 1,
                    Type = SysConstants.MenuType.Menu,
                    Icon = "el-icon-wind-power",
                    OrderIndex = 2,
                    CreatedTime = DateTime.Now,
                },
                new SysMenu()
                {
                    MenuId = "M3",
                    MenuName = "二级菜单",
                    ParentMenuId = string.Empty,
                    Url = string.Empty,
                    Level = 1,
                    Type = SysConstants.MenuType.Menu,
                    Icon = "el-icon-ice-cream-round",
                    OrderIndex = 3,
                    CreatedTime = DateTime.Now,
                },
                new SysMenu()
                {
                    MenuId = "M4",
                    MenuName = "三级多级菜单",
                    ParentMenuId = string.Empty,
                    Url = string.Empty,
                    Level = 1,
                    Type = SysConstants.MenuType.Menu,
                    Icon = "el-icon-ice-cream-round",
                    OrderIndex = 4,
                    CreatedTime = DateTime.Now,
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
                    CreatedTime = DateTime.Now,
                },
                new SysMenu()
                {
                    MenuId = "B101_R_ADD",
                    MenuName = "角色-添加",
                    ParentMenuId = "M101",
                    Level = 1,
                    Url = "r_add",
                    Type = SysConstants.MenuType.Button,
                    Icon = string.Empty,
                    OrderIndex = 0,
                    CreatedTime = DateTime.Now,
                },
                new SysMenu()
                {
                    MenuId = "B101_R_RELATION",
                    MenuName = "角色-关联角色",
                    ParentMenuId = "M101",
                    Level = 1,
                    Url = "r_relation",
                    Type = SysConstants.MenuType.Button,
                    Icon = string.Empty,
                    OrderIndex = 0,
                    CreatedTime = DateTime.Now,
                },
                new SysMenu()
                {
                    MenuId = "B101_R_PERMS_CONFIG",
                    MenuName = "角色-配置权限",
                    ParentMenuId = "M101",
                    Level = 1,
                    Url = "r_perms_config",
                    Type = SysConstants.MenuType.Button,
                    Icon = string.Empty,
                    OrderIndex = 0,
                    CreatedTime = DateTime.Now,
                },
                new SysMenu()
                {
                    MenuId = "B101_R_DELETE",
                    MenuName = "角色-删除",
                    ParentMenuId = "M101",
                    Level = 1,
                    Url = "r_delete",
                    Type = SysConstants.MenuType.Button,
                    Icon = string.Empty,
                    OrderIndex = 0,
                    CreatedTime = DateTime.Now,
                },
                new SysMenu()
                {
                    MenuId = "B101_R_EDIT",
                    MenuName = "角色-编辑",
                    ParentMenuId = "M101",
                    Level = 1,
                    Url = "r_edit",
                    Type = SysConstants.MenuType.Button,
                    Icon = string.Empty,
                    OrderIndex = 0,
                    CreatedTime = DateTime.Now,
                },
                new SysMenu()
                {
                    MenuId = "B101_U_ADD",
                    MenuName = "用户-增加",
                    ParentMenuId = "M101",
                    Level = 1,
                    Url = "u_add",
                    Type = SysConstants.MenuType.Button,
                    Icon = string.Empty,
                    OrderIndex = 0,
                    CreatedTime = DateTime.Now,
                },

                new SysMenu()
                {
                    MenuId = "B101_U_DELETE",
                    MenuName = "用户-删除",
                    ParentMenuId = "M101",
                    Level = 1,
                    Url = "u_delete",
                    Type = SysConstants.MenuType.Button,
                    Icon = string.Empty,
                    OrderIndex = 0,
                    CreatedTime = DateTime.Now,
                },
                new SysMenu()
                {
                    MenuId = "B101_U_EDIT",
                    MenuName = "用户-编辑",
                    ParentMenuId = "M101",
                    Level = 1,
                    Url = "u_edit",
                    Type = SysConstants.MenuType.Button,
                    Icon = string.Empty,
                    OrderIndex = 0,
                    CreatedTime = DateTime.Now,
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
                    CreatedTime = DateTime.Now,
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
                    CreatedTime = DateTime.Now,
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
                    CreatedTime = DateTime.Now,
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
                    CreatedTime = DateTime.Now,
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
                    CreatedTime = DateTime.Now,
                },
                new SysMenu()
                {
                    MenuId = "M402",
                    MenuName = "三级1-2",
                    ParentMenuId = "M4",
                    Level = 2,
                    Url = string.Empty,
                    Type = SysConstants.MenuType.Menu,
                    Icon = "el-icon-potato-strips",
                    OrderIndex = 2,
                    CreatedTime = DateTime.Now,
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
                    CreatedTime = DateTime.Now,
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
                    CreatedTime = DateTime.Now,
                }
            };
            modelBuilder.Entity<SysMenu>().HasData(menus);

            #endregion

            #region 菜单用户关系

            //非系统管理员不能管理菜单 
            var noPerms = new List<string>()
            {
                "M102",
            };

            var roleMenus = new List<SysRoleMenuRelation>();
            menus.ForEach(m =>
            {
                roleMenus.Add(new SysRoleMenuRelation()
                {
                    MenuId = m.MenuId,
                    RoleId = 1L,
                    CreatedTime = DateTime.Now,
                    State = 1,
                });
                if (noPerms.Contains(m.MenuId))
                {
                    // 全部都将设置关系，只是关系是是否为逻辑删除状态
                    roleMenus.Add(new SysRoleMenuRelation()
                    {
                        MenuId = m.MenuId,
                        RoleId = 2L,
                        CreatedTime = DateTime.Now,
                        Deleted = true,
                        State = 1,
                    });
                }
                else
                {
                    roleMenus.Add(new SysRoleMenuRelation()
                    {
                        MenuId = m.MenuId,
                        RoleId = 2L,
                        CreatedTime = DateTime.Now,
                        Deleted = false,
                        State = 1,
                    });
                }
            });
            modelBuilder.Entity<SysRoleMenuRelation>().HasData(roleMenus);

            #endregion
        }
    }
}