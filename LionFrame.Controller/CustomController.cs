using LionFrame.Basic;
using LionFrame.CoreCommon.AutoMapperCfg;
using LionFrame.CoreCommon.Controllers;
using LionFrame.Domain;
using LionFrame.Model;
using LionFrame.Model.ResponseDto.ResultModel;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace LionFrame.Controller
{
    /// <summary>
    /// 自定义返回结构体测试
    /// </summary>
    [Route("api/[controller]")]
    public class CustomController : BaseController
    {
        /// <summary>
        /// 统一返回数据测试
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("user")]
        public ActionResult CustomResultTest()
        {
            var user = new User()
            {
                Id = 13,
                UserName = "Levy",
                Age = 24,
                Address = new Address()
                {
                    Id = 1,
                    Province = "四川省",
                    City = "成都市",
                    Area = "武侯区",
                    Detail = "天府广场"
                }
            };

            var userDto = user.MapTo<UserDto>();

            return Succeed(userDto);
        }
        /// <summary>
        /// 统一返回数据测试
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("fail")]
        public ActionResult FailTest()
        {
            var user = new User()
            {
                Id = 13,
                UserName = "Levy",
                Age = 24,
                Address = new Address()
                {
                    Id = 1,
                    Province = "四川省",
                    City = "成都市",
                    Area = "武侯区",
                    Detail = "天府广场"
                }
            };

            var userDto = user.MapTo<UserDto>();

            return Fail(userDto, ResponseCode.Unauthorized1, "测试");
        }

        /// <summary>
        /// 分页数据测试
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("userlist")]
        public ActionResult CustomPageTest()
        {
            var user = new User()
            {
                Id = 13,
                UserName = "Levy",
                Age = 24,
                Address = new Address()
                {
                    Id = 1,
                    Province = "四川省",
                    City = "成都市",
                    Area = "武侯区",
                    Detail = "天府广场"
                }
            };
            var users = new List<User>() { user, user, user, user, user, user };
            var pageResult = new PageResponse<User>();
            pageResult.PageSize = 2;
            pageResult.CurrentPage = 1;
            pageResult.RecordTotal = users.Count;
            pageResult.Data = users;

            return Succeed(pageResult);
        }

        /// <summary>
        /// 日志测试
        /// </summary>
        /// <returns>成功</returns>
        [HttpGet, Route("log")]
        public ActionResult LogTest()
        {
            LogHelper.Logger.Trace("测试");
            LogHelper.Logger.Info("测试");
            LogHelper.Logger.Debug("测试");
            LogHelper.Logger.Error("测试");
            LogHelper.Logger.Fatal("测试");

            return Succeed("日志测试");
        }

        /// <summary>
        /// 异常测试
        /// </summary>
        /// <returns>成功</returns>
        [HttpGet, Route("ex")]
        public ActionResult ExTest()
        {
            var zero = 0;
            var a = 1 / zero;

            return Succeed("异常测试");
        }
    }
}
