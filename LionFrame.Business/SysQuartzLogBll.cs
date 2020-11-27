using System;
using System.Collections.Generic;
using System.Text;
using LionFrame.Basic.AutofacDependency;
using LionFrame.Data.SystemDao;

namespace LionFrame.Business
{
    public class SysQuartzLogBll : IScopedDependency
    {
        public SysQuartzLogDao SysQuartzLogDao { get; set; }
        
    }
}
