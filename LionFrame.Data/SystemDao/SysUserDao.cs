using EFCore.BulkExtensions;
using LionFrame.Basic.Encryptions;
using LionFrame.Data.BasicData;
using LionFrame.Domain.SystemDomain;
using LionFrame.Model;
using LionFrame.Model.RequestParam.SystemParams;
using LionFrame.Model.RequestParam.UserParams;
using LionFrame.Model.ResponseDto.ResultModel;
using LionFrame.Model.SystemBo;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LionFrame.Data.SystemDao
{
    public class SysUserDao : BaseData
    {
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="loginParam"></param>
        /// <returns></returns>
        public ResponseModel<UserCacheBo> Login(LoginParam loginParam)
        {
            CloseTracking();
            var response = new ResponseModel<UserCacheBo>();
            var user = First<SysUser>(c => c.UserName == loginParam.UserName && c.Status == 1);
            if (user == null)
            {
                return response.Fail(ResponseCode.LoginFail, "账号不存在", null);
            }
            if (user.PassWord == loginParam.Password.Md5Encrypt())
            {
                var dbData = from sysUser in CurrentDbContext.SysUsers
                             where sysUser.UserId == user.UserId && sysUser.Status == 1
                             select new UserCacheBo
                             {
                                 UserId = sysUser.UserId,
                                 UserName = sysUser.UserName,
                                 PassWord = sysUser.PassWord,
                                 Email = sysUser.Email,
                                 Sex = sysUser.Sex,
                                 RoleCacheBos = from userRoleRelation in CurrentDbContext.SysUserRoleRelations
                                                join sysRole in CurrentDbContext.SysRoles on userRoleRelation.RoleId equals sysRole.RoleId
                                                where userRoleRelation.UserId == sysUser.UserId && !userRoleRelation.Deleted && !sysRole.Deleted
                                                select new RoleCacheBo()
                                                {
                                                    RoleId = sysRole.RoleId,
                                                    RoleDesc = sysRole.RoleDesc,
                                                    Operable = sysRole.Operable,
                                                    RoleName = sysRole.RoleName,
                                                }
                             };
                // 使用include 无法使用条件判断
                //var dbData1 = CurrentDbContext.SysUsers.Where(c => c.UserId == user.UserId && c.Status == 1).Include(c => c.SysUserRoleRelations).ThenInclude(c => c.SysRole).FirstOrDefault();
                //var userCacheBo = dbData1.MapTo<UserCacheBo>();
                response.Succeed(dbData.FirstOrDefault());
                return response;
            }
            return response.Fail(ResponseCode.LoginFail, "账号或密码错误", null);
        }

        /// <summary>
        /// 根据用户名修改密码
        /// </summary>
        /// <param name="modifyPwdParam"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        public async Task<bool> ModifyPwd(ModifyPwdParam modifyPwdParam, long uid)
        {
            var result = await CurrentDbContext.SysUsers.Where(c => c.UserId == uid).BatchUpdateAsync(c => new SysUser()
            {// 使用这种方式只会修改这两个字段
                PassWord = modifyPwdParam.NewPassWord.Md5Encrypt(),
                UpdatedTime = DateTime.Now
            });
            return result > 0;
        }
    }
}
