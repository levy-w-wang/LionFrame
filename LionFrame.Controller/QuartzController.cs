using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LionFrame.CoreCommon.Controllers;
using LionFrame.CoreCommon.CustomFilter;
using LionFrame.Model.QuartzModels;
using LionFrame.Quartz;
using Microsoft.AspNetCore.Mvc;
using Quartz;

namespace LionFrame.Controller
{
    [Route("api/[controller]"), TenantFilter(1)]
    public class QuartzController : BaseUserController
    {
        public SchedulerCenter SchedulerCenter { get; set; }

        /// <summary>
        /// 添加任务
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost, Route("addtask")]
        public async Task<ActionResult> AddTask(ScheduleEntity entity)
        {
            var result = await SchedulerCenter.AddScheduleJobAsync(entity);
            return MyJson(result);
        }

        /// <summary>
        /// 暂停任务
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("stopjob")]
        public async Task<ActionResult> StopJob(JobKey jobKey)
        {
            return MyJson((await SchedulerCenter.StopOrDelScheduleJobAsync(jobKey)));
        }

        /// <summary>
        /// 删除任务
        /// </summary>
        /// <param name="jobKey"></param>
        /// <returns></returns>
        [HttpDelete, Route("removetask")]
        public async Task<ActionResult> DeleteTask(JobKey jobKey)
        {
            var result = await SchedulerCenter.StopOrDelScheduleJobAsync(jobKey, true);
            return MyJson(result);
        }

        /// <summary>
        /// 恢复运行暂停的任务
        /// </summary> 
        /// <returns></returns>
        [HttpPost, Route("resumejob")]
        public async Task<ActionResult> ResumeJob(JobKey jobKey)
        {
            return MyJson((await SchedulerCenter.ResumeJobAsync(jobKey)));
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost, Route("modifyjob")]
        public async Task<ActionResult> ModifyJob([FromBody]ScheduleEntity entity)
        {
            await SchedulerCenter.StopOrDelScheduleJobAsync(new JobKey(entity.JobName, entity.JobGroup), true);
            await SchedulerCenter.AddScheduleJobAsync(entity);
            return Succeed("修改计划任务成功！");
        }

        /// <summary>
        /// 立即执行
        /// </summary>
        /// <param name="jobKey"></param>
        /// <returns></returns>
        [HttpPost, Route("triggerjob")]
        public async Task<ActionResult> TriggerJob([FromBody]JobKey jobKey)
        {
            await SchedulerCenter.TriggerJobAsync(jobKey);
            return Succeed();
        }

        /// <summary>
        /// 启动调度
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("startschedule")]
        public async Task<ActionResult> StartSchedule()
        {
            return Succeed(await SchedulerCenter.StartScheduleAsync());
        }

        /// <summary>
        /// 停止调度
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("stopschedule")]
        public async Task<ActionResult> StopSchedule()
        {
            return Succeed(await SchedulerCenter.StopScheduleAsync());
        }
    }
}