using System.Collections.Generic;

namespace LionFrame.Model.ResponseDto.UserDtos
{
    /// <summary>
    /// 用户管理一览获取角色
    /// </summary>
    public class UserManagerDto
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string ParentId { get; set; }
        public List<string> RoleIds { get; set; }
        public List<string> RoleNames { get; set; }
    }
}
