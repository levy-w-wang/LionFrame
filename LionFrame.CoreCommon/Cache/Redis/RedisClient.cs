using LionFrame.Basic;
using LionFrame.Basic.Extensions;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace LionFrame.CoreCommon.Cache.Redis
{
    /// <summary>
    /// Redis Client
    /// </summary>
    public class RedisClient : IDisposable
    {
        private RedisConnectionFactory factory;

        public static JsonSerializerSettings JsonSettings { get; set; } = new JsonSerializerSettings
        {
            TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
            TypeNameHandling = TypeNameHandling.Auto,
            Formatting = Formatting.None
        };

        public RedisClient(IConfiguration configuration)
        {
            try
            {
                if (factory == null)
                {
                    factory = new RedisConnectionFactory(configuration);
                    _client = factory.GetConnectionMultiplexer();
                    this.DefaultDatabase = factory.DefaultDatabase;
                    UseDatabase();
                }
            }
            catch (Exception e)
            {
                LogHelper.Logger.Fatal(e, "redis 初始化失败");
            }
        }

        public int DefaultDatabase { get; set; } = 0;

        private readonly ConnectionMultiplexer _client;
        private IDatabase _db;

        public void UseDatabase(int db = -1)
        {
            if (db == -1)
                db = DefaultDatabase;
            _db = _client.GetDatabase(db);
        }


        public string StringGet(string key)
        {
            return _db.StringGet(key).ToString();
        }


        public void StringSet(string key, string data)
        {
            _db.StringSet(key, data);
        }

        public void StringSet(string key, string data, TimeSpan timeout)
        {
            _db.StringSet(key, data, timeout);
        }


        public T Get<T>(string key)
        {
            var json = StringGet(key);
            if (string.IsNullOrEmpty(json))
            {
                return default(T);
            }

            return json.ToObject<T>(JsonSettings);
        }

        public void Set<T>(string key, T data)
        {
            var json = data.ToJson(JsonSettings);
            _db.StringSet(key, json);
        }

        public void Set<T>(string key, T data, TimeSpan timeout)
        {
            var json = data.ToJson(JsonSettings);
            _db.StringSet(key, json, timeout);
        }

        /// <summary>
        /// Exist
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Exist(string key)
        {
            return _db.KeyExists(key);
        }

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Delete(string key)
        {
            return _db.KeyDelete(key);
        }

        /// <summary>
        /// Set Expire to Key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        public bool Expire(string key, TimeSpan? expiry)
        {
            return _db.KeyExpire(key, expiry);
        }

        /// <summary>
        /// 计数器  如果不存在则设置值，如果存在则添加值  如果key存在且类型不为long  则会异常
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiry">只有第一次设置有效期生效</param>
        /// <returns></returns>
        public long SetStringIncr(string key, long value = 1, TimeSpan? expiry = null)
        {
            var number = _db.StringIncrement(key, value);
            if (number == 1 && expiry != null)//只有第一次设置有效期（防止覆盖）
                _db.KeyExpireAsync(key, expiry);//设置有效期
            return number;
        }

        /// <summary>
        /// 读取计数器
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public long GetStringIncr(string key)
        {
            var value = StringGet(key);
            return value.IsNullOrEmpty() ? 0 : long.Parse(value);
        }

        /// <summary>
        /// 计数器-减少 如果不存在则设置值，如果存在则减少值  如果key存在且类型不为long  则会异常
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public long StringDecrement(string key, long value = 1)
        {
            var number = _db.StringDecrement(key, value);
            return number;
        }

        public IServer GetServer()
        {
            var endpoints = _client.GetEndPoints();
            IServer result = null;
            foreach (var endpoint in endpoints)
            {
                var server = _client.GetServer(endpoint);
                if (server.IsSlave || !server.IsConnected) continue;

                result = server;
                break;
            }

            if (result == null) throw new InvalidOperationException("Requires exactly one master endpoint (found none)");
            return result;
        }

        #region 分布式锁...

        /// <summary>
        /// 分布式锁 Token。
        /// </summary>
        private static readonly RedisValue LockToken = "LOCK_LION";

        /// <summary>
        /// 获取锁。
        /// </summary>
        /// <param name="key">锁名称。</param>
        /// <param name="seconds">过期时间（秒）。</param>
        /// <returns>是否已锁。</returns>
        public bool Lock(string key, int seconds)
        {
            return _db.LockTake(key, LockToken, TimeSpan.FromSeconds(seconds));
        }

        /// <summary>
        /// 释放锁。
        /// </summary>
        /// <param name="key">锁名称。</param>
        /// <returns>是否成功。</returns>
        public bool UnLock(string key)
        {
            return _db.LockRelease(key, LockToken);
        }

        /// <summary>
        /// 异步获取锁。
        /// </summary>
        /// <param name="key">锁名称。</param>
        /// <param name="seconds">过期时间（秒）。</param>
        /// <returns>是否成功。</returns>
        public async Task<bool> LockAsync(string key, int seconds)
        {
            return await _db.LockTakeAsync(key, LockToken, TimeSpan.FromSeconds(seconds));
        }

        /// <summary>
        /// 异步释放锁。
        /// </summary>
        /// <param name="key">锁名称。</param>
        /// <returns>是否成功。</returns>
        public async Task<bool> UnLockAsync(string key)
        {
            return await _db.LockReleaseAsync(key, LockToken);
        }

        #endregion

        #region 发布订阅

        /// <summary>
        /// 发布函数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="channel"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public long RedisPub<T>(string channel, T msg)
        {

            ISubscriber sub = _client.GetSubscriber();
            return sub.Publish(channel, msg.ToJson());
        }
        /// <summary>
        /// 订阅函数
        /// </summary>
        /// <param name="subChannel"></param>
        /// <param name="actionCallback"></param>
        public void RedisSub(string subChannel, Action<string> actionCallback)
        {
            ISubscriber sub = _client.GetSubscriber();
            sub.Subscribe(subChannel, (channel, message) =>
            {
                try
                {
                    actionCallback(message);
                }
                catch (Exception e)
                {
                    LogHelper.Logger.Fatal(e, $"频道:{subChannel},消息：{message}，处理失败");
                }
            });
        }

        /// <summary>
        /// 取消订阅某个频道
        /// </summary>
        /// <param name="channel"></param>
        public void Unsubscribe(string channel)
        {
            ISubscriber sub = _client.GetSubscriber();
            sub.Unsubscribe(channel);
        }
        /// <summary>
        /// 取消订阅所有频道
        /// </summary>
        public void UnsubscribeAll()
        {
            ISubscriber sub = _client.GetSubscriber();
            sub.UnsubscribeAll();
        }

        #endregion
        public void Dispose()
        {
            _client?.Dispose();
        }

        #region Hash  若要设置过期时间 请使用 Expire 方法

        /// <summary>
        /// 存值
        /// </summary>
        /// <param name="key">唯一标识键</param>
        /// <param name="field">标识字段</param>
        /// <param name="value">存的值</param>
        /// <returns></returns>
        public bool HashSet<T>(string key, string field, T value)
        {
            return _db.HashSet(key, field, value.ToJson(true));
        }

        /// <summary>
        /// 获取某一个值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public T HashGet<T>(string key, string field)
        {
            var json = _db.HashGet(key, field).ToString();
            return json.ToObject<T>();
        }

        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public bool HashExists(string key, string field)
        {
            return _db.HashExists(key, field);
        }

        /// <summary>
        /// 删除某一个
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public bool HashDelete(string key, string field)
        {
            return _db.HashDelete(key, field);
        }

        /// <summary>
        /// 删除某个集合
        /// </summary>
        /// <param name="key"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public long HashDelete(string key, string[] fields)
        {
            var redisFields = Array.ConvertAll(fields, item => (RedisValue)item);
            return _db.HashDelete(key, redisFields);
        }
        /// <summary>
        /// 获取所有数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public HashEntry[] HashGetAll(string key)
        {
            return _db.HashGetAll(key);
        }

        /// <summary>
        /// 获取所有的值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public RedisValue[] HashValues(string key)
        {
            return _db.HashValues(key);
        }

        /// <summary>
        /// 获取所有的 字段值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public RedisValue[] HashFields(string key)
        {
            return _db.HashKeys(key);
        }

        #endregion
    }
}
