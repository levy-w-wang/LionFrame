using LionFrame.Basic;
using LionFrame.Basic.AutofacDependency;
using LionFrame.Basic.Encryptions;
using LionFrame.Basic.Extensions;
using LionFrame.Config;
using LionFrame.CoreCommon;
using LionFrame.CoreCommon.AutoMapperCfg;
using LionFrame.CoreCommon.Cache;
using LionFrame.CoreCommon.Cache.Redis;
using LionFrame.CoreCommon.CustomException;
using LionFrame.Data.SystemDao;
using LionFrame.Domain.SystemDomain;
using LionFrame.Model;
using LionFrame.Model.RequestParam.UserParams;
using LionFrame.Model.ResponseDto.ResultModel;
using LionFrame.Model.ResponseDto.SystemDto;
using LionFrame.Model.SystemBo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LionFrame.Data.BasicData;
using LionFrame.Model.ResponseDto.UserDtos;
using Microsoft.EntityFrameworkCore;

namespace LionFrame.Business
{
    /// <summary>
    /// 用户相关业务层
    /// </summary>
    public class UserBll : IScopedDependency
    {
        public SysUserDao SysUserDao { get; set; }
        public SystemBll SystemBll { get; set; }
        public RedisClient RedisClient { get; set; }
        public LionMemoryCache LionMemoryCache { get; set; }
        public IdWorker IdWorker { get; set; }

        /// <summary>
        /// 用户登录 业务层
        /// </summary>
        /// <param name="loginParam"></param>
        /// <returns></returns>
        public async Task<ResponseModel<UserDto>> LoginAsync(LoginParam loginParam)
        {
            var result = new ResponseModel<UserDto>();
            var verificationResult = await SystemBll.VerificationLoginAsync(loginParam);
            if (verificationResult != "验证通过")
            {
                result.Fail(ResponseCode.LoginFail, verificationResult, null);
                return result;
            }

            var responseResult = await SysUserDao.LoginAsync(loginParam);
            if (responseResult.Success)
            {
                var userCache = responseResult.Data;
                CreateTokenCache(userCache);
                var userDto = userCache.MapTo<UserDto>();
                result.Succeed(userDto);
            }

            return result;
        }

        /// <summary>
        /// 重新生成缓存和token
        /// </summary>
        /// <param name="userCache"></param>
        private static void CreateTokenCache(UserCacheBo userCache)
        {
            // 将某些信息存到token的payload中，此处是放的内存缓存信息  可替换成其它的
            // aes加密的原因是避免直接被base64反编码看见
            //var encryptMsg = userCache.ToJson().EncryptAES();
            userCache.SessionId = Guid.NewGuid().ToString("N");
            var dic = new Dictionary<string, string>()
            {
                { "uid", userCache.UserId.ToString() },
                { "tenantId", userCache.TenantId.ToString() },
                { "sessionId", userCache.SessionId } //一个账号仅允许在一个地方登录
            };
            var token = TokenManager.GenerateToken(dic.ToJson(), 3 * 24);
            LionWeb.HttpContext.Response.Headers["token"] = token;
            userCache.UserToken = token;
            LionWeb.HttpContext.Response.Headers["Access-Control-Expose-Headers"] = "token"; //多个以逗号分隔 eg:token,sid

            LionUserCache.CreateUserCache(userCache);
        }

        /// <summary>
        /// 验证是否存在该用户数据
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task<bool> ExistUserAsync(string email)
        {
            return await SysUserDao.ExistAsync<SysUser>(c => c.Email == email && c.State == 1);
        }

        /// <summary>
        /// 验证验证码
        /// </summary>
        /// <param name="prefixKey"></param>
        /// <param name="email"></param>
        /// <param name="sendCaptcha"></param>
        /// <param name="deleteCaptcha">验证码是否在验证过一次后就删除 不管是否验证通过</param>
        /// <returns></returns>
        public async Task<string> VerificationCaptchaAsync(string prefixKey, string email, string sendCaptcha, bool deleteCaptcha = true)
        {
            var key = prefixKey + email;
            var captcha = await RedisClient.GetAsync<string>(key);
            if (captcha == null)
            {
                return "验证码错误或已失效";
            }

            var str = !string.Equals(sendCaptcha, captcha, StringComparison.OrdinalIgnoreCase) ? "验证码错误" : "验证通过";
            // 当验证通过 或设定 只要获取过验证码就删除  重新获取验证码
            if (str == "验证通过" || deleteCaptcha)
            {
                await RedisClient.DeleteAsync(key);
            }

            return str;
        }

