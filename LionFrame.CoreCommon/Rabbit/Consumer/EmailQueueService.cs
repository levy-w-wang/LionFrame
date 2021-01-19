using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LionFrame.Basic;
using LionFrame.Basic.Extensions;
using LionFrame.Model.SystemBo;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Hosting;
using MimeKit;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace LionFrame.CoreCommon.Rabbit.Consumer
{
    public class EmailQueueService : IHostedService, IDisposable
    {
        private readonly IConnection _rabbitConnection;
        private readonly IModel _channel;
        public EmailQueueService(IConnection rabbitConnection)
        {
            _rabbitConnection = rabbitConnection;
            _channel = _rabbitConnection.CreateModel();
        }
        /// <summary>
        /// Triggered when the application host is ready to start the service.
        /// </summary>
        /// <param name="cancellationToken">Indicates that the start process has been aborted.</param>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            // 启用QoS，每次预取1条消息，避免消息处理不过来，全部堆积在本地缓存里
            // 此处可根据系统处理情况调整 可多开几个消费者处理
            _channel.BasicQos(0, 1, false);
            _channel.QueueDeclare(queue: "LionEmailQueue",
               durable: true,
               exclusive: false,
               autoDelete: false,
               arguments: null);
            var consumer = new EventingBasicConsumer(_channel);
            _channel.BasicConsume(queue: "LionEmailQueue", autoAck: false, consumer: consumer);
            consumer.Received += (model, ea) =>
            {
                var body = Encoding.UTF8.GetString(ea.Body.Span);
                try
                {
                    var mqMailBo = body.ToObject<MqMailBo>();
                    var mailBo = mqMailBo.MailBo;
                    var message = new MimeMessage();
                    message.From.Add(new MailboxAddress(mailBo.MailFromName ?? mailBo.MailFrom, mailBo.MailFrom));
                    foreach (var mailTo in mailBo.MailTo.Replace("；", ";").Replace("，", ";").Replace(",", ";").Split(';'))
                    {
                        message.To.Add(new MailboxAddress(mailBo.MailToName ?? mailTo, mailTo));
                    }

                    message.Subject = string.Format(mqMailBo.Title);
                    message.Body = new TextPart("html")
                    {
                        Text = mqMailBo.HtmlContent
                    };
                    using (var client = new SmtpClient())
                    {
                        client.Connect(mailBo.MailHost, mailBo.MailPort, false, cancellationToken);
                        client.Authenticate(mailBo.MailFrom, mailBo.MailPwd, cancellationToken);
                        client.Send(message, cancellationToken);
                        client.Disconnect(true, cancellationToken);
                    }

                    _channel.BasicAck(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    //multiple 参数设置为 true 则表示拒绝 deliveryTag 编号之前所 有未被当前消费者确认的消息。
                    //requeue 参数设置为 true，则 RabbitMQ 会重新将这条消息存入 队列，以便可以发送给下一个订阅的消费者;如果
                    //requeue 参数设置为 false，则 RabbitMQ 立即会把消息从队列中移除，而不会把它发送给新的消费者。
                    _channel.BasicNack(ea.DeliveryTag, false, false);//发送失败直接删除  
                    LogHelper.Logger.Fatal(ex, $"邮件发送失败,body:{body}");
                }
            };
            return Task.CompletedTask;
        }

        /// <summary>
        /// Triggered when the application host is performing a graceful shutdown.
        /// </summary>
        /// <param name="cancellationToken">Indicates that the shutdown process should no longer be graceful.</param>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            _channel?.Close();
            _channel?.Dispose();

            _rabbitConnection?.Close();
            _rabbitConnection?.Dispose();
        }
    }
}
