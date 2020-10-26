using System.ComponentModel.DataAnnotations;
using LionFrame.Config;

namespace LionFrame.Model.RequestParam.SystemParams
{
    /// <summary>
    /// 新增或修改菜单
    /// </summary>
    public class IncrementMenuParam
    {
        /// <summary>
        /// 自定义管理Id - 菜单M 按钮B 开头
        /// 各个页面管理自己页面的按钮
        /// 菜单层级1  1 2
        /// 菜单层级2 101  201
        /// 菜单层级3 10101 20101
        /// 菜单层级4 1010101 2010101
        /// </summary>
        [MaxLength(128), Required]
        public string MenuId { get; set; }

        /// <summary>
        /// 菜单名字
        /// </summary>
        [MaxLength(64), Required]
        public string MenuName { get; set; }

        /// <summary>
        /// 父级ID
        /// </summary>
        [MaxLength(128)]
        public string ParentMenuId { get; set; } = "";

        /// <summary>
        /// 菜单层级 1 2 3 4
        /// </summary>
        [Range(1,10)]
        public int Level { get; set; } = 1;

        /// <summary>
        /// 菜单地址
        /// </summary>
        [MaxLength(256, ErrorMessage = "菜单地址超过长度限制")]
        public string Url { get; set; } = "";

        /// <summary>
        /// 1:菜单 2:按钮
        /// </summary>
        [Required]
        public SysConstants.MenuType Type { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        [MaxLength(128, ErrorMessage = "图标超过长度限制")]
        public string Icon { get; set; } = "";

        /// <summary>
        /// 排序 从小到大
        /// </summary>
        public int OrderIndex { get; set; }

        /// <summary>
        /// 超级管理员是roleId是1,管理员是2  用户权限默认是2
        /// 默认都会设置到超级管理员下  可以设定是否分配给管理员
        /// </summary>
        public bool AssignAdmin { get; set; }
        
    }
}
