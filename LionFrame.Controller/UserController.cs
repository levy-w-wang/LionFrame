using LionFrame.Business;
using LionFrame.CoreCommon.Controllers;
using LionFrame.Model.RequestParam;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LionFrame.Controller
{
    [Route("api/[controller]")]
    public class UserController : BaseUserController
    {
        public UserBll UserBll { get; set; }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="loginParam"></param>
        /// <returns></returns>
        [HttpPost, Route("login"), AllowAnonymous]
        public async Task<ActionResult> Login(LoginParam loginParam)
        {
            var result = await Task.FromResult(UserBll.Login(loginParam));
            return MyJson(result);
        }

        /// <summary>
        /// 测试获取当前用户
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("user")]
        public ActionResult GetCurrentUser()
        {
            return Succeed(CurrentUser);
        }
    }
}
