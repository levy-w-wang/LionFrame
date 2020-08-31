using Autofac;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Linq;

namespace LionFrame.CoreCommon
{
    /// <summary>
    /// 全局可用的系统字段
    /// </summary>
    public class LionWeb
    {
        private static IHttpContextAccessor _httpContextAccessor;
        /// <summary>
        /// Configure
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        public static void Configure(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// 当前请求HttpContext
        /// </summary>
        public static HttpContext HttpContext
        {
            get => _httpContextAccessor.HttpContext;
            set => _httpContextAccessor.HttpContext = value;
        }

        /// <summary>
        /// Environment
        /// </summary>
        public static IHostEnvironment Environment { get; set; }

        /// <summary>
        /// Configuration
        /// </summary>
        public static IConfiguration Configuration { get; set; }

        /// <summary>
        /// MemoryCache
        /// </summary>
        public static IMemoryCache MemoryCache { get; set; }

        /// <summary>
        /// autofac
        /// </summary>
        public static ILifetimeScope AutofacContainer { get; set; }

        /// <summary>
        /// 获取当前请求客户端IP
        /// </summary>
        /// <returns></returns>
        public static string GetClientIp()
        {
            var ip = HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault()?.Split(',')[0].Trim();
            if (string.IsNullOrEmpty(ip))
            {
                ip = HttpContext.Connection.RemoteIpAddress.ToString();
            }

            return ip;
        }
    }
}
