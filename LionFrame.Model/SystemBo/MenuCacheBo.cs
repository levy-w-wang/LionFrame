using LionFrame.Config;
using System.Collections.Generic;

namespace LionFrame.Model.SystemBo
{
    public class MenuCacheBo
    {
        /// <summary>
        /// 自定义管理Id - 菜单M 按钮B 开头
        /// 各个页面管理自己页面的按钮
        /// 菜单层级1  1 2
        /// 菜单层级2 101  201
        /// 菜单层级3 10101 20101
        /// 菜单层级4 1010101 2010101
        /// </summary>
        public string MenuId { get; set; }

        /// <summary>
        /// 菜单/按钮名字
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
        public List<MenuCacheBo> ChildMenus { get; set; }

        /// <summary>
        /// 当前页面的按钮权限组合名称
        /// 放置于每个router中的meta中。
        /// 在 watch $route 中，将每次点击的路由的权限放在vuex中
        /// 在页面中通过指令来权限判断
        /// </summary>
        public List<string> ButtonPerms { get; set; }
    }
}
