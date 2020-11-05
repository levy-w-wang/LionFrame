using LionFrame.Data.BasicData;
using LionFrame.Model.RequestParam.SystemParams;
using LionFrame.Model.SystemBo;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LionFrame.Domain.SystemDomain;
using Z.EntityFramework.Plus;

namespace LionFrame.Data.SystemDao
{
    public class SysMenuDao : BaseData
    {
        /// <summary>
        /// 获取当前用户可访问的菜单
        /// </summary>
        /// <param name="roleIds"></param>
        /// <returns></returns>
        public List<MenuCacheBo> GetMenus(List<long> roleIds)
        {
            CloseTracking();
            var menus = from rlaRoleMenu in CurrentDbContext.SysRoleMenuRelations
                        join menu in CurrentDbContext.SysMenus on rlaRoleMenu.MenuId equals menu.MenuId
                        where roleIds.Contains(rlaRoleMenu.RoleId) && !rlaRoleMenu.Deleted && !menu.Deleted
                        select new MenuCacheBo()
                        {
                            MenuId = menu.MenuId,
                            MenuName = menu.MenuName,
                            ParentMenuId = menu.ParentMenuId,
                            Url = menu.Url,
                            Level = menu.Level,
                            Type = menu.Type,
                            Icon = menu.Icon,
                            OrderIndex = menu.OrderIndex,
                        };
            return menus.Distinct().ToList();
        }

        /// <summary>
        /// 更新按钮
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="incrementMenu"></param>
        /// <returns></returns>
        public async Task<bool> UpdateMenuAsync(UserCacheBo currentUser, IncrementMenuParam incrementMenu)
        {
            var count = await CurrentDbContext.SysMenus.Where(c=>c.MenuId == incrementMenu.MenuId).UpdateFromQueryAsync(c=>new SysMenu()
            {
            Deleted = incrementMenu.Deleted,
            MenuName = incrementMenu.MenuName,
            OrderIndex = incrementMenu.OrderIndex,
            Icon = incrementMenu.Icon,
            Url = incrementMenu.Url,
            UpdatedBy = currentUser.UserId,
            UpdatedTime = DateTime.Now,
            });
            return count > 0;
            //var menu = await FindAsync<SysMenu>(incrementMenu.MenuId);
            //if (menu != null)
            //{
            //    menu.Deleted = incrementMenu.Deleted;
            //    menu.MenuName = incrementMenu.MenuName;
            //    menu.OrderIndex = incrementMenu.OrderIndex;
            //    menu.Icon = incrementMenu.Icon;
            //    menu.Url = incrementMenu.Url;
            //    menu.UpdatedBy = currentUser.UserId;
            //    menu.UpdatedTime = DateTime.Now;
            //    Update(menu);
            //}
            //return await SaveChangesAsync() > 0;
            
        }
    }
}
