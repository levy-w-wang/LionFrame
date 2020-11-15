using LionFrame.Basic.Encryptions;
using LionFrame.Data.BasicData;
using LionFrame.Domain.SystemDomain;
using LionFrame.Model;
using LionFrame.Model.RequestParam.UserParams;
using LionFrame.Model.ResponseDto.ResultModel;
using LionFrame.Model.SystemBo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using LionFrame.Basic.Extensions;
using LionFrame.Model.ResponseDto.UserDtos;
using Microsoft.EntityFrameworkCore;
using Z.EntityFramework.Plus;

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
            var user = First<SysUser>(c => c.Email == loginParam.Email && c.State == 1);
            if (user == null)
            {
                return response.Fail(ResponseCode.LoginFail, "账号不存在", null);
            }
            if (user.PassWord == loginParam.Password.Md5Encrypt())
            {
                var dbData = from sysUser in CurrentDbContext.SysUsers
                             where sysUser.UserId == user.UserId && sysUser.State == 1
                             select new UserCacheBo
                             {
                                 UserId = sysUser.UserId,
                                 TenantId = sysUser.TenantId,
                                 NickName = sysUser.NickName,
                                 PassWord = sysUser.PassWord,
                                 Email = sysUser.Email,
                                 Sex = sysUser.Sex,
                                 CreatedBy = sysUser.CreatedBy,
                                 RoleCacheBos = from userRoleRelation in CurrentDbContext.SysUserRoleRelations
                                                join sysRole in CurrentDbContext.SysRoles on userRoleRelation.RoleId equals sysRole.RoleId
                                                where userRoleRelation.UserId == sysUser.UserId && !userRoleRelation.Deleted && !sysRole.Deleted
                                                && userRoleRelation.State == 1
                                                select new RoleCacheBo()
                                                {
                                                    RoleId = sysRole.RoleId,
                                                    TenantId = sysRole.TenantId,
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
            var result = await CurrentDbContext.SysUsers.Where(c => c.UserId == uid).UpdateFromQueryAsync(c => new SysUser
            {// 使用这种方式只会修改这两个字段
                PassWord = modifyPwdParam.NewPassWord.Md5Encrypt(),
                UpdatedTime = DateTime.Now,
                UpdatedBy = uid,
            });
            return result > 0;
        }
        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="retrievePwdParam"></param>
        /// <returns></returns>
        public bool RetrievePwd(RetrievePwdParam retrievePwdParam, out long uid)
        {
            var user = CurrentDbContext.SysUsers.First(c => c.Email == retrievePwdParam.Email);
            user.PassWord = retrievePwdParam.Pwd.Md5Encrypt();
            user.UpdatedTime = DateTime.Now;
            user.UpdatedBy = user.UserId;
            uid = user.UserId;//找回密码 删除token
            var result = SaveChanges();

            return result > 0;
        }

        /// <summary>
        /// 获取用户管理用户一览
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="currentPage"></param>
        /// <param name="email"></param>
        /// <param name="nickName"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public async Task<PageResponse<UserManagerDto>> GetManagerUserAsync(int pageSize, int currentPage, string email, string nickName, UserCacheBo currentUser)
        {
            CloseTracking();
            Expression<Func<SysUser, bool>> userExpression = sysUser => sysUser.State == 1 && sysUser.TenantId == currentUser.TenantId;

            if (!email.IsNullOrEmpty())
            {
                userExpression = userExpression.And(c => c.Email == email);
            }
            if (!nickName.IsNullOrEmpty())
            {
                userExpression = userExpression.And(c => c.NickName.Contains(nickName));
            }

            // 分配用户管理界面 则可以管理当前所有用户
            var sysUserQueryable = CurrentDbContext.SysUsers.Where(userExpression);
            if (currentUser.CreatedBy > 0)
            {
                //非超级管理员不读取超级管理员信息
                sysUserQueryable = sysUserQueryable.Where(c => c.CreatedBy > 0);
            }
            var result = await LoadPageEntitiesProjectToAsync<SysUser, long, UserManagerDto>(sysUserQueryable, currentPage, pageSize, false, u => u.UserId);

            var uids = result.Data.Select(c => long.Parse(c.UserId));
            var rolesQueryable = from userRoleRelation in CurrentDbContext.SysUserRoleRelations
                                 join sysRole in CurrentDbContext.SysRoles on userRoleRelation.RoleId equals sysRole.RoleId
                                 where !userRoleRelation.Deleted && userRoleRelation.State == 1 && !sysRole.Deleted && sysRole.TenantId == currentUser.TenantId  && userRoleRelation.TenantId == currentUser.TenantId
                                 && uids.Contains(userRoleRelation.UserId)
                                 select new
                                 {
                                     userRoleRelation.UserId,
                                     userRoleRelation.RoleId,
                                     sysRole.RoleName,
                                     sysRole.RoleDesc,
                                 };
            var roles = await rolesQueryable.Distinct().ToListAsync();
            result.Data.ForEach(u =>
            {
                var r = roles.Where(c => c.UserId.ToString() == u.UserId).ToList();
                u.RoleIds = r.Select(c => c.RoleId.ToString()).ToList();
                u.RoleNames = r.Select(c => c.RoleName).ToList();
            });

            return result;
        }

        /// <summary>
        /// 修改管理的用户信息
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="nickName"></param>
        /// <param name="currentUser"></param>
        /// <param name="roleIds"></param>
        /// <returns></returns>
        [DbTransactionInterceptor]
        public virtual async Task<ResponseModel<bool>> ModifyManagerUserAsync(long uid, List<long> roleIds, string nickName, UserCacheBo currentUser)
        {
            var result = new ResponseModel<bool>();
            var user = await CurrentDbContext.SysUsers.FirstOrDefaultAsync(c => c.UserId == uid && c.State == 1 && c.TenantId == currentUser.TenantId);
            if (user == null)
            {
                return result.Fail("修改的用户信息不存在");
            }
            if (user.CreatedBy == 0)
            {
                return result.Fail("管理员只读");
            }

            //获取当前用户所拥有的角色
            var existRoleIds = await CurrentDbContext.SysUserRoleRelations.Where(c => c.UserId == uid && !c.Deleted && c.State == 1 && c.TenantId == currentUser.TenantId && c.CreatedBy > 0)
                .Select(c => c.RoleId).ToListAsync();

            user.NickName = nickName;

            var dRoleIds = existRoleIds.Except(roleIds).ToList();//删除此前分配的角色 不在这次保存角色中的
            var iRoleIds = roleIds.Except(existRoleIds).ToList();//插入这次新增的不在之前角色中的
            //相同部分不做改变
            if (dRoleIds.Any())
            {
                // 删除
                await CurrentDbContext.SysUserRoleRelations.Where(c => c.UserId == uid && dRoleIds.Contains(c.RoleId)&& c.TenantId == currentUser.TenantId).DeleteFromQueryAsync();
            }

            if (iRoleIds.Count > 0)
            {
                //插入
                var userRoleRelations = new List<SysUserRoleRelation>();
                iRoleIds.ForEach(roleId =>
                {
                    userRoleRelations.Add(new SysUserRoleRelation()
                    {
                        TenantId = currentUser.TenantId,
                        CreatedBy = currentUser.UserId,
                        CreatedTime = DateTime.Now,
                        RoleId = roleId,
                        State = 1,
                        UserId = uid,
                    });
                });
                await CurrentDbContext.SysUserRoleRelations.AddRangeAsync(userRoleRelations);
            }

            var count = await SaveChangesAsync();
            return count > 0 ? result.Succeed(true) : result.Fail("保存修改信息失败");
        }
    }
}
