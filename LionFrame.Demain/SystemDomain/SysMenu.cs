using LionFrame.Domain.BaseDomain;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LionFrame.Domain.SystemDomain
{
    [Table("Sys_Menu")]
    public class SysMenu : BaseCommonModel
    {
        /// <summary>
        /// 自定义管理Id - 菜单 按钮
        /// </summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None), MaxLength(128)]
        public string MenuId { get; set; }

        [MaxLength(64), Required]
        public string MenuName { get; set; }

        public string ParentMenuId { get; set; }

        [MaxLength(256), Required]
        public string Url { get; set; } = "";

        public int Type { get; set; } //0:目录 1:菜单 2:按钮

        [MaxLength(128), Required]
        public string Icon { get; set; }

        /// <summary>
        /// 排序 从小到大
        /// </summary>
        public int OrderIndex { get; set; }
    }
}
