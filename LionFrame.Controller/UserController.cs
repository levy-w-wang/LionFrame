using LionFrame.Business;
using LionFrame.CoreCommon.Controllers;
using LionFrame.Model.RequestParam;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LionFrame.Controller
{
    [Route("api/[controller]")]
    public class UserController : BaseUserController
    {
        public UserBll UserBll { get; set; }

        [HttpPost, Route("login"), AllowAnonymous]
        public ActionResult Login(LoginParam loginParam)
        {
            var result = UserBll.Login(loginParam);
            return MyJson(result);
        }
    }
}
