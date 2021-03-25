using System;
using Microsoft.Extensions.Configuration;

namespace LionFrame.Model.SystemBo
{
    /// <summary>
    /// MQ配置
    /// </summary>
    public class RabbitOption
    {
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="config"></param>
        public RabbitOption(IConfiguration config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            var section = config.GetSection("rabbit");
            section.Bind(this);
        }

        public string Uri { get; set; }
    }
}
