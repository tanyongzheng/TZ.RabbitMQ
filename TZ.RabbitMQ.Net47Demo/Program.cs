using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TZ.RabbitMQ.Client;

namespace TZ.RabbitMQ.Net47Demo
{
    class Program
    {
		static void Main(string[] args)
		{
			//SendMsg();
			SendDelayMsg();
			//LoadDelayMsg();
			//LoadMsg();
			Console.ReadKey();
		}

		private static void SendMsg()
		{
			RabbitmqConfigOptions rabbitmqOption = new RabbitmqConfigOptions();
			var queueName = "queue_test_workqueues";
			//var queueName = "test_delayqueues";
			RabbitMQProducer producer = new RabbitMQProducer(rabbitmqOption);
			producer.Open();
			for (int i = 0; i < 500; i++)
			{
				//producer.SendWorkQueues("这是工作队列消息" + i.ToString(), queueName);
				var isPublished=producer.SendWorkQueuesWithConfirmSelect("这是工作队列消息" + i.ToString(), queueName);
				Console.WriteLine($"发送{(isPublished?"成功":"失败")}");
			}
			producer.Close();
		}
		private static void SendDelayMsg()
		{
			RabbitmqConfigOptions rabbitmqOption = new RabbitmqConfigOptions();
			string queueName = "test_delayqueues";
			RabbitMQProducer producer = new RabbitMQProducer(rabbitmqOption);
			producer.Open();
			for (int i = 0; i < 500; i++)
			{
				//producer.SendDelayQueues("这是延迟消息" + i.ToString(), queueName, (double)(10000 + i * 1000), "beDeadLetter_");
				var isPublished = producer.SendDelayQueuesWithConfirmSelect("这是延迟消息" + i.ToString(), queueName, (double)(10000 + i * 1000), "beDeadLetter_");
				Console.WriteLine($"发送{(isPublished?"成功":"失败")}");
			}
			producer.Close();
		}

		private static void LoadDelayMsg()
		{
			int doSeconds = 2;
			ushort prefetchCount = 1;
			RabbitmqConfigOptions rabbitmqOption = new RabbitmqConfigOptions();
			string queueName = "test_delayqueues";
			RabbitMQConsumer consumer = new RabbitMQConsumer(rabbitmqOption);
			consumer.Open();
			consumer.SetDelayQueuesReceivedAction(delegate (string msg)
			{
				Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " 1接收到消息：" + msg);
				//Thread.Sleep(doSeconds * 100 * Program.GetRandomNum(1, 50));
			}, queueName, prefetchCount, false, 20);
		}
		private static void LoadMsg()
		{
			int doSeconds = 2;
			ushort prefetchCount = 1;
			RabbitmqConfigOptions rabbitmqOption = new RabbitmqConfigOptions();
			var queueName = "queue_test_workqueues";
			RabbitMQConsumer consumer = new RabbitMQConsumer(rabbitmqOption);
			consumer.Open();
			consumer.SetWorkQueuesReceivedAction(delegate (string msg)
			{
				Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " 1接收到消息：" + msg);
				//Thread.Sleep(doSeconds * 100 * Program.GetRandomNum(1, 50));
			}, queueName, prefetchCount, false, 20);
		}

		private static int GetRandomNum(int minValue, int maxValue)
		{
			long tick = DateTime.Now.Ticks;
			Random ran = new Random((int)(tick & 0xffffffffL) | (int)(tick >> 32));
			int result = ran.Next(minValue, maxValue);
			System.Threading.Thread.Sleep(1);
			return result;
		}
	}
}
