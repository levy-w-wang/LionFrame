using System;
using System.Collections.Generic;
using System.Text;

namespace LionFrame.Model.ResponseDto.SystemDto
{
    /// <summary>
    /// 菜单及对应按钮权限集合
    /// </summary>
    public class PermissionDto
    {
        public List<MenuDto> MenuDtos { get; set; }
        public Dictionary<string, string> ButtonPerms { get; set; }
    }
}
