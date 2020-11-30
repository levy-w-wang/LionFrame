using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using LionFrame.Basic;
using LionFrame.Business;
using LionFrame.CoreCommon;
using Quartz;

namespace LionFrame.Quartz.Listeners
{
    public class MySchedulerListener : ISchedulerListener
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="trigger"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task JobScheduled(ITrigger trigger, CancellationToken cancellationToken = new CancellationToken())
        {
            var schedule = LionWeb.AutofacContainer.Resolve<IScheduler>();
            var sysQuartzBll = LionWeb.AutofacContainer.Resolve<SysQuartzBll>();

            var state = await schedule.GetTriggerState(trigger.Key, cancellationToken);
            await sysQuartzBll.ModifyTaskState(trigger.JobKey.Group, trigger.JobKey.Name, state);
            Console.WriteLine($"{trigger.Key.Name} state:{state} JobScheduled");
        }

        public async Task JobUnscheduled(TriggerKey triggerKey, CancellationToken cancellationToken = new CancellationToken())
        {
            var schedule = LionWeb.AutofacContainer.Resolve<IScheduler>();
            var sysQuartzBll = LionWeb.AutofacContainer.Resolve<SysQuartzBll>();

            var state = await schedule.GetTriggerState(triggerKey, cancellationToken);
            await sysQuartzBll.ModifyTaskState(triggerKey.Group, triggerKey.Name, state);
            Console.WriteLine($"{triggerKey.Name} state:{state} JobUnscheduled");
        }

        public async Task TriggerFinalized(ITrigger trigger, CancellationToken cancellationToken = new CancellationToken())
        {
            var schedule = LionWeb.AutofacContainer.Resolve<IScheduler>();
            var sysQuartzBll = LionWeb.AutofacContainer.Resolve<SysQuartzBll>();

            var state = await schedule.GetTriggerState(trigger.Key, cancellationToken);
            await sysQuartzBll.ModifyTaskState(trigger.Key.Group, trigger.Key.Name, state);
            Console.WriteLine($"{trigger.Key.Name} state:{state}  TriggerFinalized");
        }

        public Task TriggerPaused(TriggerKey triggerKey, CancellationToken cancellationToken = new CancellationToken())
        {
            return Task.CompletedTask;
        }

        public Task TriggerResumed(TriggerKey triggerKey, CancellationToken cancellationToken = new CancellationToken())
        {
            return Task.CompletedTask;
        }

        public Task JobAdded(IJobDetail jobDetail, CancellationToken cancellationToken = new CancellationToken())
        {
            Console.WriteLine($"{jobDetail.Key.Name}  JobAdded");
            return Task.CompletedTask;
        }


        public async Task JobDeleted(JobKey jobKey, CancellationToken cancellationToken = new CancellationToken())
        {
            var schedule = LionWeb.AutofacContainer.Resolve<IScheduler>();
            var sysQuartzBll = LionWeb.AutofacContainer.Resolve<SysQuartzBll>();

            var state = await schedule.GetTriggerState(jobKey.ConvertKey(), cancellationToken);
            await sysQuartzBll.ModifyTaskState(jobKey.Group, jobKey.Name, state);
            Console.WriteLine($"{jobKey.Name} state:{state}  JobDeleted");
        }


        public async Task JobPaused(JobKey jobKey, CancellationToken cancellationToken = new CancellationToken())
        {
            var schedule = LionWeb.AutofacContainer.Resolve<IScheduler>();
            var sysQuartzBll = LionWeb.AutofacContainer.Resolve<SysQuartzBll>();

            var state = await schedule.GetTriggerState(jobKey.ConvertKey(), cancellationToken);
            await sysQuartzBll.ModifyTaskState(jobKey.Group, jobKey.Name, state);
            Console.WriteLine($"{jobKey.Name}  JobPaused");
        }

        public async Task JobInterrupted(JobKey jobKey, CancellationToken cancellationToken = new CancellationToken())
        {
            var schedule = LionWeb.AutofacContainer.Resolve<IScheduler>();
            var sysQuartzBll = LionWeb.AutofacContainer.Resolve<SysQuartzBll>();

            var state = await schedule.GetTriggerState(jobKey.ConvertKey(), cancellationToken);
            await sysQuartzBll.ModifyTaskState(jobKey.Group, jobKey.Name, state);
            Console.WriteLine($"{jobKey.Name}  JobInterrupted");
        }


        public async Task JobResumed(JobKey jobKey, CancellationToken cancellationToken = new CancellationToken())
        {
            var schedule = LionWeb.AutofacContainer.Resolve<IScheduler>();
            var sysQuartzBll = LionWeb.AutofacContainer.Resolve<SysQuartzBll>();

            var state = await schedule.GetTriggerState(jobKey.ConvertKey(), cancellationToken);
            await sysQuartzBll.ModifyTaskState(jobKey.Group, jobKey.Name, state);
            Console.WriteLine($"{jobKey.Name}  JobResumed");
        }

        public Task TriggersResumed(string triggerGroup, CancellationToken cancellationToken = new CancellationToken())
        {
            Console.WriteLine($"{triggerGroup}  TriggersResumed");
            return Task.CompletedTask;
        }

        public Task JobsPaused(string jobGroup, CancellationToken cancellationToken = new CancellationToken())
        {
            Console.WriteLine($"{jobGroup}  JobsPaused");
            return Task.CompletedTask;
        }

        public Task JobsResumed(string jobGroup, CancellationToken cancellationToken = new CancellationToken())
        {
            Console.WriteLine($"{jobGroup}  JobsResumed");
            return Task.CompletedTask;
        }

        public Task TriggersPaused(string triggerGroup, CancellationToken cancellationToken = new CancellationToken())
        {
            Console.WriteLine($"{triggerGroup}  TriggersPaused");
            return Task.CompletedTask;
        }

        public Task SchedulerError(string msg, SchedulerException cause, CancellationToken cancellationToken = new CancellationToken())
        {
            LogHelper.Logger.Fatal(cause, msg);
            Console.WriteLine($"SchedulerError msg:{msg} cause:{cause.Message}");
            return Task.CompletedTask;
        }

        public Task SchedulerInStandbyMode(CancellationToken cancellationToken = new CancellationToken())
        {
            Console.WriteLine($"SchedulerInStandbyMode");
            return Task.CompletedTask;
        }

        public Task SchedulerStarted(CancellationToken cancellationToken = new CancellationToken())
        {
            Console.WriteLine($"SchedulerStarted");
            return Task.CompletedTask;
        }

        public Task SchedulerStarting(CancellationToken cancellationToken = new CancellationToken())
        {
            Console.WriteLine($"SchedulerStarting");
            return Task.CompletedTask;
        }

        public Task SchedulerShutdown(CancellationToken cancellationToken = new CancellationToken())
        {
            Console.WriteLine($"SchedulerShutdown");
            return Task.CompletedTask;
        }

        public Task SchedulerShuttingdown(CancellationToken cancellationToken = new CancellationToken())
        {
            Console.WriteLine($"SchedulerShuttingdown");
            return Task.CompletedTask;
        }

        public Task SchedulingDataCleared(CancellationToken cancellationToken = new CancellationToken())
        {
            Console.WriteLine($"SchedulingDataCleared");
            return Task.CompletedTask;
        }
    }
}