using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LionFrame.MainWeb
{
    public partial class Startup
    {
        /// <summary>
        /// 初始化
        /// </summary>
        public void Init()
        {

        }

        /// <summary>
        /// 数据库操作语句显示
        /// </summary>
        public readonly ILoggerFactory DbConsoleLoggerFactory =
            LoggerFactory.Create(builder =>
            {
                builder.AddFilter((category, level) =>
                        category == DbLoggerCategory.Database.Command.Name
                        && level == LogLevel.Information)
                    .AddConsole();
            });
    }
}
