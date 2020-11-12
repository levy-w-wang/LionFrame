using System;
using System.Collections.Generic;
using System.Text;

namespace LionFrame.Model.ResponseDto.SystemDto
{
    /// <summary>
    /// 获取到的页面对应按钮权限
    /// </summary>
    public class ButtonPermsDto
    {
        public string MenuId { get; set; }
        public string MenuName { get; set; }
        public string Url { get; set; }
        public string ParentMenuId { get; set; }
    }
}
