﻿using System;
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
        public Task JobScheduled(ITrigger trigger, CancellationToken cancellationToken = new CancellationToken())
        {
            var schedule = LionWeb.AutofacContainer.Resolve<IScheduler>();
            var sysQuartzBll = LionWeb.AutofacContainer.Resolve<SysQuartzBll>();

            return Task.Factory.StartNew(async () =>
            {
                var state = await schedule.GetTriggerState(trigger.Key, cancellationToken);
                await sysQuartzBll.ModifyTaskState(trigger.JobKey.Group, trigger.JobKey.Name, state.ConvertTriggerState());
                Console.WriteLine($"{trigger.Key.Name} state:{state} JobScheduled");
            }, cancellationToken);
        }

        public Task JobUnscheduled(TriggerKey triggerKey, CancellationToken cancellationToken = new CancellationToken())
        {
            var schedule = LionWeb.AutofacContainer.Resolve<IScheduler>();
            var sysQuartzBll = LionWeb.AutofacContainer.Resolve<SysQuartzBll>();

            return Task.Factory.StartNew(async () =>
            {
                var state = await schedule.GetTriggerState(triggerKey, cancellationToken);
                await sysQuartzBll.ModifyTaskState(triggerKey.Group, triggerKey.Name, state.ConvertTriggerState());
                Console.WriteLine($"{triggerKey.Name} state:{state} JobUnscheduled");
            }, cancellationToken);
        }

        public Task TriggerFinalized(ITrigger trigger, CancellationToken cancellationToken = new CancellationToken())
        {
            var schedule = LionWeb.AutofacContainer.Resolve<IScheduler>();
            var sysQuartzBll = LionWeb.AutofacContainer.Resolve<SysQuartzBll>();

            return Task.Factory.StartNew(async () =>
            {
                var state = await schedule.GetTriggerState(trigger.Key, cancellationToken);
                await sysQuartzBll.ModifyTaskState(trigger.Key.Group, trigger.Key.Name, state.ConvertTriggerState());
                Console.WriteLine($"{trigger.Key.Name} state:{state}  TriggerFinalized");
            }, cancellationToken);
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
            return Task.Factory.StartNew(() =>
            {
                Console.WriteLine($"{jobDetail.Key.Name}  JobAdded");
            }, cancellationToken);
        }


        public Task JobDeleted(JobKey jobKey, CancellationToken cancellationToken = new CancellationToken())
        {
            var schedule = LionWeb.AutofacContainer.Resolve<IScheduler>();
            var sysQuartzBll = LionWeb.AutofacContainer.Resolve<SysQuartzBll>();

            return Task.Factory.StartNew(async () =>
            {
                var state = await schedule.GetTriggerState(jobKey.ConvertKey(), cancellationToken);
                await sysQuartzBll.ModifyTaskState(jobKey.Group, jobKey.Name, state.ConvertTriggerState());
                Console.WriteLine($"{jobKey.Name} state:{state}  JobDeleted");
            }, cancellationToken);
        }


        public Task JobPaused(JobKey jobKey, CancellationToken cancellationToken = new CancellationToken())
        {
             var schedule = LionWeb.AutofacContainer.Resolve<IScheduler>();
            var sysQuartzBll = LionWeb.AutofacContainer.Resolve<SysQuartzBll>();

            return Task.Factory.StartNew(async () =>
            {
                var state = await schedule.GetTriggerState(jobKey.ConvertKey(), cancellationToken);
                await sysQuartzBll.ModifyTaskState(jobKey.Group, jobKey.Name, state.ConvertTriggerState());
                Console.WriteLine($"{jobKey.Name}  JobPaused");
            }, cancellationToken);
        }

        public Task JobInterrupted(JobKey jobKey, CancellationToken cancellationToken = new CancellationToken())
        {
            var schedule = LionWeb.AutofacContainer.Resolve<IScheduler>();
            var sysQuartzBll = LionWeb.AutofacContainer.Resolve<SysQuartzBll>();

            return Task.Factory.StartNew(async () =>
            {
                var state = await schedule.GetTriggerState(jobKey.ConvertKey(), cancellationToken);
                await sysQuartzBll.ModifyTaskState(jobKey.Group, jobKey.Name, state.ConvertTriggerState());
                Console.WriteLine($"{jobKey.Name}  JobInterrupted");
            }, cancellationToken);
        }


        public Task JobResumed(JobKey jobKey, CancellationToken cancellationToken = new CancellationToken())
        {
            var schedule = LionWeb.AutofacContainer.Resolve<IScheduler>();
            var sysQuartzBll = LionWeb.AutofacContainer.Resolve<SysQuartzBll>();

            return Task.Factory.StartNew(async () =>
            {
                var state = await schedule.GetTriggerState(jobKey.ConvertKey(), cancellationToken);
                await sysQuartzBll.ModifyTaskState(jobKey.Group, jobKey.Name, state.ConvertTriggerState());
                Console.WriteLine($"{jobKey.Name}  JobResumed");
            }, cancellationToken);
        }

        public Task TriggersResumed(string triggerGroup, CancellationToken cancellationToken = new CancellationToken())
        {
            return Task.Factory.StartNew(() =>
            {
                Console.WriteLine($"{triggerGroup}  TriggersResumed");
            }, cancellationToken);
        }

        public Task JobsPaused(string jobGroup, CancellationToken cancellationToken = new CancellationToken())
        {
            return Task.Factory.StartNew(() =>
            {
                Console.WriteLine($"{jobGroup}  JobsPaused");
            }, cancellationToken);
        }

        public Task JobsResumed(string jobGroup, CancellationToken cancellationToken = new CancellationToken())
        {
            return Task.Factory.StartNew(() =>
            {
                Console.WriteLine($"{jobGroup}  JobsResumed");
            }, cancellationToken);
        }

        public Task TriggersPaused(string triggerGroup, CancellationToken cancellationToken = new CancellationToken())
        {
            return Task.Factory.StartNew(() =>
            {
                Console.WriteLine($"{triggerGroup}  TriggersPaused");
            }, cancellationToken);
        }

        public Task SchedulerError(string msg, SchedulerException cause, CancellationToken cancellationToken = new CancellationToken())
        {
            return Task.Factory.StartNew(() =>
            {
                LogHelper.Logger.Fatal(cause, msg);
                Console.WriteLine($"SchedulerError msg:{msg} cause:{cause.Message}");
            }, cancellationToken);
        }

        public Task SchedulerInStandbyMode(CancellationToken cancellationToken = new CancellationToken())
        {
            return Task.Factory.StartNew(() =>
            {
                Console.WriteLine($"SchedulerInStandbyMode");
            }, cancellationToken);
        }

        public Task SchedulerStarted(CancellationToken cancellationToken = new CancellationToken())
        {
            return Task.Factory.StartNew(() =>
            {
                Console.WriteLine($"SchedulerStarted");
            }, cancellationToken);
        }

        public Task SchedulerStarting(CancellationToken cancellationToken = new CancellationToken())
        {
            return Task.Factory.StartNew(() =>
            {
                Console.WriteLine($"SchedulerStarting");
            }, cancellationToken);
        }

        public Task SchedulerShutdown(CancellationToken cancellationToken = new CancellationToken())
        {
            return Task.Factory.StartNew(() =>
            {
                Console.WriteLine($"SchedulerShutdown");
            }, cancellationToken);
        }

        public Task SchedulerShuttingdown(CancellationToken cancellationToken = new CancellationToken())
        {
            return Task.Factory.StartNew(() =>
            {
                Console.WriteLine($"SchedulerShuttingdown");
            }, cancellationToken);
        }

        public Task SchedulingDataCleared(CancellationToken cancellationToken = new CancellationToken())
        {
            return Task.Factory.StartNew(() =>
            {
                Console.WriteLine($"SchedulingDataCleared");
            }, cancellationToken);
        }
    }
}