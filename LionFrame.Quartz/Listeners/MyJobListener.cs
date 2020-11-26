using System;
using System.Threading;
using System.Threading.Tasks;
using Quartz;

namespace LionFrame.Quartz.Listeners
{
    public class MyJobListener : IJobListener
    {
        public Task JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken = new CancellationToken())
        {
            context.Result = "123";
            var state = context.Scheduler.GetTriggerState(context.Trigger.Key,cancellationToken).Result;
            //Job即将执行
            return Task.Factory.StartNew(() =>
            {
                Console.WriteLine($"Job: {context.JobDetail.Key} state:{state} 即将执行");
            }, cancellationToken);
        }

        public Task JobExecutionVetoed(IJobExecutionContext context, CancellationToken cancellationToken = new CancellationToken())
        {
            return Task.Factory.StartNew(() =>
            {
                Console.WriteLine($"Job: {context.JobDetail.Key} 被否决执行");
            }, cancellationToken);
        }

        public Task JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException, CancellationToken cancellationToken = new CancellationToken())
        {
            var state = context.Scheduler.GetTriggerState(context.Trigger.Key,cancellationToken).Result;
            //Job执行完成
            return Task.Factory.StartNew(() =>
            {
                Console.WriteLine($"Job: {context.JobDetail.Key}   context.Result:{context.Result}    state:{state}   执行完成");
            }, cancellationToken);
        }

        /// <summary>
        /// Get the name of the <see cref="T:Quartz.IJobListener" />.
        /// </summary>
        public string Name { get; } = nameof(MyJobListener);
    }
}
