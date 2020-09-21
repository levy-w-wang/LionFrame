using LionFrame.Basic.AutofacDependency;
using LionFrame.CoreCommon.AopAttribute;
using LionFrame.Data;
using LionFrame.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

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

        public int TestAdd(User user)
        {
            return TestData.AddUser(user);
        }

        public Address GetAddress(int i)
        {
            return TestData.First<Address>(c => c.Id == i);
        }

        public List<User> GetUser(int i)
        {
            return TestData.CurrentDbContext.Set<User>().Where(c => true).Include(d=>d.Address).ToList();
        }

        public List<User> GetOnlyUser(int i)
        {
            //return TestData.First<User>(c => c.Id == i);
            return TestData.CurrentDbContext.Set<User>().Where(c => true).ToList();
        }
    }
}
