using System.Collections.Generic;
using System.Linq;

namespace LionFrame.Model.ResponseDto.RoleDtos
{
    /// <summary>
    /// 用户管理角色一览
    /// </summary>
    public class RoleListDto
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public string RoleDesc { get; set; }
        public IEnumerable<string> NickNames { get; set; }
    }
}
