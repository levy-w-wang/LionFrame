using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LionFrame.Basic;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace LionFrame.CoreCommon.Rabbit.Consumer
{
    public class RetryDeadLetterConsumer : IHostedService, IDisposable
    {
        private readonly IConnection _rabbitConnection;
        private readonly IModel _channel;

        /// <summary>
        /// 1 5 10 秒 重试 重试失败的丢进死信队列，不再处理
        /// </summary>
        private readonly List<int> _retryTime;

        public RetryDeadLetterConsumer(IConnection rabbitConnection)
        {
            _rabbitConnection = rabbitConnection;
            _channel = _rabbitConnection.CreateModel();
            _retryTime = new List<int>
            {
                1 * 1000,
                5 * 1000,
                10 * 1000
            };
        }

        private const string RetryCount = "retryCount";

        private const string ExchangeName = "TestExchange";
        private const string QueueName = "TestQueue";
        private const string RouteName = "TestRouteKey";

        /// <summary>
        /// Triggered when the application host is ready to start the service.
        /// </summary>
        /// <param name="cancellationToken">Indicates that the start process has been aborted.</param>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            ConsumerInit();
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += Consumer_Received;
            _channel.BasicConsume(QueueName, false, consumer);
            return Task.CompletedTask;
        }

        /// <summary>
        /// 消息接收处理委托
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Consumer_Received(object sender, BasicDeliverEventArgs ea)
        {
            bool canAck = false;
            var retryCount = 0;
            if (ea.BasicProperties.Headers != null && ea.BasicProperties.Headers.ContainsKey(RetryCount))
            {
                retryCount = (int)ea.BasicProperties.Headers[RetryCount];
            }

            try
            {
                var body = Encoding.UTF8.GetString(ea.Body.Span);
                if (body.Contains("重试"))
                {
                    throw new Exception("重试测试");
                }
                canAck = true;
            }
            catch (Exception e)
            {
                if (CanRetry(retryCount))
                {
                    SetupRetry(retryCount, ExchangeName + "@Retry", RouteName + "@Retry", ea);
                    canAck = true;//放置到重试队列后  ACK，将本队列中的删掉。然后等待重新转发到当前队列重试
                }
                else
                {
                    canAck = false;
                }
            }
            //处理ACK 或者 放置到死信中去
            try
            {
                if (canAck)
                {
                    // 重试次数大于3次，则自动加入到死信队列
                    _channel.BasicAck(ea.DeliveryTag, false);
                }
                else
                {
                    var properties = ea.BasicProperties;
                    properties.Headers = properties.Headers ?? new Dictionary<string, object>();
                    if (properties.Headers.Keys.Contains("x-orig-routing-key") == false)
                    {
                        properties.Headers.Add("x-orig-routing-key", ea.RoutingKey);
                    }
                    else
                    {
                        properties.Headers["x-orig-routing-key"] = ea.RoutingKey;
                    }
                    // 放到死信队列
                    _channel.BasicPublish(ExchangeName + "@Failed", RouteName + "@Failed", properties, ea.Body);
                    _channel.BasicNack(ea.DeliveryTag, false, false);
                    // multiple：是否批量.true:将一次性拒绝所有小于deliveryTag的消息。 requeue：被拒绝的是否重新入队列
                }
            }
            catch (Exception ex)
            {
                LogHelper.Logger.Fatal(ex, "消息消费出现异常");
            }
        }

        /// <summary>
        /// 放置到重试队列 等待时间到后，自动重试
        /// </summary>
        /// <param name="retryCount"></param>
        /// <param name="retryExchange"></param>
        /// <param name="retryRoute"></param>
        /// <param name="ea"></param>
        private void SetupRetry(int retryCount, string retryExchange, string retryRoute, BasicDeliverEventArgs ea)
        {
            var body = ea.Body;
            var properties = ea.BasicProperties;
            properties.Headers = properties.Headers ?? new Dictionary<string, object>();
            properties.Headers["x-orig-routing-key"] = ea.RoutingKey;
            properties.Expiration = _retryTime[retryCount].ToString();
            retryCount += 1;
            properties.Headers[RetryCount] = retryCount;

            try
            {
                _channel.BasicPublish(retryExchange, retryRoute, properties, body);
            }
            catch (Exception ex)
            {
                LogHelper.Logger.Fatal(ex, $"消息发送重试队列失败,重试次数:{retryCount},重试消息：{Encoding.UTF8.GetString(ea.Body.Span)}");
            }
        }

        private bool CanRetry(int retryCount)
        {
            return retryCount <= _retryTime.Count - 1;
        }

        /// <summary>
        /// 初始化相关配置
        /// </summary>
        private void ConsumerInit()
        {
            _channel.BasicQos(0, 1, false);
            _channel.ExchangeDeclare(ExchangeName, ExchangeType.Direct, true);
            _channel.QueueDeclare(QueueName, true, false, false);
            _channel.QueueBind(QueueName, ExchangeName, RouteName); //绑定队列进行正常消费
            // 重试队列定义
            var retryDic = new Dictionary<string, object>
            {
                { "x-dead-letter-exchange", ExchangeName },
                { "x-dead-letter-routing-key", RouteName }, //当消息超时时，消息转发到交换机对应路由key上
            };
            _channel.ExchangeDeclare(ExchangeName + "@Retry", ExchangeType.Direct, true);
            _channel.QueueDeclare(QueueName + "@Retry", true, false, false, retryDic);
            _channel.QueueBind(QueueName + "@Retry", ExchangeName + "@Retry", RouteName + "@Retry");
            // 死信队列定义
            _channel.ExchangeDeclare(ExchangeName + "@Failed", ExchangeType.Direct, true);
            _channel.QueueDeclare(QueueName + "@Failed", true, false, false, null);
            _channel.QueueBind(QueueName + "@Failed", ExchangeName + "@Failed", RouteName + "@Failed");
        }

        /// <summary>
        /// Triggered when the application host is performing a graceful shutdown.
        /// </summary>
        /// <param name="cancellationToken">Indicates that the shutdown process should no longer be graceful.</param>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            this.Dispose();
            return Task.CompletedTask;
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            _channel?.Close();
            _channel?.Dispose();

            _rabbitConnection?.Close();
            _rabbitConnection?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
