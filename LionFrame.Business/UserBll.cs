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
using LionFrame.Model.RequestParam.SystemParams;
using LionFrame.Model.RequestParam.UserParams;
using LionFrame.Model.ResponseDto.ResultModel;
using LionFrame.Model.ResponseDto.SystemDto;
using LionFrame.Model.SystemBo;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
                result.Data = userDto;
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
        /// 得到验证码
        /// </summary>
        /// <param name="emailTo"></param>
        /// <returns></returns>
        public async Task<string> GetRegisterCaptchaAsync(string emailTo)
        {
            var captchaNumber = CaptchaHelper.CreateRandomNumber(6);
            var htmlEmail = SystemBll.GetMailContent("注册账号", captchaNumber, 10);
            var result = await SystemBll.SendSystemMailAsync("LionFrame-注册账号", htmlEmail, emailTo);
            if (result.Contains("发送成功"))
            {
                await RedisClient.SetAsync($"{CacheKeys.REGISTERCAPTCHA}{emailTo}", captchaNumber, new TimeSpan(0, 10, 0));
            }
            return result;
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
                    return await SysUserDao.ExistAsync<SysUser>(c => c.UserName == str);
                case 2:
                    return await SysUserDao.ExistAsync<SysUser>(c => c.Email == str);
            }

            throw new CustomSystemException("验证类型错误", ResponseCode.DataTypeError);
        }

        #region 注册用户

        /// <summary>
        /// 验证注册用户
        /// </summary>
        /// <param name="registerUserParam"></param>
        /// <returns></returns>
        public async Task<string> VerificationRegisterCaptchaAsync(RegisterUserParam registerUserParam)
        {
            var key = CacheKeys.REGISTERCAPTCHA + registerUserParam.Email;
            var captcha = await RedisClient.GetAsync<string>(key);
            if (captcha == null)
            {
                return "验证码已失效";
            }

            if (!string.Equals(registerUserParam.Captcha, captcha, StringComparison.OrdinalIgnoreCase))
            {
                return "验证码错误";
            }

            await RedisClient.DeleteAsync(key);
            return "验证通过";
        }


        /// <summary>
        /// 注册用户
        /// </summary>
        /// <param name="registerUserParam"></param>
        /// <returns></returns>
        public async Task<ResponseModel<bool>> RegisterUserAsync(RegisterUserParam registerUserParam)
        {
            var result = new ResponseModel<bool>();
            var verificationResult = await VerificationRegisterCaptchaAsync(registerUserParam);
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
                SysUserRoleRelations = new List<SysUserRoleRelation>()
                {
                    new SysUserRoleRelation()
                    {
                        RoleId = 2 // 这里固定为种子数据中的超级管理员权限
                    }
                }
            };
            SysUserDao.Add(sysUser);
            var count = await SysUserDao.SaveChangesAsync();
            return count;
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
                    await RedisClient.DeleteAsync(CacheKeys.USER + currentUser.UserId);
                    LionMemoryCache.Remove(CacheKeys.USER + currentUser.UserId);

                    return result.Succeed("修改成功,请重新登录");
                }
                return result.Fail(ResponseCode.Fail, "修改失败，请稍后再试");
            }

            return result.Fail(ResponseCode.Fail, "原密码不正确");
        }
    }
}
