using LionFrame.Domain.BaseDomain;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Reflection;

namespace LionFrame.Data.BasicData
{
    /// <summary>
    /// 数据库上下文
    /// </summary>
    public class LionDbContext : DbContext
    {
        public LionDbContext(DbContextOptions<LionDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 使用反射将所有继承于BaseModel的类进行获取，并添加到数据库上下文中。 -- 也可以使用autofac来获取。
            // 避免每次都在这里添加 DbSet
            Assembly assembly = Assembly.Load("LionFrame.Domain");
            var entityBaseType = typeof(BaseModel);
            var modeTypes = assembly.GetTypes().Where(t => entityBaseType.IsAssignableFrom(t) && t != entityBaseType && !t.IsAbstract && !t.IsInterface).ToArray();
            if (modeTypes.Length <= 0)
            {
                return;
            }
            foreach (var modeType in modeTypes)
            {
                modelBuilder.Model.AddEntityType(modeType);
            }
        }
    }
}
