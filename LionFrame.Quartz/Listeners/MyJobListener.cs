﻿using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using LionFrame.Basic.Extensions;
using LionFrame.Business;
using LionFrame.Config;
using LionFrame.CoreCommon;
using LionFrame.Domain.SystemDomain;
using LionFrame.Model.QuartzModels;
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
            await SaveExecutedResult(context, jobException);

            await NotifyEmail(context, jobException);
            Console.WriteLine($"Job: {context.JobDetail.Key}   context.Result:{context.Result}    state:{state}   执行完成");
        }

        /// <summary>
        /// 处理是否发送邮件
        /// </summary>
        /// <param name="context"></param>
        /// <param name="jobException"></param>
        /// <returns></returns>
        private async Task NotifyEmail(IJobExecutionContext context, JobExecutionException jobException)
        {
            var mailMessage = int.Parse(context.JobDetail.JobDataMap.GetString(QuartzConstant.MAILMESSAGE) ?? "0").ToEnum<MailMessageEnum>();
            switch (mailMessage)
            {
                case MailMessageEnum.None:
                    break;
                case MailMessageEnum.Err:
                    if (jobException != null)
                    {
                        await SendNotifyEmail(context, jobException);
                    }
                    break;
                case MailMessageEnum.All:
                    await SendNotifyEmail(context, jobException);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="context"></param>
        /// <param name="jobException"></param>
        /// <returns></returns>
        private static async Task SendNotifyEmail(IJobExecutionContext context, JobExecutionException jobException)
        {
            var resultState = (jobException == null ? "正常" : "异常");
            var jobDetail = context.JobDetail;
            var notifyEmail = jobDetail.JobDataMap.GetString(QuartzConstant.NOTIFYEMAIL) ?? "";
 
            var result = ConvertResult(context);
            var emailContent = new StringBuilder();
            emailContent.Append($@"<p>任务执行结果:{resultState}</p>");
            emailContent.Append($@"<p>组名：{jobDetail.Key.Group}</p>");
            emailContent.Append($@"<p>任务名：{jobDetail.Key.Name}</p>");
            emailContent.Append($@"<p>任务描述：{jobDetail.Description ?? ""}</p>");
            emailContent.Append($@"<p>请求地址：{jobDetail.JobType.FullName ?? ""}</p>");
            emailContent.Append($@"<p>JobDataMap：{jobDetail.JobDataMap.ToJson()}</p>");
            if (context.Result != null)
            {
                emailContent.Append($@"<p>执行结果：{result}</p>");
            }

            if (jobException != null)
            {
                emailContent.Append($@"<p>异常信息：{jobException.Message}</p>");
                emailContent.Append($@"<p>异常堆栈：{jobException.StackTrace}</p>");
            }

            emailContent.Append($@"<p>执行耗时：{context.JobRunTime.TotalMilliseconds}毫秒</p>");
            using var container = LionWeb.AutofacContainer.BeginLifetimeScope();
            SystemBll systemBll = container.Resolve<SystemBll>();
            await systemBll.SendSystemMailAsync($"{jobDetail.Key.Name}-{resultState}-Quartz通知", emailContent.ToString(), notifyEmail, "");
        }

        /// <summary>
        /// Job执行完成
        /// </summary>
        /// <param name="context"></param>
        /// <param name="jobException"></param>
        /// <returns></returns>
        private static async Task SaveExecutedResult(IJobExecutionContext context, JobExecutionException jobException)
        {
            var jobDetail = context.JobDetail;
            var result = ConvertResult(context);

            var sysQuartzLog = new SysQuartzLog()
            {
                JobGroup = jobDetail.Key.Group,
                JobName = jobDetail.Key.Name,
                RequestPath = jobDetail.JobType.FullName ?? "",
                NextFireTime = context.NextFireTimeUtc?.LocalDateTime,
                PreviousFireTime = context.PreviousFireTimeUtc?.LocalDateTime,
                JobDataMap = jobDetail.JobDataMap.ToJson(true),
                Description = jobDetail.Description ?? "",
                Result = result,
                Exception = jobException?.Message ?? "",
                RunTimeTotalMilliseconds = context.JobRunTime.TotalMilliseconds,
                CreatedTime = DateTime.Now
            };
            using var container = LionWeb.AutofacContainer.BeginLifetimeScope();
            var sysQuartzLogBll = container.Resolve<SysQuartzLogBll>();
            var sysQuartzBll = container.Resolve<SysQuartzBll>();

            await sysQuartzLogBll.AddTaskLogAsync(sysQuartzLog);
            await sysQuartzBll.ModifyTaskLastFireTimeAsync(jobDetail.Key.Group, jobDetail.Key.Name, context.PreviousFireTimeUtc?.LocalDateTime, context.NextFireTimeUtc?.LocalDateTime);
        }

        private static string ConvertResult(IJobExecutionContext context)
        {
            var result = "";
            if (context.Result is string || context.Result is int || context.Result is bool || context.Result is long || context.Result is Enum || context.Result is double || context.Result is float || context.Result is char || context.Result is byte || context.Result is short)
            {
                result = context.Result as string;
            }
            else if (context.Result != null)
            {
                result = context.Result.ToJson();
            }

            return result;
        }

        /// <summary>
        /// Get the name of the <see cref="T:Quartz.IJobListener" />.
        /// </summary>
        public string Name { get; } = nameof(MyJobListener);
    }
}