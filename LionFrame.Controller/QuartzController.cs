using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LionFrame.Business;
using LionFrame.CoreCommon.Controllers;
using LionFrame.CoreCommon.CustomFilter;
using LionFrame.Model.QuartzModels;
using LionFrame.Model.RequestParam.QuartzParams;
using LionFrame.Quartz;
using Microsoft.AspNetCore.Mvc;
using Quartz;

namespace LionFrame.Controller
{
    /// <summary>
    /// 任务控制器  租户限制为系统账号租户ID
    /// </summary>
    [Route("api/[controller]"), TenantFilter(1)]
    public class QuartzController : BaseUserController
    {
        public SchedulerCenter SchedulerCenter { get; set; }
        public SysQuartzBll SysQuartzBll { get; set; }

        /// <summary>
        /// 分页数据
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("list")]
        public async Task<ActionResult> TaskPageList(TaskListParam taskListParam)
        {
            var result = await SysQuartzBll.GetTaskPageList(taskListParam);
            return Succeed(result);
        }

        /// <summary>
        /// 添加任务
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost, Route("addtask")]
        public async Task<ActionResult> AddTask(ScheduleEntityParam entity)
        {
            // 判断存在
            if (await SysQuartzBll.ExistTask(entity))
            {
                return Fail("任务名称已存在,请进行修改原任务，或使用新名称");
            }

            var result = await SchedulerCenter.AddScheduleJobAsync(entity);
            if (!result.Success)
            {
                return MyJson(result);
            }

            var addResult = await SysQuartzBll.AddTask(entity);
            return addResult ? Succeed() : Fail("添加失败");
        }

        /// <summary>
        /// 暂停任务
        /// </summary>
        /// <returns></returns>
        [HttpPut, Route("stopjob")]
        public async Task<ActionResult> StopJob(JobKey jobKey)
        {
            var result = await SchedulerCenter.StopOrDelScheduleJobAsync(jobKey);
            return MyJson(result);
            //if (!result.Success)
            //{
            //    return MyJson(result);
            //}

            //var updateResult = await SysQuartzBll.ModifyTaskState(jobKey.Group, jobKey.Name, MyTriggerState.Paused);
            //return updateResult ? Succeed() : Fail("暂停失败");
        }

        /// <summary>
        /// 删除任务
        /// </summary>
        /// <param name="jobKey"></param>
        /// <returns></returns>
        [HttpDelete, Route("removejob")]
        public async Task<ActionResult> DeleteJob(JobKey jobKey)
        {
            var result = await SchedulerCenter.StopOrDelScheduleJobAsync(jobKey, true);
            return MyJson(result);
            //if (!result.Success)
            //{
            //    return MyJson(result);
            //}

            //var updateResult = await SysQuartzBll.ModifyTaskState(jobKey.Group, jobKey.Name, MyTriggerState.Delete);
            //return updateResult ? Succeed() : Fail("删除失败");
        }

        /// <summary>
        /// 恢复运行暂停的任务
        /// </summary> 
        /// <returns></returns>
        [HttpPut, Route("resumejob")]
        public async Task<ActionResult> ResumeJob(JobKey jobKey)
        {
            var result = await SchedulerCenter.ResumeJobAsync(jobKey);
            return MyJson(result);
            //if (!result.Success)
            //{
            //    return MyJson(result);
            //}

            //var updateResult = await SysQuartzBll.ModifyTaskState(jobKey.Group, jobKey.Name, MyTriggerState.Normal);
            //return updateResult ? Succeed() : Fail("恢复运行失败");
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPut, Route("modifyjob")]
        public async Task<ActionResult> ModifyJob(ScheduleEntityParam entity)
        {
            var result = await SchedulerCenter.StopOrDelScheduleJobAsync(new JobKey(entity.JobName, entity.JobGroup), true);
            if (!result.Success)
            {
                return MyJson(result);
            }

            result = await SchedulerCenter.AddScheduleJobAsync(entity);
            if (result.Success)
            {
                result = await SysQuartzBll.ModifyTask(entity);
            }

            return MyJson(result);
        }

        ///// <summary>
        ///// 立即执行 不使用
        ///// </summary>
        ///// <param name="jobKey"></param>
        ///// <returns></returns>
        //[HttpPost, Route("triggerjob")]
        //public async Task<ActionResult> TriggerJob([FromBody]JobKey jobKey)
        //{
        //    await SchedulerCenter.TriggerJobAsync(jobKey);
        //    return Succeed();
        //}

        /// <summary>
        /// 启动调度
        /// </summary>
        /// <returns></returns>
        [HttpPut, Route("startschedule")]
        public async Task<ActionResult> StartSchedule()
        {
            return Succeed(await SchedulerCenter.StartScheduleAsync());
        }

        /// <summary>
        /// 停止调度
        /// </summary>
        /// <returns></returns>
        [HttpPut, Route("stopschedule")]
        public async Task<ActionResult> StopSchedule()
        {
            return Succeed(await SchedulerCenter.StopScheduleAsync());
        }

        /// <summary>
        /// 获取调度器当前状态
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("schedulestate")]
        public ActionResult ScheduleState()
        {
            return Succeed(SchedulerCenter.ScheduleAState());
        }
    }
}