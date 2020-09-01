namespace LionFrame.Model.RequestParam
{
    /// <summary>
    /// 查询含有分页的基础模型
    /// </summary>
    public class BaseRequestPageParam
    {
        /// <summary>
        /// 当前页
        /// </summary>
        public int CurrentPage { get; set; }
        /// <summary>
        /// 页大小
        /// </summary>
        public int PageSize { get; set; }
    }
}
