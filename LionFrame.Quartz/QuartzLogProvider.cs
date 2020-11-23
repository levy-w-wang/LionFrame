using System;
using LionFrame.Basic;
using Quartz.Logging;

namespace LionFrame.Quartz
{
    internal class QuartzLogProvider : ILogProvider
    {
        /// <summary>Gets the specified named logger.</summary>
        /// <param name="name">Name of the logger.</param>
        /// <returns>The logger reference.</returns>
        public Logger GetLogger(string name)
        {
            return (level, func, exception, parameters) =>
       {
           if (level >= LogLevel.Info && func != null)
           {
               LogHelper.Logger.Warn(exception, name + "[" + level + "] " + func(), parameters);
           }
           return true;
       };
        }

        /// <summary>
        /// Opens a nested diagnostics context. Not supported in EntLib logging.
        /// </summary>
        /// <param name="message">The message to add to the diagnostics context.</param>
        /// <returns>A disposable that when disposed removes the message from the context.</returns>
        public IDisposable OpenNestedContext(string message)
        {
            return null;
        }

        /// <summary>
        /// Opens a mapped diagnostics context. Not supported in EntLib logging.
        /// </summary>
        /// <param name="key">A key.</param>
        /// <param name="value">A value.</param>
        /// <param name="destructure">Determines whether to call the destructor or not.</param>
        /// <returns>A disposable that when disposed removes the map from the context.</returns>
        public IDisposable OpenMappedContext(string key, object value, bool destructure = false)
        {
            return null;
        }
    }
}
