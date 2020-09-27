using LionFrame.Basic.Encryptions;
using LionFrame.Data.BasicData;
using LionFrame.Domain.SystemDomain;
using LionFrame.Model;
using LionFrame.Model.RequestParam;
using LionFrame.Model.ResponseDto.ResultModel;
using LionFrame.Model.SystemBo;
using System.Collections.Generic;
using System.Linq;

namespace LionFrame.Data.SystemDao
{
    public class SysUserDao : BaseData
    {
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="loginParam"></param>
        /// <returns></returns>
        public ResponseModel Login(LoginParam loginParam)
        {
            CloseTracking();
            var response = new ResponseModel();
            var user = First<SysUser>(c => c.UserName == loginParam.UserName && c.Status == 1);
            if (user == null)
            {
                return response.Fail(ResponseCode.LoginFail, "账号不存在");
            }
            if (user.PassWord == loginParam.PassWord.Md5Encrypt())
            {
                var dbData = from sysUser in CurrentDbContext.SysUsers
                             where sysUser.UserId == user.UserId && sysUser.Status == 1
                             select new UserCacheBo
                             {
                                 UserId = sysUser.UserId,
                                 UserName = sysUser.UserName,
                                 PassWord = sysUser.PassWord,
                                 Email = sysUser.Email,
                                 RoleCacheBos = from userRoleRelation in CurrentDbContext.SysUserRoleRelations
                                                join sysRole in CurrentDbContext.SysRoles on userRoleRelation.RoleId equals sysRole.RoleId
                                                where userRoleRelation.UserId == sysUser.UserId && !userRoleRelation.Deleted && !sysRole.Deleted
                                                select new RoleCacheBo()
                                                {
                                                    RoleId = sysRole.RoleId,
                                                    RoleDesc = sysRole.RoleDesc,
                                                    RoleName = sysRole.RoleName,
                                                }
                             };
                // 使用include 无法使用条件判断
                //var dbData1 = CurrentDbContext.SysUsers.Where(c => c.UserId == user.UserId && c.Status == 1).Include(c => c.SysUserRoleRelations).ThenInclude(c => c.SysRole).FirstOrDefault();
                //var userCacheBo = dbData1.MapTo<UserCacheBo>();
                response.Succeed(dbData.FirstOrDefault());
                return response;
            }
            return response.Fail(ResponseCode.LoginFail, "账号或密码错误");
        }
    }
}
