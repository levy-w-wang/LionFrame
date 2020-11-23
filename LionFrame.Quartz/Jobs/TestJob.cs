using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Quartz;

namespace LionFrame.Quartz.Jobs
{
    [DisallowConcurrentExecution]
    [PersistJobDataAfterExecution]
    public class TestJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            return Task.CompletedTask;
        }
    }
}
