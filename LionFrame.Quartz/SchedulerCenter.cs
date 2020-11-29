using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using LionFrame.Basic;
using LionFrame.Basic.AutofacDependency;
using LionFrame.Basic.Extensions;
using LionFrame.Config;
using LionFrame.CoreCommon;
using LionFrame.Model;
using LionFrame.Model.QuartzModels;
using LionFrame.Model.ResponseDto.ResultModel;
using LionFrame.Quartz.Jobs;
using Quartz;

namespace LionFrame.Quartz
{
    /// <summary>
    /// 调度中心  参考网址：https://cloud.tencent.com/developer/article/1500752
    /// 数据库语句：https://github.com/quartznet/quartznet/tree/master/database/tables
    /// </summary>
    public class SchedulerCenter : IScopedDependency
    {
        /// <summary>
        /// 返回任务计划（调度器）
        /// </summary>
        /// <returns></returns>
        public IScheduler Scheduler { get; set; }

        /// <summary>
        /// 添加一个工作调度
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<BaseResponseModel> AddScheduleJobAsync(ScheduleEntityParam entity)
        {
            var result = new ResponseModel<string>();

            try
            {
                //检查任务是否已存在
                var jobKey = new JobKey(entity.JobName, entity.JobGroup);
                if (await Scheduler.CheckExists(jobKey))
                {
                    result.Fail("调度任务已存在", "");
                    return result;
                }

                if (entity.TriggerType == TriggerTypeEnum.Cron)
                {
                    if (!CronExpression.IsValidExpression(entity.Cron))
                    {
                        return result.Fail("Cron表达式不正确");
                    }
                }
                var jobDataMap = new Dictionary<string, string>()
                {
                    { QuartzConstant.NOTIFYEMAIL, entity.NotifyEmail },
                    { QuartzConstant.MAILMESSAGE, ((int) entity.MailMessage).ToString() },
                    { QuartzConstant.JOBTYPE, ((int) entity.JobType).ToString() },
                    { QuartzConstant.REQUESTPARAMETERS, entity.RequestParameters } //http Body 参数，Assembly 调用方法参数。
                };
                IJobDetail job;
                switch (entity.JobType)
                {
                    case JobTypeEnum.Http:
                        if (!entity.Headers.IsNullOrEmpty())
                        {
                            try
                            {
                                entity.Headers.ToObject<Dictionary<string, string>>();
                            }
                            catch
                            {
                                result.Fail(ResponseCode.DataFormatError, "请求头参数格式错误，json字典格式", "");
                                return result;
                            }
                        }
                        job = AddHttpJob(entity, jobDataMap);
                        break;
                    case JobTypeEnum.Assembly:
                        job = AddAssemblyJob(entity, jobDataMap);
                        break;
                    case JobTypeEnum.None:
                    default:
                        return result.Fail("未选择任务类型");
                        break;
                }
                //校验是否正确的执行周期表达式
                var trigger = entity.TriggerType == TriggerTypeEnum.Cron ? CreateCronTrigger(entity) : CreateSimpleTrigger(entity);

                await Scheduler.ScheduleJob(job, trigger);
                result.Succeed("添加任务成功");
            }
            catch (Exception ex)
            {
                LogHelper.Logger.Fatal(ex, "添加任务失败");
                result.Fail(ResponseCode.UnknownEx, ex.Message, "");
            }

            return result;
        }

        /// <summary>
        /// 调用本地方法
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="jobDataMap"></param>
        /// <returns></returns>
        private IJobDetail AddAssemblyJob(ScheduleEntityParam entity, Dictionary<string, string> jobDataMap)
        {
            // 格式 LionFrame.Quartz.Jobs.TestJob,LionFrame.Quartz
            var typeName = $"{entity.RequestPath}.{entity.RequestMethod},{entity.RequestPath}";
            var jobType = Type.GetType(typeName);
            if (jobType == null)
            {
                throw new Exception("Job类未找到");
            }
            // 定义这个工作，并将其绑定到我们的IJob实现类                
            var job = JobBuilder.Create(jobType)
                .SetJobData(new JobDataMap(jobDataMap))
                .WithDescription(entity.Description)
                .WithIdentity(entity.JobName, entity.JobGroup)
                //.StoreDurably() //孤立存储，指即使该JobDetail没有关联的Trigger，也会进行存储 也就是执行完成后，不删除
                .RequestRecovery() //请求恢复，指应用崩溃后再次启动，会重新执行该作业
                .Build();
            // 创建触发器
            return job;
        }

        /// <summary>
        /// 创建HttpJob
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="jobDataMap"></param>
        /// <returns></returns>
        private IJobDetail AddHttpJob(ScheduleEntityParam entity, Dictionary<string, string> jobDataMap)
        {
            //http请求配置
            jobDataMap.Add(QuartzConstant.REQUESTTYPE, entity.RequestMethod);
            jobDataMap.Add(QuartzConstant.REQUESTURL, entity.RequestPath);
            jobDataMap.Add(QuartzConstant.HEADERS, entity.Headers);

            // 定义这个工作，并将其绑定到我们的IJob实现类                
            IJobDetail job = JobBuilder.Create<TestJob>()
                .SetJobData(new JobDataMap(jobDataMap))
                .WithDescription(entity.Description)
                .WithIdentity(entity.JobName, entity.JobGroup)
                //.StoreDurably() //孤立存储，指即使该JobDetail没有关联的Trigger，也会进行存储 也就是执行完成后，不删除
                .RequestRecovery() //请求恢复，指应用崩溃后再次启动，会重新执行该作业
                .Build();
            // 创建触发器
            return job;
        }

