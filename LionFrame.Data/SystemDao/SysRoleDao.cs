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
            Expression<Func<SysRole, bool>> roleExpression = s => !s.Deleted;
            if (!rolePageParam.RoleName.IsNullOrEmpty())
            {
                roleExpression = roleExpression.And(c => rolePageParam.RoleName.Contains(c.RoleName));
            }

            var roleIds = currentUser.RoleIdList;
            var tempQueryable = CurrentDbContext.SysRoles.Where(roleExpression);
            var rolesQueryable = tempQueryable.Where(c => roleIds.Contains(c.RoleId))
                .Union(tempQueryable.Where(r => r.CreatedBy == currentUser.UserId));
            var resultQueryable = from sysRole in rolesQueryable
                                  select new RoleListDto()
                                  {
                                      RoleId = sysRole.RoleId.ToString(),
                                      RoleDesc = sysRole.RoleDesc,
                                      RoleName = sysRole.RoleName,
                                      UserNames = from userRole in CurrentDbContext.SysUserRoleRelations
                                                  join sysUser in CurrentDbContext.SysUsers on userRole.UserId equals sysUser.UserId
                                                  where userRole.RoleId == sysRole.RoleId && !userRole.Deleted
                                                      && userRole.State == 1 && sysUser.Status == 1 
                                                      && (sysUser.ParentUid == currentUser.UserId || sysUser.UserId == currentUser.UserId)
                                                  select sysUser.UserName
                                  };
            var result = await LoadPageEntitiesAsync(resultQueryable, rolePageParam.CurrentPage, rolePageParam.PageSize, false, c => c.RoleId);
            return result;
        }
    }
}
