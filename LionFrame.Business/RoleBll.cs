using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LionFrame.Basic.AutofacDependency;
using LionFrame.Config;
using LionFrame.Data.BasicData;
using LionFrame.Data.SystemDao;
using LionFrame.Domain.SystemDomain;
using LionFrame.Model.RequestParam.RoleParams;
using LionFrame.Model.ResponseDto.ResultModel;
using LionFrame.Model.ResponseDto.RoleDtos;
using LionFrame.Model.ResponseDto.SystemDto;
using LionFrame.Model.SystemBo;
using Microsoft.EntityFrameworkCore;
using Z.EntityFramework.Plus;

namespace LionFrame.Business
{
    public class RoleBll : IScopedDependency
    {
        public SysRoleDao SysRoleDao { get; set; }
        public SysMenuDao SysMenuDao { get; set; }

        /// <summary>
        /// 新增角色
        /// </summary>
        /// <param name="incrementRoleParam"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public async Task<BaseResponseModel> AddRoleAsync(IncrementRoleParam incrementRoleParam, UserCacheBo currentUser)
        {
            var result = new ResponseModel<string>();
            var db = SysRoleDao.CurrentDbContext;
            SysRoleDao.CloseTracking();
            if (await db.SysRoles.CountAsync(c => c.CreatedBy == currentUser.UserId && !c.Deleted) > 500)
            {
                return result.Fail("角色上限为500个");
            }
            if (await db.SysRoles.AnyAsync(c => c.CreatedBy == currentUser.UserId
                && !c.Deleted && c.RoleName == incrementRoleParam.RoleName))
            {
                return result.Fail("角色名已存在");
            }
            var role = new SysRole()
            {
                CreatedBy = currentUser.UserId,
                CreatedTime = DateTime.Now,
                Deleted = false,
                RoleDesc = incrementRoleParam.RoleDesc ?? "",
                RoleName = incrementRoleParam.RoleName,
            };
            await db.SysRoles.AddAsync(role);

            var count = await db.SaveChangesAsync();
            return count > 0 ? result.Succeed("添加成功") : result.Fail("添加失败");
        }

        /// <summary>
        /// 修改角色
        /// </summary>
        /// <param name="incrementRoleParam"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public async Task<BaseResponseModel> ModifyRoleAsync(IncrementRoleParam incrementRoleParam, UserCacheBo currentUser)
        {
            var result = new ResponseModel<string>();
            var db = SysRoleDao.CurrentDbContext;
            if (await db.SysRoles.AnyAsync(c => c.CreatedBy == currentUser.UserId && !c.Deleted && c.RoleName == incrementRoleParam.RoleName))
            {
                return result.Fail("角色名已存在");
            }
            var role = await db.SysRoles.FirstOrDefaultAsync(c => !c.Deleted && c.RoleId == incrementRoleParam.RoleId);
            if (role == null)
            {
                return result.Fail("角色不存在", "角色不存在");
            }
            if (role.RoleId != currentUser.UserId)
            {
                return result.Fail("非自建角色只读", "非自建角色只读");
            }
            role.RoleName = incrementRoleParam.RoleName;
            role.RoleDesc = incrementRoleParam.RoleDesc ?? "";
            role.UpdatedBy = currentUser.UserId;
            role.UpdatedTime = DateTime.Now;
            var count = await db.SaveChangesAsync();

            return count > 0 ? result.Succeed("修改成功") : result.Fail("修改失败", "修改失败");
        }

        /// <summary>
        /// 获取角色及关联用户一览
        /// </summary>
        /// <param name="rolePageParam"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public async Task<PageResponse<RoleListDto>> RoleListAsync(RolePageParam rolePageParam, UserCacheBo currentUser)
        {
            PageResponse<RoleListDto> roleList = await SysRoleDao.RoleListAsync(rolePageParam, currentUser);
            return roleList;
        }

