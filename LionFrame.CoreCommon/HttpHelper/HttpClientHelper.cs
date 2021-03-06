﻿using System;
using System.Net.Http;
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
            IHttpClientFactory httpClientFactory;
            using (var container = LionWeb.AutofacContainer.BeginLifetimeScope())
            {
                httpClientFactory = container.Resolve<IHttpClientFactory>();
            }
            var clientInstance = httpClientFactory.CreateClient();
            clientInstance.Timeout = TimeSpan.FromSeconds(timeOutSeconds);
            return clientInstance;
        }
    }
}