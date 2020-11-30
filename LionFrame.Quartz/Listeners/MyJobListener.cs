using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using LionFrame.Basic.Extensions;
using LionFrame.Business;
using LionFrame.CoreCommon;
using LionFrame.Domain.SystemDomain;
using Quartz;

namespace LionFrame.Quartz.Listeners
{
    public class MyJobListener : IJobListener
    {
        public async Task JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken = new CancellationToken())
        {
            //context.Result = "123";
            var state = await context.Scheduler.GetTriggerState(context.Trigger.Key, cancellationToken);
            //Job即将执行
            Console.WriteLine($"Job: {context.JobDetail.Key} state:{state} 即将执行");
        }

        public Task JobExecutionVetoed(IJobExecutionContext context, CancellationToken cancellationToken = new CancellationToken())
        {
            Console.WriteLine($"Job: {context.JobDetail.Key} 被否决执行");
            return Task.CompletedTask;
        }

        public async Task JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException, CancellationToken cancellationToken = new CancellationToken())
        {
            var state = await context.Scheduler.GetTriggerState(context.Trigger.Key, cancellationToken);

            //Job执行完成
            var sysQuartzLogBll = LionWeb.AutofacContainer.Resolve<SysQuartzLogBll>();
            var sysQuartzBll = LionWeb.AutofacContainer.Resolve<SysQuartzBll>();
            var jobDetail = context.JobDetail;
            SysQuartzLog sysQuartzLog = new SysQuartzLog()
            {
                JobGroup = jobDetail.Key.Group,
                JobName = jobDetail.Key.Name,
                RequestPath = jobDetail.GetType().FullName ?? "",
                NextFireTime = context.NextFireTimeUtc?.LocalDateTime,
                PreviousFireTime = context.PreviousFireTimeUtc?.LocalDateTime,
                JobDataMap = jobDetail.JobDataMap.ToJson(true),
                Description = jobDetail.Description ?? "",
                Result = (context.Result ?? "").ToJson(true),
                Exception = jobException?.Message ?? "",
                CreatedTime = DateTime.Now
            };
            await sysQuartzLogBll.AddTaskLog(sysQuartzLog);
            await sysQuartzBll.ModifyTaskLastFireTime(jobDetail.Key.Group, jobDetail.Key.Name, context.PreviousFireTimeUtc?.LocalDateTime, context.NextFireTimeUtc?.LocalDateTime);
            Console.WriteLine($"Job: {context.JobDetail.Key}   context.Result:{context.Result}    state:{state}   执行完成");
        }

        /// <summary>
        /// Get the name of the <see cref="T:Quartz.IJobListener" />.
        /// </summary>
        public string Name { get; } = nameof(MyJobListener);
    }
}
