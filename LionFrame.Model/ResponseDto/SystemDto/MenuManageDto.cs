using System.Collections.Generic;

namespace LionFrame.Model.ResponseDto.SystemDto
{
    public class MenuManageDto : MenuPermsDto
    {
        public new List<MenuManageDto> ChildMenus { get; set; }
        public new List<ButtonManageDto> ButtonPerms { get; set; }
        public bool Deleted { get; set; }
    }
}
