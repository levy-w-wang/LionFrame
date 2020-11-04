using System;
using System.Threading.Tasks;
using AspectCore.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;

namespace  LionFrame.Data.BasicData
{
    /// <summary>
    /// 数据库事务
    /// </summary>
    public class DbTransactionInterceptorAttribute : AbstractInterceptorAttribute
    {
        public override async Task Invoke(AspectContext context, AspectDelegate next)
        {
             var dbContext = context.ServiceProvider.GetService<LionDbContext>();
            //先判断是否已经启用了事务
            if (dbContext.Database.CurrentTransaction == null)
            {
                await dbContext.Database.BeginTransactionAsync();
                try
                {
                    await next(context);
                    dbContext.Database.CommitTransaction();
                }
                catch (Exception ex)
                {
                    dbContext.Database.RollbackTransaction();
                    throw ex;
                }
            }
            else
            {
                await next(context);
            }
        }
    }
}
