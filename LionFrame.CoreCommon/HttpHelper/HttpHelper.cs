using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using LionFrame.Basic.Extensions;

namespace LionFrame.CoreCommon.HttpHelper
{
    /// <summary>
    /// 请求帮助类
    /// </summary>
    public class HttpHelper
    {
        /// <summary>
        /// 超时时间设置 默认10秒
        /// </summary>
        private readonly int _timeOutSeconds;

        public HttpHelper(int timeOutSeconds = 10)
        {
            this._timeOutSeconds = timeOutSeconds;
        }

        /// <summary>
        /// Post请求
        /// </summary>
        /// <param name="url">url地址</param>
        /// <param name="jsonString">请求参数（Json字符串）</param>
        /// <param name="headers">webapi做用户认证</param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> PostAsync(string url, string jsonString, Dictionary<string, string> headers = null, string contentType = "application/json")
        {
            if (string.IsNullOrWhiteSpace(jsonString))
                jsonString = "{}";
            StringContent content = new StringContent(jsonString, Encoding.UTF8, contentType);
            var http = HttpClientHelper.Instance(_timeOutSeconds);
            if (headers != null && headers.Any())
            {
                foreach (var item in headers)
                {
                    http.DefaultRequestHeaders.Remove(item.Key);
                    http.DefaultRequestHeaders.TryAddWithoutValidation(item.Key, item.Value);
                }
            }

            return await http.PostAsync(new Uri(url), content);
        }

        /// <summary>
        /// Post请求
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">url地址</param>
        /// <param name="content">请求参数</param>
        /// <param name="headers">webapi做用户认证</param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> PostAsync<T>(string url, T content, Dictionary<string, string> headers = null, string contentType = "application/json") where T : class
        {
            return await PostAsync(url, content.ToJson(), headers, contentType);
        }

        /// <summary>
        /// Get请求
        /// </summary>
        /// <param name="url">url地址</param>
        /// <param name="headers">webapi做用户认证</param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> GetAsync(string url, Dictionary<string, string> headers = null)
        {
            var http = HttpClientHelper.Instance(_timeOutSeconds);
            if (headers != null && headers.Any())
            {
                //如果有headers认证等信息，则每个请求实例一个HttpClient
                foreach (var item in headers)
                {
                    http.DefaultRequestHeaders.Remove(item.Key);
                    http.DefaultRequestHeaders.TryAddWithoutValidation(item.Key, item.Value);
                }
            }

            return await http.GetAsync(url);
        }

        /// <summary>
        /// Put请求
        /// </summary>
        /// <param name="url">url地址</param>
        /// <param name="jsonString">请求参数（Json字符串）</param>
        /// <param name="headers">webapi做用户认证</param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> PutAsync(string url, string jsonString, Dictionary<string, string> headers = null, string contentType = "application/json")
        {
            var http = HttpClientHelper.Instance(_timeOutSeconds);
            if (string.IsNullOrWhiteSpace(jsonString))
                jsonString = "{}";
            StringContent content = new StringContent(jsonString, Encoding.UTF8, contentType);
            if (headers != null && headers.Any())
            {
                //如果有headers认证等信息，则每个请求实例一个HttpClient
                foreach (var item in headers)
                {
                    http.DefaultRequestHeaders.Remove(item.Key);
                    http.DefaultRequestHeaders.TryAddWithoutValidation(item.Key, item.Value);
                }

                return await http.PutAsync(url, content);
            }

            return await http.PutAsync(url, content);
        }

        /// <summary>
        /// Put请求
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">url地址</param>
        /// <param name="content">请求参数</param>
        /// <param name="headers">webapi做用户认证</param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> PutAsync<T>(string url, T content, Dictionary<string, string> headers = null, string contentType = "application/json")
        {
            return await PutAsync(url, content.ToJson(), headers, contentType);
        }

        /// <summary>
        /// Delete请求
        /// </summary>
        /// <param name="url">url地址</param>
        /// <param name="content"></param>
        /// <param name="headers"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> DeleteAsync<T>(string url, T content, Dictionary<string, string> headers = null, string contentType = "application/json")
        {
            var http = HttpClientHelper.Instance(_timeOutSeconds);
            var request = new HttpRequestMessage
            {
                Content = new StringContent(content.ToJson(), Encoding.UTF8, contentType),
                Method = HttpMethod.Delete,
                RequestUri = new Uri(url),
            };
            if (headers != null && headers.Any())
            {
                //如果有headers认证等信息，则每个请求实例一个HttpClient
                foreach (var item in headers)
                {
                    http.DefaultRequestHeaders.Remove(item.Key);
                    http.DefaultRequestHeaders.TryAddWithoutValidation(item.Key, item.Value);
                }
            }

            return await http.SendAsync(request);
        }

        /// <summary>
        ///  Delete请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="content"></param>
        /// <param name="headers"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> DeleteAsync(string url, string content, Dictionary<string, string> headers = null, string contentType = "application/json")
        {
            var http = HttpClientHelper.Instance(_timeOutSeconds);
            var request = new HttpRequestMessage
            {
                Content = new StringContent(content, Encoding.UTF8, contentType),
                Method = HttpMethod.Delete,
                RequestUri = new Uri(url),
            };
            if (headers != null && headers.Any())
            {
                //如果有headers认证等信息，则每个请求实例一个HttpClient
                foreach (var item in headers)
                {
                    http.DefaultRequestHeaders.Remove(item.Key);
                    http.DefaultRequestHeaders.TryAddWithoutValidation(item.Key, item.Value);
                }
            }

            return await http.SendAsync(request);
        }

        /// <summary>
        /// Delete请求
        /// </summary>
        /// <param name="url">url地址</param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> DeleteAsync(string url, Dictionary<string, string> headers = null)
        {
            var http = HttpClientHelper.Instance(_timeOutSeconds);
            if (headers != null && headers.Any())
            {
                //如果有headers认证等信息，则每个请求实例一个HttpClient
                foreach (var item in headers)
                {
                    http.DefaultRequestHeaders.Remove(item.Key);
                    http.DefaultRequestHeaders.TryAddWithoutValidation(item.Key, item.Value);
                }
            }

            return await http.DeleteAsync(url);
        }

        /// <summary>
        /// 发送自定义请求
        /// </summary>
        /// <param name="httpMethod">请求方式</param>
        /// <param name="url">请求地址</param>
        /// <param name="content">请求Body参数</param>
        /// <param name="headers">请求头</param>
        /// <param name="contentType">请求格式</param>
        /// <param name="timeOutSeconds">超时时间</param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> SendAsync(HttpMethod httpMethod, string url, string content, Dictionary<string, string> headers = null, string contentType = "application/json", int timeOutSeconds = 100)
        {
            var http = HttpClientHelper.Instance(timeOutSeconds);
            var request = new HttpRequestMessage
            {
                Content = new StringContent(content, Encoding.UTF8, contentType),
                Method = httpMethod,
                RequestUri = new Uri(url),
            };
            if (headers == null || !headers.Any())
            {
                return await http.SendAsync(request);
            }

            //如果有headers认证等信息，则每个请求实例一个HttpClient
            foreach (var (key, value) in headers)
            {
                http.DefaultRequestHeaders.Remove(key);
                http.DefaultRequestHeaders.TryAddWithoutValidation(key, value);
            }

            return await http.SendAsync(request);
        }
    }
}