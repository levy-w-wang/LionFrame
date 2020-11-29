using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LionFrame.Basic.AutofacDependency;
using LionFrame.Data.SystemDao;
using LionFrame.Domain.SystemDomain;

namespace LionFrame.Business
{
    public class SysQuartzLogBll : IScopedDependency
    {
        public SysQuartzLogDao SysQuartzLogDao { get; set; }

        public async Task<bool> AddTaskLog(SysQuartzLog sysQuartzLog)
        {
            await SysQuartzLogDao.AddAsync(sysQuartzLog);
            var result = await SysQuartzLogDao.SaveChangesAsync();
            return result > 0;
        }
    }
}
