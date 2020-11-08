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

        /// <summary>
        /// 用户登录 业务层
        /// </summary>
        /// <param name="loginParam"></param>
        /// <returns></returns>
        public ResponseModel<UserDto> Login(LoginParam loginParam)
        {
            var result = new ResponseModel<UserDto>();
            var verificationResult = SystemBll.VerificationLogin(loginParam).Result;
            if (verificationResult != "验证通过")
            {
                result.Fail(ResponseCode.LoginFail, verificationResult, null);
                return result;
            }
            var responseResult = SysUserDao.Login(loginParam);
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
                { "sessionId", userCache.SessionId }//一个账号仅允许在一个地方登录
            };
            var token = TokenManager.GenerateToken(dic.ToJson(), 3 * 24);
            LionWeb.HttpContext.Response.Headers["token"] = token;
            userCache.UserToken = token;
            LionWeb.HttpContext.Response.Headers["Access-Control-Expose-Headers"] = "token";//多个以逗号分隔 eg:token,sid

            LionUserCache.CreateUserCache(userCache);
        }

        /// <summary>
        /// 验证是否存在该用户数据
        /// </summary>
        /// <param name="type">1：用户名  2：邮箱</param>
        /// <param name="str"></param>
        /// <returns></returns>
        public async Task<bool> ExistUserAsync(int type, string str)
        {
            switch (type)
            {
                case 1:
                    return await SysUserDao.ExistAsync<SysUser>(c => c.UserName == str && c.Status == 1);
                case 2:
                    return await SysUserDao.ExistAsync<SysUser>(c => c.Email == str && c.Status == 1);
            }

            throw new CustomSystemException("验证类型错误", ResponseCode.DataTypeError);
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
        public async Task<ResponseModel<bool>> RegisterUserAsync(RegisterUserParam registerUserParam)
        {
            var result = new ResponseModel<bool>();
            var verificationResult = await VerificationCaptchaAsync(CacheKeys.REGISTERCAPTCHA, registerUserParam.Email, registerUserParam.Captcha, false);
            if (verificationResult != "验证通过")
            {
                result.Fail(ResponseCode.Fail, verificationResult, false);
                return result;
            }

            if (await ExistUserAsync(1, registerUserParam.UserName))
            {
                result.Fail(ResponseCode.Fail, "用户名已存在，请切换", false);
                return result;
            }

            if (await ExistUserAsync(2, registerUserParam.Email))
            {
                result.Fail(ResponseCode.Fail, "该邮箱已存在，请切换", false);
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
            var sysUser = new SysUser()
            {
                UserName = registerUserParam.UserName,
                PassWord = registerUserParam.PassWord.Md5Encrypt(),
                Email = registerUserParam.Email,
                Sex = 1,
                Status = 1,
                CreatedTime = DateTime.Now,
                UpdatedTime = DateTime.Now,
                ParentUid = 0,
                UpdatedBy = 0,
                SysUserRoleRelations = new List<SysUserRoleRelation>()
                {
                    new SysUserRoleRelation()
                    {
                        RoleId = 2, // 这里固定为种子数据中的系统管理员权限
                        State = 1,
                        CreatedTime = DateTime.Now,
                    }
                }
            };
            SysUserDao.Add(sysUser);
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
        public async Task<ResponseModel<string>> SendEmail(string email)
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

            if (!await ExistUserAsync(2, email))
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
        public async Task<ResponseModel<bool>> RetrievePwd(RetrievePwdParam retrievePwdParam)
        {
            var result = new ResponseModel<bool>();
            var verificationResult = await VerificationCaptchaAsync(CacheKeys.RETRIEVEPWDCAPTCHA, retrievePwdParam.Email, retrievePwdParam.Captcha, false);
            if (verificationResult != "验证通过")
            {
                return result.Fail(verificationResult, false);
            }

            var update = SysUserDao.RetrievePwd(retrievePwdParam, out long uid);
            if (update)
            {
                await LogoutAsync(new UserCacheBo() { UserId = uid });
            }
            return update ? result.Succeed(true) : result.Fail("修改密码失败，请稍后再试");
        }

        #endregion

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="modifyPwdParam"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public async Task<ResponseModel<string>> ModifyPwd(ModifyPwdParam modifyPwdParam, UserCacheBo currentUser)
        {
            var result = new ResponseModel<string>();
            if (modifyPwdParam.NewPassWord == modifyPwdParam.OldPassWord)
            {
                return result.Fail(ResponseCode.Fail, "不允许原密码和新密码相同的修改");
            }

            if (modifyPwdParam.OldPassWord.Md5Encrypt() == currentUser.PassWord)
            {
                var updateResult = await SysUserDao.ModifyPwd(modifyPwdParam, currentUser.UserId);
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
        /// <param name="userName"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public async Task<PageResponse<UserManagerDto>> GetManagerUserAsync(int pageSize, int currentPage, string email, string userName, UserCacheBo currentUser)
        {
            var result = await SysUserDao.GetManagerUserAsync(pageSize, currentPage, email, userName, currentUser);
            return result;
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
            //long类型前后端传值存在精度丢失
            //不能分配系统管理员角色权限和管理员权限
            if (param.RoleIds.Contains(1) || param.RoleIds.Contains(2))
            {
                result.Fail(ResponseCode.Fail, "角色选择错误", false);
                return result;
            }
            if (await ExistUserAsync(1, param.UserName))
            {
                result.Fail(ResponseCode.Fail, "用户名已存在，请切换", false);
                return result;
            }

            if (await ExistUserAsync(2, param.Email))
            {
                result.Fail(ResponseCode.Fail, "该邮箱已存在，请切换", false);
                return result;
            }
            var sysUser = new SysUser()
            {
                UserName = param.UserName,
                PassWord = param.Pwd.Md5Encrypt(),
                Email = param.Email,
                Sex = 1,
                Status = 1,
                CreatedTime = DateTime.Now,
                UpdatedTime = DateTime.Now,
                ParentUid = currentUser.UserId,
                UpdatedBy = 0,
                SysUserRoleRelations = new List<SysUserRoleRelation>()
            };
            param.RoleIds.ForEach(roleId =>
            {
                sysUser.SysUserRoleRelations.Add(new SysUserRoleRelation()
                {
                    RoleId = roleId, // 这里固定为种子数据中的系统管理员权限
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
            var result = new ResponseModel<bool>();
            //不能分配系统管理员角色权限和管理员权限
            if (long.TryParse(modifyUserParam.UserId, out var uid) || uid <= 1)
            {
                result.Fail(ResponseCode.Fail, "用户选择错误", false);
                return result;
            }
            //不能分配系统管理员角色权限和管理员权限
            if (modifyUserParam.RoleIds.Contains(1) || modifyUserParam.RoleIds.Contains(2))
            {
                result.Fail("角色选择错误", false);
                return result;
            }
            result = await SysUserDao.ModifyManagerUserAsync(uid, modifyUserParam.RoleIds, modifyUserParam.Email, currentUser);
            return result;
        }

        /// <summary>
        /// 用户管理界面删除用户
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        [DbTransactionInterceptor]
        public async Task<BaseResponseModel> RemoveManagerUserAsync(long uid, UserCacheBo currentUser)
        {
            var result = new ResponseModel<bool>();
            //获取当前用户所拥有的角色
            var existRoleIds = await SysUserDao.CurrentDbContext.SysUserRoleRelations
                .Where(c => c.UserId == uid && !c.Deleted && c.State == 1)
                .Select(c => c.RoleId).ToListAsync();
            //种子数据 2是管理员 1是系统管理员
            if (existRoleIds.Contains(1) || existRoleIds.Contains(2))
            {
                return result.Fail("管理员只读");
            }

            var db = SysUserDao.CurrentDbContext;

            if (!db.SysUsers.Any(c => c.UserId == uid && c.ParentUid == currentUser.UserId && c.Status == 1))
            {
                return result.Fail("账号ID不存在");
            }

            await db.SysUsers.Where(c => c.UserId == uid && c.ParentUid == currentUser.UserId && c.Status == 1).DeleteFromQueryAsync();
            await db.SysUserRoleRelations.Where(c => c.UserId == uid).DeleteFromQueryAsync();

            var count = await db.SaveChangesAsync();
            return count > 0 ? result.Succeed(true) : result.Fail("删除失败");
        }
    }
}
