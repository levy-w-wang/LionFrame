using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LionFrame.Basic.Extensions;
using LionFrame.Data.BasicData;
using LionFrame.Domain.SystemDomain;
using LionFrame.Model.RequestParam.RoleParams;
using LionFrame.Model.ResponseDto.ResultModel;
using LionFrame.Model.ResponseDto.RoleDtos;
using LionFrame.Model.SystemBo;

namespace LionFrame.Data.SystemDao
{
    public class SysRoleDao : BaseData
    {
        /// <summary>
        /// 获取角色及关联一览
        /// </summary>
        /// <param name="rolePageParam"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public async Task<PageResponse<RoleListDto>> RoleListAsync(RolePageParam rolePageParam, UserCacheBo currentUser)
        {
            CloseTracking();
            Expression<Func<SysRole, bool>> roleExpression = s => !s.Deleted && s.TenantId == currentUser.TenantId;
            if (!rolePageParam.RoleName.IsNullOrEmpty())
            {
                roleExpression = roleExpression.And(c => c.RoleName.Contains(rolePageParam.RoleName));
            }

            var tempQueryable = CurrentDbContext.SysRoles.Where(roleExpression);
            if (currentUser.CreatedBy > 0)
            {
                // 非管理员不看系统创建的角色
                tempQueryable = tempQueryable.Where(c => c.CreatedBy > 0);
            }

            var resultQueryable = from sysRole in tempQueryable
                select new RoleListDto()
                {
                    RoleId = sysRole.RoleId.ToString(),
                    RoleDesc = sysRole.RoleDesc,
                    RoleName = sysRole.RoleName,
                    NickNames = from userRole in CurrentDbContext.SysUserRoleRelations
                        join sysUser in CurrentDbContext.SysUsers on userRole.UserId equals sysUser.UserId
                        where userRole.RoleId == sysRole.RoleId && !userRole.Deleted && userRole.TenantId == currentUser.TenantId && userRole.State == 1 && sysUser.State == 1 && sysUser.TenantId == currentUser.TenantId
                        select sysUser.NickName
                };
            var result = await LoadPageEntitiesAsync(resultQueryable, rolePageParam.CurrentPage, rolePageParam.PageSize, false, c => c.RoleId);
            return result;
        }

        /// <summary>
        /// 用户管理界面获取可关联角色 能看到这个界面的 就有管理所有当前租户下的权限 除系统创建角色外
        /// </summary>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public async Task<PageResponse<RoleListDto>> GetCanRelationRoleList(UserCacheBo currentUser)
        {
            CloseTracking();
            Expression<Func<SysRole, bool>> roleExpression = s => !s.Deleted && s.TenantId == currentUser.TenantId && s.CreatedBy > 0;
            var tempQueryable = CurrentDbContext.SysRoles.Where(roleExpression);
            if (currentUser.CreatedBy > 0)
            {
                tempQueryable = tempQueryable.Where(c => !currentUser.RoleIdList.Contains(c.RoleId));
            }
            var resultQueryable = from sysRole in tempQueryable
                select new RoleListDto()
                {
                    RoleId = sysRole.RoleId.ToString(),
                    RoleName = sysRole.RoleName,
                };
            var result = await LoadPageEntitiesAsync(resultQueryable, 1, 1000, false, c => c.RoleId);
            return result;
        }
    }
}