        /// <summary>
        /// 角色删除
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        [DbTransactionInterceptor]
        public virtual async Task<BaseResponseModel> RemoveRoleAsync(long roleId, UserCacheBo currentUser)
        {
            var result = new ResponseModel<string>();
            var db = SysRoleDao.CurrentDbContext;
            var role = await db.SysRoles.FirstOrDefaultAsync(c => c.RoleId == roleId && !c.Deleted);
            if (role == null)
            {
                return result.Fail("角色不存在");
            }

            if (role.RoleId == 1 || role.RoleId == 2)
            {
                return result.Fail("系统角色只读");
            }

            if (role.CreatedBy != currentUser.UserId && !currentUser.RoleIdList.Contains(1))
            {
                return result.Fail("非自建角色只读");
            }
            var count = await db.SysRoles.Where(c => c.RoleId == roleId && !c.Deleted).DeleteFromQueryAsync();
            count += await db.SysUserRoleRelations.Where(c => c.RoleId == roleId).DeleteFromQueryAsync();
            count += await db.SysRoleMenuRelations.Where(c => c.RoleId == roleId).DeleteFromQueryAsync();
            count += await db.SaveChangesAsync();
            return count > 0 ? result.Succeed("角色删除成功") : result.Fail("角色删除失败");
        }

        /// <summary>
        /// 获取当前角色关联用户
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public async Task<BaseResponseModel> GetRoleUserAsync(long roleId, UserCacheBo currentUser)
        {
            SysRoleDao.CloseTracking();
            var db = SysRoleDao.CurrentDbContext;
            var userRoleRelations = db.SysUserRoleRelations.Where(c => c.RoleId == roleId
                && !c.Deleted && c.CreatedBy == currentUser.UserId);

            var users = db.SysUsers.Where(c => c.ParentUid == currentUser.UserId);

            var data = from user in users
                       join userRole in userRoleRelations on user.UserId equals userRole.UserId into ur1
                       from ur2 in ur1.DefaultIfEmpty()
                       select new
                       {
                           roleId = ur2 == null ? -1 : ur2.RoleId,
                           userName = user.UserName
                       };
            var result = await data.Distinct().ToListAsync();
            return new ResponseModel().Succeed(result);
        }

        #region 获取当前角色能访问的按钮
        /// <summary>
        /// 获取当前角色能访问的按钮
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public async Task<BaseResponseModel> GetRoleMenuAsync(long roleId, UserCacheBo currentUser)
        {
            var menus = await SysMenuDao.GetMenusAsync(roleId, currentUser.UserId);
            var result = GetMenus(menus);
            return new ResponseModel().Succeed(result);
        }
        /// <summary>
        /// 递归获取菜单结构
        /// </summary>
        /// <param name="menus"></param>
        /// <param name="parentMenuId">第一层是空字符串</param>
        /// <param name="level">菜单等级 1 2 3 4</param>
        /// <returns></returns>
        private List<MenuPermsDto> GetMenus(List<MenuPermsDto> menus, string parentMenuId = "", int level = 1)
        {
            return menus.Where(c => c.Level == level && c.ParentMenuId == parentMenuId && c.Type == SysConstants.MenuType.Menu).Select(menu => new MenuPermsDto()
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
        /// 获取当前页面按钮的权限组合
        /// </summary>
        /// <param name="menu"></param>
        /// <param name="menuMenuId"></param>
        /// <returns></returns>
        private List<ButtonPermsDto> GetButtonPerms(List<MenuPermsDto> menu, string menuMenuId)
        {
            return menu.Where(c => c.ParentMenuId == menuMenuId && c.Type == SysConstants.MenuType.Button && c.MenuName != "").Select(c => new ButtonPermsDto()
            {
                MenuName = c.MenuName,
                MenuId = c.MenuId,
                ParentMenuId = c.ParentMenuId
            }).Distinct().ToList();
        }

        #endregion
    }
}