        /// <summary>
        /// 是否频繁获取验证码
        /// </summary>
        /// <param name="key"></param>
        /// <param name="expireMinutes">过期时间</param>
        /// <param name="bufferMinutes">缓冲时间</param>
        /// <returns></returns>
        private async Task<bool> FrequentlyGetCaptchaAsync(string key, int expireMinutes, int bufferMinutes)
        {
            if (await RedisClient.ExistAsync(key))
            {
                var timeSpanLess = await RedisClient.KeyTimeToLiveAsync(key);
                if (timeSpanLess.TotalMinutes + bufferMinutes > expireMinutes)
                {
                    return true;
                }
            }

            return false;
        }


        #region 注册用户

        /// <summary>
        /// 得到验证码
        /// </summary>
        /// <param name="emailTo"></param>
        /// <returns></returns>
        public async Task<string> GetRegisterCaptchaAsync(string emailTo)
        {
            var key = $"{CacheKeys.REGISTERCAPTCHA}{emailTo}";
            var expireMinutes = 5;

            if (await FrequentlyGetCaptchaAsync(key, expireMinutes, 2))
            {
                return "获取验证码过于频繁，请稍后再获取";
            }

            var captchaNumber = CaptchaHelper.CreateRandomNumber(6);
            var htmlEmail = SystemBll.GetMailContent("注册账号", captchaNumber, expireMinutes);
            var result = await SystemBll.SendSystemMailAsync("LionFrame-注册账号", htmlEmail, emailTo);
            if (result.Contains("发送成功"))
            {
                await RedisClient.SetAsync(key, captchaNumber, new TimeSpan(0, expireMinutes, 0));
            }

            return result;
        }

        /// <summary>
        /// 注册用户
        /// </summary>
        /// <param name="registerUserParam"></param>
        /// <returns></returns>
        [DbTransactionInterceptor]
        public virtual async Task<ResponseModel<bool>> RegisterUserAsync(RegisterUserParam registerUserParam)
        {
            var result = new ResponseModel<bool>();
            var verificationResult = await VerificationCaptchaAsync(CacheKeys.REGISTERCAPTCHA, registerUserParam.Email, registerUserParam.Captcha, false);
            if (verificationResult != "验证通过")
            {
                result.Fail(ResponseCode.Fail, verificationResult, false);
                return result;
            }

            if (await ExistUserAsync(registerUserParam.Email))
            {
                result.Fail(ResponseCode.Fail, "该邮箱已存在，请切换或找回密码", false);
                return result;
            }

            var count = await SaveRegisterUserAsync(registerUserParam);
            if (count > 0)
            {
                return result.Succeed(true);
            }

            return result.Fail(ResponseCode.Fail, "保存失败，请稍后再试", false);
        }

        /// <summary>
        /// 保存注册用户
        /// </summary>
        /// <param name="registerUserParam"></param>
        /// <returns></returns>
        private async Task<int> SaveRegisterUserAsync(RegisterUserParam registerUserParam)
        {
            var tenantUser = new SysTenant()
            {
                //TenantId = IdWorker.NextId(),
                CreatedTime = DateTime.Now,
                TenantName = registerUserParam.TenantName,
                Remark = "注册",
                State = 1,
            };
            SysUserDao.Add(tenantUser);
            await SysUserDao.SaveChangesAsync();

            var currentRoleMenu = new List<SysRoleMenuRelation>();
            SeedData.InitNormalRoleMenuRelations.ForEach(roleMenu =>
            {
                currentRoleMenu.Add(new SysRoleMenuRelation()
                {
                    TenantId = tenantUser.TenantId,
                    CreatedTime = DateTime.Now,
                    MenuId = roleMenu.MenuId,
                    State = 1,
                });
            });

            var sysUser = new SysUser()
            {
                NickName = registerUserParam.NickName,
                PassWord = registerUserParam.PassWord.Md5Encrypt(),
                Email = registerUserParam.Email,
                Sex = 1,
                State = 1,
                CreatedTime = DateTime.Now,
                UpdatedTime = DateTime.Now,
                UpdatedBy = 0,
                CreatedBy = 0,
                TenantId = tenantUser.TenantId,
                SysUserRoleRelations = new List<SysUserRoleRelation>()
                {
                    new SysUserRoleRelation()
                    {
                        State = 1,
                        TenantId = tenantUser.TenantId,
                        CreatedTime = DateTime.Now,
                        SysRole = new SysRole()
                        {
                            CreatedTime = DateTime.Now,
                            RoleName = "超级管理员",
                            RoleDesc = string.Empty,
                            TenantId = tenantUser.TenantId,
                            SysRoleMenuRelations = currentRoleMenu
                        }
                    }
                }
            };
            SysUserDao.Add(sysUser);
            //var role = new SysRole()
            //{
            //    CreatedTime = DateTime.Now,
            //    RoleName = "超级管理员",
            //    RoleDesc = String.Empty,
            //    TenantId = tenantUser.TenantId,

            //};

            var count = await SysUserDao.SaveChangesAsync();
            return count;
        }

