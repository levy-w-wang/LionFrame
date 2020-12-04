using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Autofac;

namespace LionFrame.CoreCommon.HttpHelper
{
    public class HttpClientHelper
    {
        HttpClientHelper()
        {
        }

        /// <summary>
        /// 主要是为了设置请求超时时间
        /// </summary>
        /// <param name="timeOutSeconds"> 默认10秒 timeout</param>
        /// <returns></returns>
        public static HttpClient Instance(int timeOutSeconds = 10)
        {
            var httpClientFactory = LionWeb.AutofacContainer.Resolve<IHttpClientFactory>();
            var clientInstance = httpClientFactory.CreateClient();
            clientInstance.Timeout = TimeSpan.FromSeconds(timeOutSeconds);
            return clientInstance;
        }
    }
}