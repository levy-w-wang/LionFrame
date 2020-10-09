using LionFrame.Basic;
using LionFrame.Basic.AutofacDependency;
using LionFrame.Basic.Models;
using LionFrame.Config;
using LionFrame.CoreCommon.Cache.Redis;
using LionFrame.Model.RequestParam;
using System;
using System.Threading.Tasks;

namespace LionFrame.Business
{
    /// <summary>
    /// 系统相关业务类
    /// </summary>
    public class SystemBll : IScopedDependency
    {
        /// <summary>
        /// 验证码5分钟过期
        /// </summary>
        private static readonly TimeSpan CaptchaExpired = TimeSpan.FromMinutes(5);

        public RedisClient RedisClient { get; set; }

        public IdWorker IdWorker { get; set; }

        public CaptchaResult GetCaptchaResult()
        {
            var captcha = CaptchaHelper.GenerateCaptcha(75, 35, CaptchaHelper.GenerateCaptchaCode());
            var uuid = IdWorker.NextId();
            var key = CacheKeys.CAPTCHA + uuid;
            RedisClient.Set(key, captcha.Captcha, CaptchaExpired);
            captcha.Uuid = uuid.ToString();
            return captcha;
        }

        /// <summary>
        /// 验证登录时的验证码
        /// </summary>
        /// <param name="loginParam"></param>
        /// <returns></returns>
        internal async Task<string> VerificationLogin(LoginParam loginParam)
        {
            var key = CacheKeys.CAPTCHA + loginParam.Uuid;
            var captcha = await RedisClient.GetAsync<string>(key);
            if (captcha == null)
            {
                return "验证码已失效";
            }

            if (!string.Equals(loginParam.Captcha, captcha, StringComparison.OrdinalIgnoreCase))
            {
                return "验证码错误";
            }

            await RedisClient.DeleteAsync(key);
            return "验证通过";
        }
    }
}
