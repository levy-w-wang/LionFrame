using System;
using System.Collections.Generic;
using System.Text;
using LionFrame.Basic.AutofacDependency;

namespace LionFrame.Business
{
    public class TestBll : IScopedDependency
    {
        public string GetGuid()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
