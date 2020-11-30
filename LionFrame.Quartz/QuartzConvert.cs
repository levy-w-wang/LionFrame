using Quartz;

namespace LionFrame.Quartz
{
    public static class QuartzConvert
    {

        /// <summary>
        /// 由于jobkey和triggerkey设置的identity是一样的，获取triggerstate需要triggerkey，因此做一个转变
        /// </summary>
        /// <param name="jobKey"></param>
        /// <returns></returns>
        public static TriggerKey ConvertKey(this JobKey jobKey)
        {
            return new TriggerKey(jobKey.Name, jobKey.Group);
        }

        /// <summary>
        /// 由于jobkey和triggerkey设置的identity是一样的，获取triggerstate需要triggerkey，因此做一个转变
        /// </summary>
        /// <param name="triggerKey"></param>
        /// <returns></returns>
        public static JobKey ConvertKey(this TriggerKey triggerKey)
        {
            return new JobKey(triggerKey.Name, triggerKey.Group);
        }
    }
}
