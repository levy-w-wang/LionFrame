using Autofac;
using LionFrame.Business;
using LionFrame.CoreCommon;
using Microsoft.AspNetCore.Mvc;
using System;

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

        [HttpGet, Route("time")]
        public ActionResult GetTime()
        {
            return Ok(DateTime.Now);
        }
    }
}
