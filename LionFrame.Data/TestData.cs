using LionFrame.Basic.AutofacDependency;
using LionFrame.Data.BasicData;
using LionFrame.Domain;

namespace LionFrame.Data
{
    public class TestData : BaseData
    {
        public int AddAddress(Address address)
        {
            Add(address);
            return SaveChanges();
        }
    }
}
