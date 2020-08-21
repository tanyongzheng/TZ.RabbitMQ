using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace TZ.RabbitMQ.Client
{

    public class RabbitMQConsumer : RabbitMQClientBase, IDisposable
    {

        protected List<IModel> ChannelList=new List<IModel>();
        public RabbitMQConsumer(IOptions<RabbitMQClientOptions> options) : base(options)
        {
        }

        /// <summary>
        /// 打开连接
        /// </summary>
        public new void Open()
        {
            base.Open();
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        public new void Close()
        {
            foreach (var channel in ChannelList)
            {
                channel?.Close();
            }
            base.Close();
        }

        public new void Dispose()
        {
            Close();
        }

        /// <summary>
        /// 设置工作队列接收的事件
        /// </summary>
        /// <param name="action"></param>
        /// <param name="queueName"></param>
        /// <param name="prefetchCount">默认1</param>
        /// <param name="autoAck"></param>
        /// <param name="consumerCount"></param>
        public void SetWorkQueuesReceivedAction(Action<string> action, string queueName,ushort prefetchCount=1, bool autoAck=false, int consumerCount = 1)
        {
            for (var i = 0; i < consumerCount; i++)
            {
                var Channel = Connection.CreateModel();
                Channel.QueueDeclare(queue: queueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);
                //不要同时给一个消费者推送多于prefetchCount个消息
                Channel.BasicQos(prefetchSize: 0, prefetchCount: prefetchCount, global: false);
                ChannelList.Add(Channel);
                var consumer = new EventingBasicConsumer(Channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    //Console.WriteLine("处理消费者ConsumerTag:"+ea.ConsumerTag);
                    action(message);
                    //手动确认消息应答
                    Channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                };
                //autoACK自动消息应答设置为false
                Channel.BasicConsume(queue: queueName, autoAck: autoAck, consumer: consumer);
            }
        }

        /// <summary>
        /// 设置延迟队列接收的事件
        /// </summary>
        /// <param name="action"></param>
        /// <param name="queueName"></param>
        /// <param name="prefetchCount">默认1</param>
        /// <param name="autoAck"></param>
        /// <param name="consumerCount"></param>
        public void SetDelayQueuesReceivedAction(Action<string> action, string queueName, ushort prefetchCount = 1,
            bool autoAck = false, int consumerCount = 1)
        {
            if (prefetchCount < 1)
            {
                throw new Exception("consumerCount must be greater than 1 !");
            }

            var exchangeName = queueName;
            var routingKey = queueName;
            for (int i = 0; i < consumerCount; i++)
            {
                var Channel = Connection.CreateModel();
                //定义队列
                Channel.QueueDeclare(queue: queueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);
                //定义交换机
                Channel.ExchangeDeclare(exchange: exchangeName,
                    type: "direct");
                //队列绑定到交换机
                Channel.QueueBind(queue: queueName,
                    exchange: exchangeName,
                    routingKey: routingKey);
                //不要同时给一个消费者推送多于prefetchCount个消息
                Channel.BasicQos(prefetchSize: 0, prefetchCount: prefetchCount, global: false);
                ChannelList.Add(Channel);
                var consumer = new EventingBasicConsumer(Channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    //Console.WriteLine("处理消费者ConsumerTag:" + ea.ConsumerTag);
                    action(message);
                    //手动确认消息应答
                    Channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                };
                //autoACK自动消息应答设置为false
                Channel.BasicConsume(queue: queueName, autoAck: autoAck, consumer: consumer);
            }
        }

    }
}