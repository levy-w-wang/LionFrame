using System;
using System.Collections.Generic;
using System.Text;
using LionFrame.Config;

namespace LionFrame.Model.ResponseDto.SystemDto
{
    /// <summary>
    /// 传输到前端的菜单对象
    /// </summary>
    public class MenuDto
    {
        public string MenuId { get; set; }

        /// <summary>
        /// 菜单名字
        /// </summary>
        public string MenuName { get; set; }

        /// <summary>
        /// 父级ID
        /// </summary>
        public string ParentMenuId { get; set; }

        /// <summary>
        /// 菜单层级
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// 菜单地址
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 1:菜单 2:按钮
        /// </summary>
        public SysConstants.MenuType Type { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// 排序 从小到大
        /// </summary>
        public int OrderIndex { get; set; }

        /// <summary>
        /// 子菜单
        /// </summary>
        public List<MenuDto> ChildMenus { get; set; }
    }
}
