using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using LionFrame.Basic.Extensions;
using LionFrame.Config;
using LionFrame.CoreCommon.HttpHelper;
using LionFrame.Model.QuartzModels;
using LionFrame.Model.ResponseDto.ResultModel;
using Quartz;

namespace LionFrame.Quartz.Jobs
{
    //[PersistJobDataAfterExecution]
    //[DisallowConcurrentExecution]
    public class HttpJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            var maxLogCount = 20; //最多保存日志数量
            var warnTime = 20; //接口请求超过多少秒记录警告日志   

            var mailMessage = MailMessageEnum.None;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Restart(); //  开始监视代码运行时间
            try
            {
                //获取相关参数
                var requestUrl = context.JobDetail.JobDataMap.GetString(QuartzConstant.REQUESTURL);
                requestUrl = requestUrl?.IndexOf("http") == 0 ? requestUrl : "http://" + requestUrl;
                var requestParameters = context.JobDetail.JobDataMap.GetString(QuartzConstant.REQUESTPARAMETERS);
                var headersString = context.JobDetail.JobDataMap.GetString(QuartzConstant.HEADERS);
                mailMessage = (MailMessageEnum) int.Parse(context.JobDetail.JobDataMap.GetString(QuartzConstant.MAILMESSAGE) ?? "0");
                var headers = (headersString?.Trim())?.ToObject<Dictionary<string, string>>();
                RequestTypeEnum requestType = (RequestTypeEnum) int.Parse(context.JobDetail.JobDataMap.GetString(QuartzConstant.REQUESTTYPE) ?? string.Empty);

                HttpResponseMessage response = new HttpResponseMessage();


                var http = HttpHelper.Instance;
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
                        response = await http.DeleteAsync(requestUrl, headers);
                        break;
                    default:
                        response = await http.PostAsync(requestUrl, requestParameters, headers);
                        break;
                }

                var result = HttpUtility.HtmlEncode(await response.Content.ReadAsStringAsync());

                stopwatch.Stop(); //  停止监视            
                double seconds = stopwatch.Elapsed.TotalSeconds; //总秒数                                

                if (!response.IsSuccessStatusCode)
                {
                }
                else
                {
                    try
                    {
                        var httpResult = HttpUtility.HtmlDecode(result).ToObject<ResponseModel>();
                        if (!httpResult.Success)
                        {
                        }
                        else
                        {

                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                stopwatch.Stop(); //  停止监视            
                double seconds = stopwatch.Elapsed.TotalSeconds; //总秒数
            }
            finally
            {
                double seconds = stopwatch.Elapsed.TotalSeconds; //总秒数
            }
        }
    }
}