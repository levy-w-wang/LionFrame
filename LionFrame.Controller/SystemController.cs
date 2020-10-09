using LionFrame.Business;
using LionFrame.CoreCommon.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace LionFrame.Controller
{
    [Route("api/[controller]")]
    public class SystemController : BaseController
    {
        public SystemBll SystemBll { get; set; }

        [HttpGet, Route("captcha")]
        public ActionResult GetCaptcha()
        {
            var result = SystemBll.GetCaptchaResult();
            return Succeed(result);
        }
    }
}
