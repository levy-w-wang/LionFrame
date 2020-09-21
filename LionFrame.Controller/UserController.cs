using LionFrame.Business;
using LionFrame.CoreCommon.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFrame.Controller
{
    [Route("api/[controller]")]
    public class UserController : BaseUserController
    {
        public UserBll UserBll {get;set;}

        [HttpGet,Route("test"),AllowAnonymous]
        public ActionResult Test()
        {
            var result = UserBll.Test();
            return Succeed(result);
        }
    }
}
