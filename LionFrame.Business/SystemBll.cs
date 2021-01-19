using LionFrame.Basic;
using LionFrame.Basic.AutofacDependency;
using LionFrame.Basic.Extensions;
using LionFrame.Basic.Models;
using LionFrame.Config;
using LionFrame.CoreCommon.Cache.Redis;
using LionFrame.Model.SystemBo;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LionFrame.Model.RequestParam.UserParams;
using RabbitMQ.Client;

namespace LionFrame.Business
{
    /// <summary>
    /// 系统相关业务类
    /// </summary>
    public class SystemBll : IScopedDependency
    {
        public IConnection RabbitConnection { get; set; }

        /// <summary>
        /// 验证码5分钟过期
        /// </summary>
        private static readonly TimeSpan CaptchaExpired = TimeSpan.FromMinutes(5);

        public RedisClient RedisClient { get; set; }

        public IdWorker IdWorker { get; set; }

        /// <summary>
        /// 获取验证码
        /// </summary>
        /// <returns></returns>
        public async Task<CaptchaResult> GetCaptchaResultAsync()
        {
            //var captcha = CaptchaHelper.GenerateCaptcha(75, 35, CaptchaHelper.GenerateCaptchaCode());
            var captcha = CaptchaHelper.CreateImage(CaptchaHelper.GenerateCaptchaCode());
            var uuid = IdWorker.NextId();
            var key = CacheKeys.CAPTCHA + uuid;
            await RedisClient.SetAsync(key, captcha.Captcha, CaptchaExpired);
            captcha.Uuid = uuid.ToString();
            return captcha;
        }

        /// <summary>
        /// 验证登录时的验证码
        /// </summary>
        /// <param name="loginParam"></param>
        /// <returns></returns>
        internal async Task<string> VerificationLoginAsync(LoginParam loginParam)
        {
            var key = CacheKeys.CAPTCHA + loginParam.Uuid;
            var captcha = await RedisClient.GetAsync<string>(key);
            if (captcha == null)
            {
                return "验证码已失效";
            }

            await RedisClient.DeleteAsync(key);
            if (!string.Equals(loginParam.Captcha, captcha, StringComparison.OrdinalIgnoreCase))
            {
                return "验证码错误";
            }
            return "验证通过";
        }

        /// <summary>
        /// 发送邮件 正常情况下应该使用 MQ 来解耦发送邮件 -- 后面改为MQ
        /// </summary>
        /// <param name="title">邮件主题</param>
        /// <param name="htmlContent">邮件内容</param>
        /// <param name="emailTo">收件人邮箱  可以用分号分隔 发送到多个</param>
        /// <param name="emailToName">收件人姓名</param>
        /// <returns></returns>
        public async Task<string> SendSystemMailAsync(string title, string htmlContent, string emailTo, string emailToName = null)
        {
            if (title.IsNullOrEmpty() || htmlContent.IsNullOrEmpty() || emailTo.IsNullOrEmpty())
            {
                return "发送失败,参数不允许为空!";
            }
            var mailBo = new MailBo
            {
                MailToName = emailToName,
                MailTo = emailTo
            };
            var mqMailBo = new MqMailBo()
            {
                MailBo = mailBo,
                Title = title,
                HtmlContent = htmlContent
            };
            using (var channel = RabbitConnection.CreateModel())
            {
                channel.QueueDeclare(queue: "LionEmailQueue",
                                                    durable: true,
                                                 exclusive: false,
                                                 autoDelete: false,
                                                 arguments: null);
                channel.ConfirmSelect(); //等待发送成功确认
                var properties = channel.CreateBasicProperties();
                properties.Persistent = true; //持久化
                properties.MessageId = IdWorker.NextId().ToString();//消息Id
                properties.Timestamp = new AmqpTimestamp(DateTime.Now.GetJsTimeStamp());
                properties.Headers = new Dictionary<string, object>();
                var bodyJson = mqMailBo.ToJson();
                var body = Encoding.UTF8.GetBytes(bodyJson);
                channel.BasicPublish(exchange: "",
                                routingKey: "LionEmailQueue",
                                basicProperties: properties,
                                body: body);
                var isOk = channel.WaitForConfirms();
                if (isOk)
                {
                    return "发送成功";
                }

                //日志记录
                LogHelper.Logger.Error($"邮件保存MQ失败,bodyJson:{bodyJson}");
                return "发送异常";
            }
        }

        /// <summary>
        /// 获取邮件内容
        /// </summary>
        /// <param name="title">验证码主题</param>
        /// <param name="captcha">验证码</param>
        /// <param name="expireMinutes">过期时间  默认5分钟</param>
        /// <returns></returns>
        public string GetMailContent(string title, string captcha, int expireMinutes = 5)
        {
            var emailContent = $@"<div class='email' style='width:570px; border:1px solid #56A19E'>
                            <div class='hd' style='height:28px; padding-left:24px; background-color:#56A19E'>
                                               <h2 style='line-height:28px; color:#fff;'>验证码</h2>
                            </div>
                            <div class='bd' style='padding:0px 10px 10px'>
                                     <h3 style='font:bold 14px/42px arial; text-indent:10px; margin: 0;'>{title}<span style='font-size:12px; font-weight:400; color:#5A5A5A'>({expireMinutes}分钟内有效)</span></h3>                                    
                                     <div class='link-href' style='padding:20px 10px; background-color:#2EB3AF'>
                                               <div>
                                                        您的验证码是：<br>
                                                        <div>
                                                        <b><font size='5' color='#D0021B'><u>{captcha}</u></font></b>  
                                                        </div>
                                                </div>
                                     </div>
                            </div>
                   </div>";
            return emailContent;
        }
    }
}
