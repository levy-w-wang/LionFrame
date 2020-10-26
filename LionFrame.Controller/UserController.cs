using LionFrame.Business;
using LionFrame.CoreCommon.Controllers;
using LionFrame.Model;
using LionFrame.Model.RequestParam.UserParams;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
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

        /// <summary>
        /// 获取注册验证码
        /// </summary>
        /// <param name="emailTo"></param>
        /// <returns></returns>
        [Route("registercaptcha"), HttpGet, AllowAnonymous]
        public async Task<ActionResult> GetRegisterCaptcha([RegularExpression(@"^[A-Za-z0-9\u4e00-\u9fa5_-]+@[a-zA-Z0-9_-]+(\.[a-zA-Z0-9_-]+)+$", ErrorMessage = "邮件格式不正确")] string emailTo)
        {
            var result = await UserBll.GetRegisterCaptchaAsync(emailTo);
            return result.Contains("发送成功") ? Succeed() : Fail("获取邮件失败", ResponseCode.Fail);
        }

        /// <summary>
        /// 注册用户
        /// </summary>
        /// <param name="registerUserParam"></param>
        /// <returns></returns>
        [Route("register"), HttpPost, AllowAnonymous]
        public async Task<ActionResult> RegisterUser(RegisterUserParam registerUserParam)
        {
            var result = await UserBll.RegisterUserAsync(registerUserParam);
            return MyJson(result);
        }

        /// <summary>
        /// 验证用户数据是否存在,在用户输入完数据后，立即校验一次，提升用户体验
        /// </summary>
        /// <param name="type">1：用户名  2：邮箱</param>
        /// <param name="str">对应值 -- 未直接校验</param>
        /// <returns></returns>
        [Route("exist"), HttpGet, AllowAnonymous]
        public async Task<ActionResult> ExistUser(int type, string str)
        {
            var result = await UserBll.ExistUserAsync(type, str);
            if (result)
            {
                var info = type == 1 ? "用户名" : "邮箱";
                return Fail($"{info}已存在，请切换", content: $"{info}已存在，请切换");
            }
            return Succeed();
        }

        /// <summary>
        /// 修改密码
        /// 修改成功后删除当前登录信息，重新登录
        /// </summary>
        /// <param name="modifyPwdParam"></param>
        /// <returns></returns>
        [Route("modifypwd"), HttpPut]
        public async Task<ActionResult> ModifyPwd(ModifyPwdParam modifyPwdParam)
        {
            var result = await UserBll.ModifyPwd(modifyPwdParam, CurrentUser);

            return MyJson(result);
        }

        /// <summary>
        /// 登出
        /// </summary>
        /// <returns></returns>
        [Route("logout"), HttpPost]
        public async Task<ActionResult> Logout()
        {
            await UserBll.Logout(CurrentUser);
            return Succeed();
        }
    }
}
