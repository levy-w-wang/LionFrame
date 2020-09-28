using System.Collections.Generic;
using System.Linq;
using LionFrame.Config;
using LionFrame.Data.BasicData;
using LionFrame.Model.SystemBo;
using Microsoft.EntityFrameworkCore.Internal;

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
                where roleIds.Contains(rlaRoleMenu.RoleId)  && !rlaRoleMenu.Deleted && !menu.Deleted
                select new MenuCacheBo()
                {
                    RoleId = rlaRoleMenu.RoleId,
                    MenuId = menu.MenuId,
                    MenuName = menu.MenuName,
                    ParentMenuId = menu.ParentMenuId,
                    Url = menu.Url,
                    Level = menu.Level,
                    Type = menu.Type,
                    Icon = menu.Icon,
                    OrderIndex = menu.OrderIndex,
                };
            return menus.AsEnumerable().Distinct((m1,m2) => m1.MenuId == m2.MenuId).ToList();
        }
    }
}
