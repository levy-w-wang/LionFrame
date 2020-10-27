﻿using LionFrame.Basic.AutofacDependency;
using LionFrame.Config;
using LionFrame.CoreCommon.AutoMapperCfg;
using LionFrame.CoreCommon.Cache;
using LionFrame.CoreCommon.Cache.Redis;
using LionFrame.Data.SystemDao;
using LionFrame.Model.ResponseDto.SystemDto;
using LionFrame.Model.SystemBo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LionFrame.Domain.SystemDomain;
using LionFrame.Model;
using LionFrame.Model.RequestParam.SystemParams;
using LionFrame.Model.ResponseDto.ResultModel;

namespace LionFrame.Business
{
    public class MenuBll : IScopedDependency
    {
        public SysMenuDao SysMenuDao { get; set; }
        public RedisClient RedisClient { get; set; }
        public LionMemoryCache Cache { get; set; }

        /// <summary>
        /// 获取菜单对象
        /// </summary>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public List<MenuDto> GetCurrentMenuTree(UserCacheBo currentUser)
        {
            var menuKey = $"{CacheKeys.MENU_TREE}{currentUser.UserId}";
            var cacheMenus = GetMenuTreeCache(menuKey);
            if (cacheMenus != null)
            {
                return cacheMenus.MapToList<MenuDto>();
            }

            var roleIds = currentUser.RoleCacheBos.Select(c => c.RoleId).ToList();
            cacheMenus = GetMenuTreeDb(roleIds, menuKey);

            return cacheMenus.MapToList<MenuDto>();
        }

        /// <summary>
        /// 从缓存中获取菜单树
        /// </summary>
        /// <param name="menuKey"></param>
        /// <returns></returns>
        private List<MenuCacheBo> GetMenuTreeCache(string menuKey)
        {
            var cacheMenus = Cache.Get<List<MenuCacheBo>>(menuKey);
            if (cacheMenus != null)
            {
                return cacheMenus;
            }
            cacheMenus = RedisClient.Get<List<MenuCacheBo>>(menuKey);
            if (cacheMenus != null)
            {
                Cache.Set(menuKey, cacheMenus, new TimeSpan(1, 0, 0, 0));
            }
            return cacheMenus;
        }

        /// <summary>
        /// 从数据库中获取菜单树
        /// 由于菜单不经常变化故设置redis存储7天  本地缓存设置1天
        /// </summary>
        /// <param name="roleIds"></param>
        /// <param name="menuKey"></param>
        /// <returns></returns>
        private List<MenuCacheBo> GetMenuTreeDb(List<long> roleIds, string menuKey)
        {
            var menus = SysMenuDao.GetMenus(roleIds);
            var cacheMenus = GetMenus(menus);
            RedisClient.Set(menuKey, cacheMenus, new TimeSpan(7, 0, 0, 0));
            Cache.Set(menuKey, cacheMenus, new TimeSpan(1, 0, 0, 0));
            return cacheMenus;
        }

        /// <summary>
        /// 递归获取菜单结构
        /// </summary>
        /// <param name="menus"></param>
        /// <param name="parentMenuId">第一层是空字符串</param>
        /// <param name="level">菜单等级 1 2 3 4</param>
        /// <returns></returns>
        private List<MenuCacheBo> GetMenus(List<MenuCacheBo> menus, string parentMenuId = "", int level = 1)
        {
            return menus.Where(c => c.Level == level && c.ParentMenuId == parentMenuId && c.Type == SysConstants.MenuType.Menu).Select(menu => new MenuCacheBo()
            {
                MenuId = menu.MenuId,
                MenuName = menu.MenuName,
                ParentMenuId = menu.ParentMenuId,
                Url = menu.Url,
                Type = menu.Type,
                Icon = menu.Icon,
                OrderIndex = menu.OrderIndex,
                ChildMenus = GetMenus(menus, menu.MenuId, menu.Level + 1),
                ButtonPerms = GetButtonPerms(menus, menu.MenuId)
            }).OrderBy(c => c.OrderIndex).ToList();
        }

        /// <summary>
        /// 获取当前菜单的权限组合
        /// </summary>
        /// <param name="menu"></param>
        /// <param name="menuMenuId"></param>
        /// <returns></returns>
        private List<string> GetButtonPerms(List<MenuCacheBo> menu, string menuMenuId)
        {
            return menu.Where(c => c.ParentMenuId == menuMenuId && c.Type == SysConstants.MenuType.Button && c.MenuName != "").Select(c => c.MenuName).Distinct().ToList();
        }

        /// <summary>
        /// 增加菜单
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="incrementMenu"></param>
        /// <returns></returns>
        public async Task<BaseResponseModel> AddMenu(UserCacheBo currentUser, IncrementMenuParam incrementMenu)
        {
            var responseModel = new ResponseModel<bool>();
            var sysMenu = incrementMenu.MapTo<SysMenu>();
            sysMenu.SysRoleMenuRelations = new List<SysRoleMenuRelation>()
            {
                new SysRoleMenuRelation()
                {
                    RoleId = 1
                }
            };
            if (incrementMenu.AssignAdmin)
            {
                sysMenu.SysRoleMenuRelations.Add(new SysRoleMenuRelation()
                {
                    RoleId = 2
                });
            }

            sysMenu.CreatedTime = DateTime.Now;
            sysMenu.CreatedBy = currentUser.UserId;
            sysMenu.UpdatedBy = currentUser.UserId;
            await SysMenuDao.AddAsync(sysMenu); 
            var count = await SysMenuDao.SaveChangesAsync();
            return count > 0 ? responseModel.Succeed(true) : responseModel.Fail(ResponseCode.Fail, "保存失败");
        }
    }
}