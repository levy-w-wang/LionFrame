using System;
using System.Collections.Generic;
using AutoMapper;
using LionFrame.Basic.AutofacDependency;
using LionFrame.Basic.Encryptions;
using LionFrame.Basic.Extensions;
using LionFrame.CoreCommon;
using LionFrame.CoreCommon.AutoMapperCfg;
using LionFrame.Data.SystemDao;
using LionFrame.Domain.SystemDomain;
using LionFrame.Model.RequestParam;
using LionFrame.Model.ResponseDto;
using LionFrame.Model.ResponseDto.ResultModel;
using LionFrame.Model.SystemBo;

namespace LionFrame.Business
{
    public class UserBll : IScopedDependency
    {
        public SysUserDao SysUserDao { get; set; }
        public Mapper map { get; set; }

        public SysUser Test()
        {
            //SysUserDao.CloseTracking();
            return SysUserDao.First<SysUser>(c => c.UserId == 1);
        }

        /// <summary>
        /// 用户登录 业务层
        /// </summary>
        /// <param name="loginParam"></param>
        /// <returns></returns>
        public ResponseModel<UserDto> Login(LoginParam loginParam)
        {
            var result = new ResponseModel<UserDto>();
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
    }
}
