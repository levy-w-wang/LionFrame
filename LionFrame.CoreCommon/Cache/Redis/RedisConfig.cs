namespace LionFrame.CoreCommon.Cache.Redis
{
    internal class RedisConfig
    {
        public string ConnectionString { get; set; }
        public string Pwd { get; set; }
        public int DefaultDatabase { get; set; } = 0;
        public bool AbortOnConnectFail { get; set; } = true;
        public bool AllowAdmin { get; set; } = false;
        public int ConnectRetry { get; set; } = 5;
        public int ConnectTimeout { get; set; } = 3000;
        public int KeepAlive { get; set; } = 20;
        public int SyncTimeout { get; set; } = 30000;
        public bool Ssl { get; set; } = false;

    }
}
