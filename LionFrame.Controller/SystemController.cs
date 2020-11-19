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
            var result = await SystemBll.GetCaptchaResult();
            return Succeed(result);
        }
    }
}
