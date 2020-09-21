using LionFrame.Basic.AutofacDependency;
using LionFrame.Data.SystemDao;
using LionFrame.Domain.SystemDomain;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFrame.Business
{
    public class UserBll : IScopedDependency
    {
        public SysUserDao SysUserDao { get; set; }
        public SysUser Test()
        {
            //SysUserDao.CloseTracking();
            return SysUserDao.First<SysUser>(c=>c.UserId == 1);
        }
    }
}
