using System.Threading;
using System.Threading.Tasks;
using Quartz;

namespace LionFrame.Quartz.Listeners
{
    public class MyTriggerListener : ITriggerListener
    {
        /// <summary>
        /// 触发器被调用  在 VetoJobExecution 前执行
        /// </summary>
        /// <param name="trigger"></param>
        /// <param name="context"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task TriggerFired(ITrigger trigger, IJobExecutionContext context, CancellationToken cancellationToken = new CancellationToken())
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// 是否阻止继续执行job
        /// </summary>
        /// <param name="trigger"></param>
        /// <param name="context"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<bool> VetoJobExecution(ITrigger trigger, IJobExecutionContext context, CancellationToken cancellationToken = new CancellationToken())
        {
            return Task.FromResult(false); //返回true表示否决Job继续执行
        }

        /// <summary>
        /// 触发器未执行
        /// </summary>
        /// <param name="trigger"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task TriggerMisfired(ITrigger trigger, CancellationToken cancellationToken = new CancellationToken())
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// 触发器执行完成
        /// </summary>
        /// <param name="trigger"></param>
        /// <param name="context"></param>
        /// <param name="triggerInstructionCode"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task TriggerComplete(ITrigger trigger, IJobExecutionContext context, SchedulerInstruction triggerInstructionCode, CancellationToken cancellationToken = new CancellationToken())
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Get the name of the <see cref="T:Quartz.ITriggerListener" />.
        /// </summary>
        public string Name { get; } = nameof(MyTriggerListener);
    }
}
