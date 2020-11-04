using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LionFrame.Model.RequestParam.SystemParams
{
    /// <summary>
    /// 分配菜单请求参数
    /// </summary>
    public class AssignMenuParam
    {
        [Required(ErrorMessage = "分配菜单不能为空")]
        public List<string> MenuIdList { get; set; }
        /// <summary>
        /// true 为分配给非系统管理员使用 false相反
        /// </summary>
        public bool Type { get; set; }
        
    }
}
