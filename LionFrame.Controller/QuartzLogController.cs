using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LionFrame.Business;
using LionFrame.CoreCommon.Controllers;
using LionFrame.CoreCommon.CustomFilter;
using LionFrame.Model.RequestParam.QuartzParams;
using Microsoft.AspNetCore.Mvc;

namespace LionFrame.Controller
{
    [Route("api/[controller]"), TenantFilter(1)]
    public class QuartzLogController : BaseUserController
    {
        public SysQuartzLogBll SysQuartzLogBll { get; set; }

        [HttpPost, Route("list")]
        public async Task<ActionResult> TaskLogPageList(TaskLogListParam taskLogListParam)
        {
            var result = await SysQuartzLogBll.GetTaskLogPageListAsync(taskLogListParam);
            return Succeed(result);
        }
    }
}
