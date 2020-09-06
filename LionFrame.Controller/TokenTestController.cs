using LionFrame.CoreCommon;
using LionFrame.CoreCommon.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LionFrame.Controller
{
    public class TokenTestController : BaseUserController
    {
        [HttpGet]
        [Route("testtoken")]
        [AllowAnonymous]//允许所有人访问
        public ActionResult TestToken()
        {
            var token = TokenManager.GenerateToken("测试token的生成");
            Response.Headers["token"] = token;
            Response.Headers["Access-Control-Expose-Headers"] = "token";//一定要添加这一句  不然前端是取不到token字段的值的！更别提存store了。
            return Succeed(token);
        }

        [HttpPost]
        [Route("validtoken")]
        public ActionResult ValidToken()
        {
            //业务处理  token已在基类中验证
            return Succeed("成功");
        }
    }
}
