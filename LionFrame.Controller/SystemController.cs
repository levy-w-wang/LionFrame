using System.Threading.Tasks;
using LionFrame.Business;
using LionFrame.CoreCommon.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace LionFrame.Controller
{
    [Route("api/[controller]")]
    public class SystemController : BaseController
    {
        public SystemBll SystemBll { get; set; }

        /// <summary>
        /// 获取验证码
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("captcha")]
        public async Task<ActionResult> GetCaptcha()
        {
            var result = await SystemBll.GetCaptchaResultAsync();
            return result == null ? Fail("验证码获取失败") : Succeed(result);
        }

        /// <summary>
        /// Rabbit Mq Retry dead-letter 测试
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("retry_dead_letter")]
        public async Task<ActionResult> RetryDeadLetterMq(string msg)
        {
            var result = await SystemBll.RetryDeadLetterMqSendAsync(msg);
            return Succeed(result);
        }
    }
}