        /// <summary>
        /// 暂停/删除 指定的计划
        /// </summary>
        /// <param name="jobKey"></param>
        /// <param name="isDelete">停止并删除任务</param>
        /// <returns></returns>
        public async Task<BaseResponseModel> StopOrDelScheduleJobAsync(JobKey jobKey, bool isDelete = false)
        {
            var result = new ResponseModel<string>();
            try
            {
                await Scheduler.PauseJob(jobKey);
                if (isDelete)
                {
                    await Scheduler.DeleteJob(jobKey);
                    result.Succeed("删除任务计划成功");
                }
                else
                {
                    result.Succeed("暂停任务计划成功");
                }
            }
            catch (Exception ex)
            {
                result.Fail(ResponseCode.UnknownEx, "停止计划任务失败！--" + ex.Message, "");
            }

            return result;
        }

        /// <summary>
        /// 恢复运行暂停的任务
        /// </summary>
        /// <param name="jobKey">任务</param>
        public async Task<BaseResponseModel> ResumeJobAsync(JobKey jobKey)
        {
            var result = new ResponseModel<string>();
            try
            {
                //检查任务是否存在
                if (await Scheduler.CheckExists(jobKey))
                {
                    //任务已经存在则暂停任务
                    await Scheduler.ResumeJob(jobKey);
                    result.Succeed("恢复任务计划成功");
                }
                else
                {
                    result.Succeed("任务不存在");
                }
            }
            catch (Exception ex)
            {
                result.Fail(ResponseCode.UnknownEx, "恢复任务计划失败！--" + ex.Message, "");
                LogHelper.Logger.Error($"恢复任务失败！{ex}");
            }

            return result;
        }

        /// <summary>
        /// 立即执行 单独触发一次job
        /// </summary>
        /// <param name="jobKey"></param>
        /// <returns></returns>
        public async Task<bool> TriggerJobAsync(JobKey jobKey)
        {
            await Scheduler.TriggerJob(jobKey);
            return true;
        }

        /// <summary>
        /// 开启调度器
        /// </summary>
        /// <returns></returns>
        public async Task<bool> StartScheduleAsync()
        {
            //开启调度器
            if (Scheduler.InStandbyMode)
            {
                await Scheduler.Start();
            }

            return !Scheduler.InStandbyMode;
        }

        /// <summary>
        /// 停止任务调度
        /// </summary>
        public async Task<bool> StopScheduleAsync()
        {
            //判断调度是否已经关闭
            if (!Scheduler.InStandbyMode)
            {
                //等待任务运行完成
                await Scheduler.Standby(); //TODO  注意：Shutdown后Start会报错，所以这里使用暂停。必须重新实例化才可start
            }

            return Scheduler.InStandbyMode;
        }

        /// <summary>
        /// 创建TriggerBuilder
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private TriggerBuilder GetTriggerBuilder(ScheduleEntityParam entity)
        {
            return TriggerBuilder.Create() //创建
                .WithIdentity(entity.JobName, entity.JobGroup) // 标识
                .StartAt(entity.BeginTime) //开始时间
                .EndAt(entity.EndTime) //结束数据
                .WithPriority(entity.Priority) // 优先级 默认为5 相同执行时间越高越先执行
                .WithDescription(entity.Description ?? "") //描述
                .ForJob(entity.JobName, entity.JobGroup); //作业名称
        }

        /// <summary>
        /// 创建类型Simple的触发器
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private ITrigger CreateSimpleTrigger(ScheduleEntityParam entity)
        {
            var triggerBuilder = GetTriggerBuilder(entity);
            //作业触发器
            if (entity.RunTimes.HasValue && entity.RunTimes > 0)
            {
                return triggerBuilder
                    .WithSimpleSchedule(x =>
                        x.WithIntervalInSeconds(entity.IntervalSecond ?? 1) //执行时间间隔，单位秒
                        .WithRepeatCount(entity.RunTimes.Value)) //执行次数、默认从0开始
                    .Build();
            }

            return triggerBuilder
                .WithSimpleSchedule(x =>
                    x.WithIntervalInSeconds(entity.IntervalSecond ?? 1) //执行时间间隔，单位秒
                        .RepeatForever()) //无限循环

                .Build();
        }

        /// <summary>
        /// 创建类型Cron的触发器
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private ITrigger CreateCronTrigger(ScheduleEntityParam entity)
        {
            var triggerBuilder = GetTriggerBuilder(entity);
            // 作业触发器
            return triggerBuilder
                .WithCronSchedule(entity.Cron) //指定cron表达式
                .Build();
        }
    }
}