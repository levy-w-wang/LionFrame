using LionFrame.Basic.AutofacDependency;
using LionFrame.CoreCommon.AopAttribute;
using LionFrame.Data;
using LionFrame.Domain;
using System;

namespace LionFrame.Business
{
    public class TestBll : IScopedDependency
    {
        public TestData TestData { get; set; }

        [LogInterceptor]
        public virtual string GetGuid()
        {
            return Guid.NewGuid().ToString();
        }

        public int TestAdd(Address address)
        {
            return TestData.AddAddress(address);
        }

        public Address GetAddress(int i)
        {
            return TestData.First<Address>(c => c.Id == i);
        }
    }
}
