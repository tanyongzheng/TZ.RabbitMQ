using System;
using System.Configuration;
using Microsoft.Extensions.Options;
using TZ.RabbitMQ.Client;

namespace TZ.RabbitMQ.Net47Demo
{
	public class RabbitmqConfigOptions : IOptions<RabbitMQClientOptions>
	{
		private RabbitMQClientOptions rabbitMQClientOptions;
		public RabbitMQClientOptions Value
		{
			get
			{
				bool flag = this.rabbitMQClientOptions == null;
				if (flag)
				{
					this.rabbitMQClientOptions = new RabbitMQClientOptions();
					this.rabbitMQClientOptions.Host = ConfigurationManager.AppSettings["RabbitMQClient_Host"];
					this.rabbitMQClientOptions.Port = Convert.ToInt32(ConfigurationManager.AppSettings["RabbitMQClient_Port"]);
					this.rabbitMQClientOptions.VirtualHost = ConfigurationManager.AppSettings["RabbitMQClient_VirtualHost"];
					this.rabbitMQClientOptions.Username = ConfigurationManager.AppSettings["RabbitMQClient_Username"];
					this.rabbitMQClientOptions.Password = ConfigurationManager.AppSettings["RabbitMQClient_Password"];
				}
				return this.rabbitMQClientOptions;
			}
		}
	}
}
