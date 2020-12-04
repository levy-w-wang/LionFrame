using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using LionFrame.Basic.Extensions;
using LionFrame.Config;
using LionFrame.CoreCommon.HttpHelper;
using LionFrame.Model.QuartzModels;
using Quartz;

namespace LionFrame.Quartz.Jobs
{
    //[PersistJobDataAfterExecution]
    //[DisallowConcurrentExecution]
    public class HttpJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            //获取相关参数
            var requestUrl = context.JobDetail.JobDataMap.GetString(QuartzConstant.REQUESTURL);
            requestUrl = requestUrl?.IndexOf("http") == 0 ? requestUrl : "http://" + requestUrl;
            var requestParameters = context.JobDetail.JobDataMap.GetString(QuartzConstant.REQUESTPARAMETERS);
            var headersString = context.JobDetail.JobDataMap.GetString(QuartzConstant.HEADERS);
            var headers = (headersString?.Trim())?.ToObject<Dictionary<string, string>>();
            var requestType = int.Parse(context.JobDetail.JobDataMap.GetString(QuartzConstant.REQUESTTYPE) ?? "0").ToEnum<RequestTypeEnum>();

            HttpResponseMessage response;

            var http = new HttpHelper();
            switch (requestType)
            {
                case RequestTypeEnum.Get:
                    response = await http.GetAsync(requestUrl, headers);
                    break;
                case RequestTypeEnum.Post:
                    response = await http.PostAsync(requestUrl, requestParameters, headers);
                    break;
                case RequestTypeEnum.Put:
                    response = await http.PutAsync(requestUrl, requestParameters, headers);
                    break;
                case RequestTypeEnum.Delete:
                    if (requestParameters.IsNullOrEmpty())
                    {
                        response = await http.DeleteAsync(requestUrl, headers);
                    }
                    else
                    {
                        response = await http.DeleteAsync(requestUrl, requestParameters, headers);
                    }
                    break;
                case RequestTypeEnum.None:
                default:
                    response = await http.PostAsync(requestUrl, requestParameters, headers);
                    break;
            }
            var result = await response.Content.ReadAsStringAsync();

            //var result = "{}";
            //if (!response.IsSuccessStatusCode)
            //{
               
            //}
            //else
            //{
            //    result = await response.Content.ReadAsStringAsync();
            //}

            //设置响应结果
            context.Result = result;
        }
    }
}