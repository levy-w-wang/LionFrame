using System;
using System.Collections.Generic;

namespace LionFrame.Model.ResponseDto.SystemDto
{
    public class UserDto
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public DateTime LoginTime { get; set; }
        public int Sex { get; set; }
        public List<RoleDto> RoleDtos { get; set; }
    }
}
