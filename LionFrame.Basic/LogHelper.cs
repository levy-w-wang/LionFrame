using NLog;

namespace LionFrame.Basic
{
    public class LogHelper
    {
        /// <summary>
        /// NLog的实例对象
        /// </summary>
        public static Logger Logger = ConfigureNLog().GetCurrentClassLogger();
        public static LogFactory ConfigureNLog()
        {
            return NLog.Web.NLogBuilder.ConfigureNLog("NLog.config");
        }
    }
}
