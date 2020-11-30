using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LionFrame.Basic.AutofacDependency;
using LionFrame.CoreCommon.AutoMapperCfg;
using LionFrame.Data.SystemDao;
using LionFrame.Domain.SystemDomain;
using LionFrame.Model.QuartzModels;
using LionFrame.Model.RequestParam.QuartzParams;
using LionFrame.Model.ResponseDto.ResultModel;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace LionFrame.Business
{
    public class SysQuartzBll : IScopedDependency
    {
        public SysQuartzDao SysQuartzDao { get; set; }

        /// <summary>
        /// 添加任务
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> AddTask(ScheduleEntityParam entity)
        {
            var sysQuartz = entity.MapTo<SysQuartz>();
            sysQuartz.CreatedTime = DateTime.Now;
            sysQuartz.NextFireTime = null;
            sysQuartz.PreviousFireTime = null;
            await SysQuartzDao.AddAsync(sysQuartz);
            var result = await SysQuartzDao.SaveChangesAsync();
            return result > 0;
        }

        /// <summary>
        /// 更新任务状态
        /// </summary>
        /// <param name="jobGroup"></param>
        /// <param name="jobName"></param>
        /// <param name="triggerState"></param>
        /// <returns></returns>
        public async Task<bool> ModifyTaskState(string jobGroup, string jobName, TriggerState triggerState)
        {
            var result = await SysQuartzDao.CurrentDbContext.SysQuartzs.Where(c => c.JobGroup == jobGroup && c.JobName == jobName).UpdateFromQueryAsync(c => new SysQuartz()
            {
                TriggerState = triggerState,
            });
            return result > 0;
        }

        /// <summary>
        /// 更新任务最后执行时间
        /// </summary>
        /// <param name="jobGroup"></param>
        /// <param name="jobName"></param>
        /// <param name="previousFireTime"></param>
        /// <param name="nextFireTime"></param>
        /// <returns></returns>
        public async Task<bool> ModifyTaskLastFireTime(string jobGroup, string jobName, DateTime? previousFireTime, DateTime? nextFireTime)
        {
            var result = await SysQuartzDao.CurrentDbContext.SysQuartzs.Where(c => c.JobGroup == jobGroup && c.JobName == jobName).UpdateFromQueryAsync(c => new SysQuartz()
            {
                PreviousFireTime = previousFireTime,
                NextFireTime = nextFireTime
            });
            return result > 0;
        }

        /// <summary>
        /// 修改任务
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<BaseResponseModel> ModifyTask(ScheduleEntityParam entity)
        {
            var responseResult = new ResponseModel<string>();
            var result = await SysQuartzDao.CurrentDbContext.SysQuartzs.Where(c => c.JobGroup == entity.JobGroup && c.JobName == entity.JobName).UpdateFromQueryAsync(c => new SysQuartz()
            {
                BeginTime = entity.BeginTime,
                EndTime = entity.EndTime,
                Cron = entity.Cron ?? "",
                RunTimes = entity.RunTimes ?? 1,
                IntervalSecond = entity.IntervalSecond ?? 1,
                RequestPath = entity.RequestPath,
                RequestMethod = entity.RequestMethod,
                RequestParameters = entity.RequestParameters ?? "{}",
                Headers = entity.Headers ?? "{}",
                Priority = entity.Priority,
                Description = entity.Description,
                NotifyEmail = entity.NotifyEmail ?? "",
                MailMessage = entity.MailMessage,
                TriggerState = TriggerState.Normal,
            });
            return result > 0 ? responseResult.Succeed() : responseResult.Fail("最终修改失败");
        }

        /// <summary>
        /// 判断任务名称是否已存在
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> ExistTask(ScheduleEntityParam entity)
        {
            var result = await SysQuartzDao.CurrentDbContext.SysQuartzs.AnyAsync(c => c.JobGroup == entity.JobGroup && c.JobName == entity.JobName);
            return result;
        }

        /// <summary>
        /// 获取任务分页数据
        /// </summary>
        /// <param name="taskListParam"></param>
        /// <returns></returns>
        public async Task<PageResponse<SysQuartz>> GetTaskPageList(TaskListParam taskListParam)
        {
            return await SysQuartzDao.GetTaskPageList(taskListParam);
        }
    }
}