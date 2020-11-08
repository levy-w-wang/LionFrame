using System;
using System.Linq;
using System.Threading.Tasks;
using LionFrame.Basic.AutofacDependency;
using LionFrame.Data.SystemDao;
using LionFrame.Domain.SystemDomain;
using LionFrame.Model.RequestParam.RoleParams;
using LionFrame.Model.ResponseDto.ResultModel;
using LionFrame.Model.ResponseDto.RoleDtos;
using LionFrame.Model.SystemBo;
using Microsoft.EntityFrameworkCore;

namespace LionFrame.Business
{
    public class RoleBll : IScopedDependency
    {
        public SysRoleDao SysRoleDao { get; set; }

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
            var role = await db.SysRoles.FirstOrDefaultAsync(c => c.CreatedBy == currentUser.UserId && !c.Deleted && c.RoleId == incrementRoleParam.RoleId);
            if (role == null)
            {
                return result.Fail("角色不存在", "角色不存在");
            }
            if (await db.SysRoles.AnyAsync(c => c.CreatedBy == currentUser.UserId
                && !c.Deleted && c.RoleName == incrementRoleParam.RoleName))
            {
                return result.Fail("角色名已存在");
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
    }
}
