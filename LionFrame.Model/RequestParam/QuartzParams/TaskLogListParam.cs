namespace LionFrame.Model.RequestParam.QuartzParams
{
    /// <summary>
    /// Task日志分页查询
    /// </summary>
    public class TaskLogListParam : BaseRequestPageParam
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
    }
}
