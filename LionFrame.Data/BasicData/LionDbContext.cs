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
        public DbSet<SysTenant> SysTenants { get; set; }
        public DbSet<SysQuartz> SysQuartzs { get; set; }
        public DbSet<SysQuartzLog> SysQuartzLogs { get; set; }


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

            modelBuilder.Entity<SysUser>().HasOne(c => c.TenantInfo)
                .WithMany(c => c.SysUser)
                .HasForeignKey(c => c.TenantId);
            modelBuilder.Entity<SysUser>().HasIndex(c => new { c.TenantId, c.UserId });
            modelBuilder.Entity<SysUser>().HasIndex(c => c.Email).IsUnique();

            modelBuilder.Entity<SysRole>().HasIndex(c => new { c.TenantId, c.RoleId });

            modelBuilder.Entity<SysUserRoleRelation>()
                .HasKey(t => new { t.RoleId, t.UserId, t.TenantId });
            modelBuilder.Entity<SysUserRoleRelation>()
                .HasOne(ur => ur.SysUser)
                .WithMany(r => r.SysUserRoleRelations)
                .HasForeignKey(c => c.UserId);
            modelBuilder.Entity<SysUserRoleRelation>()
                .HasOne(ur => ur.SysRole)
                .WithMany(r => r.SysUserRoleRelations)
                .HasForeignKey(c => c.RoleId);

            modelBuilder.Entity<SysRoleMenuRelation>()
                .HasKey(t => new { t.RoleId, t.MenuId, t.TenantId });
            modelBuilder.Entity<SysRoleMenuRelation>()
                .HasOne(rm => rm.SysRole)
                .WithMany(r => r.SysRoleMenuRelations)
                 .HasForeignKey(c => c.RoleId);
            modelBuilder.Entity<SysRoleMenuRelation>()
                .HasOne(rm => rm.SysMenu)
                .WithMany(r => r.SysRoleMenuRelations)
                .HasForeignKey(c => c.MenuId);

            modelBuilder.Entity<SysQuartz>().HasKey(sq => new { sq.JobGroup, sq.JobName });
            modelBuilder.Entity<SysQuartz>().HasIndex(sq => sq.TriggerState);
            modelBuilder.Entity<SysQuartzLog>().HasIndex(sq => new { sq.JobGroup, sq.JobName });

            // 初始数据
            modelBuilder.InitData();
        }
    }
}