        #endregion

        #region 找回密码

        /// <summary>
        /// 发送验证码 - 找回密码
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task<ResponseModel<string>> SendEmailAsync(string email)
        {
            var response = new ResponseModel<string>();
            if (!email.IsEmail())
            {
                return response.Fail("邮箱格式错误");
            }

            var key = $"{CacheKeys.RETRIEVEPWDCAPTCHA}{email}";
            var expireMinutes = 5;
            if (await FrequentlyGetCaptchaAsync(key, expireMinutes, 2))
            {
                return response.Fail("获取验证码过于频繁，请稍后再获取");
            }

            if (!await ExistUserAsync(email))
            {
                return response.Fail("该邮箱尚未注册或无效");
            }

            var captchaNumber = CaptchaHelper.CreateRandomNumber(6);
            var htmlEmail = SystemBll.GetMailContent("找回密码", captchaNumber, expireMinutes);
            var result = await SystemBll.SendSystemMailAsync("LionFrame-找回密码", htmlEmail, email);
            if (result.Contains("发送成功"))
            {
                await RedisClient.SetAsync(key, captchaNumber, new TimeSpan(0, expireMinutes, 0));
            }

            return response.Succeed("发送成功");
        }

        /// <summary>
        /// 找回密码 -- 修改当前用户密码
        /// </summary>
        /// <param name="retrievePwdParam"></param>
        /// <returns></returns>
        public async Task<ResponseModel<bool>> RetrievePwdAsync(RetrievePwdParam retrievePwdParam)
        {
            var result = new ResponseModel<bool>();
            var verificationResult = await VerificationCaptchaAsync(CacheKeys.RETRIEVEPWDCAPTCHA, retrievePwdParam.Email, retrievePwdParam.Captcha, false);
            if (verificationResult != "验证通过")
            {
                return result.Fail(verificationResult, false);
            }

            var uid = await SysUserDao.RetrievePwdAsync(retrievePwdParam);

            await LogoutAsync(new UserCacheBo()
            {
                UserId = uid
            });

            return result.Succeed(true);
        }

        #endregion

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="modifyPwdParam"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public async Task<ResponseModel<string>> ModifyPwdAsync(ModifyPwdParam modifyPwdParam, UserCacheBo currentUser)
        {
            var result = new ResponseModel<string>();
            if (modifyPwdParam.NewPassWord == modifyPwdParam.OldPassWord)
            {
                return result.Fail(ResponseCode.Fail, "不允许原密码和新密码相同的修改");
            }

            if (modifyPwdParam.OldPassWord.Md5Encrypt() == currentUser.PassWord)
            {
                var updateResult = await SysUserDao.ModifyPwdAsync(modifyPwdParam, currentUser.UserId);
                if (updateResult)
                {
                    // 修改成功 登出 重新登录
                    await LogoutAsync(currentUser);
                    return result.Succeed("修改成功,请重新登录");
                }

                return result.Fail(ResponseCode.Fail, "修改失败，请稍后再试");
            }

            return result.Fail(ResponseCode.Fail, "原密码不正确");
        }

        /// <summary>
        /// 登出 -- 注销当前信息
        /// </summary>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public async Task LogoutAsync(UserCacheBo currentUser)
        {
            await RedisClient.DeleteAsync(CacheKeys.USER + currentUser.UserId);
            await RedisClient.DeleteAsync(CacheKeys.MENU_TREE + currentUser.UserId);
            LionMemoryCache.Remove(CacheKeys.USER + currentUser.UserId);
            LionMemoryCache.Remove(CacheKeys.MENU_TREE + currentUser.UserId);
        }

        /// <summary>
        /// 用户管理获取一览数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pageSize"></param>
        /// <param name="currentPage"></param>
        /// <param name="email"></param>
        /// <param name="nickName"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public async Task<PageResponse<UserManagerDto>> GetManagerUserAsync(int pageSize, int currentPage, string email, string nickName, UserCacheBo currentUser)
        {
            var result = await SysUserDao.GetManagerUserAsync(pageSize, currentPage, email, nickName, currentUser);
            return result;
        }

