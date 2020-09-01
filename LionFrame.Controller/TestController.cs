using Autofac;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using LionFrame.Business;
using LionFrame.CoreCommon;
using LionFrame.CoreCommon.AutoMapperCfg;
using LionFrame.Domain;
using LionFrame.Model;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using LionFrame.Basic;

namespace LionFrame.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        //构造函数注入和原生DI注入是一样的，这里就不写了
        /// <summary>
        /// 属性注入
        /// </summary>
        public TestBll TestBll { get; set; }

        public Mapper Mapper { get; set; }

        public MapperConfiguration MapperConfig { get; set; }

        /// <summary>
        /// 属性注入
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("guid")]
        public ActionResult GetGuid()
        {
            return Ok(TestBll.GetGuid());
        }

        /// <summary>
        /// 从全局变量中取
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("guid1")]
        public ActionResult GetGuid1()
        {
            var testBll = LionWeb.AutofacContainer.Resolve<TestBll>();
            return Ok(testBll.GetGuid());
        }

        [HttpGet, Route("mapper")]
        public ActionResult MapperTest()
        {
            var user = new User()
            {
                Id = 13,
                UserName = "Levy",
                Age = 24,
                Address = new Address()
                {
                    Province = "四川省",
                    City = "成都市",
                    Area = "武侯区",
                    Detail = "天府广场"
                }
            };
            var users = new List<User>() { user };
            var userDtos = users.AsQueryable().ProjectTo<UserDto>(MapperConfig).ToList();

            var userDto = Mapper.Map<UserDto>(user);

            return Ok(userDtos);
        }

        [HttpGet, Route("mapper1")]
        public ActionResult MapperTest1()
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
            var users = new List<User>() { user };

            var userDtos = users.MapToList<UserDto>();

            var userDto = user.MapTo<UserDto>();

            return Ok(userDto);
        }

        [HttpGet, Route("time")]
        public ActionResult GetTime()
        {
            LogHelper.Logger.Trace("测试123");
            return Ok(DateTime.Now);
        }
    }
}
