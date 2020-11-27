using System;
using System.Collections.Generic;
using System.Text;
using LionFrame.Business;
using LionFrame.CoreCommon.Controllers;
using LionFrame.CoreCommon.CustomFilter;
using Microsoft.AspNetCore.Mvc;

namespace LionFrame.Controller
{
    [Route("api/[controller]"), TenantFilter(1)]
    public class QuartzLogController : BaseUserController
    {
        public SysQuartzLogBll SysQuartzLogBll { get; set; }
    }
}
