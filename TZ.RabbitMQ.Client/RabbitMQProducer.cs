using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace TZ.RabbitMQ.Client
{
    public class RabbitMQProducer : RabbitMQClientBase,IDisposable
    {

        protected IModel Channel;
        public RabbitMQProducer(IOptions<RabbitMQClientOptions> options) : base(options)
        {
        }



        /// <summary>
        /// 打开连接
        /// </summary>
        public new void Open()
        {
            base.Open();
            if (Channel == null)
                Channel = Connection.CreateModel();
        }
        
        /// <summary>
        /// 关闭连接
        /// </summary>
        public new void Close()
        {
            Channel?.Close();
            base.Close();
        }

        public new void Dispose()
        {
            Close();
        }

        /// <summary>
        /// 发送工作队列消息
        /// </summary>
        /// <param name="message"></param>
        /// <param name="queueName"></param>
        /// <param name="prefetchCount">默认20</param>
        public void SendWorkQueues(string message, string queueName)
        {
            var routingKey = queueName;
            Channel.QueueDeclare(queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);
            //不要同时给一个消费者推送多于prefetchCount个消息,ushort prefetchCount=20
            //Channel.BasicQos(prefetchSize: 0, prefetchCount: prefetchCount, global: false);
            var body = Encoding.UTF8.GetBytes(message);
            var properties = Channel.CreateBasicProperties();
            properties.Persistent = true;
            properties.DeliveryMode = 2;//持久化消息
            Channel.BasicPublish(exchange: "",
                routingKey: routingKey,
                basicProperties: properties,
                body: body);
        }


        /// <summary>
        /// 发送工作队列消息
        /// </summary>
        /// <param name="message"></param>
        /// <param name="queueName"></param>
        /// <param name="prefetchCount">默认20</param>
        public void SendDelayQueues(string message, string queueName,double delayMilliseconds,string beDeadLetterPrefix="beDeadLetter_")
        {
            #region 死信到期后转入的交换机及队列
            //死信转入新的队列的路由键（消费者使用的路由键）
            var routingKey = queueName;
            var exchangeName = queueName;
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
            #endregion

            //将变成死信的队列名
            var beDeadLetterQueueName = beDeadLetterPrefix + queueName;
            //将变成死信的交换机名
            var beDeadLetterExchangeName = beDeadLetterPrefix + queueName;

            //定义一个有延迟的交换机来做死信（该消息不能有消费者，不然无法变成死信）
            Channel.ExchangeDeclare(exchange:beDeadLetterExchangeName ,
                type: "direct");
            
            //定义该延迟消息过期变成死信后转入的交换机（消费者需要绑定的交换机）
            //Channel.ExchangeDeclare(exchange: queueName,type: "direct");

            var dic = new Dictionary<string, object>();
            //dic.Add("x-expires", 30000);
            //dic.Add("x-message-ttl", 12000);//队列上消息过期时间，应小于队列过期时间  
            dic.Add("x-dead-letter-exchange", queueName);//变成死信后转向的交换机
            dic.Add("x-dead-letter-routing-key",routingKey);//变成死信后转向的路由键
            //定义将变成死信的队列
            Channel.QueueDeclare(queue: beDeadLetterQueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: dic);

            //队列绑定到交换机
            Channel.QueueBind(queue: beDeadLetterQueueName,
                exchange: beDeadLetterExchangeName,
                routingKey: routingKey);

            //不要同时给一个消费者推送多于prefetchCount个消息, ushort prefetchCount = 20
            //Channel.BasicQos(prefetchSize: 0, prefetchCount: prefetchCount, global: false);
            var body = Encoding.UTF8.GetBytes(message);
            var properties = Channel.CreateBasicProperties();
            properties.Persistent = true;
            properties.DeliveryMode = 2;//持久化消息
            //过期时间
            properties.Expiration = delayMilliseconds.ToString();
            Channel.BasicPublish(exchange: beDeadLetterExchangeName,
                routingKey: routingKey,
                basicProperties: properties,
                body: body);
        }

    }
}