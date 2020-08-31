using LionFrame.Basic.AutofacDependency;
using LionFrame.CoreCommon.AopAttribute;
using System;

namespace LionFrame.Business
{
    public class TestBll : IScopedDependency
    {
        [LogInterceptor]
        public virtual string GetGuid()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
