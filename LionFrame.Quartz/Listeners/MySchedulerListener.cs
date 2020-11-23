using System;
using System.Threading;
using System.Threading.Tasks;
using Quartz;

namespace LionFrame.Quartz.Listeners
{
    public class MySchedulerListener : ISchedulerListener
    {
        public Task JobScheduled(ITrigger trigger, CancellationToken cancellationToken = new CancellationToken())
        {
            return Task.Factory.StartNew(() =>
            {
                Console.WriteLine($"{trigger.Key.Name}  JobScheduled");
            }, cancellationToken);
        }

        public Task JobUnscheduled(TriggerKey triggerKey, CancellationToken cancellationToken = new CancellationToken())
        {
            return Task.Factory.StartNew(() =>
            {
                Console.WriteLine($"{triggerKey.Name}  JobUnscheduled");
            }, cancellationToken);
        }

        public Task TriggerFinalized(ITrigger trigger, CancellationToken cancellationToken = new CancellationToken())
        {
            return Task.Factory.StartNew(() =>
            {
                Console.WriteLine($"{trigger.Key.Name}  TriggerFinalized");
            }, cancellationToken);
        }


        public Task TriggerPaused(TriggerKey triggerKey, CancellationToken cancellationToken = new CancellationToken())
        {
            return Task.Factory.StartNew(() =>
            {
                Console.WriteLine($"{triggerKey.Name}  TriggerPaused");
            }, cancellationToken);
        }

        public Task TriggersPaused(string triggerGroup, CancellationToken cancellationToken = new CancellationToken())
        {
            return Task.Factory.StartNew(() =>
            {
                Console.WriteLine($"{triggerGroup}  TriggersPaused");
            }, cancellationToken);
        }


        public Task TriggerResumed(TriggerKey triggerKey, CancellationToken cancellationToken = new CancellationToken())
        {
            return Task.Factory.StartNew(() =>
            {
                Console.WriteLine($"{triggerKey.Name}  TriggerResumed");
            }, cancellationToken);
        }

        public Task TriggersResumed(string triggerGroup, CancellationToken cancellationToken = new CancellationToken())
        {
            return Task.Factory.StartNew(() =>
            {
                Console.WriteLine($"{triggerGroup}  TriggersResumed");
            }, cancellationToken);
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
            return Task.Factory.StartNew(() =>
            {
                Console.WriteLine($"{jobKey.Name}  JobDeleted");
            }, cancellationToken);
        }


        public Task JobPaused(JobKey jobKey, CancellationToken cancellationToken = new CancellationToken())
        {
            return Task.Factory.StartNew(() =>
            {
                Console.WriteLine($"{jobKey.Name}  JobPaused");
            }, cancellationToken);
        }

        public Task JobInterrupted(JobKey jobKey, CancellationToken cancellationToken = new CancellationToken())
        {
            return Task.Factory.StartNew(() =>
            {
                Console.WriteLine($"{jobKey.Name}  JobInterrupted");
            }, cancellationToken);
        }

        public Task JobsPaused(string jobGroup, CancellationToken cancellationToken = new CancellationToken())
        {
            return Task.Factory.StartNew(() =>
            {
                Console.WriteLine($"{jobGroup}  JobsPaused");
            }, cancellationToken);
        }

        public Task JobResumed(JobKey jobKey, CancellationToken cancellationToken = new CancellationToken())
        {
            return Task.Factory.StartNew(() =>
            {
                Console.WriteLine($"{jobKey.Name}  JobResumed");
            }, cancellationToken);
        }

        public Task JobsResumed(string jobGroup, CancellationToken cancellationToken = new CancellationToken())
        {
            return Task.Factory.StartNew(() =>
            {
                Console.WriteLine($"{jobGroup}  JobsResumed");
            }, cancellationToken);
        }

        public Task SchedulerError(string msg, SchedulerException cause, CancellationToken cancellationToken = new CancellationToken())
        {
            return Task.Factory.StartNew(() =>
            {
                Console.WriteLine($"SchedulerError");
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
