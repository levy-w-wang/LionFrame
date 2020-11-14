using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LionFrame.Basic.AutofacDependency;
using LionFrame.Basic.Extensions;
using LionFrame.Data.BasicData;
using LionFrame.Data.SystemDao;
using LionFrame.Domain.SystemDomain;
using LionFrame.Model.RequestParam.RoleParams;
using LionFrame.Model.ResponseDto.ResultModel;
using LionFrame.Model.ResponseDto.RoleDtos;
using LionFrame.Model.SystemBo;
using Microsoft.EntityFrameworkCore;
using Z.EntityFramework.Plus;

namespace LionFrame.Business
{
    public class RoleBll : IScopedDependency
    {
        public SysRoleDao SysRoleDao { get; set; }
        public SysMenuDao SysMenuDao { get; set; }
        public MenuBll MenuBll { get; set; }

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
            if (await db.SysRoles.AnyAsync(c => c.CreatedBy == currentUser.UserId && !c.Deleted && c.RoleName == incrementRoleParam.RoleName && c.RoleId != incrementRoleParam.RoleId))
            {
                return result.Fail("角色名已存在");
            }

            var checkResult = await RoleIdCheckAsync(incrementRoleParam.RoleId, currentUser);
            if (!checkResult.IsNullOrEmpty())
            {
                return result.Fail(checkResult);
            }

            var role = await db.SysRoles.FirstOrDefaultAsync(c => !c.Deleted && c.RoleId == incrementRoleParam.RoleId);

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
        /// 用户管理界面获取可关联角色
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public async Task<PageResponse<RoleListDto>> GetCanRelationRoleList(UserCacheBo currentUser)
        {
            PageResponse<RoleListDto> roleList = await SysRoleDao.GetCanRelationRoleList(currentUser);
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

            var checkResult = await RoleIdCheckAsync(roleId, currentUser);
            if (!checkResult.IsNullOrEmpty())
            {
                return result.Fail(checkResult);
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
                           userId = user.UserId,
                           userName = user.UserName
                       };
            var result = await data.Distinct().ToListAsync();
            return new ResponseModel().Succeed(result);
        }

        /// <summary>
        /// 获取当前角色能访问的按钮
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public async Task<BaseResponseModel> GetRoleMenuAsync(long roleId, UserCacheBo currentUser)
        {
            var menus = await SysMenuDao.GetMenusAsync(roleId, currentUser.UserId);
            var result = MenuBll.GetMenuList(menus);
            return new ResponseModel().Succeed(result);
        }

        /// <summary>
        /// 修改当前角色能访问的页面
        /// </summary>
        /// <param name="roleMenuParam"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        [DbTransactionInterceptor]
        public virtual async Task<BaseResponseModel> ModifyRoleMenuAsync(ModifyRoleMenuParam roleMenuParam, UserCacheBo currentUser)
        {
            var result = new ResponseModel<string>();

            var checkResult = await RoleIdCheckAsync(roleMenuParam.RoleId, currentUser);
            if (!checkResult.IsNullOrEmpty())
            {
                return result.Fail(checkResult);
            }
            var currentUserMenus = await MenuBll.GetCurrentMenuAsync(currentUser);
            var currentMenuIds = currentUserMenus.Select(c => c.MenuId).ToList();
            //检查提交的按钮是否存在当前用户管理菜单数据中，只操作存在部分 即交集
            var crossMenuIds = roleMenuParam.MenuIds.Intersect(currentMenuIds).ToList();

            // 这里不是做的全量操作，而且新增部分做插入 数据库中的不在提交数据中的部分做删除  提交的和数据库相交叉的部分不做变动
            var dbMenuIds = await SysMenuDao.LoadEntities<SysRoleMenuRelation>(c => c.RoleId == roleMenuParam.RoleId).Select(c => c.MenuId).ToListAsync();
            var dIds = dbMenuIds.Except(crossMenuIds).ToList();//需要删除的权限
            var iIds = crossMenuIds.Except(dbMenuIds).ToList();//需要插入的新权限

            var db = SysRoleDao.CurrentDbContext;
            if (dIds.Count > 0)
            {
                await db.SysRoleMenuRelations.Where(c => dIds.Contains(c.MenuId) && c.RoleId == roleMenuParam.RoleId).DeleteAsync();
            }
            if (iIds.Count > 0)
            {
                var roleMenuRelations = iIds.Select(iId => new SysRoleMenuRelation()
                {
                    CreatedBy = currentUser.UserId,
                    CreatedTime = DateTime.Now,
                    MenuId = iId,
                    State = 1,
                    Deleted = false,
                    RoleId = roleMenuParam.RoleId,
                    UpdatedBy = 0,
                    UpdatedTime = DateTime.Now,
                })
                    .ToList();
                await db.SysRoleMenuRelations.AddRangeAsync(roleMenuRelations);
            }
            await db.SaveChangesAsync();
            return result.Succeed("修改成功");
        }

        /// <summary>
        /// 校验当前角色是否能操作
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        private async Task<string> RoleIdCheckAsync(long roleId, UserCacheBo currentUser)
        {
            if (roleId == 1 || roleId == 2)
            {
                return "系统角色只读";
            }
            var role = await SysRoleDao.FirstAsync<SysRole>(c => c.RoleId == roleId && !c.Deleted);
            if (role == null)
            {
                return "角色不存在";
            }
            if (role.CreatedBy == currentUser.UserId || currentUser.RoleIdList.Contains(1))
            {
                return "";
            }
            return "操作异常，请刷新重试";
        }

        /// <summary>
        /// 修改角色所关联的用户
        /// </summary>
        /// <param name="param"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        [DbTransactionInterceptor]
        public virtual async Task<BaseResponseModel> ModifyUserRoleAsync(ModifyUserRoleParam param, UserCacheBo currentUser)
        {
            var result = new ResponseModel<string>();
            var checkResult = await RoleIdCheckAsync(param.RoleId, currentUser);
            if (!checkResult.IsNullOrEmpty())
            {
                return result.Fail(checkResult);
            }

            var db = SysRoleDao.CurrentDbContext;
            var dbUserIds = await db.SysUsers.Where(c => c.ParentUid == currentUser.UserId).Select(c => c.UserId).ToListAsync();
            var crossUserIds = param.UserIds.Intersect(dbUserIds).ToList();//检查修改的用户是否是自己管理的用户ID，谨防其它手段上传的数据
            //直接全量修改 原有角色用户关系删除 直接新增现加的
            await db.SysUserRoleRelations.Where(c => c.RoleId == param.RoleId && c.CreatedBy == currentUser.UserId).DeleteFromQueryAsync();
            if (crossUserIds.Count > 0)
            {
                var userRoles = new List<SysUserRoleRelation>();
                crossUserIds.ForEach(uid =>
                {
                    userRoles.Add(new SysUserRoleRelation()
                    {
                        UserId = uid,
                        RoleId = param.RoleId,
                        State = 1,
                        CreatedBy = currentUser.UserId,
                        CreatedTime = DateTime.Now,
                        Deleted = false,
                        UpdatedBy = 0,
                        UpdatedTime = DateTime.Now,
                    });
                });
                await db.SysUserRoleRelations.BulkInsertAsync<SysUserRoleRelation>(userRoles);
            }
            await db.SaveChangesAsync();
            return result.Succeed("修改成功");
        }
    }
}
