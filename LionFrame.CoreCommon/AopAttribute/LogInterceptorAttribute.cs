using AspectCore.DynamicProxy;
using System;
using System.Threading.Tasks;

namespace LionFrame.CoreCommon.AopAttribute
{
    /// <summary>
    /// 日志记录demo
    /// </summary>
    public class LogInterceptorAttribute : AbstractInterceptorAttribute
    {
        public override async Task Invoke(AspectContext context, AspectDelegate next)
        {
            try
            {
                //var logger = context.ServiceProvider.GetService<ILogger>();
                //logger.LogInformation("日志记录");
                Console.WriteLine($"Before method call");
                await next(context);
            }
            catch (Exception)
            {
                Console.WriteLine("method threw an exception!");
                throw;
            }
            finally
            {
                Console.WriteLine("After method call");
            }
        }
    }
}