        /// <summary>
        /// 检查角色是否可操作
        /// </summary>
        /// <param name="roleIds"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public async Task<string> CheckRoleIds(List<long> roleIds, UserCacheBo currentUser)
        {
           var userRoles = await SysUserDao.CurrentDbContext.SysUserRoleRelations.Where(c => c.TenantId == currentUser.TenantId && !c.Deleted && roleIds.Contains(c.RoleId) && c.State == 1).ToListAsync();
           if (userRoles.Any(c=>c.CreatedBy <= 0))
           {
               return "系统角色只读";
           }

           if (userRoles.Any(c=>currentUser.RoleIdList.Contains(c.RoleId)))
           {
               return "自身角色不可操作";
           }

           return "";
        }

        /// <summary>
        /// 用户管理界面创建用户
        /// </summary>
        /// <param name="param"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public async Task<BaseResponseModel> CreateManagerUserAsync(CreateUserParam param, UserCacheBo currentUser)
        {
            var result = new ResponseModel<bool>();

            // 考虑是否验证上传的角色是当前租户下的
            var checkResult = await CheckRoleIds(param.RoleIds, currentUser);
            if (!checkResult.IsNullOrEmpty())
            {
                return result.Fail(checkResult);
            }

            if (await ExistUserAsync(param.Email))
            {
                result.Fail(ResponseCode.Fail, "该邮箱已存在", false);
                return result;
            }

            var sysUser = new SysUser()
            {
                NickName = param.NickName,
                PassWord = param.Pwd.Md5Encrypt(),
                Email = param.Email,
                Sex = 1,
                State = 1,
                CreatedTime = DateTime.Now,
                UpdatedTime = DateTime.Now,
                TenantId = currentUser.TenantId,
                UpdatedBy = 0,
                CreatedBy = currentUser.UserId,
                SysUserRoleRelations = new List<SysUserRoleRelation>()
            };
            param.RoleIds.ForEach(roleId =>
            {
                sysUser.SysUserRoleRelations.Add(new SysUserRoleRelation()
                {
                    TenantId = currentUser.TenantId,
                    RoleId = roleId,
                    State = 1,
                    CreatedTime = DateTime.Now,
                    CreatedBy = currentUser.UserId,
                    Deleted = false,
                });
            });
            SysUserDao.Add(sysUser);
            var count = await SysUserDao.SaveChangesAsync();
            return count > 0 ? result.Succeed(true) : result.Fail("保存失败", false);
        }

        /// <summary>
        /// 用户管理界面编辑用户信息
        /// </summary>
        /// <param name="modifyUserParam"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public async Task<BaseResponseModel> ModifyManagerUserAsync(ModifyUserParam modifyUserParam, UserCacheBo currentUser)
        {
             // 考虑是否验证上传的角色是当前租户下的
             var checkResult = await CheckRoleIds(modifyUserParam.RoleIds, currentUser);
            if (!checkResult.IsNullOrEmpty())
            {
                return new ResponseModel().Fail(checkResult);
            }
            var  result = await SysUserDao.ModifyManagerUserAsync(modifyUserParam.UserId, modifyUserParam.RoleIds, modifyUserParam.NickName, currentUser);
            return result;
        }

        /// <summary>
        /// 用户管理界面删除用户
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        [DbTransactionInterceptor]
        public virtual async Task<BaseResponseModel> RemoveManagerUserAsync(long uid, UserCacheBo currentUser)
        {
            var result = new ResponseModel<bool>();
            var db = SysUserDao.CurrentDbContext;

            var deleteUser = await db.SysUsers.FirstOrDefaultAsync(c => c.UserId == uid && c.TenantId == currentUser.UserId && c.State == 1);
            if (deleteUser == null)
            {
                return result.Fail("账号不存在");
            }
            // 管理员创建者都是0  注册的
            if (deleteUser.CreatedBy <= 0)
            {
                return result.Fail("管理员只读");
            }

            var count = 0;
            count += await db.SysUsers.Where(c => c.UserId == uid && c.TenantId == currentUser.TenantId).DeleteFromQueryAsync();
            count += await db.SysUserRoleRelations.Where(c => c.UserId == uid && c.TenantId == currentUser.TenantId).DeleteFromQueryAsync();

            count += await db.SaveChangesAsync();
            return count > 0 ? result.Succeed(true) : result.Fail("删除失败");
        }
    }
}