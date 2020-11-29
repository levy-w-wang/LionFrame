using System;
using System.Collections.Generic;
using System.Text;
using LionFrame.Model.QuartzModels;
using Quartz;

namespace LionFrame.Quartz
{
    public static class TriggerStateConvert
    {
        /// <summary>
        /// 将系统的任务状态转换成自定义状态
        /// </summary>
        /// <param name="triggerState"></param>
        /// <returns></returns>
        public static MyTriggerState ConvertTriggerState(this TriggerState triggerState)
        {
            switch (triggerState)
            {
                case TriggerState.Normal:
                    return MyTriggerState.Normal;
                    break;
                case TriggerState.Paused:
                    return MyTriggerState.Paused;
                    break;
                case TriggerState.Complete:
                    return MyTriggerState.Complete;
                    break;
                case TriggerState.Error:
                    return MyTriggerState.Error;
                    break;
                case TriggerState.Blocked:
                    return MyTriggerState.Blocked;
                    break;
                case TriggerState.None:
                    return MyTriggerState.None;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(triggerState), triggerState, null);
            }
        }

        /// <summary>
        /// 由于jobkey和triggerkey设置的identity是一样的，获取triggerstate需要triggerkey，因此做一个转变
        /// </summary>
        /// <param name="jobKey"></param>
        /// <returns></returns>
        public static TriggerKey ConvertKey(this JobKey jobKey)
        {
            return new TriggerKey(jobKey.Name,jobKey.Group);
        }
    }
}
