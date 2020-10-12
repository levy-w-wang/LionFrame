using LionFrame.Domain.SystemDomain;
using Microsoft.EntityFrameworkCore;

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

        public DbSet<SysUser> SysUsers { get; set; }
        public DbSet<SysRole> SysRoles { get; set; }
        public DbSet<SysUserRoleRelation> SysUserRoleRelations { get; set; }
        public DbSet<SysMenu> SysMenus { get; set; }
        public DbSet<SysRoleMenuRelation> SysRoleMenuRelations { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region 使用这种方式 就不需要上面的dbset 但是在实体类中也不能加导航属性
            // 使用反射将所有继承于BaseModel的类进行获取，并添加到数据库上下文中。 -- 也可以使用autofac来获取。
            // 避免每次都在这里添加 DbSet
            //Assembly assembly = Assembly.Load("LionFrame.Domain");
            //var entityBaseType = typeof(BaseModel);
            //var modeTypes = assembly.GetTypes().Where(t => entityBaseType.IsAssignableFrom(t) && t != entityBaseType && !t.IsAbstract && !t.IsInterface).ToArray();
            //if (modeTypes.Length <= 0)
            //{
            //    return;
            //}
            //foreach (var modeType in modeTypes)
            //{
            //    modelBuilder.Model.AddEntityType(modeType);
            //}
            #endregion

            modelBuilder.Entity<SysUser>().HasIndex(c => c.UserName).IsUnique();
            modelBuilder.Entity<SysUser>().HasIndex(c => c.Email).IsUnique();

            modelBuilder.Entity<SysUserRoleRelation>()
                .HasKey(t => new { t.RoleId, t.UserId });
            modelBuilder.Entity<SysUserRoleRelation>()
                .HasOne(ur => ur.SysUser)
                .WithMany(r => r.SysUserRoleRelations)
                .HasForeignKey(c => c.UserId);
            modelBuilder.Entity<SysUserRoleRelation>()
                .HasOne(ur => ur.SysRole)
                .WithMany(r => r.SysUserRoleRelations)
                .HasForeignKey(c => c.RoleId);

            modelBuilder.Entity<SysRoleMenuRelation>()
                .HasKey(t => new { t.RoleId, t.MenuId });
            modelBuilder.Entity<SysRoleMenuRelation>()
                .HasOne(rm => rm.SysRole)
                .WithMany(r => r.SysRoleMenuRelations)
                 .HasForeignKey(c => c.RoleId);
            modelBuilder.Entity<SysRoleMenuRelation>()
                .HasOne(rm => rm.SysMenu)
                .WithMany(r => r.SysRoleMenuRelations)
                .HasForeignKey(c => c.MenuId);


            // 初始数据
            modelBuilder.InitData();
        }
    }
}
