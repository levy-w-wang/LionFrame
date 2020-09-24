using System.Collections.Generic;
using System.Linq;

namespace LionFrame.Model.SystemBo
{
    public class UserCacheBo
    {
        public long UserId { get; set; }
        public string UserName { get; set; }
        public string PassWord { get; set; }
        public string Email { get; set; }
        public string UserToken { get; set; }
        public string LoginIp { get; set; }
        public string LoginTime { get; set; }
        public IEnumerable<RoleCacheBo> RoleCacheBos { get; set; }
        public IQueryable<MenuCacheBo> MenuCacheBos { get; set; }
    }
}
