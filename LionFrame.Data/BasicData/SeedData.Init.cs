using System;
using System.Collections.Generic;
using System.Text;
using LionFrame.Basic.Encryptions;
using LionFrame.Config;
using LionFrame.Domain.SystemDomain;

namespace LionFrame.Data.BasicData
{
    public static partial class SeedData
    {
        /// <summary>
        /// 初始租户信息
        /// </summary>
        public static List<SysTenant> InitTenants => new List<SysTenant>()
        {
            new SysTenant()
            {
                TenantId = 1,
                TenantName = "系统管理员",
                Remark = "系统管理员",
                State = 1,
                CreatedTime = DateTime.Now,
            },
            new SysTenant()
            {
                TenantId = 2,
                TenantName = "管理员",
                Remark = "用户    ",
                State = 1,
                CreatedTime = DateTime.Now,
            }
        };

        /// <summary>
        /// 初始用户
        /// </summary>
        public static List<SysUser> InitUsers => new List<SysUser>()
        {
            new SysUser()
            {
                UserId = 1L,
                NickName = "levy",
                PassWord = "qwer1234".Md5Encrypt(),
                Email = "levywang123@gmail.com",
                Sex = 1,
                State = 1,
                CreatedTime = DateTime.Now,
                UpdatedTime = DateTime.Now,
                TenantId = 1,
                UpdatedBy = 0,
            },
            new SysUser()
            {
                UserId = 2L,
                NickName = "levy1",
                PassWord = "qwer1234".Md5Encrypt(),
                Email = "levy_wang@qq.com",
                Sex = 1,
                State = 1,
                CreatedTime = DateTime.Now,
                UpdatedTime = DateTime.Now,
                TenantId = 2,
                UpdatedBy = 0,
            }
        };

        /// <summary>
        /// 初始角色
        /// </summary>
        public static List<SysRole> InitRoles => new List<SysRole>()
        {
            new SysRole()
            {
                RoleId = 1L,
                TenantId = 1,
                RoleName = "系统管理员",
                RoleDesc = "系统创建",
                CreatedTime = DateTime.Now,
            },
            new SysRole()
            {
                RoleId = 2L,
                TenantId = 2,
                RoleName = "超级管理员",
                RoleDesc = "系统创建",
                CreatedTime = DateTime.Now,
            }
        };

        /// <summary>
        /// 初始用户角色关系
        /// </summary>
        public static List<SysUserRoleRelation> InitUserRoleRelations => new List<SysUserRoleRelation>()
        {
            new SysUserRoleRelation()
            {
                RoleId = 1L,
                UserId = 1L,
                TenantId = 1,
                CreatedTime = DateTime.Now,
                State = 1,
            },
            new SysUserRoleRelation()
            {
                RoleId = 2L,
                UserId = 2L,
                TenantId = 2,
                CreatedTime = DateTime.Now,
                State = 1,
            }
        };

        /// <summary>
        /// 初始按钮
        /// </summary>
        public static List<SysMenu> InitMenus => new List<SysMenu>()
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
                MenuId = "M5",
                MenuName = "任务调度",
                ParentMenuId = string.Empty,
                Url = string.Empty,
                Level = 1,
                Type = SysConstants.MenuType.Menu,
                Icon = "lion-icon-renwutiaodu",
                OrderIndex = 1,
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
            },
            new SysMenu()
            {
                MenuId = "M501",
                MenuName = "任务管理",
                ParentMenuId = "M5",
                Level = 2,
                Url = "quartz/task",
                Type = SysConstants.MenuType.Menu,
                Icon = "lion-icon-renwu",
                OrderIndex = 1,
                CreatedTime = DateTime.Now,
            },
            new SysMenu()
            {
                MenuId = "M502",
                MenuName = "任务日志",
                ParentMenuId = "M5",
                Level = 2,
                Url = "quartz/tasklog",
                Type = SysConstants.MenuType.Menu,
                Icon = "lion-icon-rizhi",
                OrderIndex = 2,
                CreatedTime = DateTime.Now,
            }
        };


        //非系统管理员不能管理菜单 
        public static List<string> InitNoPerms = new List<string>()
        {
            "M102","M5","M501","M502"
        };

        /// <summary>
        /// 通用用户注册关系  注意修改
        /// </summary>
        public static List<SysRoleMenuRelation> InitNormalRoleMenuRelations = new List<SysRoleMenuRelation>();

        /// <summary>
        /// 初始化角色按钮关系
        /// </summary>
        public static List<SysRoleMenuRelation> InitRoleMenuRelations
        {
            get
            {
                var roleMenus = new List<SysRoleMenuRelation>();
                InitMenus.ForEach(m =>
                {
                    var tempRoleMenuRelation = new SysRoleMenuRelation()
                    {
                        TenantId = 1,
                        MenuId = m.MenuId,
                        RoleId = 1L,
                        CreatedTime = DateTime.Now,
                        State = 1,
                    };
                    var tempNormalRoleMenuRelation = new SysRoleMenuRelation()
                    {
                        TenantId = 2,
                        MenuId = m.MenuId,
                        RoleId = 2L,
                        CreatedTime = DateTime.Now,
                        State = 1,
                    };
                    if (InitNoPerms.Contains(m.MenuId))
                    {
                        // 全部都将设置关系，只是关系是是否为逻辑删除状态
                        tempNormalRoleMenuRelation.State = -1;
                        tempNormalRoleMenuRelation.Deleted = true;
                    }

                    roleMenus.Add(tempRoleMenuRelation);
                    roleMenus.Add(tempNormalRoleMenuRelation);
                    InitNormalRoleMenuRelations.Add(tempNormalRoleMenuRelation);
                });
                return roleMenus;
            }
        }
    }
}