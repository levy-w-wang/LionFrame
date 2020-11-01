using System;
using System.Collections.Generic;
using System.Text;

namespace LionFrame.Model.ResponseDto.SystemDto
{
    public class ButtonManageDto : ButtonPermsDto
    {
        public bool Deleted { get; set; }
    }
}
