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
                                                && userRoleRelation.State == 1
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
            var result = await CurrentDbContext.SysUsers.Where(c => c.UserId == uid).UpdateFromQueryAsync(c => new SysUser()
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
        /// <param name="userName"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public async Task<PageResponse<UserManagerDto>> GetManagerUserAsync(int pageSize, int currentPage, string email, string userName, UserCacheBo currentUser)
        {
            CloseTracking();
            Expression<Func<SysUser, bool>> userExpression = sysUser => sysUser.Status == 1;

            if (!email.IsNullOrEmpty())
            {
                userExpression = userExpression.And(c => c.Email == email);
            }
            if (!userName.IsNullOrEmpty())
            {
                userExpression = userExpression.And(c => userName.Contains(c.UserName));
            }

            var sysUserQueryable = CurrentDbContext.SysUsers.Where(userExpression);
            var userConcat = sysUserQueryable.Where(c => c.UserId == currentUser.UserId)
                .Union(sysUserQueryable.Where(c => c.ParentUid == currentUser.UserId));
            var result = await LoadPageEntitiesProjectToAsync<SysUser, long, UserManagerDto>(userConcat, currentPage, pageSize, false, u => u.UserId);

            var uids = result.Data.Select(c => long.Parse(c.UserId));
            var rolesQueryable = from userRoleRelation in CurrentDbContext.SysUserRoleRelations
                                 join sysRole in CurrentDbContext.SysRoles on userRoleRelation.RoleId equals sysRole.RoleId
                                 where !userRoleRelation.Deleted && userRoleRelation.State == 1 && !sysRole.Deleted
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
        /// <param name="email"></param>
        /// <param name="currentUser"></param>
        /// <param name="roleIds"></param>
        /// <returns></returns>
        [DbTransactionInterceptor]
        public virtual async Task<ResponseModel<bool>> ModifyManagerUserAsync(long uid, List<long> roleIds, string email, UserCacheBo currentUser)
        {
            var result = new ResponseModel<bool>();
            var user = await CurrentDbContext.SysUsers.FirstOrDefaultAsync(c => c.UserId == uid && c.Status == 1 && c.ParentUid == currentUser.UserId);
            if (user == null)
            {
                return result.Fail("修改的用户信息不存在");
            }
            //获取当前用户所拥有的角色
            var existRoleIds = await CurrentDbContext.SysUserRoleRelations.Where(c => c.UserId == uid && !c.Deleted && c.State == 1)
                .Select(c => c.RoleId).ToListAsync();
            //种子数据 2是管理员 1是系统管理员
            if (existRoleIds.Contains(1) || existRoleIds.Contains(2))
            {
                return result.Fail("管理员只读");
            }

            if (await ExistAsync<SysUser>(c => c.UserId != uid && c.Status == 1 && c.Email == email))
            {
                return result.Fail("修改的邮箱已存在");
            }

            user.Email = email;

            var dRoleIds = existRoleIds.Except(roleIds).ToList();//删除此前分配的角色 不在这次保存角色中的
            var iRoleIds = roleIds.Except(existRoleIds).ToList();//插入这次新增的不在之前角色中的
            //相同部分不做改变
            if (dRoleIds.Any())
            {
                // 删除
                await CurrentDbContext.SysUserRoleRelations.Where(c => c.UserId == uid && dRoleIds.Contains(c.RoleId)).DeleteFromQueryAsync();
            }

            if (iRoleIds.Count > 0)
            {
                //插入
                var userRoleRelations = new List<SysUserRoleRelation>();
                iRoleIds.ForEach(roleId =>
                {
                    userRoleRelations.Add(new SysUserRoleRelation()
                    {
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
