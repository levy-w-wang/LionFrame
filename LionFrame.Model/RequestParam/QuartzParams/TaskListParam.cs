using LionFrame.Model.QuartzModels;
using Quartz;

namespace LionFrame.Model.RequestParam.QuartzParams
{
    /// <summary>
    /// 任务一览请求参数
    /// </summary>
    public class TaskListParam : BaseRequestPageParam
    {
        /// <summary>
        /// 组名
        /// </summary>
        public string Group { get; set; }
        /// <summary>
        /// 任务名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 任务描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 任务状态
        /// </summary>
        public TriggerState? TriggerState { get; set; }
    }
}
