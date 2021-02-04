using System;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace LionFrame.CoreCommon.Cache.Redis
{
    internal class RedisConnectionFactory
    {
        public IConfiguration _Configuration;
        public ConnectionMultiplexer CurrentConnectionMultiplexer { get; set; }

        public int DefaultDatabase { get; private set; }
        public RedisConnectionFactory(IConfiguration configuration)
        {
            this._Configuration = configuration;
        }

        public ConnectionMultiplexer GetConnectionMultiplexer()
        {
            if (CurrentConnectionMultiplexer == null || !CurrentConnectionMultiplexer.IsConnected)
            {
                if (CurrentConnectionMultiplexer != null)
                {
                    CurrentConnectionMultiplexer.Dispose();
                }

                CurrentConnectionMultiplexer = InitConnectionMultiplexer();
            }

            return CurrentConnectionMultiplexer;
        }


        private ConnectionMultiplexer InitConnectionMultiplexer()
        {
            var redisConfig = new RedisConfig();
            var config = _Configuration.GetSection("Redis");
            if (config == null)
            {
                throw new Exception("请在配置文件中设置redis配置信息");
            }
            config.Bind(redisConfig);

            ConnectionMultiplexer connectionMultiplexer;

            if (!string.IsNullOrWhiteSpace(redisConfig.Pwd) && !redisConfig.ConnectionString.ToLower().Contains("password"))
            {
                redisConfig.ConnectionString += $",password={redisConfig.Pwd}";
            }

            var redisConfiguration = ConfigurationOptions.Parse(redisConfig.ConnectionString);
            redisConfiguration.AbortOnConnectFail = redisConfig.AbortOnConnectFail;
            redisConfiguration.AllowAdmin = redisConfig.AllowAdmin;
            redisConfiguration.ConnectRetry = redisConfig.ConnectRetry;
            redisConfiguration.ConnectTimeout = redisConfig.ConnectTimeout;
            redisConfiguration.DefaultDatabase = redisConfig.DefaultDatabase;
            redisConfiguration.KeepAlive = redisConfig.KeepAlive;
            redisConfiguration.SyncTimeout = redisConfig.SyncTimeout;
            redisConfiguration.Ssl = redisConfig.Ssl;
            DefaultDatabase = redisConfig.DefaultDatabase;
            connectionMultiplexer = ConnectionMultiplexer.Connect(redisConfiguration);

            return connectionMultiplexer;
        }
    }
}
